# Hank Mod Pack guide

Hank Mod Pack is one DLL with eleven entries in Options > Mods, all named `Hank Mod Pack: ...`.
This guide covers how to use it. See `mods/cs/HankModPack/CHANGELOG.md` for changes.

## Quick menu

Hold **D-pad down**. The default buttons (all changeable in Settings > Binds & buttons):

| Button | Action |
|---|---|
| Cross | Follow cam |
| Circle | Quick Actions |
| Square | Spawn menu |
| Triangle | Cameras |
| R1 / R2 | POV camera / Drone camera |
| L1 | Input visualizer on / off |
| L2 | Themes (opens the theme window) |
| L3 | Detach both skis |
| Share | Mods |
| Options | Settings / Binds / Themes |
| R3 | Look behind (free look) |

Releasing D-pad down closes the menu unless a window was opened.

## Quick Actions

Three sets of your own shortcuts. Open **Quick Actions**, pick a set and press a pad button to run the command given to that button.
In the set list: **Y** edits the set's buttons, **X** renames the set, **A** opens it; Share / Options / Esc close. While editing, choose a
button, then a command from the tabs (Favorites, Commands, Chat, Cameras). X clears a button, B goes back. In an open set, L1 / R1 change set.

## Settings / Binds / Themes

- **Camera settings**: tune the POV, Follow cam and Drone cameras live, with presets.
- **Binds & buttons**: choose the pad button or keyboard key for every quick-menu action, and bind game console commands to keyboard
  keys (A assign, X run now, Y favorite). A key already in use asks what to do (replace, pick another, keep).
- **Theme**: change the HUD theme with L1 / R1 (also reachable from the Themes button).

## Input Visualizer

Replaces the game's F1 input display with a drawn PS4 or Xbox One controller (keyboard and mouse when no pad is connected).
Colours: White, Black, HUD theme, or a named theme; every theme is only the controller in its colours.
Toggle it from the quick menu, or let it follow the game's F1 display. In a replay it follows the replay's Input Visualizer button; the
replay editor has an Input Visualizer tab (theme, size, position) whose values apply until the replay ends. The controller shows the
replayed rider's inputs there.

## Improved on-foot camera

After the skis are detached the left stick walks and the right stick turns the camera, which stays where it is left. L1 / R1 lock it,
L3 puts it back behind the rider (the game does not receive that click), R3 keeps its game action. The setting is "Improved on-foot camera"
(Off / Auto after detaching / Always).

## Replay editor

R2 opens the effects window while a keyframe panel is open (L2 held = bigger steps). Sony pads get PlayStation button pictures.

## Settings files

`JibbersMods/Hank Mod Pack/Settings/<mod name>.json`, one per mod (for example `Quick Menu.json`, `HUD.json`). Files from earlier
versions in `JibbersMods/<id>.settings.json` are imported and deleted on first launch. Options > Mods lists only the settings with no other
place; the others are still saved in these files.

## Troubleshooting

- Input Visualizer does not replace the game's display in a replay: check `jibbers_hotload.log` for lines starting with `Controller overlay:`.
- Something does not react in the quick menu: the action may have no button (Settings > Binds & buttons).
