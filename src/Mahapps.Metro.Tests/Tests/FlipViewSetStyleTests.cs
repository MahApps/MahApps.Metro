// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// The FlipView in the Windows 10 look, read off FlipView_themeresources_v1.xaml and
    /// PipsPager_themeresources.xaml of microsoft-ui-xaml (WinUI 2). The Metro look of the
    /// FlipView is not touched by it.
    /// </summary>
    [TestFixture]
    public class FlipViewSetStyleTests : WindowTestFixture<TestWindow>
    {
        private const string Win10Key = "MahApps.Styles.FlipView.Win10";
        private const string WinUIKey = "MahApps.Styles.FlipView.WinUI";

        private FlipView Show(string styleKey, Orientation orientation = Orientation.Horizontal, bool showIndex = false)
        {
            var flipView = new FlipView
            {
                Style = (Style)this.window!.FindResource(styleKey),
                Width = 400,
                Height = 200,
                Orientation = orientation,
                ShowIndex = showIndex,
            };
            flipView.Items.Add(new FlipViewItem { Content = "one" });
            flipView.Items.Add(new FlipViewItem { Content = "two" });
            flipView.Items.Add(new FlipViewItem { Content = "three" });
            this.window.Content = flipView;
            this.window.UpdateLayout();
            return flipView;
        }

        [TestCase(3)]
        [TestCase(12)]
        [Description("A dot of the Metro index stays whole when more pages than fit share the row, and takes the room it always took when they do fit.")]
        public void TheMetroIndexDotIsNeverCut(int pages)
        {
            var flipView = new FlipView
            {
                Style = (Style)this.window!.FindResource("MahApps.Styles.FlipView"),
                Width = 412,
                Height = 212,
                ShowIndex = true,
            };
            for (var i = 0; i < pages; i++)
            {
                flipView.Items.Add(new FlipViewItem { Content = i });
            }

            this.window.Content = flipView;
            this.window.UpdateLayout();

            var index = flipView.FindChild<ListBox>("PART_Index")!;
            var item = (ListBoxItem)index.ItemContainerGenerator.ContainerFromIndex(1);
            var dot = item.FindChild<System.Windows.Shapes.Ellipse>("Bd")!;

            // the layout rounds to whole device pixels, which are no whole units on a scaled display
            var pixel = 1 / VisualTreeHelper.GetDpi(dot).PixelsPerDip;

            Assert.Multiple(() =>
                {
                    Assert.That(dot.ActualWidth, Is.EqualTo(15d).Within(pixel), "the dot is as wide");
                    Assert.That(dot.ActualHeight, Is.EqualTo(15d).Within(pixel), "as it is high");
                    Assert.That(System.Windows.Controls.Primitives.LayoutInformation.GetLayoutClip(dot), Is.Null, "nothing of the dot is cut away");
                    if (pages == 3)
                    {
                        Assert.That(item.DesiredSize.Width, Is.EqualTo(15d + 8d + 20d).Within(pixel), "the dot, the padding of the theme and the margin, as before");
                    }
                });
        }

        [TestCase("MahApps.Styles.FlipView", Win10Key)]
        [TestCase(Win10Key, "MahApps.Styles.FlipView")]
        [TestCase("MahApps.Styles.FlipView", WinUIKey)]
        [Description("The dots of the index are list box items with a style of their own, which says where their content sits, so the theme style of the list box item has no alignment left to look up in a list box that is not there, as while the look changes.")]
        public void TheIndexBindsNothingItCannotFind(string fromKey, string toKey)
        {
            var warnings = BindingWarnings.During(() =>
                {
                    var flipView = this.Show(fromKey, showIndex: true);
                    flipView.Style = (Style)this.window!.FindResource(toKey);
                    this.window.UpdateLayout();
                    flipView.Style = (Style)this.window.FindResource(fromKey);
                    this.window.UpdateLayout();
                    ClipAssert.Pump();
                });

            Assert.That(warnings.Where(m => m.Contains(nameof(ListBoxItem))), Is.Empty);
        }

        [TestCase("MahApps.Styles.FlipView", Win10Key)]
        [TestCase(Win10Key, WinUIKey)]
        [TestCase(WinUIKey, "MahApps.Styles.FlipView")]
        [Description("A flip view whose banner has come up can change its look. The banner grows by an animation from the height its template gives it, and once the old template is gone that height is no number any more.")]
        public void TheLookCanChangeOnceTheBannerIsUp(string fromKey, string toKey)
        {
            var flipView = this.Show(fromKey);
            ClipAssert.Pump(300);

            Assert.That(() =>
                            {
                                flipView.Style = (Style)this.window!.FindResource(toKey);
                                this.window.UpdateLayout();
                                ClipAssert.Pump(600);
                            },
                        Throws.Nothing);
            // the layout rounds to whole device pixels, which are no whole units on a scaled display
            var pixel = 1 / VisualTreeHelper.GetDpi(flipView).PixelsPerDip;

            Assert.That(flipView.FindChild<Grid>("PART_BannerGrid")!.ActualHeight, Is.EqualTo(30d).Within(pixel), "the banner of the new look is up as well");
        }

        [Test]
        [Description("The next and previous buttons of the original are 20 by 36 for a horizontal flip view and 36 by 20 for a vertical one.")]
        public void TheWin10ButtonsHaveTheSizeOfTheOriginal()
        {
            var horizontal = this.Show(Win10Key);
            var back = horizontal.FindChild<Button>("PART_BackButton")!;
            var forward = horizontal.FindChild<Button>("PART_ForwardButton")!;

            var vertical = this.Show(Win10Key, Orientation.Vertical);
            var up = vertical.FindChild<Button>("PART_UpButton")!;
            var down = vertical.FindChild<Button>("PART_DownButton")!;

            Assert.Multiple(() =>
                {
                    Assert.That((back.Width, back.Height), Is.EqualTo((20d, 36d)), "back");
                    Assert.That((forward.Width, forward.Height), Is.EqualTo((20d, 36d)), "forward");
                    Assert.That((up.Width, up.Height), Is.EqualTo((36d, 20d)), "up");
                    Assert.That((down.Width, down.Height), Is.EqualTo((36d, 20d)), "down");
                });
        }

        [TestCase("PART_BackButton", "", Orientation.Horizontal)]
        [TestCase("PART_ForwardButton", "", Orientation.Horizontal)]
        [TestCase("PART_UpButton", "", Orientation.Vertical)]
        [TestCase("PART_DownButton", "", Orientation.Vertical)]
        [Description("The arrows are the glyphs of the original at size 12.")]
        public void TheWin10ArrowsAreTheGlyphsOfTheOriginal(string partName, string glyph, Orientation orientation)
        {
            var flipView = this.Show(Win10Key, orientation);
            var button = flipView.FindChild<Button>(partName)!;
            var icon = button.FindChild<FontIcon>();

            Assert.That(icon, Is.Not.Null, "the button should show a font icon");
            Assert.Multiple(() =>
                {
                    Assert.That(icon!.Glyph, Is.EqualTo(glyph));
                    Assert.That(icon.FontSize, Is.EqualTo(12d));
                });
        }

        [Test]
        [Description("The original has no frame around the whole control, so the hover border of the Metro look is switched off.")]
        public void TheWin10LookHasNoHoverBorder()
        {
            var flipView = this.Show(Win10Key);
            var border = flipView.FindChild<Border>("PART_HoverBorder")!;

            Assert.Multiple(() =>
                {
                    Assert.That(flipView.MouseHoverBorderEnabled, Is.False);
                    Assert.That(border.BorderThickness, Is.EqualTo(new Thickness(0)));
                });
        }

        [TestCase(Win10Key)]
        [TestCase(WinUIKey)]
        [Description("The buttons of the original only show while the pointer is over the flip view. IsMouseOver cannot be set, so the style of the button is read for the trigger.")]
        public void TheButtonsShowOnlyUnderThePointer(string styleKey)
        {
            var flipView = this.Show(styleKey);
            ClipAssert.Pump();
            var forward = flipView.FindChild<Button>("PART_ForwardButton")!;

            var trigger = forward.Style.Triggers
                                 .OfType<DataTrigger>()
                                 .FirstOrDefault(t => t.Binding is Binding { Path.Path: "IsMouseOver", RelativeSource: { Mode: RelativeSourceMode.FindAncestor } source }
                                                      && source.AncestorType == typeof(FlipView));

            Assert.Multiple(() =>
                {
                    Assert.That(forward.Opacity, Is.EqualTo(0d), "no pointer, no button");
                    Assert.That(forward.Visibility, Is.EqualTo(Visibility.Visible), "the visibility stays with the flip view");
                    Assert.That(trigger, Is.Not.Null, "a trigger on IsMouseOver of the flip view");
                    Assert.That(trigger!.Value?.ToString(), Is.EqualTo("True"));
                    Assert.That(trigger.Setters.OfType<Setter>().Any(s => s.Property == UIElement.OpacityProperty && Equals(s.Value, 1d)), Is.True, "which sets the opacity to 1");
                });
        }

        [Test]
        [Description("The dot of the index grows to size 6 under the pointer, as the pip of the PipsPager does. IsMouseOver cannot be set, so the template is read for the trigger.")]
        public void TheWin10IndexDotGrowsUnderThePointer()
        {
            var flipView = this.Show(Win10Key, showIndex: true);
            this.window!.UpdateLayout();
            var index = flipView.FindChild<ListBox>("PART_Index")!;
            var item = (ListBoxItem)index.ItemContainerGenerator.ContainerFromIndex(1);
            var trigger = item.Template.Triggers
                              .OfType<Trigger>()
                              .FirstOrDefault(t => t.Property == UIElement.IsMouseOverProperty);

            Assert.That(trigger, Is.Not.Null, "a trigger on IsMouseOver");
            Assert.That(trigger!.Setters.OfType<Setter>().Any(s => s.TargetName == "Dot" && s.Property.Name == "FontSize" && Equals(s.Value, 6d)), Is.True, "which sets the size to 6");
        }

        [TestCase(Win10Key)]
        [TestCase(WinUIKey)]
        [Description("The original sets IsTabStop to false on all four buttons, and they are invisible until the pointer comes, so Tab must not land on them. The arrow keys are handled by the flip view.")]
        public void TheNavigationButtonsAreNoTabStops(string styleKey)
        {
            var flipView = this.Show(styleKey);
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    foreach (var name in new[] { "PART_BackButton", "PART_ForwardButton", "PART_UpButton", "PART_DownButton" })
                    {
                        Assert.That(flipView.FindChild<Button>(name)!.IsTabStop, Is.False, name);
                    }
                });
        }

        [Test]
        [Description("The index is a row of dots in the strong control fill, a selected one of size 6 and the others 4, each in a cell of 12 by 20.")]
        public void TheWin10IndexIsADotWithoutAccent()
        {
            var flipView = this.Show(Win10Key, showIndex: true);
            flipView.SelectedIndex = 0;
            this.window!.UpdateLayout();
            var index = flipView.FindChild<ListBox>("PART_Index")!;
            var selected = (ListBoxItem)index.ItemContainerGenerator.ContainerFromIndex(0);
            var other = (ListBoxItem)index.ItemContainerGenerator.ContainerFromIndex(1);
            var selectedIcon = selected.FindChild<FontIcon>()!;
            var otherIcon = other.FindChild<FontIcon>()!;

            Assert.Multiple(() =>
                {
                    Assert.That(selected.Style, Is.SameAs(this.window.FindResource("MahApps.Styles.ListBoxItem.FlipView.Index.Win10")));
                    Assert.That(selectedIcon.Glyph, Is.EqualTo(""));
                    Assert.That(selectedIcon.FontSize, Is.EqualTo(6d), "selected");
                    Assert.That(otherIcon.FontSize, Is.EqualTo(4d), "not selected");
                    Assert.That((selected.Width, selected.Height), Is.EqualTo((12d, 20d)), "cell");
                    Assert.That(otherIcon.Foreground, Is.SameAs(this.window.FindResource("MahApps.Brushes.WinUI.ControlStrongFillDefault")));
                });
        }

        [Test]
        [Description("The buttons of the original are 16 by 38 for a horizontal flip view and 38 by 16 for a vertical one, with a margin of 1.")]
        public void TheWinUIButtonsHaveTheSizeOfTheOriginal()
        {
            var horizontal = this.Show(WinUIKey);
            var back = horizontal.FindChild<Button>("PART_BackButton")!;
            var forward = horizontal.FindChild<Button>("PART_ForwardButton")!;

            var vertical = this.Show(WinUIKey, Orientation.Vertical);
            var up = vertical.FindChild<Button>("PART_UpButton")!;
            var down = vertical.FindChild<Button>("PART_DownButton")!;

            Assert.Multiple(() =>
                {
                    Assert.That((back.Width, back.Height), Is.EqualTo((16d, 38d)), "back");
                    Assert.That((forward.Width, forward.Height), Is.EqualTo((16d, 38d)), "forward");
                    Assert.That((up.Width, up.Height), Is.EqualTo((38d, 16d)), "up");
                    Assert.That((down.Width, down.Height), Is.EqualTo((38d, 16d)), "down");
                    Assert.That(back.Margin, Is.EqualTo(new Thickness(1)));
                    Assert.That(up.Margin, Is.EqualTo(new Thickness(1)));
                });
        }

        [TestCase("PART_BackButton", "", Orientation.Horizontal)]
        [TestCase("PART_ForwardButton", "", Orientation.Horizontal)]
        [TestCase("PART_UpButton", "", Orientation.Vertical)]
        [TestCase("PART_DownButton", "", Orientation.Vertical)]
        [Description("The arrows are the glyphs of the original at size 8.")]
        public void TheWinUIArrowsAreTheGlyphsOfTheOriginal(string partName, string glyph, Orientation orientation)
        {
            var flipView = this.Show(WinUIKey, orientation);
            var icon = flipView.FindChild<Button>(partName)!.FindChild<FontIcon>();

            Assert.That(icon, Is.Not.Null, "the button should show a font icon");
            Assert.Multiple(() =>
                {
                    Assert.That(icon!.Glyph, Is.EqualTo(glyph));
                    Assert.That(icon.FontSize, Is.EqualTo(8d));
                });
        }

        [Test]
        [Description("The arrow of the original shrinks to 0.875 of its size while the button is pressed.")]
        public void TheWinUIArrowShrinksWhilePressed()
        {
            var flipView = this.Show(WinUIKey);
            var forward = flipView.FindChild<Button>("PART_ForwardButton")!;
            var presenter = forward.FindChild<ContentPresenter>("PART_ContentPresenter")!;
            var scale = (ScaleTransform)presenter.RenderTransform;
            Assert.That((scale.ScaleX, scale.ScaleY), Is.EqualTo((1d, 1d)), "at rest");

            typeof(ButtonBase).GetMethod("SetIsPressed", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(forward, new object[] { true });
            forward.UpdateLayout();

            var pressed = (ScaleTransform)presenter.RenderTransform;
            Assert.That((pressed.ScaleX, pressed.ScaleY), Is.EqualTo((0.875d, 0.875d)), "pressed");
        }

        [Test]
        [Description("WinUI rounds the flip view and cuts its content to the corners, with the radius all controls of the set share.")]
        public void TheWinUIFlipViewClipsItsContentToTheCorners()
        {
            var flipView = this.Show(WinUIKey);
            ClipAssert.Pump();
            var border = flipView.FindChild<Border>("Border");
            var content = ClipAssert.ContentGrid(flipView);

            Assert.That(border, Is.Not.Null, "a border that carries the radius");
            Assert.Multiple(() =>
                {
                    Assert.That(border!.CornerRadius, Is.EqualTo(new CornerRadius(4)));
                    Assert.That(ControlsHelper.GetCornerRadius(flipView), Is.EqualTo(this.window!.FindResource("MahApps.CornerRadius.WinUI.Control")));
                    ClipAssert.CoversElement(content, "content grid");
                    // the radius is 4, so a point a hair inside the corner is already outside of the rounding
                    Assert.That(content.Clip.FillContains(new Point(content.ActualWidth / 2, content.ActualHeight / 2)), Is.True, "middle");
                    Assert.That(content.Clip.FillContains(new Point(0.3, 0.3)), Is.False, "top left");
                    Assert.That(content.Clip.FillContains(new Point(content.ActualWidth - 0.3, 0.3)), Is.False, "top right");
                    Assert.That(content.Clip.FillContains(new Point(content.ActualWidth - 0.3, content.ActualHeight - 0.3)), Is.False, "bottom right");
                    Assert.That(content.Clip.FillContains(new Point(0.3, content.ActualHeight - 0.3)), Is.False, "bottom left");
                });
        }

        [Test]
        [Description("The index of the WinUI look is a PipsPager in place of the list of dots, in the style of the WinUI set even when the flip view only got its own style.")]
        public void TheWinUIFlipViewShowsAPipsPagerAsIndex()
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            flipView.Items.Add(new FlipViewItem { Content = "four" });
            this.window!.UpdateLayout();
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager");

            Assert.That(pager, Is.Not.Null, "a pager as the index");
            Assert.Multiple(() =>
                {
                    Assert.That(flipView.FindChild<ListBox>("PART_Index"), Is.Null, "no list of dots");
                    Assert.That(pager!.NumberOfPages, Is.EqualTo(4));
                    Assert.That(pager.Style, Is.Not.Null);
                    Assert.That(pager.Style.BasedOn, Is.SameAs(this.window.FindResource("MahApps.Styles.PipsPager.WinUI")));
                    Assert.That(pager.NormalPipStyle, Is.SameAs(this.window.FindResource("MahApps.Styles.Button.PipsPager.Pip.WinUI")));
                });
        }

        [Test]
        [Description("The pager and the flip view follow each other, and a value the pager pulls back into its pages ends the same on both without a loop.")]
        public void ThePagerAndTheFlipViewFollowEachOther()
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            flipView.Items.Add(new FlipViewItem { Content = "four" });
            this.window!.UpdateLayout();
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;

            flipView.SelectedIndex = 2;
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(2), "flip view to pager");

            pager.SetCurrentValue(PipsPager.SelectedPageIndexProperty, 1);
            Assert.That(flipView.SelectedIndex, Is.EqualTo(1), "pager to flip view");

            flipView.GoForward();
            Assert.That(pager.SelectedPageIndex, Is.EqualTo(2), "go forward");
        }

        [Test]
        [Description("An index past the last page that code sets on the pager inside the template is refused by the selector of the flip view, since the binding hands it over before the pager can pull it back. Both stay on the old page, which is consistent, and nothing is raised.")]
        public void AnIndexPastTheLastPageThroughThePagerIsRefused()
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            flipView.Items.Add(new FlipViewItem { Content = "four" });
            this.window!.UpdateLayout();
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;
            flipView.SelectedIndex = 2;

            var changes = 0;
            flipView.SelectionChanged += (_, _) => changes++;
            Assert.DoesNotThrow(() =>
                {
                    pager.SetCurrentValue(PipsPager.SelectedPageIndexProperty, 9);
                    ClipAssert.Pump();
                });

            Assert.Multiple(() =>
                {
                    Assert.That(pager.SelectedPageIndex, Is.EqualTo(2), "pager");
                    Assert.That(flipView.SelectedIndex, Is.EqualTo(2), "flip view");
                    Assert.That(changes, Is.EqualTo(0), "selection changes");
                });
        }

        [Test]
        [Description("An index of -1 on a flip view with items ends on the first one, as the pager of WinUI pulls a negative index to 0. This is deliberate.")]
        public void ANegativeIndexEndsOnTheFirstItemInTheWinUILook()
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;
            flipView.SelectedIndex = 2;

            flipView.SelectedIndex = -1;
            ClipAssert.Pump();

            Assert.Multiple(() =>
                {
                    Assert.That(pager.SelectedPageIndex, Is.EqualTo(0), "pager");
                    Assert.That(flipView.SelectedIndex, Is.EqualTo(0), "flip view");
                });
        }

        [Test]
        [Description("The pager counts the items, with one item there is one pip, and with none there are no pips, no exception and no selection that comes back on the flip view.")]
        public void ThePagerFollowsTheItems()
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;
            flipView.SelectedIndex = 2;

            flipView.Items.Add(new FlipViewItem { Content = "four" });
            flipView.Items.Add(new FlipViewItem { Content = "five" });
            Assert.That(pager.NumberOfPages, Is.EqualTo(5), "two more");

            Assert.DoesNotThrow(() =>
                {
                    flipView.Items.Clear();
                    this.window!.UpdateLayout();
                });

            Assert.Multiple(() =>
                {
                    Assert.That(pager.NumberOfPages, Is.EqualTo(0), "none");
                    Assert.That(flipView.SelectedIndex, Is.EqualTo(-1), "no selection");
                    Assert.That(flipView.SelectedItem, Is.Null);
                    Assert.That(pager.TemplateSettings.PipsPagerItems, Is.Empty, "no pips");
                    Assert.That(flipView.FindChild<Button>("PART_ForwardButton")!.Visibility, Is.EqualTo(Visibility.Hidden));
                    Assert.That(flipView.FindChild<Button>("PART_BackButton")!.Visibility, Is.EqualTo(Visibility.Hidden));
                });

            flipView.Items.Add(new FlipViewItem { Content = "only" });
            this.window!.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(pager.NumberOfPages, Is.EqualTo(1));
                    Assert.That(pager.TemplateSettings.PipsPagerItems, Has.Count.EqualTo(1), "one pip");
                    Assert.That(flipView.SelectedIndex, Is.EqualTo(0));
                    Assert.That(pager.SelectedPageIndex, Is.EqualTo(0));
                });
        }

        [TestCase(NavigationIndexPlacement.Top, 0, 3, 0, 1, Orientation.Horizontal, HorizontalAlignment.Right, VerticalAlignment.Center)]
        [TestCase(NavigationIndexPlacement.Bottom, 0, 3, 2, 1, Orientation.Horizontal, HorizontalAlignment.Right, VerticalAlignment.Center)]
        [TestCase(NavigationIndexPlacement.Left, 0, 1, 0, 3, Orientation.Vertical, HorizontalAlignment.Center, VerticalAlignment.Bottom)]
        [TestCase(NavigationIndexPlacement.Right, 2, 1, 0, 3, Orientation.Vertical, HorizontalAlignment.Center, VerticalAlignment.Bottom)]
        [TestCase(NavigationIndexPlacement.TopOverItem, 0, 3, 1, 1, Orientation.Horizontal, HorizontalAlignment.Right, VerticalAlignment.Top)]
        [TestCase(NavigationIndexPlacement.BottomOverItem, 0, 3, 1, 1, Orientation.Horizontal, HorizontalAlignment.Right, VerticalAlignment.Bottom)]
        [TestCase(NavigationIndexPlacement.LeftOverItem, 1, 1, 0, 3, Orientation.Vertical, HorizontalAlignment.Left, VerticalAlignment.Bottom)]
        [TestCase(NavigationIndexPlacement.RightOverItem, 1, 1, 0, 3, Orientation.Vertical, HorizontalAlignment.Right, VerticalAlignment.Bottom)]
        [Description("The pager sits in the grid where the list of dots sits in the Metro look, runs along the side it is put on, and takes the alignment of the flip view where the Metro look does.")]
        public void IndexPlacementLaysOutThePager(NavigationIndexPlacement placement,
                                                  int column,
                                                  int columnSpan,
                                                  int row,
                                                  int rowSpan,
                                                  Orientation orientation,
                                                  HorizontalAlignment horizontalAlignment,
                                                  VerticalAlignment verticalAlignment)
        {
            var flipView = this.Show(WinUIKey, showIndex: true);
            flipView.IndexPlacement = placement;
            flipView.IndexHorizontalAlignment = HorizontalAlignment.Right;
            flipView.IndexVerticalAlignment = VerticalAlignment.Bottom;
            this.window!.UpdateLayout();
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;

            Assert.Multiple(() =>
                {
                    Assert.That((Grid.GetColumn(pager), Grid.GetColumnSpan(pager), Grid.GetRow(pager), Grid.GetRowSpan(pager)), Is.EqualTo((column, columnSpan, row, rowSpan)));
                    Assert.That(pager.Orientation, Is.EqualTo(orientation));
                    Assert.That(pager.HorizontalAlignment, Is.EqualTo(horizontalAlignment), "horizontal alignment");
                    Assert.That(pager.VerticalAlignment, Is.EqualTo(verticalAlignment), "vertical alignment");
                });
        }

        [Test]
        [Description("A style under the key of the pager in the resources of the flip view changes the pager.")]
        public void AStyleInTheResourcesChangesThePager()
        {
            var flipView = new FlipView
            {
                Style = (Style)this.window!.FindResource(WinUIKey),
                Width = 400,
                Height = 200,
                ShowIndex = true,
            };
            flipView.Resources.Add("MahApps.Styles.PipsPager.FlipView",
                                   new Style(typeof(PipsPager), (Style)this.window.FindResource("MahApps.Styles.PipsPager.WinUI"))
                                   {
                                       Setters = { new Setter(PipsPager.MaxVisiblePipsProperty, 3) }
                                   });
            for (var i = 0; i < 6; i++)
            {
                flipView.Items.Add(new FlipViewItem { Content = i });
            }

            this.window.Content = flipView;
            this.window.UpdateLayout();

            Assert.That(flipView.FindChild<PipsPager>("PART_PipsPager")!.MaxVisiblePips, Is.EqualTo(3));
        }

        [Test]
        [Description("ShowIndex false collapses the pager.")]
        public void ShowIndexFalseHidesThePager()
        {
            var flipView = this.Show(WinUIKey);
            var pager = flipView.FindChild<PipsPager>("PART_PipsPager")!;
            Assert.That(pager.Visibility, Is.EqualTo(Visibility.Collapsed));

            flipView.ShowIndex = true;
            Assert.That(pager.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test]
        [Description("The arrows round with the radius of the flip view, which the button reads off its flip view with a path in parentheses.")]
        public void TheWinUIArrowsAreRounded()
        {
            var flipView = this.Show(WinUIKey);
            var expected = this.window!.FindResource("MahApps.CornerRadius.WinUI.Control");

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetCornerRadius(flipView.FindChild<Button>("PART_ForwardButton")!), Is.EqualTo(expected));
                    Assert.That(ControlsHelper.GetCornerRadius(flipView.FindChild<Button>("PART_BackButton")!), Is.EqualTo(expected));
                });
        }

        [Test]
        [Description("The face of a button follows the theme: after a change to a dark one it is the brush of that theme and not the one the window started with.")]
        public void TheButtonFaceFollowsTheTheme()
        {
            try
            {
                var flipView = this.Show(WinUIKey);
                ThemeManager.Current.ChangeTheme(this.window!, "Dark.Blue");
                this.window.UpdateLayout();
                ClipAssert.Pump();

                var root = flipView.FindChild<Button>("PART_ForwardButton")!.FindChild<Border>("Root")!;
                var expected = (SolidColorBrush)this.window.FindResource("MahApps.Brushes.FlipView.WinUI.NextPreviousButtonBackground");

                Assert.That(((SolidColorBrush)root.Background).Color, Is.EqualTo(expected.Color));
                Assert.That(expected.Color, Is.EqualTo(((SolidColorBrush)this.window.FindResource("MahApps.Brushes.WinUI.AcrylicInAppFillDefault")).Color));
            }
            finally
            {
                ThemeManager.Current.ChangeTheme(this.window!, "Light.Blue");
            }
        }
    }
}
