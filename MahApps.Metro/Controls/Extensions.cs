using System;
using System.Windows.Threading;
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    public static class Extensions
    {
        public static T Invoke<T>([NotNull] this DispatcherObject dispatcherObject, [NotNull] Func<T> func)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }
            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                return func();
            }
            else
            {
                return (T)dispatcherObject.Dispatcher.Invoke(new Func<T>(func));
            }
        }

        public static void Invoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action setValueAction)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }
            if (setValueAction == null)
            {
                throw new ArgumentNullException(nameof(setValueAction));
            }
            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                setValueAction();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(setValueAction);
            }
        }
    }
}