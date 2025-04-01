// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace MetroDemo.Core
{
    /// <summary>
    /// The BindingProxy can be used to avoid the following error:
    ///
    ///     System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression: Path= ...
    ///
    /// WPF doesn't know which FrameworkElement to use to get the DataContext,
    /// because this Control doesn't belong to the visual or logical tree.
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>Identifies the <see cref="Data"/> dependency property.</summary>
        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register(nameof(Data),
                                          typeof(object),
                                          typeof(BindingProxy));

        /// <summary>
        /// Gets or sets the real data which should be bind.
        /// </summary>
        public object? Data
        {
            get => this.GetValue(DataProperty);
            set => this.SetValue(DataProperty, value);
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}