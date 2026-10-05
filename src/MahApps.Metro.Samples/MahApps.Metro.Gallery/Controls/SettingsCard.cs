// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Gallery.Controls
{
    /// <summary>
    /// One row of the settings, the way the WinUI Gallery draws them: an icon, the header with a
    /// line under it saying what it does, and the control to change it on the right.
    /// </summary>
    public class SettingsCard : HeaderedContentControl
    {
        /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register(nameof(Description),
                                          typeof(string),
                                          typeof(SettingsCard),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the line under the header.
        /// </summary>
        public string? Description
        {
            get => (string?)this.GetValue(DescriptionProperty);
            set => this.SetValue(DescriptionProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(SettingsCard),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the icon in front of the header.
        /// </summary>
        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }
    }
}
