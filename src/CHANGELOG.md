# Hank Mod Pack changelog

## 1.8.6

### Fixed
- Replay editor / Follow cam / POV disappearing after opening another replay: the game can throw a native error in its camera update while a replay loads, which made the game bypass the pack's camera hook until restart. The error is now absorbed and the hook stays active.

## 1.8.5

### Changed
- Effects window (R2): the keyframe presets tab is now called **Keyframes settings** and is the last tab.

## 1.8.4

### Fixed
- Follow cam and POV: every number (orbit, reactivity, lead-ahead, pull back, shake, smoothing, stabilizer, speed FOV boost, eye offsets, look limits, shoulder and head settings...) is now an effect keyframe value and blends between keyframes. Before, only a few (angle, distance, height, aim, tilt, focus, horizon, shift, pitch, FOV) did.
- Effects window (R2): the Follow cam / POV sliders now show and edit the keyframe value at the playhead instead of the global setting that keyframes override.

## 1.8.3

### Fixed
- Follow cam / POV no longer lock the game speed in the replay: an empty (0) keyframe time scale is ignored, and the keyed time scale is only written when it changes, so the game speed controls keep working.

## 1.8.2

### Fixed
- Loading keyframes crashed the game: the mod no longer creates game keyframes itself. A preset is applied to the game keyframes the replay already has (add them first, then load).
- Save / Load / Delete keyframes are now the last rows of the keyframe panel.

## 1.8.1

### Fixed
- Effects window legend: "hold: more" now shows L2 (the key that gives bigger steps) instead of R2.

## 1.8.0

### Added
- Effects window (R2): a **Keyframes** section with Save (what to save, save now), Load and Delete, so presets no longer need the big keyframe panel.

### Fixed
- Loading saved keyframes now gives the replay all the game keyframes of the preset (it used to apply them only to the keyframes the replay already had, usually just the first one).

## 1.7.12

### Fixed
- Replay editor, Lights: Map Lights colour keeps working when the map's lights are looked up again (their original colour is remembered instead of the tinted one being taken as the original).

## 1.7.11

### Changed
- Save keyframes writes the frames it stores (FX and game, with the clip length) to the log, to track down saves that only keep the first keyframe.

## 1.7.10

### Fixed
- Replay editor: the Cam Lamp's intensity and range have a visible effect (x2 and x20 of the F3 values, the lamp that worked before).

## 1.7.9

### Fixed
- Replay editor: the Cam Lamp switch works again. The game keeps no Cam Lamp light in the scene, so the lamp is made on the replay camera with the F3 menu's Cam Lamp colour, intensity and range (and follows the Lights keyframes).

## 1.7.8

### Fixed
- Replay editor, Lights: the colour of the lamps now changes. Map Lights tints every light of the map (each keeps its own shade), Head Lamp and Cam Lamp are found by the colour the F3 menu shows for them.

## 1.7.7

### Fixed
- Replay editor: the F3 options (Sun size, Moon light, Clouds, Snowfall, Fog, Powder, the three lamps' colour, intensity and range) are now effect keyframes: they blend between keyframes while the replay plays and go back to the game's values when it ends. Before, they only changed live and stayed on the last value.
- Weather and Lights have the add / delete keyframe buttons and show their dot on the FX timeline.

## 1.7.6

### Changed
- Effects window: Weather and Lights list their sub-tabs; click one (or A) to open its options, Back / B returns to the list. No more left / right arrows to switch.
- The F3 options sit with their sub-tab: Sun size with Sun direction and height, Moon light intensity with Moon phase, Time speed with Time of day.

## 1.7.5

### Changed
- Replay editor: the F3 menu options (Weather and Lights tabs) are ready as soon as a replay starts: the mod opens the F3 menu invisibly, reads it and closes it, so F3 no longer has to be pressed.

## 1.7.4

### Changed
- Replay editor: Sun, Moon, Clouds, Snowfall, Fog and Powder are sub-tabs of **Weather**; Map Lights, Head Lamp and Cam Lamp are sub-tabs of **Lights**. In the keyframe panel they are sections inside the tab; in the effects window a first row (Section / Lamp) switches between them with left / right.

## 1.7.3

### Changed
- Replay editor: every category of the F3 menu has its own tab in the keyframe panel and in the effects window: Sun, Moon, Clouds, Snowfall, Fog, Powder, Map Lights, Head Lamp and Cam Lamp. The Weather tab keeps the time of day, the sun and moon keyframes and the time speed.
- Update message: Circle / B / Esc closes it at once.

## 1.7.2

### Fixed
- The Triangle / Y or Alt Gr update key only works on the game menu: as soon as a rider is in the world (the game is being played) it does nothing and the message goes away.

## 1.7.1

### Fixed
- The F3 options read once stay available in the Weather and Lights tabs for the whole session (they were dropped when a replay started). A line asks to open F3 once when nothing was read yet.
- The mod windows use the game's font again (the 1.6.8 font change is removed).

## 1.7.0

### Added
- Replay editor, **Lights** tab: red / green / blue for the game's three lamps (Map Lights, Head Lamp, Cam Lamp), their Intensity and Range from the F3 menu, and the Cam Lamp switch (the game's own lamp, nothing is created).
- Replay editor, **Weather** tab: the F3 menu's Sun size and speed, Moon light, Clouds, Snowfall, Fog and Powder options, next to the time of day.

## 1.6.9

### Fixed
- The F3 menu options are read as soon as the menu is opened, even outside a replay (so they are ready, and listed in the log, before the replay editor starts).

## 1.6.8

### Fixed
- The effects window (and the other mod windows) keep the same font: it is picked once from the game's menus instead of changing at each redraw.

## 1.6.7

### Fixed
- Replay editor: the F3 menu is now found among all the game's menus (by its rows), so its lights and weather options reach the Lights / Weather tabs.

## 1.6.6

### Changed
- Replay editor: the cam lamp is now the game's own lamp (no extra light is created); its colour, intensity, range and angle are set in the Lights tab and put back when it is switched off.
- The F3 menu is now found by its title, so its lights and weather options show in the Lights / Weather tabs.

## 1.6.5

### Added
- Replay editor: new **Lights** tab (keyframe panel and the R2 effects window): CamLight on/off, colour (red / green / blue), intensity, range and angle, plus the lights of the game's F3 menu.
- Replay editor: the **Time of day** tab is now **Weather** and also holds the other weather options of the F3 menu.

## 1.6.4

### Fixed
- Works again with the new Jibbers Playtest build (Steam 26688799): the pack is built against the updated modloader bindings, so it shows in game again.

## 1.6.3

### Changed
- Quick Actions: the D-pad rows show the picture of the direction (up, down, left, right) instead of text.

## 1.6.2

### Added
- **Updates from GitHub**: at launch the pack checks `update/HankModPack.dll` of its own repository. On the game menu a message shows the details
  (`update/whatsnew.txt`); **Triangle** or **Alt Gr** installs the new version (used from the next launch). `update.json` in `JibbersMods/Hank Mod Pack`:
  `ask` (default), `auto` (installs by itself) or `off`. Nothing is shown when up to date; a failed update says why.
  Installing removes older copies of the pack and the standalone DLLs of the mods it replaces. The message shows the install keys as button pictures (Triangle / Y, Alt Gr);
  when the pack is up to date a small green message says so with its version, and an update announces the version it installed.
- **Quick Actions starting pattern** (once, only when empty): Cross = detach both skis, Square = Save map, Triangle = FPS menu, Circle = `/login bob`,
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
