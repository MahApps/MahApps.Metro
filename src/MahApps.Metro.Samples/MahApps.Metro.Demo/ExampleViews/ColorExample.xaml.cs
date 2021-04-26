// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using ControlzEx.Theming;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for ColorExample.xaml
    /// </summary>
    public partial class ColorExample : UserControl
    {
        public ColorExample()
        {
            this.InitializeComponent();

            ThemeManager.Current.ThemeChanged += this.ThemeManager_ThemeChanged;
        }

        private void ThemeManager_ThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            (this.DataContext as MainWindowViewModel)?.UpdateThemeResources();
        }
    }
}