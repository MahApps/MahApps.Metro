// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;

namespace MetroDemo.Behaviors
{
    public class DateTimeNowBehavior : Behavior<DateTimePicker>
    {
        private DispatcherTimer _dispatcherTimer;

        protected override void OnAttached()
        {
            base.OnAttached();
            _dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1),
                                                   DispatcherPriority.DataBind,
                                                   (sender, args) => AssociatedObject.SelectedDateTime = DateTime.Now,
                                                   Dispatcher.CurrentDispatcher);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            _dispatcherTimer.Stop();
        }
    }
}