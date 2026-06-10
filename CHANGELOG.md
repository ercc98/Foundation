## [1.2.0] - 10 June 2026
	- Declared InputSystem (1.18.0) and Cinemachine (3.1.6) as package dependencies so the package compiles in any consumer project (previously referenced by the asmdef but undeclared)
	- Renamed the PlayMode test assembly from "Editor" to "ErccDev.Foundation.Tests" (meta GUID preserved); test asmdef now also references Unity.Cinemachine
	- Aligned the Runtime asmdef rootNamespace to "ErccDev.Foundation" so newly created scripts get the correct default namespace
	- Added PlayMode tests for previously-untested modules: PauseService, InputModule<T> base, CameraShakerBase, AchievementManagerBase, and SwipeInputSystem
	- SwipeInputSystem: extracted the tap/swipe decision into a private ResolveGesture seam (behavior-identical) so the gesture logic is testable without simulating input
## [1.1.0] - 05 June 2026
	- Added CollectionCatalog (Core/Collection): one authored SO holding every CollectionEntryDefinition — the single source of truth shared by the manager and any game-side consumer
	- CollectionCatalog exposes Entries/Count, Get(id)/Get<T>(id)/TryGet, Invalidate(), and an editor-only "Refresh From Project" context menu
	- Moved UnlockCollectionEntryReward into the Collection module (bridges Achievements -> Collection; guid preserved)
	- BREAKING: CollectionManagerBase no longer holds a List<CollectionEntryDefinition> entries; assign a CollectionCatalog instead (Find/TotalCount/completion read the catalog)
	- Added Pinch input (Input/Pinch): two-finger recognizer reporting fingers moving closer/apart
	- IPinchInput + PinchInputSystem expose PinchedIn/PinchedOut events plus Scale/DeltaPixels/Midpoint for zoom mapping
	- PinchInputConfig sets the DPI-scaled jitter threshold and whether extra fingers are ignored
	- Added Notification system (Core/Notifications): generic in-game toast queue for unlocks
	- INotificationService + NotificationManagerBase show one notification at a time honoring per-toast duration
	- Static NotificationService facade pushes any notification (rewards, level-ups...) in one line
	- Abstract NotificationViewBase lets games skin the popup; the Foundation ships no visuals
	- Optional, removable source bridges (AchievementNotificationSource, CollectionNotificationSource) keep the core decoupled from achievements/collections
	- Added Collection system (Core/Collection): persistent compendium/album of unique entries
	- CollectionProgressData ScriptableObject saved through the Foundation save system
	- Engine is reward-agnostic; optional CollectionRewardGranter grants Reward assets via the OnDiscovered event
	- CollectionCompletionCondition bridges to Achievements
## [1.0.3] - 01 June 2026
	- Added BillboardScript
	- Added ITouchInput
	- Added Pause Services
	- Added Tutorial scripts
	- Added Achievements and Rewards system (Core/Achievements)
	- Modified some scripts and directories for a better nomenclature
## [1.0.2] - 11 November 2025
	- Added unit test and audio, input  and loader scripts
## [1.0.1] - 10 November 2025
	- Added Animations, events, factories, gameplays and pooling scripts
## [1.0.0] - 10 November 2025
	- Initial Version
