namespace Caliburn.Metro.Demo.Services
{
    public interface IServiceLocator
    {
        #region Public Methods and Operators

        T GetInstance<T>() where T : class;

        #endregion
    }
}