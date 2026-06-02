namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Pluggable rule that decides when an achievement unlocks.
    /// Mirrors ITutorialCondition, plus normalized progress for UI bars.
    /// </summary>
    public interface IAchievementCondition
    {
        void  Initialize(IAchievementContext context);
        bool  IsCompleted();
        float Progress01 { get; }   // 0..1, for progress UI
        void  Cleanup();
    }
}
