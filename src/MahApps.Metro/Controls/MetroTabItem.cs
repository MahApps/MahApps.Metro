// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// An extended TabItem with a metro style.
    /// </summary>
    public class MetroTabItem : TabItem
    {
        public MetroTabItem()
        {
            this.DefaultStyleKey = typeof(MetroTabItem);
        }

        /// <summary>Identifies the <see cref="CloseButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register(nameof(CloseButtonEnabled),
                                        typeof(bool),
                                        typeof(MetroTabItem),
                                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets whether the Close Button is visible.
        /// </summary>
        public bool CloseButtonEnabled
        {
            get => (bool)this.GetValue(CloseButtonEnabledProperty);
            set => this.SetValue(CloseButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="CloseTabCommand"/> dependency property.</summary>
        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register(nameof(CloseTabCommand),
                                        typeof(ICommand),
                                        typeof(MetroTabItem));

        /// <summary>
        /// Gets or sets the command that is executed when the Close Button is clicked.
        /// </summary>
        public ICommand? CloseTabCommand
        {
            get => (ICommand?)this.GetValue(CloseTabCommandProperty);
            set => this.SetValue(CloseTabCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseTabCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register(nameof(CloseTabCommandParameter),
                                        typeof(object),
                                        typeof(MetroTabItem),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter which is passed to the close button command.
        /// </summary>
        public object? CloseTabCommandParameter
        {
            get => this.GetValue(CloseTabCommandParameterProperty);
            set => this.SetValue(CloseTabCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonMargin"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonMarginProperty =
            DependencyProperty.Register(nameof(CloseButtonMargin),
                                        typeof(Thickness),
                                        typeof(MetroTabItem),
                                        new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets or sets the Margin of the close Button.
        /// </summary>
        public Thickness CloseButtonMargin
        {
            get => (Thickness)this.GetValue(CloseButtonMarginProperty);
            set => this.SetValue(CloseButtonMarginProperty, value);
        }
    }
}