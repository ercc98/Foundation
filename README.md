# ErccDev Foundation

ErccDev Foundation provides a set of reusable Unity utilities intended to speed up common gameplay, audio, and scene-management tasks across projects.

## Features
- **Core**: Event utilities, object factories, pooling helpers, save-system primitives, and lightweight animation helpers.
- **Audio**: Service wrapper for playing SFX and music with minimal boilerplate.
- **Input**: Input abstractions plus a swipe-detection system for touch and mouse.
- **Loader**: Helpers for scene loading and transition orchestration.

## Installation (UPM via Git URL)
1. Open Unity and navigate to **Window → Package Manager**.
2. Click the **+** button and select **Add package from git URL...**.
3. Enter the repository URL: `https://github.com/ercc98/Foundation.git`.
4. Click **Add** to install the package.

### Manifest snippet
If you prefer editing `Packages/manifest.json` directly, add the dependency:

```json
"dependencies": {
  "com.erccdev.foundation": "https://github.com/ercc98/Foundation.git",
  // ...other dependencies
}
```

## Usage notes
- Utilities are organized by module (Core, Audio, Input, Loader); import the namespace relevant to the system you want to use.
- Components are designed to be drop-in and lightweight—check the corresponding module folders for examples and prefab assets.
- The package follows standard Unity assembly definition conventions so it can be safely included in editor and runtime assemblies.

## Contributing
Issues and pull requests are welcome. Please follow Unity C# coding conventions and keep module-specific changes scoped to their respective folders.

