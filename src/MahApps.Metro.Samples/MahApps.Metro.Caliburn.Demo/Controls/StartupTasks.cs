using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;
using Caliburn.Metro.Demo.Services;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Caliburn.Metro.Demo.Controls
{
    public delegate void StartupTask();

    public class StartupTasks
    {
        private readonly IServiceLocator serviceLocator;

        [ImportingConstructor]
        public StartupTasks(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        [Export(typeof(StartupTask))]
        public void ApplyBindingScopeOverride()
        {
            var getNamedElements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
                {
                    var metroWindow = o as MetroWindow;
                    if (metroWindow == null)
                    {
                        return getNamedElements(o);
                    }

                    var list = new List<FrameworkElement>(getNamedElements(o));
                    var type = o.GetType();
                    var fields =
                        o.GetType()
                         .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                         .Where(f => f.DeclaringType == type);
                    var flyouts =
                        fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                              .Select(f => f.GetValue(o))
                              .Cast<FlyoutsControl>();
                    list.AddRange(flyouts);
                    return list;
                };
        }

        [Export(typeof(StartupTask))]
        public void ApplyViewLocatorOverride()
        {
            var viewLocator = this.serviceLocator.GetInstance<IViewLocator>();
            Micro.ViewLocator.GetOrCreateViewType = viewLocator.GetOrCreateViewType;
        }
    }
}