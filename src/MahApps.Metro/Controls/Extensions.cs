// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
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
                return dispatcherObject.Dispatcher.Invoke(func);
            }
        }

        public static void Invoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action invokeAction)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                invokeAction();
            }
            else
            {
                dispatcherObject.Dispatcher.Invoke(invokeAction);
            }
        }

        /// <summary> 
        ///   Executes the specified action asynchronously with the DispatcherPriority.Background on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherObject">The dispatcher object where the action runs.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        /// <param name="priority">The dispatcher priority.</param> 
        public static void BeginInvoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, invokeAction);
        }

        public static void BeginInvoke<T>([NotNull] this T dispatcherObject, [NotNull] Action<T> invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
            where T : DispatcherObject
        {
            if (dispatcherObject == null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction == null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, new Action(() => invokeAction(dispatcherObject)));
        }

        /// <summary> 
        ///   Executes the specified action if the element is loaded or at the loaded event if it's not loaded.
        /// </summary>
        /// <param name="element">The element where the action should be run.</param>
        /// <param name="invokeAction">An action that takes no parameters.</param>
        public static void ExecuteWhenLoaded([NotNull] this FrameworkElement element, [NotNull] Action invokeAction)
        {
            if (element.IsLoaded)
            {
                element.Invoke(invokeAction);
            }
            else
            {
                void ElementLoaded(object o, RoutedEventArgs a)
                {
                    element.Loaded -= ElementLoaded;
                    element.Invoke(invokeAction);
                }

                element.Loaded += ElementLoaded;
            }
        }
    }
}