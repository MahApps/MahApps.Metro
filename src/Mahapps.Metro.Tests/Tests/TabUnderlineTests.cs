// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The line under a tab is painted from four brushes, one per state. What is measured here is that
    /// a state nobody handed a brush to falls back to the nearest one that was, instead of leaving the
    /// line unpainted, and that both templates ask the same element whether the mouse is on the tab.
    /// </summary>
    [TestFixture]
    public class TabUnderlineTests
    {
        private HeaderedControlHelperTestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<HeaderedControlHelperTestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            string[] properties =
            [
                TabControlHelper.UnderlineBrushProperty.Name,
                TabControlHelper.UnderlineSelectedBrushProperty.Name,
                TabControlHelper.UnderlineMouseOverBrushProperty.Name,
                TabControlHelper.UnderlineMouseOverSelectedBrushProperty.Name
            ];

            this.window?.TestTabControl.ClearDependencyProperties(properties);
            this.window?.TestTabItem.ClearDependencyProperties(properties);
            this.window?.TestMetroTabControl.ClearDependencyProperties(properties);
            this.window?.TestMetroTabItem.ClearDependencyProperties(properties);
        }

        [Test]
        [Description("A line under the tab that is showing, painted with the brush for that state.")]
        public void TheSelectedTabTakesTheBrushForThatState()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.TestTabControl.SetCurrentValue(TabControlHelper.UnderlineSelectedBrushProperty, Brushes.HotPink);
            this.window.TestMetroTabControl.SetCurrentValue(TabControlHelper.UnderlineSelectedBrushProperty, Brushes.HotPink);

            Assert.That(Line(this.window.TestTabItem), Is.EqualTo(Brushes.HotPink));
            Assert.That(Line(this.window.TestMetroTabItem), Is.EqualTo(Brushes.HotPink));
        }

        [Test]
        [Description("Handed no brush for that state, the line is painted the way the other tabs are rather than not at all.")]
        public void TheSelectedTabFallsBackToThePlainBrush()
        {
            Assert.That(this.window, Is.Not.Null);

            this.window!.TestTabControl.SetCurrentValue(TabControlHelper.UnderlineBrushProperty, Brushes.Beige);
            this.window.TestTabControl.SetCurrentValue(TabControlHelper.UnderlineSelectedBrushProperty, null);
            this.window.TestMetroTabControl.SetCurrentValue(TabControlHelper.UnderlineBrushProperty, Brushes.Beige);
            this.window.TestMetroTabControl.SetCurrentValue(TabControlHelper.UnderlineSelectedBrushProperty, null);

            Assert.That(Line(this.window.TestTabItem), Is.EqualTo(Brushes.Beige));
            Assert.That(Line(this.window.TestMetroTabItem), Is.EqualTo(Brushes.Beige));
        }

        [Test]
        [Description("Both templates ask the strip whether the mouse is on the tab, so the line keeps its colour while the mouse is on the line itself.")]
        public void BothTemplatesAskTheStripAboutTheMouse()
        {
            Assert.That(this.window, Is.Not.Null);

            Assert.That(WhatIsAsked(this.window!.TestTabItem), Is.EqualTo("Border"), "the plain tab item asks the strip");
            Assert.That(WhatIsAsked(this.window.TestMetroTabItem), Is.EqualTo("Border"), "and so should the Metro one");
        }

        private static Brush? Line(TabItem item)
        {
            return item.FindChild<Underline>("Underline")?.BorderBrush;
        }

        /// <summary>
        /// The element the template watches for the mouse in the trigger that paints the line of the tab
        /// that is showing. Neither the mouse nor the keyboard can be handed to a window that is off
        /// screen, so the template is asked instead.
        /// </summary>
        private static string? WhatIsAsked(TabItem item)
        {
            foreach (var trigger in item.Template.Triggers)
            {
                if (trigger is not MultiTrigger many)
                {
                    continue;
                }

                var paintsTheLine = false;

                foreach (var setter in many.Setters)
                {
                    if (setter is Setter { TargetName: "Underline" } line
                        && line.Property == Border.BorderBrushProperty
                        && line.Value is System.Windows.Data.Binding { Path: not null } binding
                        && binding.Path.PathParameters.Contains(TabControlHelper.UnderlineMouseOverSelectedBrushProperty))
                    {
                        paintsTheLine = true;
                    }
                }

                if (paintsTheLine == false)
                {
                    continue;
                }

                foreach (var condition in many.Conditions)
                {
                    if (condition.Property == UIElement.IsMouseOverProperty)
                    {
                        return condition.SourceName;
                    }
                }
            }

            return null;
        }
    }
}
