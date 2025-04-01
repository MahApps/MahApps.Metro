// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Input;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for CustomDialogContent.xaml
    /// </summary>
    public partial class CustomDialogExample : UserControl
    {
        public CustomDialogExample()
        {
            this.InitializeComponent();

            this.Loaded += (_, _) => { this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First)); };
        }
    }
}