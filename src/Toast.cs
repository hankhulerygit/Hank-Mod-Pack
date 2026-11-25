using Jibbers.Sdk;
using Jibbers.Sdk.Interop;
using RickBackflip;

namespace HankModPack;

/// <summary>A short message at the top of the screen, in the game's own font (used for "loaded" and "update available"). Safe to call from any thread.</summary>
internal static class Toast
{
    static OwnedGameObject? root;
    static int serial;

    /// <summary>Always queued from a worker thread: building the message from inside a game callback (the quick menu's own update) crashed the game.</summary>
    internal static void Show(string message, float seconds = 5f, int tries = 6, bool good = false)
        => Task.Run(() => MainThread.Run(() => Build(message, seconds, tries, good)));

    /// <summary>good = a small green confirmation (for example "up to date"), otherwise the usual message with a red first line.</summary>
    static void Build(string message, float seconds, int tries, bool good)
    {
        {
            try
            {
                var font = HankFonts.Get();
                if (font == 0)
                {
                    // The game's fonts are not there yet (still loading): try again a few seconds later.
                    if (tries > 0) Task.Delay(4000).ContinueWith(_ => Show(message, seconds, tries - 1, good));
                    return;
                }
                Hide();
                var my = ++serial;
                root = OverlayCanvas.BuildRoot("HankModPack.Toast", 30500);
                var lines = message.Split('\n').Length;
                // A last line asking to install ("... to install"): drawn with the button pictures (Triangle / Y, Alt Gr) instead of words.
                var action = !good && lines > 1 && message.Split('\n')[^1].Contains("to install");
                var textLines = action ? lines - 1 : lines;
                var panelH = good ? 48f : textLines <= 1 ? 70f : 44f + 44f + Math.Max(0, textLines - 2) * 28f + 20f;
                if (action) panelH += 56f;
                var width = good ? 460f : 720f;
                // On the right of the screen, clear of the game menu: placed from the left edge with the real canvas width (the canvas rectangle itself can be stale at launch).
                var canvasW = 1920f;
                try { canvasW = ScreenLayout.CanvasSize().Width; } catch { }
                var left = Math.Max(20f, canvasW - width - 40f);
                var top = new Vector2(0f, 1f);   // from the top-left corner: the panel stays clear of the game menu and does not depend on the canvas width
                using var child = OwnedGameObject.Create("Text");
                var tmp = child.AddComponent("TMPro.TextMeshProUGUI");
                Unity.Call(tmp, "set_font", font);
                UnityProperties.Set(tmp, "set_raycastTarget", false);
                UnityProperties.Set(tmp, "set_richText", true);
                UnityProperties.Set(tmp, "set_alignment", Unity.Class("TMPro.TextAlignmentOptions").Field("Center").Get<int>(0));
                UnityProperties.Set(tmp, "set_fontSize", good ? 22f : 24f);
                try { UnityProperties.Set(tmp, "set_color", new Color(1, 1, 1, 1)); } catch { }
                try { UnityProperties.Set(tmp, "set_outlineWidth", 0.25f); UnityProperties.Set(tmp, "set_outlineColor", new Color32(10, 10, 24, 255)); } catch { }
                // First line: bigger and red. Text from outside (the update details) cannot carry tags.
                var clean = new string(message.Where(c => !char.IsControl(c) || c == '\n').ToArray()).Replace('<', '(').Replace('>', ')');
                if (action) clean = clean[..clean.LastIndexOf('\n')];
                // First line: bigger and red. Middle lines (the details): smaller. A last line that asks for something ("... to install", "Restart ..."): big and coloured.
                var parts = clean.Split('\n');
                string rich;
                if (good) rich = "<color=#5BE38C>" + clean + "</color>";
                else if (parts.Length == 1) rich = "<size=130%><color=#FF4040><b>" + clean + "</b></color></size>";
                else
                {
                    var sb = new System.Text.StringBuilder("<size=130%><color=#FF4040><b>" + parts[0] + "</b></color></size>");
                    for (var i = 1; i < parts.Length; i++)
                    {
                        var last = i == parts.Length - 1 && (parts[i].Contains("to install") || parts[i].StartsWith("Restart"));
                        sb.Append('\n').Append(last ? "<size=135%><color=#FFD23F><b>" + parts[i] + "</b></color></size>" : "<size=78%>" + parts[i] + "</size>");
                    }
                    rich = sb.ToString();
                }
                Unity.Call(tmp, "set_text", Il2Cpp.String(rich));
                var rect = Unity.Call(tmp, "get_rectTransform");
                UnityProperties.Set(rect, "set_anchorMin", top);
                UnityProperties.Set(rect, "set_anchorMax", top);
                UnityProperties.Set(rect, "set_pivot", top);
                UnityProperties.Set(rect, "set_anchoredPosition", new Vector2(left + 15f, -46));
                UnityProperties.Set(rect, "set_sizeDelta", new Vector2(width - 30f, 1000f));
                // The panel follows the real text height, so every window font fits (wide fonts wrap onto more lines).
                try
                {
                    var measured = Unity.Call<float>(tmp, "get_preferredHeight");
                    if (measured > 1f) panelH = Math.Max(good ? 48f : 70f, 16f + measured + 16f + (action ? 56f : 0f));
                }
                catch { }
                UnityProperties.Set(rect, "set_sizeDelta", new Vector2(width - 30f, panelH - (good ? 20f : 30f) - (action ? 56f : 0f)));
                using (var bg = OwnedGameObject.Create("Panel"))
                {
                    var img = bg.AddComponent("UnityEngine.UI.Image");
                    UnityProperties.Set(img, "set_raycastTarget", false);
                    try { UnityProperties.Set(img, "set_color", new Color(0.03f, 0.03f, 0.06f, 0.95f)); } catch { }
                    var r = Unity.Call(img, "get_rectTransform");
                    UnityProperties.Set(r, "set_anchorMin", top); UnityProperties.Set(r, "set_anchorMax", top); UnityProperties.Set(r, "set_pivot", top);
                    UnityProperties.Set(r, "set_anchoredPosition", new Vector2(left, -30));
                    UnityProperties.Set(r, "set_sizeDelta", new Vector2(width, panelH));
                    bg.AttachTo(root.Transform);
                }
                child.AttachTo(root.Transform);
                if (action) ActionRow(font, left, width, 30f + panelH - 58f);
                Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(_ => MainThread.Run(() => { if (serial == my) Hide(); }));
            }
            catch { Hide(); }
        }
    }

    static readonly List<nint> textures = new();

    /// <summary>"[Triangle] / [Y] or [Alt Gr] to install", centred in the panel; the row starts rowY below the top of the canvas.</summary>
    static void ActionRow(nint font, float left, float width, float rowY)
    {
        const float pic = 72f, picH = 36f, rowH = 44f;
        var total = pic + 22f + pic + 38f + 108f + 150f + 5 * 8f;
        var x = left + (width - total) / 2f;
        var py = rowY + (rowH - picH) / 2f;

        Picture(Tex(ButtonArt.Render(PadBtn.Y, PadStyle.PlayStation, out var w1, out var h1), w1, h1), x, py, pic, picH); x += pic + 8f;
        Words(font, "/", 32f, "#FFFFFF", x, rowY, 22f, rowH); x += 22f + 8f;
        Picture(Tex(ButtonArt.Render(PadBtn.Y, PadStyle.Xbox, out var w2, out var h2), w2, h2), x, py, pic, picH);
        var (yl, yc) = ButtonArt.Label(PadBtn.Y, PadStyle.Xbox);
        Words(font, $"<b>{yl}</b>", 26f, $"#{(int)(yc.R * 255):X2}{(int)(yc.G * 255):X2}{(int)(yc.B * 255):X2}", x, rowY, pic, rowH); x += pic + 8f;
        Words(font, "or", 28f, "#C8D0E0", x, rowY, 38f, rowH); x += 38f + 8f;
        var capW = 100f; var capH = 38f;
        Picture(Tex(ButtonArt.RenderRounded((int)capW * 2, (int)capH * 2, 14f, new ButtonArt.Rgba(0.17f, 0.19f, 0.25f), new ButtonArt.Rgba(0.75f, 0.8f, 0.9f), 3f), (int)capW * 2, (int)capH * 2),
            x, rowY + (rowH - capH) / 2f, capW, capH);
        Words(font, "<b>Alt Gr</b>", 23f, "#FFFFFF", x, rowY, capW, rowH); x += capW + 8f + 8f;
        Words(font, "<b>to install</b>", 30f, "#FFD23F", x, rowY, 150f, rowH);
    }

    static nint Tex(byte[] rgba, int w, int h)
    {
        var t = HudArt.MakeTexture(rgba, w, h);
        textures.Add(t);
        return t;
    }

    static void Picture(nint texture, float x, float y, float w, float h)
    {
        using var go = OwnedGameObject.Create("Image");
        var raw = go.AddComponent("UnityEngine.UI.RawImage");
        UnityProperties.Set(raw, "set_raycastTarget", false);
        Unity.Call(raw, "set_texture", texture);
        Place(Unity.Call(raw, "get_rectTransform"), x, y, w, h);
        go.AttachTo(root!.Transform);
    }

    static void Words(nint font, string rich, float size, string hex, float x, float y, float w, float h)
    {
        using var go = OwnedGameObject.Create("Words");
        var tmp = go.AddComponent("TMPro.TextMeshProUGUI");
        Unity.Call(tmp, "set_font", font);
        UnityProperties.Set(tmp, "set_raycastTarget", false);
        UnityProperties.Set(tmp, "set_richText", true);
        UnityProperties.Set(tmp, "set_alignment", Unity.Class("TMPro.TextAlignmentOptions").Field("Center").Get<int>(0));
        UnityProperties.Set(tmp, "set_fontSize", size);
        try { UnityProperties.Set(tmp, "set_outlineWidth", 0.2f); UnityProperties.Set(tmp, "set_outlineColor", new Color32(10, 10, 24, 255)); } catch { }
        Unity.Call(tmp, "set_text", Il2Cpp.String($"<color={hex}>{rich}</color>"));
        Place(Unity.Call(tmp, "get_rectTransform"), x, y, w, h);
        go.AttachTo(root!.Transform);
    }

    /// <summary>Placed from the top-left corner of the screen canvas (y counts down).</summary>
    static void Place(nint rect, float x, float y, float w, float h)
    {
        var top = new Vector2(0f, 1f);
        UnityProperties.Set(rect, "set_anchorMin", top); UnityProperties.Set(rect, "set_anchorMax", top); UnityProperties.Set(rect, "set_pivot", top);
        UnityProperties.Set(rect, "set_anchoredPosition", new Vector2(x, -y));
        UnityProperties.Set(rect, "set_sizeDelta", new Vector2(w, h));
    }

    /// <summary>Removes the message now (a menu was opened).</summary>
    internal static void Dismiss() => Task.Run(() => MainThread.Run(Hide));

    static void Hide()
    {
        try { root?.Dispose(); } catch { }
        root = null;
        foreach (var t in textures) { try { Unity.Class("UnityEngine.Object").Method("Destroy", 1).Invoke(0, t); } catch { } }
        textures.Clear();
    }
}
