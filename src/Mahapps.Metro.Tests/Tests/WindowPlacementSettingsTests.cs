// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class WindowPlacementSettingsTests
    {
        /// <summary>
        /// A placement store that keeps everything in memory, the way a caller would write one that
        /// persists somewhere other than the application settings.
        /// </summary>
        private sealed class RecordingPlacementSettings : IWindowPlacementSettings
        {
            public List<string> Calls { get; } = new();

            public WindowPlacementSetting? Placement { get; set; }

            public bool UpgradeSettings { get; set; }

            public void Reload()
            {
                this.Calls.Add(nameof(this.Reload));
            }

            public void Upgrade()
            {
                this.Calls.Add(nameof(this.Upgrade));
            }

            public void Save()
            {
                this.Calls.Add(nameof(this.Save));
            }

            public void Reset()
            {
                this.Calls.Add(nameof(this.Reset));
            }
        }

        /// <summary>
        /// Settings that are broken the way a corrupted configuration file is: reloading throws, and so
        /// does reading the placement afterwards.
        /// </summary>
        private sealed class BrokenPlacementSettings : IWindowPlacementSettings
        {
            public WindowPlacementSetting? Placement
            {
                get => throw new MahAppsException("the settings file seems to be corrupted");
                set { }
            }

            public bool UpgradeSettings { get; set; }

            public void Reload()
            {
                throw new MahAppsException("the settings file seems to be corrupted");
            }

            public void Upgrade()
            {
            }

            public void Save()
            {
            }

            public void Reset()
            {
            }
        }

        [Test]
        public void ShouldRoundTripThePlacementThroughAFile()
        {
            var file = Path.Combine(Path.GetTempPath(), $"mahapps-placement-{Guid.NewGuid():N}.xml");

            try
            {
                var settings = new WindowPlacementFileSettings(file)
                               {
                                   Placement = new WindowPlacementSetting
                                               {
                                                   showCmd = 3,
                                                   minPosition = new Point(-1, -2),
                                                   maxPosition = new Point(-3, -4),
                                                   normalPosition = new Rect(10, 20, 300, 400)
                                               }
                               };

                settings.Save();

                Assert.That(File.Exists(file), Is.True, "the store should write the file it was given");

                var reloaded = new WindowPlacementFileSettings(file);
                reloaded.Reload();

                Assert.That(reloaded.Placement, Is.Not.Null);
                Assert.That(reloaded.Placement!.showCmd, Is.EqualTo(3u));
                Assert.That(reloaded.Placement.minPosition, Is.EqualTo(new Point(-1, -2)));
                Assert.That(reloaded.Placement.maxPosition, Is.EqualTo(new Point(-3, -4)));
                Assert.That(reloaded.Placement.normalPosition, Is.EqualTo(new Rect(10, 20, 300, 400)));
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Test]
        public void ShouldReportNoPlacementWhenTheFileIsMissing()
        {
            var file = Path.Combine(Path.GetTempPath(), $"mahapps-placement-{Guid.NewGuid():N}.xml");
            var settings = new WindowPlacementFileSettings(file);

            Assert.That(() => settings.Reload(), Throws.Nothing, "a missing file is the normal first start, not an error");
            Assert.That(settings.Placement, Is.Null);
        }

        [Test]
        public void ShouldForgetThePlacementOnReset()
        {
            var file = Path.Combine(Path.GetTempPath(), $"mahapps-placement-{Guid.NewGuid():N}.xml");

            try
            {
                var settings = new WindowPlacementFileSettings(file)
                               {
                                   Placement = new WindowPlacementSetting { normalPosition = new Rect(1, 2, 3, 4) }
                               };
                settings.Save();

                settings.Reset();

                Assert.That(settings.Placement, Is.Null, "reset should drop the placement");
                Assert.That(File.Exists(file), Is.False, "reset should remove the file");
            }
            finally
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        [Test]
        public void ShouldSaveTheWindowPlacementIntoTheFile()
        {
            var file = Path.Combine(Path.GetTempPath(), $"mahapps-placement-{Guid.NewGuid():N}.xml");
            var settings = new WindowPlacementFileSettings(file);

            var window = new TestWindow
                         {
                             Width = 800,
                             Height = 600,
                             ShowInTaskbar = false,
                             Left = int.MinValue,
                             Top = int.MinValue
                         };
            window.SetCurrentValue(MetroWindow.SaveWindowPositionProperty, true);
            window.SetCurrentValue(MetroWindow.WindowPlacementSettingsProperty, settings);

            try
            {
                window.Show();
                window.Close();

                Assert.That(File.Exists(file), Is.True, "closing the window should have written the placement");

                var reloaded = new WindowPlacementFileSettings(file);
                reloaded.Reload();
                Assert.That(reloaded.Placement, Is.Not.Null);
                Assert.That(reloaded.Placement!.normalPosition.IsEmpty, Is.False);
            }
            finally
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

        [Test]
        public void ShouldOpenTheWindowWhenTheSettingsAreBroken()
        {
            var window = new TestWindow
                         {
                             Width = 800,
                             Height = 600,
                             ShowInTaskbar = false,
                             Left = int.MinValue,
                             Top = int.MinValue
                         };
            window.SetCurrentValue(MetroWindow.SaveWindowPositionProperty, true);
            window.SetCurrentValue(MetroWindow.WindowPlacementSettingsProperty, new BrokenPlacementSettings());

            try
            {
                Assert.That(() => window.Show(), Throws.Nothing, "a broken settings store must not stop the window from opening");
            }
            finally
            {
                window.Close();
            }
        }

        [Test]
        public async Task ShouldSaveThePlacementThroughSettingsOfTheCaller()
        {
            var settings = new RecordingPlacementSettings();
            var window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);

            window.SetCurrentValue(MetroWindow.SaveWindowPositionProperty, true);
            window.SetCurrentValue(MetroWindow.WindowPlacementSettingsProperty, settings);

            window.Close();

            Assert.That(settings.Calls, Does.Contain("Save"), "the behavior should save through the settings of the caller");
            Assert.That(settings.Placement, Is.Not.Null, "the placement should have been handed to the settings");
            Assert.That(settings.Placement!.normalPosition.IsEmpty, Is.False, "the placement should carry the bounds of the window");
        }

        [Test]
        public void ShouldReadThePlacementFromSettingsOfTheCaller()
        {
            var settings = new RecordingPlacementSettings
                           {
                               Placement = new WindowPlacementSetting
                                           {
                                               showCmd = 1,
                                               normalPosition = new Rect(120, 130, 400, 300)
                                           }
                           };

            var window = new TestWindow
                         {
                             Width = 800,
                             Height = 600,
                             ShowInTaskbar = false,
                             Left = int.MinValue,
                             Top = int.MinValue
                         };
            window.SetCurrentValue(MetroWindow.SaveWindowPositionProperty, true);
            window.SetCurrentValue(MetroWindow.WindowPlacementSettingsProperty, settings);

            try
            {
                window.Show();

                Assert.That(settings.Calls, Does.Contain("Reload"), "the behavior should read through the settings of the caller");
                Assert.That(window.RestoreBounds.Width, Is.EqualTo(400).Within(1), "the window should take the width from the settings");
                Assert.That(window.RestoreBounds.Height, Is.EqualTo(300).Within(1), "the window should take the height from the settings");
            }
            finally
            {
                window.Close();
            }
        }
    }
}
