# Hank Mod Pack changelog

## Unreleased

### Added
- **Input Visualizer** (new mod, `PadOverlay`): replaces the game's F1 input display with a drawn PS4 or Xbox One
  controller, or a keyboard and mouse when no pad is connected. White, black or any HUD theme; every theme is just the
  controller in its colours (no background panel). Size, position and opacity settings. Toggle from the quick menu (L1).
  - Replay editor: new **Input Visualizer** tab (show, theme, size, position) that applies until the replay ends.
  - In a replay the controller shows the replayed rider's inputs, is drawn above the timeline and replaces the game's own display.
  - The drawing is computed in the background and cached, so changing a theme no longer stalls the game.
- **Quick menu**: new default layout (Triangle = Cameras, L1 = Input visualizer, L2 = Themes, L3 = Detach both skis,
  Options = Settings / Binds / Themes). New **Themes** action opens the theme window directly. The layout is applied once to existing configurations.
- **Improved on-foot camera**: after detaching, the right stick turns the camera and it stays where it is left;
  L1 / R1 lock it, L3 puts it back behind the rider, R3 is left to the game.
- Quick Actions: a set's buttons are edited from the set list (Y edit, X rename). The action chooser has tabs.
- Replay editor: R2 opens the effects window (L2 = big steps); PlayStation icons for Sony pads.

### Changed
- Options > Mods only lists the settings that cannot be changed elsewhere: quick-menu buttons, camera values (Play Cameras and
  Replay Camera), HUD show / theme, spawn position and the replay Input Visualizer options are hidden there (still saved and used).
- Settings files moved to `JibbersMods/Hank Mod Pack/Settings/<mod name>.json`. The old `JibbersMods/<id>.settings.json` files are
  imported and deleted on first launch.
- "Shortcuts" is now called **Quick Actions**.
- The pack has eleven mods (the info entry plus ten features).

### Removed
- The Quick Actions entry of the Settings page (edit sets from the Quick Actions window).

### Known limitations
- Following the replayed rider's inputs, hiding the game's replay input display and the on-foot camera behaviours are built from the game's
  own objects and have not been verified in game on every setup. Send the `Controller overlay:` lines of `jibbers_hotload.log` if one does not work.
- The Input Visualizer has no quick-menu window yet (Show / Theme / Size / Position from the quick menu).
