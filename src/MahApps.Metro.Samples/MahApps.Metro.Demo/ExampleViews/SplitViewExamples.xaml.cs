// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    ///     Interaction logic for SplitViewExamples.xaml
    /// </summary>
    public partial class SplitViewExamples : UserControl
    {
        public SplitViewExamples()
        {
            this.InitializeComponent();

            // The Tag is used to handle closing
            this.SimpleSplitview.Tag = false;
        }

        private void Splitview_PaneClosing(object sender, SplitViewPaneClosingEventArgs e)
        {
            var splitView = sender as SplitView;

            if (splitView == null)
                return;

            e.Cancel = (bool)splitView.Tag;
        }
    }
}