# Changelog

## 1.52.8
- Replay camera: diagnostic `TS probe` log lines (the game's time scale, Recorder values) to find how the game plays and exports a keyframed time scale.

## 1.52.7
- Replay camera: the Time Scale keyed in the game's keyframes is applied while a mod camera (POV / Follow cam / Drone) is active (game clock only, not in the video export yet).

## 1.52.6
- Replay camera: FOV diagnostic log lines at each loop (the FOV drift turned out to come from a controller).

## 1.52.5
- Effects window: **Learn the body axes** button in the POV tab.

## 1.52.4
- Performance: per-frame reflection cached in the key assignment window (suspected cause of a crash while it was open).

## 1.52
- HUD: real textures for Steel plate, Wood, Wool, Winter and Summer, on the speed and spawn windows, the Quick Menu, the key window and the effects window.
- Quick Menu: only the assigned actions are shown, ordered by button, smaller; theme rows last; direct camera shortcuts (Cross = POV, Circle = Follow cam, press again = off) with four new assignable actions.
- D-pad down + L3 opens the button assignment window (visible and working); crash fixes while it is open.
- D-pad up: a short press always opens the game's drone view, a long press leaves the play camera without opening it.
- Replay effects window: bottom-right hint, game tab order, D-pad left / right on choices with arrows, automatic keyframes, R3 duplicate, L3 duplicate all, "more" for the R2 hold, background from the HUD theme.
- Effect settings reset when another replay opens.
