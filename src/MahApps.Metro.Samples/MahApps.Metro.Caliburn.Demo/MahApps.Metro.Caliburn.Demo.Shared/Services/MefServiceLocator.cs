using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Caliburn.Metro.Demo.Services
{
    [Export(typeof(IServiceLocator))]
    public class MefServiceLocator : IServiceLocator
    {
        private readonly CompositionContainer compositionContainer;

        [ImportingConstructor]
        public MefServiceLocator(CompositionContainer compositionContainer)
        {
            this.compositionContainer = compositionContainer;
        }

        public T GetInstance<T>() where T : class
        {
            var instance = this.compositionContainer.GetExportedValue<T>();
            if (instance != null)
            {
                return instance;
            }

            throw new Exception($"Could not locate any instances of contract {typeof(T)}.");
        }
    }
}