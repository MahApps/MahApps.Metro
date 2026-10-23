// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Hands a name its own template, so that it can trim, and leaves everything else to the
    /// content presenter. The text a content presenter builds for a name is made in code, where no
    /// implicit style reaches it, and a <see cref="ContentPresenter.ContentTemplate"/> would be used
    /// for an element as well, which would then show as the name of its type.
    /// </summary>
    public class TrimmedTextTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the template for a name while the presenter recognizes access keys.
        /// </summary>
        public DataTemplate? AccessTextTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for a name while the presenter does not recognize access keys.
        /// </summary>
        public DataTemplate? TextTemplate { get; set; }

        /// <inheritdoc />
        public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        {
            // a name with a format of its own keeps the one the presenter builds, since only that one applies the format
            if (item is not string || container is not ContentPresenter presenter || presenter.ContentStringFormat is not null)
            {
                return base.SelectTemplate(item, container);
            }

            return presenter.RecognizesAccessKey ? this.AccessTextTemplate : this.TextTemplate;
        }
    }
}
