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

            Assert.Multiple(() =>
                {
                    Assert.That((dot.ActualWidth, dot.ActualHeight), Is.EqualTo((15d, 15d)));
                    Assert.That(System.Windows.Controls.Primitives.LayoutInformation.GetLayoutClip(dot), Is.Null, "nothing of the dot is cut away");
                    if (pages == 3)
                    {
                        Assert.That(item.DesiredSize.Width, Is.EqualTo(15d + 8d + 20d), "the dot, the padding of the theme and the margin, as before");
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
            Assert.That(flipView.FindChild<Grid>("PART_BannerGrid")!.ActualHeight, Is.EqualTo(30d), "the banner of the new look is up as well");
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
        [Description("The WinUI cell of a dot is 12 by 24, and 24 by 12 beside the content, where the Windows 10 one is 12 by 20 and 20 by 12.")]
        public void TheWinUIIndexCellIsALittleLongerThanTheWin10One()
        {
            var horizontal = this.Show(WinUIKey, showIndex: true);
            var cell = (ListBoxItem)horizontal.FindChild<ListBox>("PART_Index")!.ItemContainerGenerator.ContainerFromIndex(0);
            var cellSize = (cell.Width, cell.Height);

            var vertical = this.Show(WinUIKey, showIndex: true);
            vertical.IndexPlacement = NavigationIndexPlacement.Left;
            this.window!.UpdateLayout();
            var verticalCell = (ListBoxItem)vertical.FindChild<ListBox>("PART_Index")!.ItemContainerGenerator.ContainerFromIndex(0);
            var verticalSize = (verticalCell.Width, verticalCell.Height);

            var win10 = this.Show(Win10Key, showIndex: true);
            win10.IndexPlacement = NavigationIndexPlacement.Left;
            this.window.UpdateLayout();
            var win10Cell = (ListBoxItem)win10.FindChild<ListBox>("PART_Index")!.ItemContainerGenerator.ContainerFromIndex(0);

            Assert.Multiple(() =>
                {
                    Assert.That(cell.Style, Is.SameAs(this.window.FindResource("MahApps.Styles.ListBoxItem.FlipView.Index.WinUI")));
                    Assert.That(cellSize, Is.EqualTo((12d, 24d)), "top");
                    Assert.That(verticalSize, Is.EqualTo((24d, 12d)), "left");
                    Assert.That((win10Cell.Width, win10Cell.Height), Is.EqualTo((20d, 12d)), "Windows 10 left");
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
