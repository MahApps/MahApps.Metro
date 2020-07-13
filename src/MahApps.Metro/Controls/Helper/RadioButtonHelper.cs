// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class RadioButtonHelper
    {
        public static readonly DependencyProperty RadioSizeProperty
            = DependencyProperty.RegisterAttached("RadioSize",
                                                  typeof(double),
                                                  typeof(RadioButtonHelper),
                                                  new FrameworkPropertyMetadata(18.0));

        /// <summary>Helper for getting <see cref="RadioSizeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="RadioSizeProperty"/> from.</param>
        /// <returns>RadioSize property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static double GetRadioSize(UIElement element)
        {
            return (double)element.GetValue(RadioSizeProperty);
        }

        /// <summary>Helper for setting <see cref="RadioSizeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="RadioSizeProperty"/> on.</param>
        /// <param name="value">RadioSize property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetRadioSize(UIElement element, double value)
        {
            element.SetValue(RadioSizeProperty, value);
        }

        public static readonly DependencyProperty RadioCheckSizeProperty
            = DependencyProperty.RegisterAttached("RadioCheckSize",
                                                  typeof(double),
                                                  typeof(RadioButtonHelper),
                                                  new FrameworkPropertyMetadata(10.0));

        /// <summary>Helper for getting <see cref="RadioCheckSizeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="RadioCheckSizeProperty"/> from.</param>
        /// <returns>RadioCheckSize property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static double GetRadioCheckSize(UIElement element)
        {
            return (double)element.GetValue(RadioCheckSizeProperty);
        }

        /// <summary>Helper for setting <see cref="RadioCheckSizeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="RadioCheckSizeProperty"/> on.</param>
        /// <param name="value">RadioCheckSize property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetRadioCheckSize(UIElement element, double value)
        {
            element.SetValue(RadioCheckSizeProperty, value);
        }

        public static readonly DependencyProperty RadioStrokeThicknessProperty
            = DependencyProperty.RegisterAttached("RadioStrokeThickness",
                                                  typeof(double),
                                                  typeof(RadioButtonHelper),
                                                  new FrameworkPropertyMetadata(1.0));

        /// <summary>Helper for getting <see cref="RadioStrokeThicknessProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="RadioStrokeThicknessProperty"/> from.</param>
        /// <returns>RadioStrokeThickness property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static double GetRadioStrokeThickness(UIElement element)
        {
            return (double)element.GetValue(RadioStrokeThicknessProperty);
        }

        /// <summary>Helper for setting <see cref="RadioStrokeThicknessProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="RadioStrokeThicknessProperty"/> on.</param>
        /// <param name="value">RadioStrokeThickness property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetRadioStrokeThickness(UIElement element, double value)
        {
            element.SetValue(RadioStrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty ForegroundPointerOverProperty
            = DependencyProperty.RegisterAttached("ForegroundPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="ForegroundPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="ForegroundPointerOverProperty"/> from.</param>
        /// <returns>ForegroundPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetForegroundPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(ForegroundPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="ForegroundPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="ForegroundPointerOverProperty"/> on.</param>
        /// <param name="value">ForegroundPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetForegroundPointerOver(UIElement element, Brush value)
        {
            element.SetValue(ForegroundPointerOverProperty, value);
        }

        public static readonly DependencyProperty ForegroundPressedProperty
            = DependencyProperty.RegisterAttached("ForegroundPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="ForegroundPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="ForegroundPressedProperty"/> from.</param>
        /// <returns>ForegroundPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetForegroundPressed(UIElement element)
        {
            return (Brush)element.GetValue(ForegroundPressedProperty);
        }

        /// <summary>Helper for setting <see cref="ForegroundPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="ForegroundPressedProperty"/> on.</param>
        /// <param name="value">ForegroundPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetForegroundPressed(UIElement element, Brush value)
        {
            element.SetValue(ForegroundPressedProperty, value);
        }

        public static readonly DependencyProperty ForegroundDisabledProperty
            = DependencyProperty.RegisterAttached("ForegroundDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="ForegroundDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="ForegroundDisabledProperty"/> from.</param>
        /// <returns>ForegroundDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetForegroundDisabled(UIElement element)
        {
            return (Brush)element.GetValue(ForegroundDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="ForegroundDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="ForegroundDisabledProperty"/> on.</param>
        /// <param name="value">ForegroundDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetForegroundDisabled(UIElement element, Brush value)
        {
            element.SetValue(ForegroundDisabledProperty, value);
        }

        public static readonly DependencyProperty BackgroundPointerOverProperty
            = DependencyProperty.RegisterAttached("BackgroundPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BackgroundPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BackgroundPointerOverProperty"/> from.</param>
        /// <returns>BackgroundPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBackgroundPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(BackgroundPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="BackgroundPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BackgroundPointerOverProperty"/> on.</param>
        /// <param name="value">BackgroundPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBackgroundPointerOver(UIElement element, Brush value)
        {
            element.SetValue(BackgroundPointerOverProperty, value);
        }

        public static readonly DependencyProperty BackgroundPressedProperty
            = DependencyProperty.RegisterAttached("BackgroundPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BackgroundPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BackgroundPressedProperty"/> from.</param>
        /// <returns>BackgroundPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBackgroundPressed(UIElement element)
        {
            return (Brush)element.GetValue(BackgroundPressedProperty);
        }

        /// <summary>Helper for setting <see cref="BackgroundPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BackgroundPressedProperty"/> on.</param>
        /// <param name="value">BackgroundPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBackgroundPressed(UIElement element, Brush value)
        {
            element.SetValue(BackgroundPressedProperty, value);
        }

        public static readonly DependencyProperty BackgroundDisabledProperty
            = DependencyProperty.RegisterAttached("BackgroundDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BackgroundDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BackgroundDisabledProperty"/> from.</param>
        /// <returns>BackgroundDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBackgroundDisabled(UIElement element)
        {
            return (Brush)element.GetValue(BackgroundDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="BackgroundDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BackgroundDisabledProperty"/> on.</param>
        /// <param name="value">BackgroundDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBackgroundDisabled(UIElement element, Brush value)
        {
            element.SetValue(BackgroundDisabledProperty, value);
        }

        public static readonly DependencyProperty BorderBrushPointerOverProperty
            = DependencyProperty.RegisterAttached("BorderBrushPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BorderBrushPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BorderBrushPointerOverProperty"/> from.</param>
        /// <returns>BorderBrushPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBorderBrushPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(BorderBrushPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="BorderBrushPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BorderBrushPointerOverProperty"/> on.</param>
        /// <param name="value">BorderBrushPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBorderBrushPointerOver(UIElement element, Brush value)
        {
            element.SetValue(BorderBrushPointerOverProperty, value);
        }

        public static readonly DependencyProperty BorderBrushPressedProperty
            = DependencyProperty.RegisterAttached("BorderBrushPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BorderBrushPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BorderBrushPressedProperty"/> from.</param>
        /// <returns>BorderBrushPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBorderBrushPressed(UIElement element)
        {
            return (Brush)element.GetValue(BorderBrushPressedProperty);
        }

        /// <summary>Helper for setting <see cref="BorderBrushPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BorderBrushPressedProperty"/> on.</param>
        /// <param name="value">BorderBrushPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBorderBrushPressed(UIElement element, Brush value)
        {
            element.SetValue(BorderBrushPressedProperty, value);
        }

        public static readonly DependencyProperty BorderBrushDisabledProperty
            = DependencyProperty.RegisterAttached("BorderBrushDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="BorderBrushDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="BorderBrushDisabledProperty"/> from.</param>
        /// <returns>BorderBrushDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetBorderBrushDisabled(UIElement element)
        {
            return (Brush)element.GetValue(BorderBrushDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="BorderBrushDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="BorderBrushDisabledProperty"/> on.</param>
        /// <param name="value">BorderBrushDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetBorderBrushDisabled(UIElement element, Brush value)
        {
            element.SetValue(BorderBrushDisabledProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseFillProperty
            = DependencyProperty.RegisterAttached("OuterEllipseFill",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseFillProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseFillProperty"/> from.</param>
        /// <returns>OuterEllipseFill property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseFill(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseFillProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseFillProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseFillProperty"/> on.</param>
        /// <param name="value">OuterEllipseFill property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseFill(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseFillProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseFillPointerOverProperty
            = DependencyProperty.RegisterAttached("OuterEllipseFillPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseFillPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseFillPointerOverProperty"/> from.</param>
        /// <returns>OuterEllipseFillPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseFillPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseFillPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseFillPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseFillPointerOverProperty"/> on.</param>
        /// <param name="value">OuterEllipseFillPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseFillPointerOver(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseFillPointerOverProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseFillPressedProperty
            = DependencyProperty.RegisterAttached("OuterEllipseFillPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseFillPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseFillPressedProperty"/> from.</param>
        /// <returns>OuterEllipseFillPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseFillPressed(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseFillPressedProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseFillPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseFillPressedProperty"/> on.</param>
        /// <param name="value">OuterEllipseFillPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseFillPressed(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseFillPressedProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseFillDisabledProperty
            = DependencyProperty.RegisterAttached("OuterEllipseFillDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseFillDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseFillDisabledProperty"/> from.</param>
        /// <returns>OuterEllipseFillDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseFillDisabled(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseFillDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseFillDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseFillDisabledProperty"/> on.</param>
        /// <param name="value">OuterEllipseFillDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseFillDisabled(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseFillDisabledProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseStrokeProperty
            = DependencyProperty.RegisterAttached("OuterEllipseStroke",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseStrokeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseStrokeProperty"/> from.</param>
        /// <returns>OuterEllipseStroke property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseStroke(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseStrokeProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseStrokeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseStrokeProperty"/> on.</param>
        /// <param name="value">OuterEllipseStroke property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseStroke(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseStrokeProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseStrokePointerOverProperty
            = DependencyProperty.RegisterAttached("OuterEllipseStrokePointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseStrokePointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseStrokePointerOverProperty"/> from.</param>
        /// <returns>OuterEllipseStrokePointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseStrokePointerOver(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseStrokePointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseStrokePointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseStrokePointerOverProperty"/> on.</param>
        /// <param name="value">OuterEllipseStrokePointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseStrokePointerOver(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseStrokePointerOverProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseStrokePressedProperty
            = DependencyProperty.RegisterAttached("OuterEllipseStrokePressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseStrokePressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseStrokePressedProperty"/> from.</param>
        /// <returns>OuterEllipseStrokePressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseStrokePressed(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseStrokePressedProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseStrokePressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseStrokePressedProperty"/> on.</param>
        /// <param name="value">OuterEllipseStrokePressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseStrokePressed(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseStrokePressedProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseStrokeDisabledProperty
            = DependencyProperty.RegisterAttached("OuterEllipseStrokeDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseStrokeDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseStrokeDisabledProperty"/> from.</param>
        /// <returns>OuterEllipseStrokeDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseStrokeDisabled(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseStrokeDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseStrokeDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseStrokeDisabledProperty"/> on.</param>
        /// <param name="value">OuterEllipseStrokeDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseStrokeDisabled(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseStrokeDisabledProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedFillProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedFill",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedFillProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedFillProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedFill property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedFill(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedFillProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedFillProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedFillProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedFill property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedFill(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedFillProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedFillPointerOverProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedFillPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedFillPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedFillPointerOverProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedFillPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedFillPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedFillPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedFillPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedFillPointerOverProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedFillPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedFillPointerOver(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedFillPointerOverProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedFillPressedProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedFillPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedFillPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedFillPressedProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedFillPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedFillPressed(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedFillPressedProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedFillPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedFillPressedProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedFillPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedFillPressed(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedFillPressedProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedFillDisabledProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedFillDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedFillDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedFillDisabledProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedFillDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedFillDisabled(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedFillDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedFillDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedFillDisabledProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedFillDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedFillDisabled(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedFillDisabledProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedStrokeProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedStroke",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedStrokeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedStrokeProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedStroke property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedStroke(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedStrokeProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedStrokeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedStrokeProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedStroke property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedStroke(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedStrokeProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedStrokePointerOverProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedStrokePointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedStrokePointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedStrokePointerOverProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedStrokePointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedStrokePointerOver(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedStrokePointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedStrokePointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedStrokePointerOverProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedStrokePointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedStrokePointerOver(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedStrokePointerOverProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedStrokePressedProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedStrokePressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedStrokePressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedStrokePressedProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedStrokePressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedStrokePressed(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedStrokePressedProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedStrokePressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedStrokePressedProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedStrokePressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedStrokePressed(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedStrokePressedProperty, value);
        }

        public static readonly DependencyProperty OuterEllipseCheckedStrokeDisabledProperty
            = DependencyProperty.RegisterAttached("OuterEllipseCheckedStrokeDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="OuterEllipseCheckedStrokeDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="OuterEllipseCheckedStrokeDisabledProperty"/> from.</param>
        /// <returns>OuterEllipseCheckedStrokeDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetOuterEllipseCheckedStrokeDisabled(UIElement element)
        {
            return (Brush)element.GetValue(OuterEllipseCheckedStrokeDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="OuterEllipseCheckedStrokeDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="OuterEllipseCheckedStrokeDisabledProperty"/> on.</param>
        /// <param name="value">OuterEllipseCheckedStrokeDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetOuterEllipseCheckedStrokeDisabled(UIElement element, Brush value)
        {
            element.SetValue(OuterEllipseCheckedStrokeDisabledProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphFillProperty
            = DependencyProperty.RegisterAttached("CheckGlyphFill",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphFillProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphFillProperty"/> from.</param>
        /// <returns>CheckGlyphFill property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphFill(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphFillProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphFillProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphFillProperty"/> on.</param>
        /// <param name="value">CheckGlyphFill property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphFill(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphFillProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphFillPointerOverProperty
            = DependencyProperty.RegisterAttached("CheckGlyphFillPointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphFillPointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphFillPointerOverProperty"/> from.</param>
        /// <returns>CheckGlyphFillPointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphFillPointerOver(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphFillPointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphFillPointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphFillPointerOverProperty"/> on.</param>
        /// <param name="value">CheckGlyphFillPointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphFillPointerOver(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphFillPointerOverProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphFillPressedProperty
            = DependencyProperty.RegisterAttached("CheckGlyphFillPressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphFillPressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphFillPressedProperty"/> from.</param>
        /// <returns>CheckGlyphFillPressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphFillPressed(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphFillPressedProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphFillPressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphFillPressedProperty"/> on.</param>
        /// <param name="value">CheckGlyphFillPressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphFillPressed(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphFillPressedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphFillDisabledProperty
            = DependencyProperty.RegisterAttached("CheckGlyphFillDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphFillDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphFillDisabledProperty"/> from.</param>
        /// <returns>CheckGlyphFillDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphFillDisabled(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphFillDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphFillDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphFillDisabledProperty"/> on.</param>
        /// <param name="value">CheckGlyphFillDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphFillDisabled(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphFillDisabledProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphStrokeProperty
            = DependencyProperty.RegisterAttached("CheckGlyphStroke",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphStrokeProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphStrokeProperty"/> from.</param>
        /// <returns>CheckGlyphStroke property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphStroke(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphStrokeProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphStrokeProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphStrokeProperty"/> on.</param>
        /// <param name="value">CheckGlyphStroke property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphStroke(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphStrokeProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphStrokePointerOverProperty
            = DependencyProperty.RegisterAttached("CheckGlyphStrokePointerOver",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphStrokePointerOverProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphStrokePointerOverProperty"/> from.</param>
        /// <returns>CheckGlyphStrokePointerOver property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphStrokePointerOver(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphStrokePointerOverProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphStrokePointerOverProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphStrokePointerOverProperty"/> on.</param>
        /// <param name="value">CheckGlyphStrokePointerOver property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphStrokePointerOver(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphStrokePointerOverProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphStrokePressedProperty
            = DependencyProperty.RegisterAttached("CheckGlyphStrokePressed",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphStrokePressedProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphStrokePressedProperty"/> from.</param>
        /// <returns>CheckGlyphStrokePressed property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphStrokePressed(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphStrokePressedProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphStrokePressedProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphStrokePressedProperty"/> on.</param>
        /// <param name="value">CheckGlyphStrokePressed property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphStrokePressed(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphStrokePressedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphStrokeDisabledProperty
            = DependencyProperty.RegisterAttached("CheckGlyphStrokeDisabled",
                                                  typeof(Brush),
                                                  typeof(RadioButtonHelper),
                                                  new PropertyMetadata(default(Brush)));

        /// <summary>Helper for getting <see cref="CheckGlyphStrokeDisabledProperty"/> from <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CheckGlyphStrokeDisabledProperty"/> from.</param>
        /// <returns>CheckGlyphStrokeDisabled property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static Brush GetCheckGlyphStrokeDisabled(UIElement element)
        {
            return (Brush)element.GetValue(CheckGlyphStrokeDisabledProperty);
        }

        /// <summary>Helper for setting <see cref="CheckGlyphStrokeDisabledProperty"/> on <paramref name="element"/>.</summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CheckGlyphStrokeDisabledProperty"/> on.</param>
        /// <param name="value">CheckGlyphStrokeDisabled property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetCheckGlyphStrokeDisabled(UIElement element, Brush value)
        {
            element.SetValue(CheckGlyphStrokeDisabledProperty, value);
        }
    }
}