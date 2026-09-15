// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// A view that says so every time one of it is built. The transitions page puts it in a data
    /// template and counts, which is how many views a change of content really costs.
    /// </summary>
    public class CountedView : ContentControl
    {
        /// <summary>Raised in the constructor, so once for every view that is built.</summary>
        public static event EventHandler? Built;

        public CountedView()
        {
            Built?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// One of the two things the counting sample flips between. Two types rather than one, because a
    /// data template is picked by type: this is the shape of a view model first application, where
    /// every screen has a template of its own.
    /// </summary>
    public class CountedPage
    {
        public CountedPage(string caption)
        {
            this.Caption = caption;
        }

        public string Caption { get; }
    }

    /// <summary>The other one, with a data template of its own.</summary>
    public class OtherCountedPage : CountedPage
    {
        public OtherCountedPage(string caption)
            : base(caption)
        {
        }
    }
}
