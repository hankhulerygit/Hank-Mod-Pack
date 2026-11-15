# Hank Mod Pack changelog

## Unreleased

### Added
- **Updates from GitHub**: at launch the pack checks `update/HankModPack.dll` of its own repository. A message on screen shows the new version and
  its details (`update/whatsnew.txt`); Quick menu > Settings > Y installs it (used from the next launch). Modes in
  `JibbersMods/Hank Mod Pack/update.json`: `ask` (default), `auto`, `off`. Nothing is shown when up to date; a failed update says why.
  Installing removes older copies of the pack and the standalone DLLs of the mods it replaces.
- **Quick Actions starting pattern** (once, only when empty): Cross = detach both skis, Square / Triangle = left / right ski, Circle = `/login bob`,
  L1 = Add Ghost, R1 = Ghost Menu. The other buttons are free.
- **Input Visualizer** (mod `PadOverlay`): replaces the game's F1 display with a drawn PS4 / Xbox One controller (keyboard and mouse without a pad),
  in White, Black or any HUD theme, with size, position and opacity. Toggle from the quick menu (L1); replay editor tab; shows the replayed rider's inputs.

### Changed
- **Quick Actions** is one list with every pad button (press = run, hold = change, empty = choose); the three sets are gone.
- **Files**: everything the pack writes is in `JibbersMods/Hank Mod Pack` (quick menu keys and actions, favorites, camera presets, POV rig).
  Loose files from earlier versions are moved at launch; the old `*.done` markers are folded into `state.txt` and deleted.
- Quick menu default layout: Triangle = Cameras, L1 = Input visualizer, L2 = Themes, L3 = Detach both skis, Options = Settings / Binds / Themes.
  Applied once to existing configurations; your later choices are kept.
- Options > Mods lists only the settings that cannot be changed elsewhere.
- Settings files moved to `JibbersMods/Hank Mod Pack/Settings/<mod name>.json`; the old `JibbersMods/<id>.settings.json` files are imported and deleted.
- The pack has eleven mods (the info entry plus ten features).

### Fixed
- On-foot camera: the right stick no longer keeps moving the camera after a respawn; **R3 alone** looks behind; the camera no longer ends farther
  after each replay exit (quick-menu buttons no longer zoom it).
- Input Visualizer: no longer turns itself back on after leaving the replay editor, the pause menu or a replay; it follows the game's display only on F1.
- HUD and Input Visualizer wait for the loading screen to end.
- POV camera: the rider's head is hidden for every player (re-scanned while the camera is on).

### Removed
- The three Quick Actions sets and their Settings entry.

### Known limitations
- The on-foot camera, replay input following, loading-screen timing, head hiding and GitHub updates are built from the game's own objects and are not
  verified in game on every setup. Send the `Controller overlay:` / `updates:` lines of `jibbers_hotload.log` if one does not work.
- The Input Visualizer has no quick-menu window yet (Show / Theme / Size / Position from the quick menu).
