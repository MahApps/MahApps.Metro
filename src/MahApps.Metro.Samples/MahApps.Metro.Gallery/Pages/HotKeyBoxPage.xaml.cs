// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

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

            this.HotKeyExample.Watch(this.HotKey,
                                     HotKeyBox.HotKeyProperty,
                                     HotKeyBox.AreModifierKeysRequiredProperty,
                                     IsEnabledProperty);
            this.HotKeyExample.Watch("Attached",
                                     this.HotKey,
                                     TextBoxHelper.WatermarkProperty,
                                     TextBoxHelper.ClearTextButtonProperty);
        }
    }
}
