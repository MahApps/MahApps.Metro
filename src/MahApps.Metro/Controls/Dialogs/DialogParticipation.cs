// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    public static class DialogParticipation
    {
        /// <summary>
        /// Where a registration lives until the context it was made for is gone.
        /// <para/>
        /// A plain dictionary would be the end of both of them, since closing a window does not change
        /// the attached property and nothing else takes the entry out. A weak reference on the key is
        /// not enough either: the element that is registered is usually the window, and its DataContext
        /// is the very context under which it is filed, so the entry would keep itself alive. A
        /// <see cref="ConditionalWeakTable{TKey,TValue}"/> is the one that copes with that circle, since
        /// its entry only lives as long as somebody else still holds the key.
        /// </summary>
        private static readonly ConditionalWeakTable<object, DependencyObject> ContextRegistrationIndex = new ConditionalWeakTable<object, DependencyObject>();

        public static readonly DependencyProperty RegisterProperty
            = DependencyProperty.RegisterAttached("Register",
                                                  typeof(object),
                                                  typeof(DialogParticipation),
                                                  new PropertyMetadata(null, RegisterPropertyChangedCallback));

        private static void RegisterPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.OldValue != null)
            {
                ContextRegistrationIndex.Remove(dependencyPropertyChangedEventArgs.OldValue);
            }

            if (dependencyPropertyChangedEventArgs.NewValue != null)
            {
#if NETCOREAPP || NET472_OR_GREATER
                ContextRegistrationIndex.AddOrUpdate(dependencyPropertyChangedEventArgs.NewValue, dependencyObject);
#else
                // AddOrUpdate arrived in net472 and is not on the legacy target, so there the old entry
                // goes first and the new one after it.
                ContextRegistrationIndex.Remove(dependencyPropertyChangedEventArgs.NewValue);
                ContextRegistrationIndex.Add(dependencyPropertyChangedEventArgs.NewValue, dependencyObject);
#endif
            }
        }

        public static void SetRegister(DependencyObject element, object? context)
        {
            element.SetValue(RegisterProperty, context);
        }

        public static object? GetRegister(DependencyObject element)
        {
            return element.GetValue(RegisterProperty);
        }

        internal static bool IsRegistered(object context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return ContextRegistrationIndex.TryGetValue(context, out _);
        }

        internal static DependencyObject GetAssociation(object context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (ContextRegistrationIndex.TryGetValue(context, out var element) == false)
            {
                throw new KeyNotFoundException($"The context `{context}` is not registered.");
            }

            return element;
        }
    }
}