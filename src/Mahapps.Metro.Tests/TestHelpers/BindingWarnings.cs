// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MahApps.Metro.Tests.TestHelpers
{
    /// <summary>
    /// Collects what the binding engine writes to its trace while an action runs, the warnings a bound
    /// property gives when it finds no source among them.
    /// </summary>
    public sealed class BindingWarnings : TraceListener
    {
        public List<string> Messages { get; } = new();

        public static List<string> During(Action action)
        {
            var listener = new BindingWarnings();
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;
            try
            {
                action();
            }
            finally
            {
                PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);
            }

            return listener.Messages;
        }

        public override void Write(string? message)
        {
        }

        public override void WriteLine(string? message)
        {
            if (message is not null)
            {
                this.Messages.Add(message);
            }
        }
    }
}
