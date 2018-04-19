namespace Caliburn.Metro.Demo.Services
{
    public interface IServiceLocator
    {
        T GetInstance<T>() where T : class;
    }
}