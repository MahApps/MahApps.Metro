// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ContentControlExPage.xaml
    /// </summary>
    public partial class ContentControlExPage : UserControl
    {
        public ContentControlExPage()
        {
            this.InitializeComponent();

            this.CasingExample.Watch(this.Cased,
                                     ContentControl.ContentProperty,
                                     ContentControlEx.ContentCharacterCasingProperty);

            this.InheritExample.Watch("The first one", this.Outer, ContentControlEx.ContentCharacterCasingProperty);
            this.InheritExample.Watch("The one it holds", this.Inner, ContentControlEx.ContentCharacterCasingProperty);
            this.InheritExample.Watch("The one with the panel", this.Blocked, ContentControlEx.ContentCharacterCasingProperty);

            this.AccessExample.Watch(this.Access,
                                     ContentControl.ContentProperty,
                                     ContentControlEx.RecognizesAccessKeyProperty);

            this.ChromeExample.Watch(this.Chrome, ControlzEx.WindowChrome.IsHitTestVisibleInChromeProperty);
        }
    }
}
