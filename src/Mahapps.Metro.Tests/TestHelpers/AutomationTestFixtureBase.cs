// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace MahApps.Metro.Tests.TestHelpers
{
    public class AutomationTestFixtureBase<TFixture> : AutomationTestBase, IClassFixture<TFixture>
        where TFixture : class
    {
        protected readonly TFixture fixture;

        public AutomationTestFixtureBase(TFixture fixture)
            : base()
        {
            this.fixture = fixture;
        }
    }
}