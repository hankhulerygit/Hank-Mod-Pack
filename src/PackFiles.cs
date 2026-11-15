using Jibbers.Sdk;

namespace HankModPack;

/// <summary>
/// Where the pack keeps its own files: everything is in <c>JibbersMods/Hank Mod Pack</c>, nothing loose in <c>JibbersMods</c>.
/// A file an earlier version left in <c>JibbersMods</c> is moved there the first time it is asked for. The one-time markers
/// (the old <c>*.done</c> files) are folded into a single <c>state.txt</c> and the loose marker files are deleted.
/// </summary>
internal static class PackFiles
{
    internal static string Dir => System.IO.Path.Combine(Bootstrap.ModsDir, "Hank Mod Pack");
    static readonly object gate = new();
    static HashSet<string>? done;

    /// <summary>The path of a pack file; a copy left in the old place is moved (or deleted when the new place has one already).</summary>
    internal static string Of(string name)
    {
        var dst = System.IO.Path.Combine(Dir, name);
        try
        {
            Directory.CreateDirectory(Dir);
            var old = System.IO.Path.Combine(Bootstrap.ModsDir, name);
            if (File.Exists(old))
            {
                if (File.Exists(dst)) File.Delete(old); else File.Move(old, dst);
            }
        }
        catch { /* the file stays where it is: it is read from the new place or created there */ }
        return dst;
    }

    /// <summary>At load: moves every pack file an earlier version left loose in JibbersMods, and folds the old *.done markers into state.txt.</summary>
    internal static void Tidy()
    {
        try
        {
            foreach (var n in new[] { "POV rig.json", "Camera Presets.json", "quick_menu_keyboard.json", "quick_menu_favorites.json", "quick_menu_sets.json" }) Of(n);
            foreach (var f in Directory.GetFiles(Bootstrap.ModsDir, "*.done"))
            {
                var n = System.IO.Path.GetFileName(f);
                if (n.StartsWith("quick_menu_") || n.StartsWith("quick_actions_")) Done(n);
            }
        }
        catch { }
    }

    static string StateFile => System.IO.Path.Combine(Dir, "state.txt");

    static HashSet<string> Load()
    {
        if (done is not null) return done;
        done = new HashSet<string>();
        try { if (File.Exists(StateFile)) foreach (var l in File.ReadAllLines(StateFile)) if (l.Trim() is { Length: > 0 } t) done.Add(t); } catch { }
        return done;
    }

    static void Save()
    {
        try { Directory.CreateDirectory(Dir); File.WriteAllLines(StateFile, done!.OrderBy(x => x)); } catch { }
    }

    /// <summary>Whether a one-time step (named like its old marker file, "quick_menu_layout7.done") has been done already.</summary>
    internal static bool Done(string marker)
    {
        lock (gate)
        {
            var set = Load();
            try
            {
                var legacy = System.IO.Path.Combine(Bootstrap.ModsDir, marker);
                if (File.Exists(legacy)) { set.Add(marker); File.Delete(legacy); Save(); }
            }
            catch { }
            return set.Contains(marker);
        }
    }

    internal static void MarkDone(string marker)
    {
        lock (gate) { if (Load().Add(marker)) Save(); }
    }
}
