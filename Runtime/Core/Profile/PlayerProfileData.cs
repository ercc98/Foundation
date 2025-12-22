using System;
using UnityEngine;

namespace ErccDev.Foundation.Core.Save
{
    [CreateAssetMenu(menuName = "ErccDev/Profile/Player Profile", fileName = "PlayerProfileData")]
    public sealed class PlayerProfileData : ScriptableObject
    {
        [SerializeField] private string fileName = "save.json";

        public string FileName => fileName;

        [Header("Identity")]
        [SerializeField] private string playerId;
        [SerializeField] private string displayName;

        [Header("Locale")]
        [SerializeField] private string languageCode = "en";
        [SerializeField] private string countryCode = "MX";

        [Header("Lifecycle (UTC)")]
        [SerializeField] private long createdAtUtcTicks;
        [SerializeField] private long lastLoginUtcTicks;
        [SerializeField] private long lastSaveUtcTicks;

        [Header("App / Save")]
        [SerializeField] private string lastAppVersion;
        [SerializeField] private int saveSchemaVersion = 1;

        [Header("Consents")]
        [SerializeField] private bool analyticsConsent = true;

        public string PlayerId => playerId;
        public string DisplayName { get => displayName; set => displayName = value; }
        public string LanguageCode { get => languageCode; set => languageCode = value; }
        public string CountryCode { get => countryCode; set => countryCode = value; }
        public int SaveSchemaVersion => saveSchemaVersion;
        public string LastAppVersion { get => lastAppVersion; set => lastAppVersion = value; }
        public bool AnalyticsConsent { get => analyticsConsent; set => analyticsConsent = value; }

        public DateTime CreatedAtUtc => createdAtUtcTicks == 0 ? DateTime.MinValue : new DateTime(createdAtUtcTicks, DateTimeKind.Utc);
        public DateTime LastLoginUtc => lastLoginUtcTicks == 0 ? DateTime.MinValue : new DateTime(lastLoginUtcTicks, DateTimeKind.Utc);
        public DateTime LastSaveUtc => lastSaveUtcTicks == 0 ? DateTime.MinValue : new DateTime(lastSaveUtcTicks, DateTimeKind.Utc);

        /// <summary>Call on boot before using the profile.</summary>
        public void EnsureInitialized()
        {
            if (string.IsNullOrWhiteSpace(playerId))
                playerId = Guid.NewGuid().ToString("N");

            if (createdAtUtcTicks == 0)
                createdAtUtcTicks = DateTime.UtcNow.Ticks;

            lastAppVersion = Application.version;
            lastLoginUtcTicks = DateTime.UtcNow.Ticks;
        }

        public void MarkSaved()
        {
            lastSaveUtcTicks = DateTime.UtcNow.Ticks;
            lastAppVersion = Application.version;
        }
    }
}
