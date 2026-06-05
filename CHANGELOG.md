## [Unreleased]
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
