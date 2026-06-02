using System;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Read/notify surface for UI and game code. The base manager implements it; swap in
    /// a fake for tests.
    /// </summary>
    public interface IAchievementService
    {
        bool  IsUnlocked(string achievementId);
        float GetProgress01(string achievementId);
        void  Evaluate();   // re-check pending conditions and unlock any that completed

        event Action<AchievementDefinition> OnUnlocked;
    }
}
