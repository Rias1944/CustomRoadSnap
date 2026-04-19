# CustomRoadSnap — Cities: Skylines II Mod

![Version](https://img.shields.io/badge/version-1.1-blue)
![Game Version](https://img.shields.io/badge/game%20version-1.5.6f-green)
[![Paradox Mods](https://img.shields.io/badge/Paradox%20Mods-Download-orange)](https://mods.paradoxplaza.com/mods/141606/Windows)

A mod for **Cities: Skylines II** that snaps roads to a **configurable angle** while the Net Tool is active.

## Features

- Configure the snap angle (1–90°) via a **slider in mod settings**
- Default: 30° (gives axes at 0°, 30°, 60°, 90°, … — ideal for hex-grid cities)
- Toggle button in the **Tool Options** panel when the road tool is active
- Keyboard shortcut to toggle snapping on/off (configurable in mod settings)
- Minimal performance impact — only active when the Net Tool is selected

## 📦 Installation

### Option 1: Paradox Mods (Recommended)
1. Subscribe to the mod on **[Paradox Mods](https://mods.paradoxplaza.com/mods/141606/Windows)**
2. The mod will be automatically downloaded and installed
3. Enable the mod in **Options → Mods** menu in-game

### Option 2: Manual Installation (GitHub)
1. Download the latest [`CustomRoadSnap-v1.1.zip`](https://github.com/Rias1944/CustomRoadSnap/releases/latest)
2. Extract the ZIP file
3. Copy the contents to:  
   `%AppData%\..\LocalLow\Colossal Order\Cities Skylines II\Mods\CustomRoadSnap\`
4. Enable the mod in **Options → Mods** menu in-game

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
