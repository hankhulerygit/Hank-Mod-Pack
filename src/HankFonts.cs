using Jibbers.Sdk;
using Jibbers.Sdk.Interop;
using RickBackflip;

namespace HankModPack;

/// <summary>
/// The font of the pack's windows (quick menu, effects window, HUD, messages). Fonts that travel inside the mod's DLL
/// (the game's own Lacquer, Roboto, Roboto Bold, Liberation Sans, Inconsolata, Geologica, Perfect DOS and three of the Mod Tool's: Bangers, Orbitron,
/// Press Start 2P). They are made into text fonts when the mod starts, one per frame, so they do not depend on what the game has loaded.
/// "Game default" keeps what the pack used before; a font that could not be made falls back to it. Main thread only.
/// </summary>
internal static class HankFonts
{
    public const string Default = "Game default";
    public static readonly string[] Names = { "Game default", "Lacquer", "Roboto", "Roboto Bold", "Liberation Sans", "Inconsolata", "Geologica", "Perfect DOS", "Audiowide", "Bangers", "Black Ops One", "Bowlby One SC", "Bungee", "Bungee Shade", "Chango", "Creepster", "Faster One", "Fredericka the Great", "Gloria Hallelujah", "Kaushan Script", "Knewave", "Lilita One", "Lobster", "Londrina Solid", "Monoton", "Nosifer", "Orbitron", "Pacifico", "Paytone One", "Permanent Marker", "Press Start 2P", "Righteous", "Rock Salt", "Rubik Glitch", "Rubik Spray Paint", "Rubik Wet Paint", "Russo One", "Sedgwick Ave Display", "Shrikhand", "Special Elite", "Squada One", "Titan One", "Ultra", "Arial Black", "Impact", "Georgia", "Courier New", "Trebuchet MS", "Verdana" };

    // The font file inside the DLL (Fonts\<file>.ttf) of each font: the game's seven, then the Mod Tool's.
    static readonly Dictionary<string, string> Files = new()
    {
        ["Lacquer"] = "Lacquer", ["Roboto"] = "Roboto", ["Roboto Bold"] = "RobotoBold", ["Liberation Sans"] = "LiberationSans", ["Inconsolata"] = "Inconsolata", ["Geologica"] = "Geologica", ["Perfect DOS"] = "PerfectDOS", ["Audiowide"] = "Audiowide", ["Bangers"] = "Bangers", ["Black Ops One"] = "BlackOpsOne", ["Bowlby One SC"] = "BowlbyOneSC", ["Bungee"] = "Bungee", ["Bungee Shade"] = "BungeeShade", ["Chango"] = "Chango", ["Creepster"] = "Creepster", ["Faster One"] = "FasterOne", ["Fredericka the Great"] = "FrederickatheGreat", ["Gloria Hallelujah"] = "GloriaHallelujah", ["Kaushan Script"] = "KaushanScript", ["Knewave"] = "Knewave", ["Lilita One"] = "LilitaOne", ["Lobster"] = "Lobster", ["Londrina Solid"] = "LondrinaSolid", ["Monoton"] = "Monoton", ["Nosifer"] = "Nosifer", ["Orbitron"] = "Orbitron", ["Pacifico"] = "Pacifico", ["Paytone One"] = "PaytoneOne", ["Permanent Marker"] = "PermanentMarker", ["Press Start 2P"] = "PressStart2P", ["Righteous"] = "Righteous", ["Rock Salt"] = "RockSalt", ["Rubik Glitch"] = "RubikGlitch", ["Rubik Spray Paint"] = "RubikSprayPaint", ["Rubik Wet Paint"] = "RubikWetPaint", ["Russo One"] = "RussoOne", ["Sedgwick Ave Display"] = "SedgwickAveDisplay", ["Shrikhand"] = "Shrikhand", ["Special Elite"] = "SpecialElite", ["Squada One"] = "SquadaOne", ["Titan One"] = "TitanOne", ["Ultra"] = "Ultra",
    };

    // Windows' own fonts (C:\Windows\Fonts), read from there when they exist: nothing of them is inside the DLL.
    static readonly Dictionary<string, string> SystemFiles = new()
    {
        ["Arial Black"] = "ariblk.ttf", ["Impact"] = "impact.ttf", ["Georgia"] = "georgia.ttf", ["Courier New"] = "cour.ttf", ["Trebuchet MS"] = "trebuc.ttf", ["Verdana"] = "verdana.ttf",
    };

    // Words of the game's font name (letters and digits only, lower case) that make a font this one, and words that rule it out.
    static readonly Dictionary<string, (string[] Keys, string[] Not)> Match = new()
    {
        ["Lacquer"] = (new[] { "lacquer" }, Array.Empty<string>()),
        ["Roboto"] = (new[] { "robotoregular", "roboto" }, new[] { "bold", "black", "light", "medium", "thin", "condensed" }),
        ["Roboto Bold"] = (new[] { "robotobold" }, Array.Empty<string>()),
        ["Liberation Sans"] = (new[] { "liberationsans" }, Array.Empty<string>()),
        ["Inconsolata"] = (new[] { "inconsolata" }, Array.Empty<string>()),
        ["Geologica"] = (new[] { "geologica" }, Array.Empty<string>()),
        ["Perfect DOS"] = (new[] { "perfectdos" }, Array.Empty<string>()),
        ["Bangers"] = (new[] { "bangers" }, Array.Empty<string>()),
        ["Orbitron"] = (new[] { "orbitron" }, Array.Empty<string>()),
        ["Press Start 2P"] = (new[] { "pressstart" }, Array.Empty<string>()),
    };

    static readonly Dictionary<string, GameRef> cache = new();
    static readonly HashSet<string> failed = new();
    static bool listed;

    /// <summary>The chosen font name (the Window font setting of the HUD mod).</summary>
    public static string Current
    {
        get
        {
            var set = HankHudMod.Instance?.FontName is { Length: > 0 } n && Array.IndexOf(Names, n) >= 0 ? n : Default;
            // A font just picked counts at once, before the setting has been saved and read back.
            if (pending is not null) { if (pending == set || Environment.TickCount64 - pendingAt > 3000) pending = null; else return pending; }
            return set;
        }
    }
    static string? pending; static long pendingAt;
    public static void Pick(string name) { pending = name; pendingAt = Environment.TickCount64; }

    public static string Neighbour(int step)
    {
        var i = Math.Max(0, Array.IndexOf(Names, Current));
        return Names[((i + step) % Names.Length + Names.Length) % Names.Length];
    }

    /// <summary>The font to give a text: the chosen one, else the game's usual font (0 while the game has none yet).</summary>
    public static nint Get()
    {
        var name = Current;
        if (name == Default || failed.Contains(name)) return TextBanner.FindFont();
        try
        {
            if (cache.TryGetValue(name, out var held) && !Unity.IsNull(held.Target)) return held.Target;
            var made = Make(name);
            if (made != 0) { cache[name] = new GameRef(made); return made; }
            failed.Add(name);
            Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{name}' is not loaded in the game right now; the game's font is used instead");
        }
        catch (Exception e)
        {
            failed.Add(name);
            Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{name}' could not be made ({e.Message}); the game's font is used instead");
        }
        return TextBanner.FindFont();
    }

    static string Norm(string s) => new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

    static bool Is(string name, string key)
    {
        var n = Norm(name);
        if (!Match.TryGetValue(key, out var spec)) return false;
        var (keys, not) = spec;
        return keys.Any(k => n.Contains(k)) && !not.Any(x => n.Contains(x));
    }

    /// <summary>Makes the next font that is not ready yet: called once per frame (from the HUD mod's frame) until all of them are.</summary>
    public static void Tick()
    {
        if (allDone) return;
        if (TextBanner.FindFont() == 0) return;   // the game's text system is not ready yet
        foreach (var n in Names)
        {
            if (n == Default || cache.ContainsKey(n) || failed.Contains(n)) continue;
            try
            {
                var made = Make(n);
                if (made != 0) cache[n] = new GameRef(made); else failed.Add(n);
            }
            catch (Exception e) { failed.Add(n); Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{n}' could not be made ({e.Message})"); }
            return;   // one per frame
        }
        allDone = true;
        Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: ready: {string.Join(", ", cache.Keys)}" + (failed.Count > 0 ? $"; not available: {string.Join(", ", failed)}" : ""));
    }
    static bool allDone;

    /// <summary>The font file of the DLL, written once next to the pack's files (Unity reads a font from a path).</summary>
    static string? FontPath(string key)
    {
        if (SystemFiles.TryGetValue(key, out var sys))
        {
            var p = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), sys);
            return System.IO.File.Exists(p) ? p : null;
        }
        if (!Files.TryGetValue(key, out var file)) return null;
        var res = "HankModPack.Fonts." + file + ".ttf";
        using var stream = typeof(HankFonts).Assembly.GetManifestResourceStream(res);
        if (stream is null) return null;
        var dir = System.IO.Path.Combine(PackFiles.Dir, "Fonts");
        System.IO.Directory.CreateDirectory(dir);
        var path = System.IO.Path.Combine(dir, file + ".ttf");
        if (!System.IO.File.Exists(path) || new System.IO.FileInfo(path).Length != stream.Length)
        {
            using var output = System.IO.File.Create(path);
            stream.CopyTo(output);
        }
        return path;
    }

    static unsafe nint Make(string key)
    {
        var assets = Unity.Class("TMPro.TMP_FontAsset");
        var found = new List<string>();
        // 0. The font file carried by the DLL: a text font asset made from the file path, or from a Unity font made from it.
        try
        {
            var path = FontPath(key);
            if (path is null) Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}': the font file is not inside the DLL");
            else
            {
                if (!signaturesLogged)
                {
                    signaturesLogged = true;
                    foreach (var m in assets.Methods.Where(m => m.Name == "CreateFontAsset"))
                        Jibbers.Sdk.Log.Info("hank_mod_pack", "font: TMP_FontAsset.CreateFontAsset(" + string.Join(", ", m.ParameterTypes.Zip(m.ParameterNames, (t, n) => t + " " + n)) + ")");
                    foreach (var m in Unity.Class("UnityEngine.Font").Methods.Where(m => m.Name == ".ctor"))
                        Jibbers.Sdk.Log.Info("hank_mod_pack", "font: Font.ctor(" + string.Join(", ", m.ParameterTypes) + ")");
                }
                var asset = MakeFromFile(assets, path);
                if (asset != 0 && !Unity.IsNull(asset))
                {
                    Keep(asset);
                    try
                    {
                        var game = TextBanner.FindFont();
                        var fallbacks = Unity.Call(asset, "get_fallbackFontAssetTable");
                        if (game != 0 && fallbacks != 0) Il2Cpp.ClassOf(fallbacks).Method("Add", 1).Invoke(fallbacks, game);
                    }
                    catch (Exception e) when (e is InvalidOperationException or MissingMethodException or Il2CppException) { }
                    Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}' loaded from the mod");
                    return asset;
                }
                Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}': no way to make a text font from the file worked");
            }
        }
        catch (Exception e) { Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}' from the mod's file failed ({e.Message}); trying the game's fonts"); }
        // 1. A text font asset the game already has with this name.
        foreach (var a in Unity.FindObjects(assets))
        {
            if (Unity.IsNull(a)) continue;
            var n = Unity.Name(a); found.Add("TMP:" + n);
            if (Is(n, key)) { Keep(a); Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}' = the game's text font '{n}'"); return a; }
        }
        // 2. A font the game has loaded, made into a text font asset.
        var fromFont = assets.Method("CreateFontAsset", "UnityEngine.Font");
        if (!fromFont.IsNull)
            foreach (var f in Unity.FindObjects(Unity.Class("UnityEngine.Font")))
            {
                if (Unity.IsNull(f)) continue;
                var n = Unity.Name(f); found.Add("Font:" + n);
                if (!Is(n, key)) continue;
                var asset = fromFont.Invoke(0, f);
                if (asset == 0 || Unity.IsNull(asset)) continue;
                Keep(asset);
                try
                {
                    var game = TextBanner.FindFont();
                    var fallbacks = Unity.Call(asset, "get_fallbackFontAssetTable");
                    if (game != 0 && fallbacks != 0) Il2Cpp.ClassOf(fallbacks).Method("Add", 1).Invoke(fallbacks, game);
                }
                catch (Exception e) when (e is InvalidOperationException or MissingMethodException or Il2CppException) { }
                Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: '{key}' = made from the game's font '{n}'");
                return asset;
            }
        if (!listed) { listed = true; Jibbers.Sdk.Log.Info("hank_mod_pack", "font: loaded in the game: " + string.Join(", ", found.Distinct())); }
        return 0;
    }

    static bool signaturesLogged;

    /// <summary>Calls the overloads of TMP_FontAsset.CreateFontAsset the game has, giving each parameter a value by its name and type.</summary>
    static unsafe nint MakeFromFile(Il2CppClass assets, string path)
    {
        var fontClass = Unity.Class("UnityEngine.Font");
        nint font = 0;
        foreach (var m in assets.Methods.Where(m => m.Name == "CreateFontAsset" && m.IsStatic).OrderBy(m => m.ParameterTypes.Length > 0 && m.ParameterTypes[0] == "System.String" ? 0 : 1))
        {
            var types = m.ParameterTypes; var names = m.ParameterNames;
            if (types.Length == 0) continue;
            var fromPath = types[0] == "System.String" && names[0].ToLowerInvariant().Contains("path");
            var fromFont = types[0] == "UnityEngine.Font";
            if (!fromPath && !fromFont) continue;
            try
            {
                if (fromFont && font == 0)
                {
                    var ctor = fontClass.Method(".ctor", "System.String");
                    if (ctor.IsNull) continue;
                    font = fontClass.New();
                    ctor.Invoke(font, Il2Cpp.String(path));
                    Keep(font);
                }
                var store = new long[types.Length];
                var args = new nint[types.Length];
                fixed (long* p = store)
                {
                    for (var a = 0; a < types.Length; a++)
                    {
                        var n = names[a].ToLowerInvariant();
                        if (types[a] == "System.String") { args[a] = a == 0 ? Il2Cpp.String(path) : Il2Cpp.String(""); continue; }
                        if (types[a] == "UnityEngine.Font") { args[a] = font; continue; }
                        long v = 0;
                        if (n.Contains("sampling")) v = 90;
                        else if (n.Contains("padding")) v = 9;
                        else if (n.Contains("width") || n.Contains("height")) v = 1024;
                        else if (n.Contains("rendermode")) v = 4165;        // SDFAA
                        else if (n.Contains("population")) v = 1;           // Dynamic
                        else if (n.Contains("multiatlas")) v = 1;
                        store[a] = v;
                        args[a] = (nint)(p + a);
                    }
                    var asset = m.Invoke(0, args);
                    if (asset != 0 && !Unity.IsNull(asset)) return asset;
                }
            }
            catch (Exception e) when (e is InvalidOperationException or MissingMethodException or Il2CppException or ArgumentException)
            {
                Jibbers.Sdk.Log.Info("hank_mod_pack", $"font: CreateFontAsset({string.Join(", ", types)}) failed: {e.Message}");
            }
        }
        return 0;
    }

    static void Keep(nint obj)
    {
        try { UnityProperties.Set(obj, "set_hideFlags", 32); } catch (MissingMethodException) { }
    }
}
