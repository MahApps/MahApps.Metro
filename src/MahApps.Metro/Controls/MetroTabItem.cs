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
            DefaultStyleKey = typeof(MetroTabItem);
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register(nameof(CloseButtonEnabled),
                                        typeof(bool),
                                        typeof(MetroTabItem),
                                        new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets/sets whether the Close Button is visible.
        /// </summary>
        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, BooleanBoxes.Box(value)); }
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register(nameof(CloseTabCommand),
                                        typeof(ICommand),
                                        typeof(MetroTabItem));

        /// <summary>
        /// Gets/sets the command that is executed when the Close Button is clicked.
        /// </summary>
        public ICommand CloseTabCommand
        {
            get { return (ICommand)GetValue(CloseTabCommandProperty); }
            set { SetValue(CloseTabCommandProperty, value); }
        }

        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register(nameof(CloseTabCommandParameter),
                                        typeof(object),
                                        typeof(MetroTabItem),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets/sets the command parameter which is passed to the close button command.
        /// </summary>
        public object CloseTabCommandParameter
        {
            get { return GetValue(CloseTabCommandParameterProperty); }
            set { SetValue(CloseTabCommandParameterProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonMarginProperty =
            DependencyProperty.Register(nameof(CloseButtonMargin),
                                        typeof(Thickness),
                                        typeof(MetroTabItem),
                                        new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets/sets the Close Button Margin.
        /// </summary>
        public Thickness CloseButtonMargin
        {
            get { return (Thickness)GetValue(CloseButtonMarginProperty); }
            set { SetValue(CloseButtonMarginProperty, value); }
        }
    }
}