// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for TextExamples.xaml
    /// </summary>
    public partial class TextExamples : UserControl
    {
        public static readonly string[] NUD_StringFormats =
        {
            "C",
            "P",
            "N0",
            "{}you have {0:N0} pieces",
            "X"
        };

        public TextExamples()
        {
            this.InitializeComponent();
        }
    }
}