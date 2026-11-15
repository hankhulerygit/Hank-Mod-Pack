namespace HankModPack;

/// <summary>
/// "The game is really playable": the HUD and the input visualizer wait for it, so they do not show over the loading screen.
/// The game builds its HUD a few seconds before the loading screen goes away, so being "in game" is not enough: it also has to
/// have been in game for a moment, with the frames running smoothly again (a loading screen makes long frames).
/// </summary>
internal static class LoadGate
{
    static long lastTick, inGameSince, lastHitch, lastCall;

    /// <summary>False while a map is loading. True when nobody feeds it (the HUD mod is off): then nothing is held back.</summary>
    internal static bool Ready { get; private set; } = true;

    /// <summary>A new map: everything waits again.</summary>
    internal static void Reset() { inGameSince = 0; lastHitch = Environment.TickCount64; Ready = false; }

    /// <summary>Called every frame by the HUD with whether the game's own HUD exists.</summary>
    internal static void Tick(bool inGame)
    {
        var now = Environment.TickCount64;
        if (now == lastTick) return;
        var dt = lastTick == 0 ? 0 : now - lastTick;
        lastTick = now; lastCall = now;
        if (dt > 250) lastHitch = now;
        if (!inGame) { inGameSince = 0; Ready = false; return; }
        if (inGameSince == 0) inGameSince = now;
        Ready = now - inGameSince >= 3000 && now - lastHitch >= 2000;
    }

    /// <summary>For code that does not feed the gate: the gate counts only while the HUD keeps ticking it.</summary>
    internal static bool Open => Environment.TickCount64 - lastCall > 1500 || Ready;
}
