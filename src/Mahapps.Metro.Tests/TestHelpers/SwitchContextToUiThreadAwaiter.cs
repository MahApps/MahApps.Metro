// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace MahApps.Metro.Tests.TestHelpers
{
    public class SwitchContextToUiThreadAwaiter : INotifyCompletion
    {
        private readonly Dispatcher uiContext;

        public SwitchContextToUiThreadAwaiter(Dispatcher uiContext)
        {
            this.uiContext = uiContext;
        }

        public SwitchContextToUiThreadAwaiter GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted
        {
            get { return false; }
        }

        public void OnCompleted(Action continuation)
        {
            this.uiContext.Invoke(new Action(continuation));
        }

        public void GetResult()
        {
        }
    }
}