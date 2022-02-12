// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;
using Xunit.Sdk;

namespace MahApps.Metro.Tests.TestHelpers
{
    public class DisplayTestMethodNameAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            var message = $"Setup for test '{methodUnderTest.Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            var message = $"TearDown for test '{methodUnderTest.Name}' with Thread.CurrentThread: {Thread.CurrentThread.ManagedThreadId}" +
                          $" and Current.Dispatcher.Thread: {Application.Current.Dispatcher.Thread.ManagedThreadId}";
            Debug.WriteLine(message);
        }
    }
}