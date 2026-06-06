# ErccDev Foundation

## Purpose
A reusable Unity package of "solid base" scripts the author drops into **new game projects to save time**.
The goal is a library of classes commonly needed across games (events, audio, input, saving, pooling,
scene loading, tutorials, cameras, etc.) so each new project starts from a tested foundation instead of scratch.

Package: `com.erccdev.foundation` · Unity `6000.2` · depends on Addressables (asmdef also references
InputSystem & Cinemachine). Assembly: `ErccDev.Foundation`, root namespace `Foundation`.
Distributed via UPM git URL: `https://github.com/ercc98/Foundation.git`.

## Layout
- `Runtime/` — the library, organized by module. Each file's namespace is `ErccDev.Foundation.<Module>...`.
  - `Core/` — `Events` (EventBus), `Factories`, `Pooling`, `Save`, `Pause`, `Input`, `Tutorial`,
    `Gameplay` (session controller), `Animations`, `Achievements` (+ shared `Reward` assets),
    `Collection` (compendium/album, driven by an authored `CollectionCatalog` SO — the single source
    of truth the manager and any game-side UI share), `Notifications` (in-game toast queue for unlocks).
  - `Audio/` — `AudioManagerBase` + `IAudioService`, ScriptableObject sound groups/entries/categories.
  - `Input/` — touch & swipe systems (`SwipeInputSystem`, `SteeringTouchInputSystem` — 2-axis steering
    via `IHorizontalTouchInput`/`IVerticalTouchInput`; `PinchInputSystem` — two-finger pinch via
    `IPinchInput`, reports fingers moving closer/apart and exposes `Scale`/`DeltaPixels` for zoom mapping).
  - `Loader/` — scene loading (`SceneLoader` / `ISceneLoader`).
  - `Cameras/` — `CameraShakerBase` + shake profiles.
  - `Rendering/` — `BillboardSpriteRenderer`.
  - `Data/`, `Services/`, `Bootstrap/` — profile/settings data, data service base, splash controller.
- `Tests/PlayMode/` — PlayMode tests mirroring the Runtime folder structure (`Editor.asmdef`).

## Design principles
- **Follow SOLID.** Keep classes single-responsibility, depend on the interfaces (the `IThing` abstractions),
  keep base classes open for extension via `virtual`/`abstract` members, and keep interfaces small and focused.
  This is the primary architectural target for the library.
- **But conserve the author's code personality.** Do NOT rewrite working code into a generic/textbook style.
  Preserve the existing voice: terse static facades and helpers, aligned-assignment formatting, `switch`
  expressions for category lookups, `?.`/null-guard early returns, lightweight `DontDestroyOnLoad` singletons,
  region-comment banners (`// ---------- Static API ----------`), and concise XML summaries. Apply SOLID by
  extending and shaping new code to fit this style — not by imposing a different one.

## Conventions (match these when adding code)
- **Interface + base class pairs**: ship an `IThing` interface alongside an abstract `ThingBase : MonoBehaviour`
  so games subclass/wire in the editor. See `AudioManagerBase`, `CameraShakerBase`, `TutorialManagerBase`.
- **Static facades over swappable services**: e.g. `SaveService` is a static class wrapping a swappable
  `ISaveService` (`SetDefault(...)` for tests/encryption/cloud). `EventBus` exposes a static API over a
  `DontDestroyOnLoad` singleton.
- **Segregate interfaces by concern**: when one component serves several roles, split the contract so consumers
  depend only on what they need. See touch input — shared touch-level state lives on `ITouchInput`
  (`IsTouching`, `PointerPosition`, `StartTouch`/`EndTouch`), while per-axis steering is split into
  `IHorizontalTouchInput` (`SteeringX`, `MovedLeft`/`MovedRight`) and `IVerticalTouchInput`
  (`SteeringY`, `MovedUp`/`MovedDown`); `SteeringTouchInputSystem` implements all three. Likewise each
  distinct gesture gets its own focused contract rather than overloading one — `IPinchInput` (two-finger
  pinch) is separate, so a future rotate/multi-finger gesture would be its own interface + system too.
- **ScriptableObject-driven config**: sound groups, swipe/touch configs, camera shake profiles, tutorial config
  are SOs authored in the editor.
- **Lightweight & drop-in**: minimal dependencies between modules; keep changes scoped to the relevant module folder.
- Namespaces follow folder path under `ErccDev.Foundation`.

## Working in this repo
- Unity project root is one level up (`ErccDevCorePackage`); this `Foundation` folder is the package under `Assets/`.
- A `unity-mcp` MCP server is available for driving the Unity editor directly when connected.
- When adding a feature, add a matching PlayMode test under `Tests/PlayMode/<Module>/`.
- Versioning: bump `package.json` `version` and update `CHANGELOG.md` on release (keep them in sync).
