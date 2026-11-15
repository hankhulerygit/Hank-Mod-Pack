using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using Jibbers.Sdk;

namespace HankModPack;

/// <summary>
/// Keeps Hank Mod Pack up to date from its own GitHub repository, without waiting for the loader's catalog.
/// Replace <c>update/HankModPack.dll</c> in the repository and the next game launch picks it up.
///
/// How it stays safe: HTTPS only, from the one repository named below; the download must match the git blob hash GitHub itself reports for the file,
/// must be a .NET assembly called HankModPack, and the running DLL is only renamed (never overwritten), so a failure leaves the working one in place.
/// "mode" in JibbersMods\Hank Mod Pack\update.json: "ask" (default) shows the new version on the game menu and Triangle or Alt Gr installs it, "auto" installs by itself, "off" never checks.
/// </summary>
internal static class ModUpdater
{
    /// <summary>The repository, branch and file the updates come from.</summary>
    internal const string Repo = "hankhulerygit/Hank-Mod-Pack", Branch = "main", FilePath = "update/HankModPack.dll";

    static int started;
    static readonly object gate = new();
    static bool busy;

    /// <summary>Idle, Checking, Available, Installing, Installed, UpToDate or Failed: what the Settings page of the quick menu shows.</summary>
    internal static volatile string State = "Idle";
    internal static bool Enabled => Mode != "off";
    internal static string Mode { get; private set; } = "ask";
    static string? current, pendingSha, pendingUrl, pendingNotes;
    static Action<string>? logger;

    /// <summary>The version the running DLL has (from its assembly), e.g. "1.52.9".</summary>
    internal static string Version { get { var v = typeof(ModUpdater).Assembly.GetName().Version; return v is null ? "?" : $"{v.Major}.{v.Minor}.{Math.Max(v.Build, 0)}"; } }

    internal static string StatusText => State switch
    {
        "Checking" => "checking...", "Available" => "install new version", "Installing" => "installing...",
        "Installed" => "restart the game", "UpToDate" => "up to date", "Failed" => "failed (see log)", _ => "check",
    };

    internal static void Start(Action<string> log)
    {
        if (Interlocked.Exchange(ref started, 1) != 0) return;
        logger = log;
        Mode = ReadMode();
        Task.Run(async () =>
        {
            try
            {
                current = CurrentDllPath();
                if (current is null) { log("updates: could not find the running DLL, skipped."); return; }
                CleanOld(current, log);
                RemoveOldVersions(current, log);
                ShowWhatsNew(log);
                if (Mode == "off") { log("updates: off (update.json)."); return; }
                await Run(install: Mode == "auto");
            }
            catch (Exception e) { log("updates: " + e.Message); }
        });
    }

    /// <summary>Quick menu > Settings > Y: installs the new version if there is one, otherwise checks again.</summary>
    internal static void FromMenu()
    {
        if (!Enabled) { Toast.Show("Updates are off (update.json)"); return; }
        var install = State == "Available";
        logger?.Invoke("updates: asked from the menu (" + State + (install ? ", installing)" : ", checking)"));
        Toast.Show(install ? "Hank Mod Pack: installing the update..." : State == "Installed" ? "Hank Mod Pack is already updated\nRestart the game to use it" : "Hank Mod Pack: checking for updates...", 4f);
        if (State == "Installed") return;
        Task.Run(async () =>
        {
            try
            {
                await Run(install);
                if (State == "UpToDate") Toast.Show("Hank Mod Pack is up to date", 4f);
            }
            catch (Exception e) { logger?.Invoke("updates: " + e.Message); }
        });
    }

    static int watching;

    /// <summary>On the game menu, while a new version waits: Triangle (pad) or Alt Gr (keyboard) installs it. Stops after a few minutes or when the scene changes.</summary>
    static void Watch()
    {
        if (Interlocked.Exchange(ref watching, 1) != 0) return;
        Task.Run(async () =>
        {
            try
            {
                var end = Environment.TickCount64 + 180_000;
                while (watching == 1 && State == "Available" && Environment.TickCount64 < end)
                {
                    await Task.Delay(120);
                    var go = Input.IsDown(Key.RightAlt);
                    for (var slot = 0; slot < 4 && !go; slot++)
                    {
                        var pad = Gamepad.Read(slot);
                        if (pad.Connected && (pad.Buttons & PadButton.Y) != 0) go = true;
                    }
                    if (!go) continue;
                    logger?.Invoke("updates: install asked on the game menu (Triangle / Alt Gr).");
                    Toast.Show("Hank Mod Pack: installing the update...", 4f);
                    watching = 0;
                    await Run(install: true);
                    return;
                }
            }
            catch (Exception e) { logger?.Invoke("updates: " + e.Message); }
            finally { watching = 0; }
        });
    }

    /// <summary>The scene changed (the game starts): the waiting message and the Triangle / Alt Gr watch stop.</summary>
    internal static void LeaveMenu()
    {
        if (Interlocked.Exchange(ref watching, 0) == 1) Toast.Dismiss();
    }

    static string ConfigPath => Path.Combine(Bootstrap.ModsDir, "Hank Mod Pack", "update.json");

    static string ReadMode()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
                File.WriteAllText(ConfigPath, "{\n  \"mode\": \"ask\",\n  \"_modes\": \"ask = tell me on the game menu, Triangle or Alt Gr installs; auto = install by itself at launch; off = never check\"\n}\n");
                return "ask";
            }
            var m = (JsonNode.Parse(File.ReadAllText(ConfigPath)) as JsonObject)?["mode"]?.GetValue<string>()?.Trim().ToLowerInvariant();
            return m switch { "off" => "off", "auto" => "auto", _ => "ask" };
        }
        catch { return "ask"; }
    }

    static string? CurrentDllPath()
    {
        var here = typeof(ModUpdater).Assembly.Location;
        if (!string.IsNullOrEmpty(here) && File.Exists(here)) return here;
        foreach (var p in new[] { Path.Combine(Bootstrap.ModsDir, "cs", "HankModPack.dll"), Path.Combine(Bootstrap.ModsDir, "HankModPack.dll") })
            if (File.Exists(p)) return p;
        return null;
    }

    /// <summary>The previous DLL of an earlier update is renamed to .old (a loaded DLL can be renamed, not overwritten): it goes at the next launch.</summary>
    static void CleanOld(string current, Action<string> log)
    {
        try { var old = current + ".old"; if (File.Exists(old)) File.Delete(old); }
        catch (Exception e) { log("updates: could not delete the old DLL: " + e.Message); }
    }

    // The ids of the mods that now live inside the pack: a separate DLL that still holds one of them is an old standalone version of that mod.
    static readonly string[] PackIds = { "join_spawn", "rick_backflip", "look_around", "timeline_fx", "replay_camera", "hank_hud", "play_cameras", "pad_overlay" };

    /// <summary>
    /// Installing the pack removes what it replaces: older copies of Hank Mod Pack under another file name, the standalone Improved Spawn / Replay Editor /
    /// Overlay / Look Around DLLs and the leftovers of an update. The mods of the catalog (jibbers_*.dll) and anything else are never touched.
    /// </summary>
    static void RemoveOldVersions(string current, Action<string> log)
    {
        try
        {
            var folders = new[] { Path.GetDirectoryName(current)!, Bootstrap.ModsDir, Path.Combine(Bootstrap.ModsDir, "cs") }.Where(Directory.Exists).Distinct();
            var utf16 = PackIds.Select(id => System.Text.Encoding.Unicode.GetBytes(id)).ToArray();
            foreach (var dir in folders)
                foreach (var file in Directory.EnumerateFiles(dir, "*.dll"))
                {
                    if (string.Equals(Path.GetFullPath(file), Path.GetFullPath(current), StringComparison.OrdinalIgnoreCase)) continue;
                    var name = Path.GetFileName(file);
                    if (name.StartsWith("jibbers_", StringComparison.OrdinalIgnoreCase) || name.Equals("version.dll", StringComparison.OrdinalIgnoreCase)) continue;
                    string? why = null;
                    try { if (AssemblyName.GetAssemblyName(file).Name == "HankModPack") why = "an older Hank Mod Pack"; } catch { continue; }   // not a .NET DLL: not ours
                    if (why is null)
                    {
                        var bytes = File.ReadAllBytes(file);
                        if (utf16.Any(id => IndexOf(bytes, id) >= 0)) why = "a standalone version of a mod that is in the pack";
                    }
                    if (why is null) continue;
                    try { File.Delete(file); log($"updates: removed {file} ({why})."); }
                    catch (Exception e) { log($"updates: could not remove {file}: {e.Message}"); }
                }
            foreach (var dir in folders)
                foreach (var left in Directory.EnumerateFiles(dir, "HankModPack*.dll.new").Concat(Directory.EnumerateFiles(dir, "HankModPack*.dll.old")))
                    try { if (!left.Equals(current + ".old", StringComparison.OrdinalIgnoreCase)) File.Delete(left); } catch { }
        }
        catch (Exception e) { log("updates: could not look for old versions: " + e.Message); }
    }

    static int IndexOf(byte[] hay, byte[] needle)
    {
        for (var i = 0; i + needle.Length <= hay.Length; i++)
        {
            var k = 0;
            while (k < needle.Length && hay[i + k] == needle[k]) k++;
            if (k == needle.Length) return i;
        }
        return -1;
    }

    /// <summary>The hash git gives a file ("blob &lt;size&gt;\0&lt;bytes&gt;"): GitHub reports it, so the download can be checked against it.</summary>
    internal static string GitBlobSha(byte[] data)
    {
        using var sha = SHA1.Create();
        var head = Encoding.ASCII.GetBytes("blob " + data.Length + "\0");
        sha.TransformBlock(head, 0, head.Length, null, 0);
        sha.TransformFinalBlock(data, 0, data.Length);
        return Convert.ToHexString(sha.Hash!).ToLowerInvariant();
    }

    static async Task Run(bool install)
    {
        lock (gate) { if (busy) return; busy = true; }
        var log = logger ?? (_ => { });
        try
        {
            if (current is null) { State = "Failed"; return; }
            if (pendingSha is null || pendingUrl is null || !install) await Look(log);
            if (State != "Available" || !install) return;
            await Install(log);
        }
        catch (Exception e)
        {
            State = "Failed"; log("updates: " + e);
            var why = e is HttpRequestException { StatusCode: { } code } ? $"GitHub answered {(int)code}" + (code == System.Net.HttpStatusCode.NotFound ? ": repository or update/HankModPack.dll not found" : "") : e is TaskCanceledException ? "GitHub did not answer in time" : e.GetType().Name + ": " + e.Message;
            Toast.Show("Hank Mod Pack update failed\n" + why + "\n(details in jibbers_hotload.log, lines \"updates:\")", 9f);
        }
        finally { lock (gate) busy = false; }
    }

    /// <summary>The details of the update: <c>update/whatsnew.txt</c> of the repository (a few short lines).</summary>
    static async Task<string?> Notes(HttpClient http, Action<string> log)
    {
        try
        {
            // Through the same GitHub API that finds the DLL (no cached copy): the file's content comes base64-encoded.
            var meta = JsonNode.Parse(await http.GetStringAsync($"https://api.github.com/repos/{Repo}/contents/update/whatsnew.txt?ref={Branch}")) as JsonObject;
            var b64 = meta?["content"]?.GetValue<string>();
            if (b64 is null) { log("updates: whatsnew.txt has no content."); return null; }
            var text = Encoding.UTF8.GetString(Convert.FromBase64String(b64.Replace("\n", "").Replace("\r", ""))).TrimStart('\uFEFF');
            var notes = string.Join("\n", text.Replace("\r", "").Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).Take(10).Select(l => l.Length > 90 ? l[..90] + "..." : l));
            log($"updates: details read ({notes.Split('\n').Length} lines).");
            return notes;
        }
        catch (Exception e) { log("updates: no details (update/whatsnew.txt): " + e.Message); return null; }
    }

    /// <summary>After an update was installed: its details, for a few seconds, at the first launch with the new version.</summary>
    static void ShowWhatsNew(Action<string> log)
    {
        try
        {
            var file = Path.Combine(PackFiles.Dir, "whatsnew.txt");
            if (!File.Exists(file)) return;
            var notes = File.ReadAllText(file).Trim();
            File.Delete(file);
            Toast.Show("Hank Mod Pack updated - what's new" + (notes.Length > 0 ? "\n" + notes : ""), 9f);
            log("updates: showed the details of the update.");
        }
        catch (Exception e) { log("updates: " + e.Message); }
    }

    static HttpClient NewClient()
    {
        var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        http.DefaultRequestHeaders.UserAgent.ParseAdd("HankModPack-updater");
        http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        return http;
    }

    /// <summary>Asks GitHub which version of the file is there (no download): "Available" when it is not the running one.</summary>
    static async Task Look(Action<string> log)
    {
        State = "Checking";
        using var http = NewClient();
        var api = $"https://api.github.com/repos/{Repo}/contents/{FilePath}?ref={Branch}";
        log("updates: asking " + api);
        var meta = JsonNode.Parse(await http.GetStringAsync(api)) as JsonObject;
        var remoteSha = meta?["sha"]?.GetValue<string>();
        var url = meta?["download_url"]?.GetValue<string>();
        if (remoteSha is null || url is null || !url.StartsWith("https://", StringComparison.Ordinal)) { State = "Failed"; log("updates: GitHub gave no file."); return; }
        if (GitBlobSha(File.ReadAllBytes(current!)) == remoteSha) { State = "UpToDate"; log("updates: Hank Mod Pack is up to date."); return; }
        pendingSha = remoteSha; pendingUrl = url; State = "Available";
        pendingNotes = await Notes(http, log);
        log("updates: a new Hank Mod Pack is on GitHub.");
        if (Mode != "auto")
        {
            Toast.Show("Hank Mod Pack: a new version is available\n" + (pendingNotes is { Length: > 0 } ? pendingNotes + "\n" : "") + "Triangle or Alt Gr to install", 25f);
            Watch();
        }
    }

    static async Task Install(Action<string> log)
    {
        State = "Installing";
        using var http = NewClient();
        var data = await http.GetByteArrayAsync(pendingUrl!);
        if (GitBlobSha(data) != pendingSha) { State = "Failed"; log("updates: the download does not match GitHub's hash, dropped."); return; }
        if (data.Length < 1024 || data[0] != (byte)'M' || data[1] != (byte)'Z') { State = "Failed"; log("updates: the file is not a DLL, dropped."); return; }

        var tmp = current + ".new";
        File.WriteAllBytes(tmp, data);
        try
        {
            if (AssemblyName.GetAssemblyName(tmp).Name != "HankModPack") { State = "Failed"; log("updates: the file is not Hank Mod Pack, dropped."); File.Delete(tmp); return; }
            var old = current + ".old";
            if (File.Exists(old)) File.Delete(old);
            File.Move(current!, old);          // allowed while the game runs: the loaded DLL keeps working
            File.Move(tmp, current!);
            State = "Installed";
            log($"updates: installed the new Hank Mod Pack ({AssemblyName.GetAssemblyName(current!).Version}); it is used from the next launch.");
            try { File.WriteAllText(Path.Combine(PackFiles.Dir, "whatsnew.txt"), pendingNotes ?? ""); } catch { }
            Toast.Show("Hank Mod Pack updated\n" + (pendingNotes is { Length: > 0 } ? pendingNotes + "\n" : "") + "Restart the game to use it", 8f);
        }
        catch
        {
            State = "Failed";
            try { if (!File.Exists(current!) && File.Exists(current + ".old")) File.Move(current + ".old", current!); } catch { }
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { }
            throw;
        }
    }
}
