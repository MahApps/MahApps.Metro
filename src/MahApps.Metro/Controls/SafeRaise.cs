// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls
{
    internal static class SafeRaise
    {
        public static void Raise(EventHandler eventToRaise, object sender)
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, EventArgs.Empty);
            }
        }

        public static void Raise(EventHandler<EventArgs> eventToRaise, object sender)
        {
            Raise(eventToRaise, sender, EventArgs.Empty);
        }

        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, T args)
            where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, args);
            }
        }

        public delegate T GetEventArgs<T>()
            where T : EventArgs;

        public static void Raise<T>(EventHandler<T> eventToRaise, object sender, GetEventArgs<T> getEventArgs)
            where T : EventArgs
        {
            if (eventToRaise != null)
            {
                eventToRaise(sender, getEventArgs());
            }
        }
    }
}