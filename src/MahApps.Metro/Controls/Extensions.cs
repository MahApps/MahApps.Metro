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
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (func is null)
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

        /// <summary>
        /// Runs the action on the thread the dispatcher object belongs to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A dispatcher that is on its way out takes nothing on any more, and asking it to would throw a
        /// cancelled task at whoever asked. That happens to a window on a thread of its own whose
        /// dispatcher is shut down while the window is still standing: it is still on the events it
        /// subscribed to, and the next one raised would bring the application down. There is nobody left
        /// to run the action for such a window, so it is not run.
        /// </para>
        /// <para>
        /// Both halves of the shutdown are asked about, the way CommandManager does it before it queues
        /// its requery. A dispatcher torn down through WM_DESTROY goes straight to the end of the
        /// shutdown without ever passing the start, so it finishes with HasShutdownFinished set and
        /// HasShutdownStarted still clear.
        /// </para>
        /// </remarks>
        public static void Invoke([NotNull] this DispatcherObject dispatcherObject, [NotNull] Action invokeAction)
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            if (dispatcherObject.Dispatcher.CheckAccess())
            {
                invokeAction();
            }
            else if (!dispatcherObject.Dispatcher.HasShutdownStarted && !dispatcherObject.Dispatcher.HasShutdownFinished)
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
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
            {
                throw new ArgumentNullException(nameof(invokeAction));
            }

            dispatcherObject.Dispatcher?.BeginInvoke(priority, invokeAction);
        }

        public static void BeginInvoke<T>([NotNull] this T dispatcherObject, [NotNull] Action<T> invokeAction, DispatcherPriority priority = DispatcherPriority.Background)
            where T : DispatcherObject
        {
            if (dispatcherObject is null)
            {
                throw new ArgumentNullException(nameof(dispatcherObject));
            }

            if (invokeAction is null)
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