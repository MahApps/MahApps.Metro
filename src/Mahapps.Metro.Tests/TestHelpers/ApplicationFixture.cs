// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using Xunit;

namespace MahApps.Metro.Tests.TestHelpers
{
    public class ApplicationFixture : IDisposable
    {
        public ApplicationFixture()
        {
            // ... initialize
            TestHost.Initialize();
        }

        public void Dispose()
        {
            // ... clean up
            GC.Collect();
            Dispatcher.ExitAllFrames();
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
    }

    [CollectionDefinition("ApplicationFixtureCollection")]
    public class ApplicationFixtureCollectionClass : ICollectionFixture<ApplicationFixture>
    {
    }
}