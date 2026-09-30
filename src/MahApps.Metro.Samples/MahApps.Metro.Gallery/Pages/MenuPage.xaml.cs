// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : UserControl
    {
        private readonly CultureInfo cultureBeforeThePage = CultureInfo.CurrentUICulture;

        public MenuPage()
        {
            this.InitializeComponent();

            this.BarExample.Watch(this.Bar,
                                  Menu.IsMainMenuProperty,
                                  IsEnabledProperty,
                                  FlowDirectionProperty);
            this.BarExample.Watch("Layout", this.Bar, WidthProperty);

            this.ContextExample.Watch("Layout", this.Strip, WidthProperty, HeightProperty);

            this.Win10Example.Watch(this.Win10Bar, Menu.IsMainMenuProperty, IsEnabledProperty, FlowDirectionProperty);
            this.Win10Example.Watch("Layout", this.Win10Strip, WidthProperty, HeightProperty);

            this.WinUIExample.Watch(this.WinUIBar, Menu.IsMainMenuProperty, IsEnabledProperty, FlowDirectionProperty);
            this.WinUIExample.Watch("Layout", this.WinUIStrip, WidthProperty, HeightProperty);

            // the languages WPF carries the words of its own text box menu in, and the one the
            // reader is most likely in already
            var languages = new[] { "en-US", "de-DE", "fr-FR", "ja-JP", "zh-CN" }.Select(CultureInfo.GetCultureInfo).ToList();

            this.LanguagePicker.ItemsSource = languages;
            this.LanguagePicker.SelectedItem = languages.FirstOrDefault(language => language.Name == this.cultureBeforeThePage.Name) ?? languages[0];

            // the culture belongs to the thread, so the page hands it back when the reader leaves,
            // rather than leaving the rest of the gallery in a language nobody asked it for
            this.Loaded += (_, _) => this.SpeakTheChosenLanguage();
            this.Unloaded += (_, _) => Speak(this.cultureBeforeThePage);
        }

        private static void Speak(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        private void LanguageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.SpeakTheChosenLanguage();
            }
        }

        private void SpeakTheChosenLanguage()
        {
            if (this.LanguagePicker.SelectedItem is CultureInfo culture)
            {
                Speak(culture);
            }
        }
    }
}
