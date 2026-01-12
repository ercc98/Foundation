
namespace ErccDev.Foundation.Core.Tutorial
{
    public interface ITutorialCondition
    {
        void Initialize(ITutorialContext context);
        bool IsCompleted();
        void Cleanup();
    }
}