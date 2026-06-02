namespace ErccDev.Foundation.Core.Achievements
{
    /// <summary>
    /// Something granted to the player (currency, item, skin...). The Foundation defines
    /// the contract; each game implements what granting actually does via the context.
    /// </summary>
    public interface IReward
    {
        void Grant(IAchievementContext context);
    }
}
