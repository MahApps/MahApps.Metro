// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Every control a Windows set hands out writes in the font of that set and at the size of its
    /// controls, 14, the ControlContentThemeFontSize of both UWP and WinUI. Before, most of them
    /// borrowed the Metro size of 12 through the style they stand on. Where Microsoft writes a part
    /// smaller, as the 12 of the TabView header, that part has a size of its own through
    /// HeaderedControlHelper, so the control itself stays at 14.
    ///
    /// A scroll viewer and the items of a list take their font from what they sit in, in the default
    /// set as in a Windows one: the list gives them the font of the set, and a FontSize set on a
    /// combo box, a menu or a tab control still reaches its items and whatever is in a scroll viewer.
    /// </summary>
    [TestFixture]
    public class SetTypographyTests : WindowTestFixture<TestWindow>
    {
        private const string Default = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
        private const string Win10 = "pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml";
        private const string WinUI = "pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Controls.xaml";

        // what the host of a control writes in, so that a control taking its font from there shows it
        private const double HostSize = 30;
        private const string HostFamily = "Arial";

        /// <summary>
        /// What takes its font from what it sits in rather than from its look.
        /// </summary>
        private static readonly Type[] Followers =
        {
            typeof(ScrollViewer),
            typeof(ComboBoxItem),
            typeof(ListBoxItem),
            typeof(ListViewItem),
            typeof(MenuItem),
            typeof(TabItem)
        };

        /// <summary>
        /// What Microsoft writes at the Caption step, 12, rather than at the size of its controls.
        /// </summary>
        private static readonly Type[] Captions =
        {
            typeof(ToolTip)
        };

        private const string Win10Family = "Segoe UI";
        private const string WinUIFamily = "Segoe UI Variable Text, Segoe UI";
        private const string WinUICaptionFamily = "Segoe UI Variable Small, Segoe UI";

        // the pack scheme is there only once the application of the tests is up, which is after
        // NUnit has asked for its cases, so the controls of a set are walked inside the test
        [TestCase(Win10, Win10Family)]
        [TestCase(WinUI, WinUIFamily)]
        [Description("Every control of a Windows set writes in the font of its set at the size of its controls, apart from the scroll viewer and the items of a list, which take it from what they sit in.")]
        public void EveryControlWritesInTheFontOfItsSet(string set, string family)
        {
            var dictionary = Load(set);
            var defaults = Load(Default);
            var types = dictionary.Keys.OfType<Type>()
                                  .Where(t => typeof(Control).IsAssignableFrom(t) && !t.IsAbstract && !typeof(Window).IsAssignableFrom(t))
                                  .OrderBy(t => t.Name)
                                  .ToList();

            Assert.That(types, Is.Not.Empty);
            Assert.Multiple(() =>
                {
                    foreach (var type in types)
                    {
                        var follows = Followers.Any(f => f.IsAssignableFrom(type));
                        if (follows)
                        {
                            var metro = this.Show(defaults, type, null);
                            Assert.That(metro.FontSize, Is.EqualTo(HostSize), $"{type.Name} follows what it sits in in the default set too");
                        }

                        var caption = Captions.Any(c => c.IsAssignableFrom(type));
                        var control = this.Show(dictionary, type, null);

                        Assert.That(control.FontSize, Is.EqualTo(follows ? HostSize : caption ? 12 : 14), $"the size of {type.Name}");
                        Assert.That(control.FontFamily.Source, Is.EqualTo(follows ? HostFamily : caption ? CaptionFamily(set, family) : family), $"the family of {type.Name}");
                    }
                });
        }

        [TestCase(Win10, Win10Family)]
        [TestCase(WinUI, WinUIFamily)]
        [Description("A keyed style of a look writes in the font of that look or takes the one of what it sits in, never the one of another look.")]
        public void EveryKeyedStyleOfALookWritesInTheFontOfItsLook(string set, string family)
        {
            var dictionary = Load(set);
            var look = set == Win10 ? ".Win10" : ".WinUI";
            var styles = Keys(dictionary)
                         .Where(k => k.EndsWith(look, StringComparison.Ordinal) || k.IndexOf(look + ".", StringComparison.Ordinal) >= 0)
                         .Select(k => (Key: k, Style: dictionary[k] as Style))
                         .Where(s => s.Style?.TargetType is { IsAbstract: false } t
                                     && typeof(Control).IsAssignableFrom(t)
                                     && !typeof(Window).IsAssignableFrom(t)
                                     && !typeof(ContextMenu).IsAssignableFrom(t)
                                     && t.GetConstructor(Type.EmptyTypes) is not null
                                     // the buttons of a calendar take the font of the calendar they are in
                                     && !typeof(CalendarButton).IsAssignableFrom(t)
                                     && !typeof(CalendarDayButton).IsAssignableFrom(t))
                         .OrderBy(s => s.Key)
                         .ToList();

            Assert.That(styles, Is.Not.Empty);
            Assert.Multiple(() =>
                {
                    foreach (var (key, style) in styles)
                    {
                        var control = this.Show(dictionary, style!.TargetType, style);
                        var source = control.FontFamily.Source;

                        Assert.That(source == family || source == CaptionFamily(set, family) || source == HostFamily || IsSymbolFont(source), Is.True, $"the family of {key} is {source}");
                    }
                });
        }

        [Test]
        [Description("A FontSize set on a list box reaches its items, as it does in the default set.")]
        public void TheItemsOfAListFollowTheList()
        {
            var dictionary = Load(WinUI);
            var item = new ListBoxItem { Content = "42" };
            var list = new ListBox { FontSize = 20, FontFamily = new FontFamily(HostFamily), Items = { item } };
            var host = new Border { Child = list };
            host.Resources.MergedDictionaries.Add(dictionary);
            this.window!.Content = host;
            this.window.UpdateLayout();

            Assert.Multiple(() =>
                {
                    Assert.That(item.FontSize, Is.EqualTo(20));
                    Assert.That(item.FontFamily.Source, Is.EqualTo(HostFamily));
                });
        }

        [Test]
        [Description("The size of the controls of a look is a key, so an application that wants them larger overrides that key.")]
        public void TheControlsOfALookCanBeMadeLarger()
        {
            var dictionary = Load(WinUI);
            var button = new Button { Content = "42" };
            var host = new Border { Child = button };
            host.Resources.MergedDictionaries.Add(dictionary);
            host.Resources["MahApps.Font.Size.Control.WinUI"] = 16d;
            this.window!.Content = host;
            this.window.UpdateLayout();

            Assert.That(button.FontSize, Is.EqualTo(16));
        }

        private static string CaptionFamily(string set, string family)
        {
            return set == WinUI ? WinUICaptionFamily : family;
        }

        private static bool IsSymbolFont(string source)
        {
            return source.IndexOf("MDL2", StringComparison.Ordinal) >= 0 || source.IndexOf("Fluent Icons", StringComparison.Ordinal) >= 0;
        }

        private static ResourceDictionary Load(string set)
        {
            return new ResourceDictionary { Source = new Uri(set, UriKind.Absolute) };
        }

        private static IEnumerable<string> Keys(ResourceDictionary dictionary)
        {
            return dictionary.Keys.OfType<string>().Concat(dictionary.MergedDictionaries.SelectMany(Keys)).Distinct();
        }

        private Control Show(ResourceDictionary dictionary, Type type, Style? style)
        {
            var control = (Control)Activator.CreateInstance(type, nonPublic: true)!; // the dialogs of the library are made only by it
            var host = new Border();
            TextElement.SetFontSize(host, HostSize);
            TextElement.SetFontFamily(host, new FontFamily(HostFamily));
            host.Resources.MergedDictionaries.Add(dictionary);

            if (control is ContextMenu menu)
            {
                // a context menu cannot have a parent; it takes the style of the set and the keys of the application
                menu.Style = style ?? (Style?)dictionary[typeof(ContextMenu)];
                host.ContextMenu = menu;
            }
            else if (control is ToolTip tip)
            {
                // nor can a tool tip; it hangs on what it explains the same way
                tip.Style = style ?? (Style?)dictionary[typeof(ToolTip)];
                host.ToolTip = tip;
            }
            else
            {
                if (style is not null)
                {
                    control.Style = style;
                }

                host.Child = control;
            }

            this.window!.Content = host;
            this.window.UpdateLayout();

            return control;
        }
    }
}
