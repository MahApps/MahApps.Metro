// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for HotKeyBoxPage.xaml
    /// </summary>
    public partial class HotKeyBoxPage : UserControl
    {
        public HotKeyBoxPage()
        {
            this.InitializeComponent();

            Options(this.HotKeyExample, this.HotKey);
            Options(this.Win10Example, this.Win10);
            Options(this.WinUIExample, this.WinUI);
        }

        private static void Options(ControlExample example, HotKeyBox box)
        {
            example.Watch(box,
                          HotKeyBox.HotKeyProperty,
                          HotKeyBox.AreModifierKeysRequiredProperty,
                          IsEnabledProperty);
            example.Watch("Attached",
                          box,
                          TextBoxHelper.WatermarkProperty,
                          TextBoxHelper.ClearTextButtonProperty);
        }
    }
}
