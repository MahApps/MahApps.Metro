// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class PipsPagerTests : WindowTestFixture<TestWindow>
    {
        private CultureInfo? previousUICulture;
        private CultureInfo? previousCulture;

        [SetUp]
        public void UseEnglish()
        {
            this.previousUICulture = CultureInfo.CurrentUICulture;
            this.previousCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [TearDown]
        public void RestoreCulture()
        {
            CultureInfo.CurrentUICulture = this.previousUICulture!;
            CultureInfo.CurrentCulture = this.previousCulture!;
        }

        private PipsPager Show(PipsPager pager)
        {
            this.window!.Content = pager;
            this.window.UpdateLayout();
            return pager;
        }

        [TestCase("MahApps.Styles.PipsPager.Win10")]
        [TestCase("MahApps.Styles.PipsPager.WinUI")]
        [Description("The pips and the arrows of a look take their corners from where they are, so neither the pip the pager measures outside the tree nor a change of look leaves a binding that finds no pager.")]
        public void ALookBindsNothingItCannotFind(string key)
        {
            var warnings = BindingWarnings.During(() =>
                {
                    var pager = this.Show(new PipsPager
                                          {
                                              NumberOfPages = 10,
                                              PreviousButtonVisibility = PipsPagerButtonVisibility.Visible,
                                              NextButtonVisibility = PipsPagerButtonVisibility.Visible
                                          });
                    pager.Style = (Style)this.window!.FindResource(key);
                    pager.UpdateLayout();
                    pager.Style = (Style)this.window.FindResource("MahApps.Styles.PipsPager");
                    pager.UpdateLayout();
                    ClipAssert.Pump();
                });

            Assert.That(warnings.Where(m => m.Contains(nameof(PipsPager))), Is.Empty);
        }

        private static Button PreviousButton(PipsPager pager) => pager.FindChild<Button>("PART_PreviousPageButton")!;

        private static Button NextButton(PipsPager pager) => pager.FindChild<Button>("PART_NextPageButton")!;

        private static void Click(ButtonBase button) => button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

        private static List<Button> Pips(PipsPager pager)
        {
            pager.UpdateLayout();
            var pips = pager.FindChild<ItemsControl>("PART_Pips")!;
            return Enumerable.Range(0, pips.Items.Count)
                             .Select(i => pips.ItemContainerGenerator.ContainerFromIndex(i))
                             .Select(c => c.FindChildren<Button>(true).First())
                             .ToList();
        }

        [Test]
        [Description("A new pager starts where the one of WinUI does.")]
        public void DefaultsAreThoseOfWinUI()
        {
            var pager = new PipsPager();

            Assert.Multiple(() =>
                {
                    Assert.That(pager.NumberOfPages, Is.EqualTo(-1));
                    Assert.That(pager.SelectedPageIndex, Is.EqualTo(0));
                    Assert.That(pager.MaxVisiblePips, Is.EqualTo(5));
                    Assert.That(pager.PreviousButtonVisibility, Is.EqualTo(PipsPagerButtonVisibility.Collapsed));
                    Assert.That(pager.NextButtonVisibility, Is.EqualTo(PipsPagerButtonVisibility.Collapsed));
                    Assert.That(pager.WrapMode, Is.EqualTo(PipsPagerWrapMode.None));
                    Assert.That(pager.Orientation, Is.EqualTo(Orientation.Horizontal));
                    Assert.That(pager.TemplateSettings, Is.Not.Null);
                });
        }

        [Test]
        [Description("A page index outside the pages is pulled back to the nearest page, and the event comes once, for the page it ends on.")]
        public void TheIndexIsKeptWithinThePages()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 3 });
            var indexes = new List<int>();
            pager.SelectedIndexChanged += (_, _) => indexes.Add(pager.SelectedPageIndex);

            pager.SelectedPageIndex = 7;
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(2));

            pager.SelectedPageIndex = -1;
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(0));

            Assert.That(indexes, Is.EqualTo(new[] { 2, 0 }));
        }

        [Test]
        [Description("Fewer pages than the selected one pull the selection back to the last page.")]
        public void FewerPagesPullTheSelectionBack()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5, SelectedPageIndex = 4 });

            pager.NumberOfPages = 2;

            Assert.That(pager.SelectedPageIndex, Is.EqualTo(1));
        }

        [Test]
        [Description("There is one pip per page, numbered from one.")]
        public void ThereIsOnePipPerPage()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 4 });
            Assert.That(pager.TemplateSettings.PipsPagerItems, Is.EqualTo(new[] { 1, 2, 3, 4 }));

            pager.NumberOfPages = 2;
            Assert.That(pager.TemplateSettings.PipsPagerItems, Is.EqualTo(new[] { 1, 2 }));

            pager.NumberOfPages = 0;
            Assert.That(pager.TemplateSettings.PipsPagerItems, Is.Empty);
        }

        [Test]
        [Description("With an unknown number of pages there are as many pips as may be seen, and one more whenever the last one is reached.")]
        public void UnknownPagesGrowWithTheSelection()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = -1, MaxVisiblePips = 5 });
            Assert.That(pager.TemplateSettings.PipsPagerItems, Has.Count.EqualTo(5));

            pager.SelectedPageIndex = 4;
            Assert.That(pager.TemplateSettings.PipsPagerItems, Has.Count.EqualTo(6));

            pager.SelectedPageIndex = 9;
            Assert.That(pager.TemplateSettings.PipsPagerItems, Has.Count.EqualTo(10));
        }

        [Test]
        [Description("With an unknown number of pages the next arrow goes on to a new page and never wraps to the first.")]
        public void NextAtTheLastUnknownPageAddsOne()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = -1, WrapMode = PipsPagerWrapMode.Wrap, SelectedPageIndex = 4 });

            Click(NextButton(pager));

            Assert.That(pager.SelectedPageIndex, Is.EqualTo(5));
        }

        [TestCase(PipsPagerWrapMode.None, 2, 0)]
        [TestCase(PipsPagerWrapMode.Wrap, 0, 2)]
        [Description("The arrows move a page on or back, and wrap around at the ends if asked to.")]
        public void TheArrowsMoveBetweenPages(PipsPagerWrapMode wrapMode, int nextFromLast, int previousFromFirst)
        {
            var pager = this.Show(new PipsPager
                                  {
                                      NumberOfPages = 3,
                                      WrapMode = wrapMode,
                                      PreviousButtonVisibility = PipsPagerButtonVisibility.Visible,
                                      NextButtonVisibility = PipsPagerButtonVisibility.Visible
                                  });

            pager.SelectedPageIndex = 2;
            Click(NextButton(pager));
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(nextFromLast));

            pager.SelectedPageIndex = 0;
            Click(PreviousButton(pager));
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(previousFromFirst));

            pager.SelectedPageIndex = 1;
            Click(NextButton(pager));
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(2));
            Click(PreviousButton(pager));
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(1));

            foreach (var pages in new[] { 0, 1 })
            {
                pager.NumberOfPages = pages;
                var index = pager.SelectedPageIndex;
                Click(NextButton(pager));
                Click(PreviousButton(pager));
                Assert.That(pager.SelectedPageIndex, Is.EqualTo(index), $"{pages} pages");
            }
        }

        [Test]
        [Description("An arrow with nowhere to go is hidden and switched off, unless the pager wraps around.")]
        public void TheArrowsHideAtTheEdges()
        {
            var pager = this.Show(new PipsPager
                                  {
                                      NumberOfPages = 3,
                                      PreviousButtonVisibility = PipsPagerButtonVisibility.Visible,
                                      NextButtonVisibility = PipsPagerButtonVisibility.Visible
                                  });
            var previous = PreviousButton(pager);
            var next = NextButton(pager);

            Assert.Multiple(() =>
                {
                    Assert.That(previous.Visibility, Is.EqualTo(Visibility.Visible));
                    Assert.That(previous.Opacity, Is.EqualTo(0), "previous on the first page");
                    Assert.That(previous.IsEnabled, Is.False, "previous on the first page");
                    Assert.That(next.Opacity, Is.EqualTo(1), "next on the first page");
                    Assert.That(next.IsEnabled, Is.True, "next on the first page");
                });

            pager.SelectedPageIndex = 1;
            Assert.Multiple(() =>
                {
                    Assert.That(previous.Opacity, Is.EqualTo(1), "previous on the second page");
                    Assert.That(previous.IsEnabled, Is.True, "previous on the second page");
                });

            pager.SelectedPageIndex = 2;
            Assert.Multiple(() =>
                {
                    Assert.That(next.Opacity, Is.EqualTo(0), "next on the last page");
                    Assert.That(next.IsEnabled, Is.False, "next on the last page");
                });

            pager.WrapMode = PipsPagerWrapMode.Wrap;
            pager.SelectedPageIndex = 0;
            Assert.Multiple(() =>
                {
                    Assert.That(previous.Opacity, Is.EqualTo(1), "previous on the first page with wrapping");
                    Assert.That(previous.IsEnabled, Is.True, "previous on the first page with wrapping");
                });

            pager.NumberOfPages = 0;
            Assert.Multiple(() =>
                {
                    Assert.That(previous.Opacity, Is.EqualTo(0), "previous without pages");
                    Assert.That(previous.IsEnabled, Is.False, "previous without pages");
                    Assert.That(next.IsEnabled, Is.False, "next without pages");
                });

            pager.PreviousButtonVisibility = PipsPagerButtonVisibility.Collapsed;
            Assert.That(previous.Visibility, Is.EqualTo(Visibility.Collapsed));
        }

        [Test]
        [Description("An arrow that shows on pointer over is there only while the pointer or the keyboard focus is in the pager.")]
        public void VisibleOnPointerOverShowsTheArrowOnlyWithThePointerOrFocus()
        {
            var pager = this.Show(new PipsPager
                                  {
                                      NumberOfPages = 5,
                                      SelectedPageIndex = 2,
                                      PreviousButtonVisibility = PipsPagerButtonVisibility.VisibleOnPointerOver
                                  });
            var previous = PreviousButton(pager);

            Assert.Multiple(() =>
                {
                    Assert.That(previous.Opacity, Is.EqualTo(0), "without the pointer");
                    Assert.That(previous.IsEnabled, Is.True, "without the pointer");
                });

            pager.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseEnterEvent });
            Assert.That(previous.Opacity, Is.EqualTo(1), "with the pointer");

            pager.RaiseEvent(new MouseEventArgs(Mouse.PrimaryDevice, 0) { RoutedEvent = Mouse.MouseLeaveEvent });
            Assert.That(previous.Opacity, Is.EqualTo(0), "after the pointer left");

            Assert.That(Pips(pager)[2].Focus(), Is.True, "the pip should take the focus");
            Assert.That(previous.Opacity, Is.EqualTo(1), "with the keyboard focus on a pip");

            Keyboard.ClearFocus();
            Assert.That(previous.Opacity, Is.EqualTo(0), "after the focus left");
        }

        [Test]
        [Description("Clicking a pip selects its page, once.")]
        public void ClickingAPipSelectsIt()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5 });
            var raised = 0;
            pager.SelectedIndexChanged += (_, _) => raised++;

            Click(Pips(pager)[2]);

            Assert.Multiple(() =>
                {
                    Assert.That(pager.SelectedPageIndex, Is.EqualTo(2));
                    Assert.That(raised, Is.EqualTo(1));
                });
        }

        [Test]
        [Description("The pip of the selected page wears the selected style, every other one the normal style.")]
        public void TheSelectedPipWearsTheSelectedStyle()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5, SelectedPageIndex = 1 });

            void AssertStyles(int selected)
            {
                var pips = Pips(pager);
                Assert.That(pips, Has.Count.EqualTo(5));
                Assert.Multiple(() =>
                    {
                        for (var i = 0; i < pips.Count; i++)
                        {
                            Assert.That(pips[i].Style, Is.SameAs(i == selected ? pager.SelectedPipStyle : pager.NormalPipStyle), $"pip {i} with page {selected} selected");
                        }
                    });
            }

            Assert.That(pager.SelectedPipStyle, Is.Not.Null);
            Assert.That(pager.NormalPipStyle, Is.Not.Null);
            Assert.That(pager.SelectedPipStyle, Is.Not.SameAs(pager.NormalPipStyle));

            AssertStyles(1);

            pager.SelectedPageIndex = 3;
            AssertStyles(3);
        }

        [TestCase(12d, 12d, 10, 5, 60d)]
        [TestCase(12d, 12d, 3, 5, 36d)]
        [TestCase(12d, 12d, -1, 5, 60d)]
        [TestCase(12d, 12d, 0, 5, 0d)]
        [TestCase(12d, 12d, 10, 0, 0d)]
        [TestCase(12d, 12d, 10, -3, 0d)]
        [TestCase(10d, 14d, 10, 5, 54d)]
        [Description("The window on the pips is as big as the pips that may be seen, and one selected pip among them, and none without pages or room.")]
        public void TheScrollViewerSizeIsThatOfTheVisiblePips(double defaultPipSize, double selectedPipSize, int numberOfPages, int maxVisiblePips, double expected)
        {
            var method = typeof(PipsPager).GetMethod("CalculateScrollViewerSize", BindingFlags.NonPublic | BindingFlags.Static)!;

            var size = (double)method.Invoke(null, new object[] { defaultPipSize, selectedPipSize, numberOfPages, maxVisiblePips })!;

            Assert.That(size, Is.EqualTo(expected));
        }

        [TestCase(Orientation.Horizontal)]
        [TestCase(Orientation.Vertical)]
        [Description("The pips are seen no more than MaxVisiblePips at once, along the pager.")]
        public void TheWindowIsAsBigAsMaxVisiblePips(Orientation orientation)
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 20, MaxVisiblePips = 5, Orientation = orientation });
            var viewer = pager.FindChild<ScrollViewer>("PART_ScrollViewer")!;
            var pip = Pips(pager)[0];
            var horizontal = orientation == Orientation.Horizontal;

            Assert.Multiple(() =>
                {
                    Assert.That(pip.DesiredSize.Width, Is.GreaterThan(0));
                    Assert.That(horizontal ? viewer.MaxWidth : viewer.MaxHeight, Is.EqualTo(5 * (horizontal ? pip.DesiredSize.Width : pip.DesiredSize.Height)));
                    Assert.That(horizontal ? viewer.MaxHeight : viewer.MaxWidth, Is.EqualTo(horizontal ? pip.DesiredSize.Height : pip.DesiredSize.Width));
                    Assert.That(horizontal ? viewer.ViewportWidth : viewer.ViewportHeight, Is.EqualTo(horizontal ? viewer.MaxWidth : viewer.MaxHeight));
                });

            pager.MaxVisiblePips = 3;
            pager.UpdateLayout();
            Assert.That(horizontal ? viewer.MaxWidth : viewer.MaxHeight, Is.EqualTo(3 * (horizontal ? pip.DesiredSize.Width : pip.DesiredSize.Height)));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(18)]
        [TestCase(19)]
        [Description("The selected pip is scrolled to the middle of the window, as far as the ends allow it.")]
        public void TheSelectedPipScrollsToTheMiddle(int index)
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 20, MaxVisiblePips = 5 });
            var viewer = pager.FindChild<ScrollViewer>("PART_ScrollViewer")!;

            pager.SelectedPageIndex = index;
            pager.UpdateLayout();
            pager.UpdateLayout();

            var container = pager.FindChild<ItemsControl>("PART_Pips")!.ItemContainerGenerator.ContainerFromIndex(index) as FrameworkElement;
            var center = container!.TranslatePoint(new Point(container.ActualWidth / 2, 0), pager.FindChild<ItemsControl>("PART_Pips")).X;
            var expected = Math.Max(0, Math.Min(viewer.ScrollableWidth, center - viewer.ViewportWidth / 2));

            Assert.Multiple(() =>
                {
                    Assert.That(viewer.ScrollableWidth, Is.GreaterThan(0));
                    Assert.That(viewer.HorizontalOffset, Is.EqualTo(expected).Within(0.5));
                    if (index == 0)
                    {
                        Assert.That(viewer.HorizontalOffset, Is.EqualTo(0));
                    }

                    if (index == 19)
                    {
                        Assert.That(viewer.HorizontalOffset, Is.EqualTo(viewer.ScrollableWidth).Within(0.5));
                    }
                });
        }

        [Test]
        [Description("A pip that asks to be brought into view doesn't scroll the pips.")]
        public void AFocusedPipDoesNotScroll()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 20, MaxVisiblePips = 5 });
            var viewer = pager.FindChild<ScrollViewer>("PART_ScrollViewer")!;
            pager.UpdateLayout();

            Pips(pager)[19].BringIntoView();
            pager.UpdateLayout();

            Assert.That(viewer.HorizontalOffset, Is.EqualTo(0));
        }

        private static void PressKey(IInputElement target, Key key, PipsPager pager)
        {
            // as the input manager does, the key tunnels down first and bubbles up if nobody took it
            var args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(pager), 0, key) { RoutedEvent = Keyboard.PreviewKeyDownEvent };
            target.RaiseEvent(args);
            if (!args.Handled)
            {
                args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(pager), 0, key) { RoutedEvent = Keyboard.KeyDownEvent };
                target.RaiseEvent(args);
            }

            Assert.That(args.Handled, Is.EqualTo(key is Key.Left or Key.Right or Key.Up or Key.Down or Key.Enter), $"{key} handled");
        }

        [TestCase(Orientation.Horizontal, Key.Right, Key.Left)]
        [TestCase(Orientation.Horizontal, Key.Down, Key.Up)]
        [TestCase(Orientation.Vertical, Key.Down, Key.Up)]
        [TestCase(Orientation.Vertical, Key.Right, Key.Left)]
        [Description("Left and up go to the previous pip, right and down to the next one, in whichever direction the pager runs, and the selection stays.")]
        public void ArrowKeysMoveTheFocusBetweenPips(Orientation orientation, Key next, Key previous)
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5, SelectedPageIndex = 1, Orientation = orientation });
            var pips = Pips(pager);
            Assert.That(pips[1].Focus(), Is.True);

            PressKey(Keyboard.FocusedElement, next, pager);
            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[2]), "after the next key");

            PressKey(Keyboard.FocusedElement, previous, pager);
            PressKey(Keyboard.FocusedElement, previous, pager);
            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[0]), "after two previous keys");

            Assert.That(pager.SelectedPageIndex, Is.EqualTo(1));
        }

        [Test]
        [Description("The key is handled at the first pip, and the focus stays where it is.")]
        public void ThePreviousKeyAtTheFirstPipIsHandledAndKeepsTheFocus()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5, SelectedPageIndex = 0 });
            var pips = Pips(pager);
            Assert.That(pips[0].Focus(), Is.True);

            PressKey(pips[0], Key.Left, pager);

            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[0]));
        }

        [Test]
        [Description("Enter on a focused pip presses it and so selects its page.")]
        public void EnterOnAFocusedPipSelectsIt()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5 });
            var pips = Pips(pager);

            PressKey(pips[2], Key.Enter, pager);

            Assert.That(pager.SelectedPageIndex, Is.EqualTo(2));
        }

        [Test]
        [Description("A pip says which page it is, and how many there are, the arrows say where they go.")]
        public void PipsAndArrowsHaveNames()
        {
            var pager = this.Show(new PipsPager
                                  {
                                      NumberOfPages = 5,
                                      PreviousButtonVisibility = PipsPagerButtonVisibility.Visible,
                                      NextButtonVisibility = PipsPagerButtonVisibility.Visible
                                  });
            var pip = Pips(pager)[2];

            Assert.Multiple(() =>
                {
                    Assert.That(AutomationProperties.GetName(pip), Is.EqualTo("Page 3"));
#if NETCOREAPP
                    Assert.That(AutomationProperties.GetPositionInSet(pip), Is.EqualTo(3));
                    Assert.That(AutomationProperties.GetSizeOfSet(pip), Is.EqualTo(5));
#endif
                    Assert.That(AutomationProperties.GetName(PreviousButton(pager)), Is.EqualTo("Previous Page"));
                    Assert.That(AutomationProperties.GetName(NextButton(pager)), Is.EqualTo("Next Page"));
                });

            pager.NumberOfPages = 8;
#if NETCOREAPP
            Assert.That(AutomationProperties.GetSizeOfSet(Pips(pager)[2]), Is.EqualTo(8), "after the number of pages changed");
#endif
        }

        [Test]
        [Description("Tabbing into the pager lands on the selected pip, and the other pips are no tab stops.")]
        public void TabbingInLandsOnTheSelectedPip()
        {
            var before = new Button { Content = "before" };
            var pager = new PipsPager { NumberOfPages = 5, SelectedPageIndex = 3 };
            var panel = new StackPanel();
            panel.Children.Add(before);
            panel.Children.Add(pager);
            this.window!.Content = panel;
            this.window.UpdateLayout();
            var pips = Pips(pager);

            Assert.That(before.Focus(), Is.True);
            before.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[3]), "after one tab");

            ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            Assert.That(pager.IsKeyboardFocusWithin, Is.False, "after two tabs");
        }

        [Test]
        [Description("Focus from outside lands on the pip of the selected page, focus from a pip goes where it is asked to.")]
        public void FocusFromOutsideGoesToTheSelectedPip()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 5, SelectedPageIndex = 3 });
            var pips = Pips(pager);

            pips[0].Focus();
            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[3]), "from outside");

            pips[1].Focus();
            Assert.That(Keyboard.FocusedElement, Is.SameAs(pips[1]), "from a pip");
        }

        [Test]
        [Description("An arrow that shows on pointer over stays while the focus moves from pip to pip.")]
        public void TheArrowStaysWhileTheFocusMovesBetweenPips()
        {
            var pager = this.Show(new PipsPager
                                  {
                                      NumberOfPages = 5,
                                      SelectedPageIndex = 2,
                                      PreviousButtonVisibility = PipsPagerButtonVisibility.VisibleOnPointerOver
                                  });
            var pips = Pips(pager);
            Assert.That(pips[2].Focus(), Is.True);
            Assert.That(PreviousButton(pager).Opacity, Is.EqualTo(1));

            Assert.That(pips[3].Focus(), Is.True);

            Assert.That(PreviousButton(pager).Opacity, Is.EqualTo(1));
        }

        private const string Win10Key = "MahApps.Styles.PipsPager.Win10";
        private const string WinUIKey = "MahApps.Styles.PipsPager.WinUI";

        private PipsPager ShowLook(string styleKey, Orientation orientation = Orientation.Horizontal, int numberOfPages = 3, int maxVisiblePips = 5)
        {
            return this.Show(new PipsPager
                             {
                                 Style = (Style)this.window!.FindResource(styleKey),
                                 NumberOfPages = numberOfPages,
                                 MaxVisiblePips = maxVisiblePips,
                                 Orientation = orientation,
                                 PreviousButtonVisibility = PipsPagerButtonVisibility.Visible,
                                 NextButtonVisibility = PipsPagerButtonVisibility.Visible,
                                 SelectedPageIndex = 1
                             });
        }

        /// <summary>
        /// IsMouseOver and IsPressed cannot be set, so the template is read for what its triggers write.
        /// </summary>
        private static Setter[] TemplateSetters(Control control, DependencyProperty trigger)
        {
            return control.Template.Triggers.OfType<Trigger>().Where(t => t.Property == trigger).SelectMany(t => t.Setters.OfType<Setter>()).ToArray();
        }

        private object Resource(object? key) => this.window!.FindResource(key is DynamicResourceExtension dynamic ? dynamic.ResourceKey : key);

        [TestCase(Win10Key, 20d)]
        [TestCase(WinUIKey, 24d)]
        [Description("The pip of the original is the dot of EA3B in a cell of 12 by 20 (Windows 10) or 12 by 24 (WinUI), 20 by 12 or 24 by 12 when the pips run top to bottom, of size 4 and of size 6 when selected, in the strong control fill and under the pointer in the secondary text.")]
        public void TheFluentPipIsTheOriginal(string key, double longSide)
        {
            var pager = this.ShowLook(key);
            var pips = Pips(pager);
            var normal = pips[0];
            var selected = pips[1];
            var strong = this.window!.FindResource("MahApps.Brushes.WinUI.ControlStrongFillDefault");
            var secondary = this.window.FindResource("MahApps.Brushes.WinUI.TextSecondary");

            var over = TemplateSetters(normal, UIElement.IsMouseOverProperty).Single(s => s.TargetName == "Dot" && s.Property == Control.ForegroundProperty);
            var overSelected = TemplateSetters(selected, UIElement.IsMouseOverProperty).Single(s => s.TargetName == "Dot" && s.Property == Control.ForegroundProperty);

            Assert.Multiple(() =>
                {
                    Assert.That(normal.Style, Is.SameAs(pager.NormalPipStyle));
                    Assert.That(selected.Style, Is.SameAs(pager.SelectedPipStyle));
                    Assert.That(normal.FindChild<FontIcon>()!.Glyph, Is.EqualTo(""));
                    Assert.That(normal.FindChild<FontIcon>()!.FontSize, Is.EqualTo(4d), "normal");
                    Assert.That(selected.FindChild<FontIcon>()!.FontSize, Is.EqualTo(6d), "selected");
                    Assert.That(normal.FindChild<FontIcon>()!.Foreground, Is.SameAs(strong), "normal");
                    Assert.That(selected.FindChild<FontIcon>()!.Foreground, Is.SameAs(strong), "selected");
                    Assert.That((normal.Width, normal.Height), Is.EqualTo((12d, longSide)), "cell");
                    Assert.That(this.Resource(over.Value), Is.SameAs(secondary), "under the pointer");
                    Assert.That(this.Resource(overSelected.Value), Is.SameAs(secondary), "selected under the pointer");
                    Assert.That(normal.Margin, Is.EqualTo(new Thickness(0)));
                });

            var vertical = Pips(this.ShowLook(key, Orientation.Vertical))[0];
            Assert.That((vertical.Width, vertical.Height), Is.EqualTo((longSide, 12d)), "vertical cell");
        }

        [TestCase(Win10Key, 20d)]
        [TestCase(WinUIKey, 24d)]
        [Description("The arrows of the original are square, EDDB for the previous and EDDC for the next page at size 8, turned by -90 degrees when the pips run left to right, and a pressed one is scaled to 0.875.")]
        public void TheFluentArrowsAreTheOriginal(string key, double size)
        {
            // what a trigger or a binding found in the tree is read before the next pager takes the window
            var horizontal = this.ShowLook(key);
            ClipAssert.Pump();
            var previous = PreviousButton(horizontal);
            var next = NextButton(horizontal);
            var pressed = TemplateSetters(previous, ButtonBase.IsPressedProperty).Single(s => s.TargetName is not null && s.Property == UIElement.RenderTransformProperty);
            var corner = previous.FindChild<Border>("RootGrid")!.CornerRadius;
            var pipCorner = Pips(horizontal)[0].FindChild<Border>("RootGrid")!.CornerRadius;
            var angles = (Previous: (previous.RenderTransform as RotateTransform)?.Angle, Next: (next.RenderTransform as RotateTransform)?.Angle);
            var origin = previous.RenderTransformOrigin;
            var sizes = ((previous.Width, previous.Height), (next.Width, next.Height));
            var icon = previous.FindChild<FontIcon>()!;
            var nextIcon = next.FindChild<FontIcon>()!;
            var (glyph, fontSize, foreground) = (icon.Glyph, icon.FontSize, icon.Foreground);

            var vertical = this.ShowLook(key, Orientation.Vertical);

            Assert.Multiple(() =>
                {
                    Assert.That(sizes, Is.EqualTo(((size, size), (size, size))));
                    Assert.That(glyph, Is.EqualTo(""), "previous");
                    Assert.That(nextIcon.Glyph, Is.EqualTo(""), "next");
                    Assert.That(fontSize, Is.EqualTo(8d));
                    Assert.That(foreground, Is.SameAs(this.window!.FindResource("MahApps.Brushes.WinUI.ControlStrongFillDefault")));
                    Assert.That(angles, Is.EqualTo(((double?)-90d, (double?)-90d)), "horizontal");
                    Assert.That(origin, Is.EqualTo(new Point(0.5, 0.5)));
                    Assert.That(PreviousButton(vertical).RenderTransform, Is.Not.InstanceOf<RotateTransform>(), "previous vertical");
                    Assert.That(NextButton(vertical).RenderTransform, Is.Not.InstanceOf<RotateTransform>(), "next vertical");
                    Assert.That(((ScaleTransform)pressed.Value).ScaleX, Is.EqualTo(0.875));
                    Assert.That(((ScaleTransform)pressed.Value).ScaleY, Is.EqualTo(0.875));
                    Assert.That(corner, Is.EqualTo(new CornerRadius(key == WinUIKey ? 4 : 0)), "arrow");
                    Assert.That(pipCorner, Is.EqualTo(new CornerRadius(key == WinUIKey ? 4 : 0)), "pip");
                });
        }

        [TestCase(Win10Key, 20d)]
        [TestCase(WinUIKey, 24d)]
        [Description("The window on the pips is measured on the cell of the look, along the pager by as many cells as may be seen, and across it by one.")]
        public void TheWindowFollowsTheCellOfTheLook(string key, double longSide)
        {
            var horizontal = this.ShowLook(key, numberOfPages: 20);
            var horizontalViewer = horizontal.FindChild<ScrollViewer>("PART_ScrollViewer")!;
            var vertical = this.ShowLook(key, Orientation.Vertical, 20);
            var verticalViewer = vertical.FindChild<ScrollViewer>("PART_ScrollViewer")!;

            Assert.Multiple(() =>
                {
                    Assert.That((horizontalViewer.MaxWidth, horizontalViewer.MaxHeight), Is.EqualTo((5 * 12d, longSide)), "horizontal");
                    Assert.That((verticalViewer.MaxWidth, verticalViewer.MaxHeight), Is.EqualTo((longSide, 5 * 12d)), "vertical");
                });
        }

        [Test]
        [Description("The pips are measured again when the orientation or the styles of the pips change.")]
        public void ThePipsAreMeasuredAgainAfterAChange()
        {
            var pager = this.ShowLook(WinUIKey, numberOfPages: 20);
            var viewer = pager.FindChild<ScrollViewer>("PART_ScrollViewer")!;
            Assert.That((viewer.MaxWidth, viewer.MaxHeight), Is.EqualTo((60d, 24d)), "horizontal");

            pager.Orientation = Orientation.Vertical;
            pager.UpdateLayout();
            Assert.That((viewer.MaxWidth, viewer.MaxHeight), Is.EqualTo((24d, 60d)), "after the orientation changed");

            pager.NormalPipStyle = (Style)this.window!.FindResource("MahApps.Styles.Button.PipsPager.Pip.Win10");
            pager.SelectedPipStyle = (Style)this.window.FindResource("MahApps.Styles.Button.PipsPager.Pip.Selected.Win10");
            pager.UpdateLayout();
            Assert.That((viewer.MaxWidth, viewer.MaxHeight), Is.EqualTo((20d, 60d)), "after the styles changed");

            pager.Orientation = Orientation.Horizontal;
            pager.UpdateLayout();
            Assert.That((viewer.MaxWidth, viewer.MaxHeight), Is.EqualTo((60d, 20d)), "after both");
        }

        [Test]
        [Description("The Metro pip is the ellipse the index of the FlipView has always shown.")]
        public void TheMetroPipIsTheOldIndexEllipse()
        {
            var pager = this.Show(new PipsPager { NumberOfPages = 3 });
            var size = (double)pager.FindResource("FlipViewIndexItemSize");
            var pips = Pips(pager);
            var selected = pips[0].FindChildren<Ellipse>(true).Single();
            var normal = pips[1].FindChildren<Ellipse>(true).Single();

            Assert.Multiple(() =>
                {
                    Assert.That(size, Is.EqualTo(15));
                    Assert.That(selected.Width, Is.EqualTo(size));
                    Assert.That(selected.Height, Is.EqualTo(size));
                    Assert.That(normal.Width, Is.EqualTo(size), "the selected pip is no bigger than the others");
                    Assert.That(selected.Fill, Is.SameAs(pager.FindResource("MahApps.Brushes.SystemControlHighlightListAccentMedium")));
                    Assert.That(selected.Stroke, Is.SameAs(pager.FindResource("MahApps.Brushes.SystemControlForegroundBaseMedium")));
                    Assert.That(normal.Fill, Is.SameAs(pager.FindResource("MahApps.Brushes.SystemControlTransparent")));
                    Assert.That(normal.Stroke, Is.SameAs(pager.FindResource("MahApps.Brushes.SystemControlForegroundBaseMedium")));
                });
        }
    }
}
