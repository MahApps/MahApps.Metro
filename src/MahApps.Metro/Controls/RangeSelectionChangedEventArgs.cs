// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This delegate must used by handlers of the RangeSelectionChangedEvent event.
    /// </summary>
    /// <param name="sender">The current element along the event's route.</param>
    /// <param name="e">The event arguments containing additional information about the event.</param>
    /// <returns>Nothing.</returns>
    public delegate void RangeSelectionChangedEventHandler<T>(object sender, RangeSelectionChangedEventArgs<T> e);

    /// <summary>
    /// This RangeSelectionChangedEventArgs class contains old and new value when 
    /// RangeSelectionChanged is raised.
    /// </summary>
    /// <seealso cref="RoutedEventArgs" />
    /// <typeparam name="T"></typeparam>
    public class RangeSelectionChangedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Return the old lower value.
        /// </summary>
        public T OldLowerValue { get; }

        /// <summary>
        /// Return the new lower value.
        /// </summary>
        public T NewLowerValue { get; }

        /// <summary>
        /// Return the old upper value.
        /// </summary>
        public T OldUpperValue { get; }

        /// <summary>
        /// Return the new upper value.
        /// </summary>
        public T NewUpperValue { get; }

        /// <summary>
        /// This is an instance constructor for the RangeSelectionChangedEventArgs class.
        /// </summary>
        /// <param name="oldLowerValue">The old lower property value</param>
        /// <param name="newLowerValue">The new lower property value</param>
        /// <param name="oldUpperValue">The old upper property value</param>
        /// <param name="newUpperValue">The new upper property value</param>
        public RangeSelectionChangedEventArgs(T oldLowerValue, T newLowerValue, T oldUpperValue, T newUpperValue)
        {
            this.OldLowerValue = oldLowerValue;
            this.NewLowerValue = newLowerValue;
            this.OldUpperValue = oldUpperValue;
            this.NewUpperValue = newUpperValue;
        }

        /// <summary>
        /// This is an instance constructor for the RoutedPropertyChangedEventArgs class.
        /// It is constructed with a reference to the event being raised.
        /// </summary>
        /// <param name="oldLowerValue">The old lower property value</param>
        /// <param name="newLowerValue">The new lower property value</param>
        /// <param name="oldUpperValue">The old upper property value</param>
        /// <param name="newUpperValue">The new upper property value</param>
        /// <param name="routedEvent">RoutedEvent</param>
        public RangeSelectionChangedEventArgs(T oldLowerValue, T newLowerValue, T oldUpperValue, T newUpperValue, RoutedEvent routedEvent)
            : this(oldLowerValue, newLowerValue, oldUpperValue, newUpperValue)
        {
            this.RoutedEvent = routedEvent;
        }

        /// <summary>
        /// This method is used to perform the proper type casting in order to
        /// call the type-safe RoutedPropertyChangedEventHandler delegate for the IsCheckedChangedEvent event.
        /// </summary>
        /// <param name="genericHandler">The handler to invoke.</param>
        /// <param name="genericTarget">The current object along the event's route.</param>
        /// <returns>Nothing.</returns>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (RangeSelectionChangedEventHandler<T>)genericHandler;
            handler(genericTarget, this);
        }
    }
}