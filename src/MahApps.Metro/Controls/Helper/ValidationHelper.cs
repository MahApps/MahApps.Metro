// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Identifies the CloseOnMouseLeftButtonDown attached property.
        /// </summary>
        public static readonly DependencyProperty CloseOnMouseLeftButtonDownProperty
            = DependencyProperty.RegisterAttached("CloseOnMouseLeftButtonDown",
                                                  typeof(bool),
                                                  typeof(ValidationHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets whether if the popup can be closed by left mouse button down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetCloseOnMouseLeftButtonDown(UIElement element)
        {
            return (bool)element.GetValue(CloseOnMouseLeftButtonDownProperty);
        }

        /// <summary>
        /// Sets whether if the popup can be closed by left mouse button down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetCloseOnMouseLeftButtonDown(UIElement element, bool value)
        {
            element.SetValue(CloseOnMouseLeftButtonDownProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// Identifies the ShowValidationErrorOnMouseOver attached property.
        /// </summary>
        public static readonly DependencyProperty ShowValidationErrorOnMouseOverProperty
            = DependencyProperty.RegisterAttached("ShowValidationErrorOnMouseOver",
                                                  typeof(bool),
                                                  typeof(ValidationHelper),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetShowValidationErrorOnMouseOver(UIElement element)
        {
            return (bool)element.GetValue(ShowValidationErrorOnMouseOverProperty);
        }

        /// <summary>
        /// Sets whether the validation error text will be shown when hovering the validation triangle.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetShowValidationErrorOnMouseOver(UIElement element, bool value)
        {
            element.SetValue(ShowValidationErrorOnMouseOverProperty, BooleanBoxes.Box(value));
        }
    }
}