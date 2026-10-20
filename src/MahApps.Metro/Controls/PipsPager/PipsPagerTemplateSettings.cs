// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from PipsPagerTemplateSettings.h of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System.Collections.ObjectModel;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// What the template of a <see cref="PipsPager"/> binds to.
    /// </summary>
    public sealed class PipsPagerTemplateSettings : DependencyObject
    {
        /// <summary>
        /// Gets the pips the template shows, one per page, as the page numbers counted from one.
        /// </summary>
        public ObservableCollection<int> PipsPagerItems { get; } = new();
    }
}
