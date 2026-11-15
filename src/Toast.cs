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
                var font = TextBanner.FindFont();
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
                var panelH = good ? 48f : 38f + lines * 34f;
                var width = good ? 460f : 900f;
                var top = new Vector2(0f, 1f);   // from the top-left corner: the panel stays clear of the game menu and does not depend on the canvas width
                using (var bg = OwnedGameObject.Create("Panel"))
                {
                    var img = bg.AddComponent("UnityEngine.UI.Image");
                    UnityProperties.Set(img, "set_raycastTarget", false);
                    try { UnityProperties.Set(img, "set_color", new Color(0.03f, 0.03f, 0.06f, 0.95f)); } catch { }
                    var r = Unity.Call(img, "get_rectTransform");
                    UnityProperties.Set(r, "set_anchorMin", top); UnityProperties.Set(r, "set_anchorMax", top); UnityProperties.Set(r, "set_pivot", top);
                    UnityProperties.Set(r, "set_anchoredPosition", new Vector2(430, -30));
                    UnityProperties.Set(r, "set_sizeDelta", new Vector2(width, panelH));
                    bg.AttachTo(root.Transform);
                }
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
                var nl = clean.IndexOf('\n');
                var rich = good ? "<color=#5BE38C>" + clean + "</color>"
                         : nl < 0 ? "<size=130%><color=#FF4040><b>" + clean + "</b></color></size>"
                                  : "<size=130%><color=#FF4040><b>" + clean[..nl] + "</b></color></size>" + clean[nl..];
                Unity.Call(tmp, "set_text", Il2Cpp.String(rich));
                var rect = Unity.Call(tmp, "get_rectTransform");
                UnityProperties.Set(rect, "set_anchorMin", top);
                UnityProperties.Set(rect, "set_anchorMax", top);
                UnityProperties.Set(rect, "set_pivot", top);
                UnityProperties.Set(rect, "set_anchoredPosition", new Vector2(445, -46));
                UnityProperties.Set(rect, "set_sizeDelta", new Vector2(width - 30f, panelH - (good ? 20f : 38f)));
                child.AttachTo(root.Transform);
                Task.Delay(TimeSpan.FromSeconds(seconds)).ContinueWith(_ => MainThread.Run(() => { if (serial == my) Hide(); }));
            }
            catch { Hide(); }
        }
    }

    /// <summary>Removes the message now (a menu was opened).</summary>
    internal static void Dismiss() => Task.Run(() => MainThread.Run(Hide));

    static void Hide()
    {
        try { root?.Dispose(); } catch { }
        root = null;
    }
}
