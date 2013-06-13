namespace Caliburn.Metro.Demo.Services
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;

    [Export(typeof(IServiceLocator))]
    public class MefServiceLocator : IServiceLocator
    {
        #region Fields

        private readonly CompositionContainer compositionContainer;

        #endregion

        #region Constructors and Destructors

        [ImportingConstructor]
        public MefServiceLocator(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        #endregion

        #region Public Methods and Operators

        public T GetInstance<T>() where T : class
        {
            var instance = this.compositionContainer.GetExportedValue<T>();
            if (instance != null)
            {
                return instance;
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", typeof(T)));
        }

        #endregion
    }
}