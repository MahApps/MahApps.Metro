﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MahApps.Metro.Controls
{
    public static class ToggleButtonHelper
    {
        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        public static readonly DependencyProperty ContentDirectionProperty =
            DependencyProperty.RegisterAttached(
                "ContentDirection",
                typeof(FlowDirection),
                typeof(ToggleButtonHelper),
                new FrameworkPropertyMetadata(FlowDirection.LeftToRight));

        /// <summary>
        /// This property can be used to handle the style for CheckBox and RadioButton
        /// LeftToRight means content left and button right and RightToLeft vise versa
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ToggleButton))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [Category(AppName.MahApps)]
        public static FlowDirection GetContentDirection(UIElement element)
        {
            return (FlowDirection)element.GetValue(ContentDirectionProperty);
        }

        public static void SetContentDirection(UIElement element, FlowDirection value)
        {
            element.SetValue(ContentDirectionProperty, value);
        }
    }
}