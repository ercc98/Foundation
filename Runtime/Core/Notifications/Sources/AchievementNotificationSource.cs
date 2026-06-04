using UnityEngine;
using ErccDev.Foundation.Core.Achievements;

namespace ErccDev.Foundation.Core.Notifications.Sources
{
    /// <summary>
    /// Optional bridge: turns achievement unlocks into notifications. Kept separate from both
    /// systems on purpose — neither the achievement engine nor the notification core knows the
    /// other exists. Games that don't want toasts simply don't add the component. Mirrors the
    /// CollectionRewardGranter pattern.
    /// </summary>
    public class AchievementNotificationSource : MonoBehaviour
    {
        [Header("Wiring")]
        [SerializeField] private MonoBehaviour achievementService;   // IAchievementService

        [Header("Display")]
        [SerializeField] private string headline = "Achievement unlocked";
        [SerializeField] private float  duration = NotificationData.DefaultDuration;

        private IAchievementService _achievements;

        private void OnEnable()
        {
            _achievements = achievementService as IAchievementService;
            if (_achievements != null)
                _achievements.OnUnlocked += Push;
        }

        private void OnDisable()
        {
            if (_achievements != null)
                _achievements.OnUnlocked -= Push;
        }

        private void Push(AchievementDefinition def)
        {
            if (def == null) return;
            NotificationService.Notify(new NotificationData(
                title:    string.IsNullOrEmpty(def.title) ? headline : def.title,
                message:  def.description,
                icon:     def.icon,
                category: NotificationCategory.Achievement,
                duration: duration));
        }
    }
}
