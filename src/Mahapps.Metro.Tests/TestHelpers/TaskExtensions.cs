// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace MahApps.Metro.Tests.TestHelpers
{
    public static class TaskExtensions
    {
        /// <summary>
        /// How long one step of a test gets before it is treated as stuck. Every step here takes
        /// well under a second, so this leaves room on a loaded build agent and still comes back
        /// inside the five minutes the blame collector allows.
        /// </summary>
        private static readonly TimeSpan StepTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Waits for <paramref name="task"/> and gives up with a message naming
        /// <paramref name="what"/> where it does not come back.
        /// </summary>
        /// <remarks>
        /// These tests wait on WPF events, and an event that never arrives holds the test host until
        /// somebody kills it. The run then names the test it stopped in and nothing else, which is
        /// not enough to tell one wait inside that test from the next.
        /// </remarks>
        public static async Task Within(this Task task, string what)
        {
            var finished = await Task.WhenAny(task, Task.Delay(StepTimeout)).ConfigureAwait(true);

            if (finished != task)
            {
                Assert.Fail($"gave up waiting for {what} after {StepTimeout.TotalSeconds:0} seconds");
            }

            await task.ConfigureAwait(true);
        }
    }
}
