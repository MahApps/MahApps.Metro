// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The buttons a text control shows next to its clear button.
    /// </summary>
    /// <remarks>
    /// A button is a visual element and has one parent, so a collection belongs to one control.
    /// Handing the same one to two controls through a style setter does not work; write the
    /// buttons on the control itself.
    /// </remarks>
    public class TextBoxButtonCollection : ObservableCollection<TextBoxButton>
    {
    }
}
