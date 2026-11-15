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

One list with **every button of the pad**. Open **Quick Actions** from the quick menu, then press a button:

- a button with a command **runs it**;
- **hold** a button to change its command;
- an empty button ("not assigned") opens the command chooser (tabs: Favorites, Commands, Chat, Cameras);
- **X** clears a button in the chooser, **B** goes back.

Starting pattern (given once, only when Quick Actions is still empty; everything can be changed):

| Button | Command |
|---|---|
| Cross | detach both skis |
| Square | save map |
| Triangle | FPS menu |
| Circle | `/login bob` |
| L1 | Add Ghost |
| R1 | Ghost Menu |

The other buttons are left free. Your own assignments are never overwritten by an update.

## Settings / Binds / Themes

- **Camera settings**: tune the POV, Follow cam and Drone cameras live, with presets.
- **Binds & buttons**: choose the pad button or keyboard key for every quick-menu action, and bind game console commands to keyboard
  keys (A assign, X run now, Y favorite). A key already in use asks what to do (replace, pick another, keep).
- **Theme**: change the HUD theme with L1 / R1 (also reachable from the Themes button).

## Input Visualizer

Replaces the game's F1 input display with a drawn PS4 or Xbox One controller (keyboard and mouse when no pad is connected).
Colours: White, Black, HUD theme, or a named theme; every theme is only the controller in its colours.
Toggle it from the quick menu, or press F1 to follow the game's display. It stays as you left it when you pause, open the replay editor or
leave a replay. In a replay the editor has an Input Visualizer tab (theme, size, position) whose values apply until the replay ends, and the
controller shows the replayed rider's inputs. The HUD and the visualizer wait for the loading screen to end.

## Improved on-foot camera

After the skis are detached the left stick walks and the right stick turns the camera, which stays where it is left. L1 / R1 lock it,
L3 puts it back behind the rider, **R3 alone looks behind**. Respawning, resetting or performing a trick gives the camera back to the game.
The setting is "Improved on-foot camera" (Off / Auto after detaching / Always).

## Replay editor

R2 opens the effects window while a keyframe panel is open (L2 held = bigger steps). Sony pads get PlayStation button pictures.
The POV camera hides the rider's head (re-checked while it plays).

## Updates from GitHub

At launch the pack checks `update/HankModPack.dll` in its own GitHub repository. On the game menu, a message shows what changed; press **Triangle** (pad) or
**Alt Gr** (keyboard) to install it. It is used from the next launch of the game. Nothing in the quick menu: updates are only offered on the game menu.

- `JibbersMods/Hank Mod Pack/update.json`: `ask` (default), `auto` (installs by itself) or `off` (never checks).
- Nothing is shown when the pack is up to date. A failed update says why (for example HTTP 404: repository or file not found).
- Installing removes older copies of the pack and the standalone DLLs of the mods it replaces.

## Files

Everything the pack writes is in `JibbersMods/Hank Mod Pack`: `Settings/<mod name>.json` (one per mod), the quick menu keys and Quick Actions,
favorites, `Camera Presets.json`, `POV rig.json`, `update.json`, and `state.txt` (one-time steps already done). Loose files from earlier
versions in `JibbersMods` are moved there at launch and the old `*.done` markers are folded into `state.txt`. Options > Mods lists only the settings
with no other place; the others are still saved in these files.

## Troubleshooting

- Input Visualizer does not replace the game's display in a replay: check `jibbers_hotload.log` for lines starting with `Controller overlay:`.
- Something does not react in the quick menu: the action may have no button (Settings > Binds & buttons).
- An update does not work: check the lines starting with `updates:` in `jibbers_hotload.log`. Only one `HankModPack.dll` should be in the game folders.
