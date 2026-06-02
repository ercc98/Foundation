using System;
using System.Collections.Generic;

namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Serializable unlock state, persisted via ISaveService. Kept deliberately small —
    /// only what has been earned needs to survive between sessions.
    /// </summary>
    [Serializable]
    public class AchievementProgress
    {
        public List<string> unlockedIds = new();
    }
}
