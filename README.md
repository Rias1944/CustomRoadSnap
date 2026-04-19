# CustomRoadSnap — Cities: Skylines II Mod

A mod for **Cities: Skylines II** that snaps roads to a **configurable angle** while the Net Tool is active.

## Features

- Configure the snap angle (1–90°) via a **slider in mod settings**
- Default: 30° (gives axes at 0°, 30°, 60°, 90°, … — ideal for hex-grid cities)
- Toggle button in the **Tool Options** panel when the road tool is active
- Keyboard shortcut to toggle snapping on/off (configurable in mod settings)
- Minimal performance impact — only active when the Net Tool is selected

## Requirements

- Cities: Skylines II (tested on 1.5.x)
- [HarmonyX](https://thunderstore.io/c/cities-skylines-ii/p/BepInEx/BepInExPack/) (included via NuGet)

## Installation

1. Download the latest release
2. Copy the `CustomRoadSnap` folder into:  
   `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\`
3. Enable the mod in the **Options → Mods** menu in-game
4. Open **Options → Custom Road Snap** and set your preferred snap angle

## Building from Source

### Prerequisites
- Visual Studio 2022+ with .NET Framework 4.8
- Node.js 18+
- Cities: Skylines II installed (game DLLs referenced via `Library/`)

### Steps

```bash
# Install UI dependencies
cd ui
npm install

# Build UI
npm run build

# Build C# project
# Open CustomRoadSnap.slnx in Visual Studio and build
```

## How It Works

The mod uses **HarmonyX** to patch the game's road placement system:

- `CustomRoadSnapPreSystem` removes the vanilla 90° snap flag before `NetToolSystem` runs
- `CustomRoadSnapPostSystem` computes snap axes from your configured angle and injects guide lines
- A Harmony postfix on `GetRaycastResult` snaps the cursor position to the nearest axis
- The snap angle is read dynamically every frame — changing it in settings takes effect immediately

The UI button is injected into CS2's `MouseToolOptions` panel using the official CS2 modding `moduleRegistry.extend` pattern.

## License

MIT

- Automatically snaps road placement to 120° increments (60°, 120°)
- Toggle button in the **Tool Options** panel (Einrasten/Snap section) when the road tool is active
- Keyboard shortcut to toggle snapping on/off (configurable in mod settings)
- Minimal performance impact — only active when the Net Tool is selected

## Requirements

- Cities: Skylines II (tested on 1.5.x)
- [HarmonyX](https://thunderstore.io/c/cities-skylines-ii/p/BepInEx/BepInExPack/) (included via NuGet)

## Installation

1. Download the latest release
2. Copy the `RoadSnap120` folder into:  
   `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\`
3. Enable the mod in the **Options → Mods** menu in-game

## Building from Source

### Prerequisites
- Visual Studio 2022+ with .NET Framework 4.8
- Node.js 18+
- Cities: Skylines II installed (game DLLs referenced via `Library/`)

### Steps

```bash
# Install UI dependencies
cd ui
npm install

# Build UI
npm run build

# Build C# project
# Open RoadSnap120.slnx in Visual Studio and build,
# or use dotnet/msbuild — the DeployUI target copies the .mjs automatically
```

## How It Works

The mod uses **HarmonyX** to patch the game's road placement system:

- `PreUpdateSystem` patches the direction vector before the game processes road placement, snapping the angle to the nearest 120° multiple
- `PostUpdateSystem` applies a secondary correction pass
- Snapping is only active when `SnapEnabled` is `true` (toggled via UI button or keyboard shortcut)

The UI button is injected into CS2's `MouseToolOptions` panel using the official CS2 modding `moduleRegistry.extend` pattern.

## License

MIT
