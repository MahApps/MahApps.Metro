// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// A menu row is a list row of the same set, so each of the two Windows sets hands the menu the
    /// height its own list rows stand at, the padding they keep and the chevron its own controls
    /// point with. GH-3328 asked for the Windows looks; this is the bar, the menu on the right
    /// button and the item in both of them.
    /// </summary>
    [TestFixture]
    public class MenuSetStyleTests
    {
        private const string Default = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
        private const string Win10 = "pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml";
        private const string WinUI = "pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml";

        private TestWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(true);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [TearDown]
        public void TearDown()
        {
            if (this.window is not null)
            {
                this.window.Content = null;
            }
        }

        [TestCase("MahApps.Styles.MenuItem.Win10", "MahApps.Styles.ComboBoxItem.Win10")]
        [TestCase("MahApps.Styles.MenuItem.WinUI", "MahApps.Styles.ComboBoxItem.WinUI")]
        [Description("A menu row and a list row are the same thing in both Windows sets, so the menu stands at the height that set gives a row of a list.")]
        public void TheMenuRowIsTheRowOfItsList(string key, string rowKey)
        {
            var item = new MenuItem { Style = (Style)Application.Current.FindResource(key) };
            var row = new ComboBoxItem { Style = (Style)Application.Current.FindResource(rowKey) };

            Assert.That(item.MinHeight, Is.EqualTo(row.MinHeight));
        }

        [TestCase("MahApps.Styles.MenuItem.Win10", "MahApps.Templates.MenuItem.TopLevel.Win10", "MahApps.Templates.MenuItem.Submenu.Win10")]
        [TestCase("MahApps.Styles.MenuItem.WinUI", "MahApps.Templates.MenuItem.TopLevel.WinUI", "MahApps.Templates.MenuItem.Submenu.WinUI")]
        [Description("Four roles, two templates: the bar wears the one and the flyout the other, since what tells a header from an item is the chevron rather than a second layout.")]
        public void EveryRoleWearsOneOfTheTwoTemplates(string key, string topLevelKey, string submenuKey)
        {
            var style = (Style)Application.Current.FindResource(key);
            var topLevel = Application.Current.FindResource(topLevelKey);
            var submenu = Application.Current.FindResource(submenuKey);

            Assert.Multiple(() =>
                {
                    Assert.That(Setter(style, MenuItemRole.TopLevelHeader, Control.TemplateProperty), Is.SameAs(topLevel));
                    Assert.That(Setter(style, MenuItemRole.TopLevelItem, Control.TemplateProperty), Is.SameAs(topLevel));
                    Assert.That(Setter(style, MenuItemRole.SubmenuHeader, Control.TemplateProperty), Is.SameAs(submenu));
                    Assert.That(Setter(style, MenuItemRole.SubmenuItem, Control.TemplateProperty), Is.SameAs(submenu));
                });
        }

        [TestCase("MahApps.Styles.MenuItem.Win10", 11, 5, 11, 7)]
        [TestCase("MahApps.Styles.MenuItem.WinUI", 11, 4, 11, 5)]
        [Description("The padding of a role comes out of a trigger in the style this one stands on, and a trigger beats a plain setter, so every role has to bring its own along.")]
        public void EveryRoleBringsThePaddingItStandsIn(string key, double left, double top, double right, double bottom)
        {
            var style = (Style)Application.Current.FindResource(key);
            var padding = new Thickness(left, top, right, bottom);

            Assert.Multiple(() =>
                {
                    Assert.That(Setter(style, MenuItemRole.SubmenuHeader, Control.PaddingProperty), Is.EqualTo(padding));
                    Assert.That(Setter(style, MenuItemRole.SubmenuItem, Control.PaddingProperty), Is.EqualTo(padding));
                    Assert.That(Setter(style, MenuItemRole.TopLevelHeader, Control.PaddingProperty), Is.Not.Null);
                    Assert.That(Setter(style, MenuItemRole.TopLevelItem, Control.PaddingProperty), Is.Not.Null);
                });
        }

        [TestCase("MahApps.Templates.MenuItem.Submenu.Win10", "\uE76C")]
        [TestCase("MahApps.Templates.MenuItem.Submenu.WinUI", "\uE974")]
        [Description("Each set points at what stands behind a header with the chevron its own controls point with, and both of them tick a row that is checked with the same mark.")]
        public void TheChevronIsTheOneOfItsSet(string templateKey, string glyph)
        {
            var template = (ControlTemplate)Application.Current.FindResource(templateKey);

            var glyphs = Descendants<FontIcon>((DependencyObject)template.LoadContent())
                         .Select(icon => icon.Glyph)
                         .ToList();

            Assert.Multiple(() =>
                {
                    Assert.That(glyphs, Does.Contain(glyph), "the chevron of the set");
                    Assert.That(glyphs, Does.Contain(CheckMark), "and the mark on a row that is ticked");
                });
        }

        [TestCase("MahApps.Templates.MenuItem.Submenu.Win10")]
        [TestCase("MahApps.Templates.MenuItem.Submenu.WinUI")]
        [Description("A header says with the chevron that there is another menu behind it; an item has nothing there and gives the room back.")]
        public void OnlyAHeaderShowsTheChevron(string templateKey)
        {
            var template = (ControlTemplate)Application.Current.FindResource(templateKey);

            var chevron = Descendants<FontIcon>((DependencyObject)template.LoadContent())
                .Single(icon => icon.Glyph != CheckMark);

            var brought = template.Triggers
                                  .OfType<Trigger>()
                                  .Where(trigger => trigger.Property == MenuItem.RoleProperty && Equals(trigger.Value, MenuItemRole.SubmenuHeader))
                                  .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                                  .Any(setter => setter.Property == UIElement.VisibilityProperty && Equals(setter.Value, Visibility.Visible));

            Assert.Multiple(() =>
                {
                    Assert.That(chevron.Visibility, Is.EqualTo(Visibility.Collapsed), "an item shows no chevron");
                    Assert.That(brought, Is.True, "and a header brings it out");
                });
        }

        [TestCase(Win10, "MahApps.Styles.Separator.Menu.Win10")]
        [TestCase(WinUI, "MahApps.Styles.Separator.Menu.WinUI")]
        [Description("The line between two groups of a menu is looked up by a key rather than by type, so a set that wants one of its own has to hand it over under that key.")]
        public void TheLineBetweenTwoGroupsBelongsToTheSet(string set, string expected)
        {
            var dictionary = new ResourceDictionary { Source = new Uri(set) };

            var separator = dictionary[MenuItem.SeparatorStyleKey] as Style;

            Assert.That(separator, Is.Not.Null, "the set hands over no line of its own");
            Assert.That(separator!.BasedOn, Is.SameAs(dictionary[expected]));
        }

        [TestCase(Win10, "MahApps.Styles.ContextMenu.Win10", "MahApps.Styles.MenuItem.Win10")]
        [TestCase(WinUI, "MahApps.Styles.ContextMenu.WinUI", "MahApps.Styles.MenuItem.WinUI")]
        [Description("The menu a text box puts on the right button is a named resource rather than an implicit style, so a set that wants its own has to hand one over under the same name.")]
        public void TheMenuATextBoxPutsOnTheRightButtonFollowsTheSet(string set, string menuKey, string itemKey)
        {
            var dictionary = new ResourceDictionary { Source = new Uri(set) };

            var contextMenu = (ContextMenu)dictionary["MahApps.TextBox.ContextMenu"];

            Assert.That(contextMenu.Style, Is.SameAs(dictionary[menuKey]));
            // the rows ask for their style by key while the menu holds one, so what they end up with is the style of that key in the application
            Assert.That(contextMenu.Items.OfType<MenuItem>().Select(item => item.Style), Is.All.SameAs(Application.Current.FindResource(itemKey)));
        }

        [TestCase("MahApps.Brushes.Menu.Win10.FlyoutBackground", "MahApps.Brushes.ComboBox.Win10.DropDownBackground")]
        [TestCase("MahApps.Brushes.Menu.Win10.FlyoutBorderBrush", "MahApps.Brushes.ComboBox.Win10.DropDownBorderBrush")]
        [TestCase("MahApps.Brushes.Menu.WinUI.FlyoutBackground", "MahApps.Brushes.ComboBox.WinUI.DropDownBackground")]
        [Description("The flyout a menu comes up in is the drop-down of the combo box of the same set. A theme an application makes at run time is read on its own, with nothing around it to fall back on, so a brush named after one that stands further down the same file comes out empty there and the flyout shows the white of the window it is drawn on.")]
        public void TheFlyoutOfAMenuIsTheDropDownOfItsComboBox(string menuKey, string comboBoxKey)
        {
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red);

            Assert.That(theme, Is.Not.Null);
            Assert.That(theme!.Resources[menuKey], Is.InstanceOf<SolidColorBrush>());
            Assert.That(((SolidColorBrush)theme.Resources[menuKey]!).Color, Is.EqualTo(((SolidColorBrush)theme.Resources[comboBoxKey]!).Color));
        }

        [Test]
        [Description("WinUI draws the line around a flyout darker than the flyout itself rather than lighter, so that the edge of it reads as an edge and not as a lit rim.")]
        public void TheLineAroundTheWinUIFlyoutIsDarkerThanTheFlyout()
        {
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red);

            var line = (SolidColorBrush)theme!.Resources["MahApps.Brushes.Menu.WinUI.FlyoutBorderBrush"]!;
            var flyout = (SolidColorBrush)theme.Resources["MahApps.Brushes.Menu.WinUI.FlyoutBackground"]!;

            Assert.Multiple(() =>
                {
                    Assert.That(line.Color.A, Is.GreaterThan(0), "there is a line at all");
                    Assert.That(line.Color.R + line.Color.G + line.Color.B, Is.LessThan(flyout.Color.R + flyout.Color.G + flyout.Color.B), "and it is darker than what it stands around");
                });
        }

        [TestCase("MahApps.Templates.MenuItem.TopLevel.WinUI")]
        [TestCase("MahApps.Templates.MenuItem.Submenu.WinUI")]
        [TestCase("MahApps.Templates.ContextMenu.WinUI")]
        [Description("A WinUI flyout casts its own shadow: it falls straight down rather than to one side, and it is softer and further than the one the Metro look casts.")]
        public void TheWinUIFlyoutCastsTheShadowOfItsSet(string templateKey)
        {
            var template = (ControlTemplate)Application.Current.FindResource(templateKey);

            var shadows = template.Triggers
                                  .OfType<Trigger>()
                                  .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                                  .Where(setter => setter.Property == UIElement.EffectProperty)
                                  .Select(setter => (setter.Value as DynamicResourceExtension)?.ResourceKey)
                                  .ToList();

            Assert.That(shadows, Is.Not.Empty, "the flyout casts no shadow at all");
            Assert.That(shadows, Is.All.EqualTo("MahApps.DropShadowEffect.Menu.WinUI"));
        }

        [Test]
        [Description("And every other brush a menu asks for is there as well. What a theme file has and a theme made at run time has not is everything around it, so this is the one place where a name pointing at another name of the same file comes out empty.")]
        public void AThemeMadeAtRunTimeCarriesTheSameMenuBrushes()
        {
            var fromAFile = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Themes/Dark.Blue.xaml") };
            var madeAtRunTime = RuntimeThemeGenerator.Current.GenerateRuntimeTheme("Dark", Colors.Red);

            var keys = fromAFile.Keys
                                .OfType<string>()
                                .Where(key => key.StartsWith("MahApps.Brushes.Menu", StringComparison.Ordinal))
                                .ToList();

            Assert.That(keys, Is.Not.Empty, "the theme file carries no menu brushes at all");
            Assert.That(keys.Where(key => madeAtRunTime!.Resources[key] is not SolidColorBrush), Is.Empty);
        }

        [Test]
        [Description("WinUI rounds a row by the number it rounds a control by, and the flyout that row stands in by the larger number it rounds a flyout by.")]
        public void TheWinUIMenuIsRoundedByTheNumbersOfItsSet()
        {
            var item = new MenuItem { Style = (Style)Application.Current.FindResource("MahApps.Styles.MenuItem.WinUI") };
            var contextMenu = new ContextMenu { Style = (Style)Application.Current.FindResource("MahApps.Styles.ContextMenu.WinUI") };

            Assert.Multiple(() =>
                {
                    Assert.That(ControlsHelper.GetCornerRadius(item), Is.EqualTo(Application.Current.FindResource("MahApps.CornerRadius.WinUI.Control")));
                    Assert.That(ControlsHelper.GetCornerRadius(contextMenu), Is.EqualTo(Application.Current.FindResource("MahApps.CornerRadius.WinUI.Overlay")));
                });
        }

        [TestCase("MahApps.Styles.Menu.Win10", "MahApps.Styles.MenuItem.Win10", "MahApps.Templates.MenuItem.TopLevel.Win10")]
        [TestCase("MahApps.Styles.Menu.WinUI", "MahApps.Styles.MenuItem.WinUI", "MahApps.Templates.MenuItem.TopLevel.WinUI")]
        [Description("A menu bar carries no fill of its own in either Windows set: what it shows is the tile under the pointer, and that tile stands as high as a row.")]
        public void TheBarCarriesNothingButItsTiles(string menuKey, string itemKey, string templateKey)
        {
            var menu = this.Show(menuKey, itemKey);

            var first = (MenuItem)menu.Items[0];

            Assert.Multiple(() =>
                {
                    Assert.That((menu.Background as SolidColorBrush)?.Color.A, Is.EqualTo((byte)0), "the bar itself is not filled");
                    Assert.That(first.Template, Is.SameAs(Application.Current.FindResource(templateKey)), "and a tile of it wears the template of the bar");
                    Assert.That(first.ActualHeight, Is.GreaterThanOrEqualTo(32), "which stands as high as a row of the set");
                });
        }

        [Test]
        [Description("Merging the default set changes nothing about the menu it has always drawn: its item still picks one of the four templates the framework asks for by key, and its line between two groups is the one it always drew.")]
        public void TheMetroMenuIsStillTheMetroMenu()
        {
            var dictionary = new ResourceDictionary { Source = new Uri(Default) };

            var style = (Style)dictionary["MahApps.Styles.MenuItem"];
            var template = style.Setters
                                .OfType<Setter>()
                                .Where(setter => setter.Property == Control.TemplateProperty)
                                .Select(setter => setter.Value)
                                .Single();

            Assert.Multiple(() =>
                {
                    Assert.That((template as DynamicResourceExtension)?.ResourceKey, Is.InstanceOf<ComponentResourceKey>());
                    Assert.That(((Style)dictionary[MenuItem.SeparatorStyleKey]).BasedOn, Is.Null);
                });
        }

        private const string CheckMark = "\uE73E";

        private static object? Setter(Style style, MenuItemRole role, DependencyProperty property)
        {
            return style.Triggers
                        .OfType<Trigger>()
                        .Where(trigger => trigger.Property == MenuItem.RoleProperty && Equals(trigger.Value, role))
                        .SelectMany(trigger => trigger.Setters.OfType<Setter>())
                        .Where(setter => setter.Property == property)
                        .Select(setter => setter.Value)
                        .LastOrDefault();
        }

        private static IEnumerable<T> Descendants<T>(DependencyObject root)
            where T : DependencyObject
        {
            foreach (var child in LogicalTreeHelper.GetChildren(root).OfType<DependencyObject>())
            {
                if (child is T match)
                {
                    yield return match;
                }

                foreach (var nested in Descendants<T>(child))
                {
                    yield return nested;
                }
            }
        }

        private Menu Show(string menuKey, string itemKey)
        {
            Assert.That(this.window, Is.Not.Null);

            var menu = new Menu
                       {
                           Style = (Style)Application.Current.FindResource(menuKey),
                           HorizontalAlignment = HorizontalAlignment.Left,
                           VerticalAlignment = VerticalAlignment.Top
                       };

            menu.Resources.Add(typeof(MenuItem), new Style(typeof(MenuItem), (Style)Application.Current.FindResource(itemKey)));

            var file = new MenuItem { Header = "File" };
            file.Items.Add(new MenuItem { Header = "New", InputGestureText = "Ctrl+N" });
            file.Items.Add(new Separator());
            file.Items.Add(new MenuItem { Header = "Export as" });
            menu.Items.Add(file);
            menu.Items.Add(new MenuItem { Header = "Help" });

            this.window!.Content = menu;
            this.window.UpdateLayout();
            ClipAssert.Pump();

            return menu;
        }
    }
}
