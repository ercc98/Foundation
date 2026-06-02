namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Service locator handed to conditions and rewards so they can reach the game's
    /// systems (inventory, currency, stats...) without the Foundation knowing about them.
    /// Mirrors ITutorialContext.
    /// </summary>
    public interface IAchievementContext
    {
        bool TryGet<T>(out T service) where T : class;
        T    Get<T>() where T : class;
    }
}
