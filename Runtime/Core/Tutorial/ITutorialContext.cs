namespace ErccDev.Foundation.Core.Tutorial
{
    public interface ITutorialContext
    {
        bool TryGet<T>(out T service) where T : class;
        T Get<T>() where T : class;
    }
}