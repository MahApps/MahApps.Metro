// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Windows
{
    /// <summary>
    /// Interaction logic for BackdropWindow.xaml
    /// </summary>
    public partial class BackdropWindow
    {
        public BackdropWindow()
        {
            this.InitializeComponent();

            // a sample window shows what the default set gives it, whatever the gallery around it wears
            DefaultStyleSet.Apply(this);
        }
    }
}
