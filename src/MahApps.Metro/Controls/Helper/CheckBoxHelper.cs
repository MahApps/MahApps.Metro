using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class CheckBoxHelper
    {
        public static readonly DependencyProperty CheckSizeProperty
            = DependencyProperty.RegisterAttached(
                "CheckSize",
                typeof(double),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(18.0));

        /// <summary>
        /// Gets the size of the CheckBox itself.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static double GetCheckSize(DependencyObject obj)
        {
            return (double)obj.GetValue(CheckSizeProperty);
        }

        /// <summary>
        /// Sets the size of the CheckBox itself.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckSize(DependencyObject obj, double value)
        {
            obj.SetValue(CheckSizeProperty, value);
        }

        #region Checked

        public static readonly DependencyProperty CheckGlyphCheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphChecked",
                typeof(object),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the Glyph for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static object GetCheckGlyphChecked(DependencyObject obj)
        {
            return (object)obj.GetValue(CheckGlyphCheckedProperty);
        }

        /// <summary>
        /// Sets the Glyph for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphChecked(DependencyObject obj, object value)
        {
            obj.SetValue(CheckGlyphCheckedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphCheckedTemplateProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphCheckedTemplate",
                typeof(DataTemplate),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets the GlyphTemplate for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static DataTemplate GetCheckGlyphCheckedTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CheckGlyphCheckedTemplateProperty);
        }

        /// <summary>
        /// Sets the GlyphTemplate for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphCheckedTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CheckGlyphCheckedTemplateProperty, value);
        }

        public static readonly DependencyProperty CheckBackgroundBrushCheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckBackgroundBrushChecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the check Background for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBackgroundBrushChecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBackgroundBrushCheckedProperty);
        }

        /// <summary>
        /// Sets the check Background for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBackgroundBrushChecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBackgroundBrushCheckedProperty, value);
        }

        public static readonly DependencyProperty CheckBorderBrushCheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckBorderBrushChecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the check BorderBrush for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBorderBrushChecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBorderBrushCheckedProperty);
        }

        /// <summary>
        /// Sets the check BorderBrush for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBorderBrushChecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBorderBrushCheckedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphForegroundBrushCheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphForegroundBrushChecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the the check glyph Foreground for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckGlyphForegroundBrushChecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckGlyphForegroundBrushCheckedProperty);
        }

        /// <summary>
        /// Sets the the check glyph Foreground for IsChecked = true.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphForegroundBrushChecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckGlyphForegroundBrushCheckedProperty, value);
        }

        #endregion

        #region Unchecked

        public static readonly DependencyProperty CheckGlyphUncheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphUnchecked",
                typeof(object),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the Glyph for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static object GetCheckGlyphUnchecked(DependencyObject obj)
        {
            return (object)obj.GetValue(CheckGlyphUncheckedProperty);
        }

        /// <summary>
        /// Sets the Glyph for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphUnchecked(DependencyObject obj, object value)
        {
            obj.SetValue(CheckGlyphUncheckedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphUncheckedTemplateProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphUncheckedTemplate",
                typeof(DataTemplate),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets the GlyphTemplate for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static DataTemplate GetCheckGlyphUncheckedTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CheckGlyphUncheckedTemplateProperty);
        }

        /// <summary>
        /// Sets the GlyphTemplate for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphUncheckedTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CheckGlyphUncheckedTemplateProperty, value);
        }

        public static readonly DependencyProperty CheckBackgroundBrushUncheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckBackgroundBrushUnchecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the check BackgroundBrush for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBackgroundBrushUnchecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBackgroundBrushUncheckedProperty);
        }

        /// <summary>
        /// Sets the the BackgroundBrush for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBackgroundBrushUnchecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBackgroundBrushUncheckedProperty, value);
        }

        public static readonly DependencyProperty CheckBorderBrushUncheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckBorderBrushUnchecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the check BorderBrush for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBorderBrushUnchecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBorderBrushUncheckedProperty);
        }

        /// <summary>
        /// Sets the check BorderBrush for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBorderBrushUnchecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBorderBrushUncheckedProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphForegroundBrushUncheckedProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphForegroundBrushUnchecked",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the check Foreground for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckGlyphForegroundBrushUnchecked(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckGlyphForegroundBrushUncheckedProperty);
        }

        /// <summary>
        /// Sets the check Foreground for IsChecked = false.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphForegroundBrushUnchecked(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckGlyphForegroundBrushUncheckedProperty, value);
        }

        #endregion

        #region Intermediate

        public static readonly DependencyProperty CheckGlyphIntermediateProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphIntermediate",
                typeof(object),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the Glyph for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static object GetCheckGlyphIntermediate(DependencyObject obj)
        {
            return (object)obj.GetValue(CheckGlyphIntermediateProperty);
        }

        /// <summary>
        /// Sets the Glyph for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphIntermediate(DependencyObject obj, object value)
        {
            obj.SetValue(CheckGlyphIntermediateProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphIndeterminateTemplateProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphIndeterminateTemplate",
                typeof(DataTemplate),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// Gets the GlyphTemplate for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static DataTemplate GetCheckGlyphIndeterminateTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CheckGlyphIndeterminateTemplateProperty);
        }

        /// <summary>
        /// Sets the GlyphTemplate for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphIndeterminateTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(CheckGlyphIndeterminateTemplateProperty, value);
        }

        public static readonly DependencyProperty CheckBackgroundBrushIntermediateProperty
            = DependencyProperty.RegisterAttached(
                "CheckBackgroundBrushIntermediate",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the glyph BackgroundBrush for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBackgroundBrushIntermediate(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBackgroundBrushIntermediateProperty);
        }

        /// <summary>
        /// Sets the glyph BackgroundBrush for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBackgroundBrushIntermediate(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBackgroundBrushIntermediateProperty, value);
        }

        public static readonly DependencyProperty CheckBorderBrushIntermediateProperty
            = DependencyProperty.RegisterAttached(
                "CheckBorderBrushIntermediate",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the glyph BorderBrush for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckBorderBrushIntermediate(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckBorderBrushIntermediateProperty);
        }

        /// <summary>
        /// Sets the glyph BorderBrush for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckBorderBrushIntermediate(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckBorderBrushIntermediateProperty, value);
        }

        public static readonly DependencyProperty CheckGlyphForegroundBrushIntermediateProperty
            = DependencyProperty.RegisterAttached(
                "CheckGlyphForegroundBrushIntermediate",
                typeof(Brush),
                typeof(CheckBoxHelper),
                new FrameworkPropertyMetadata(default(Brush)));

        /// <summary>
        /// Gets the glyph Foreground for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static Brush GetCheckGlyphForegroundBrushIntermediate(DependencyObject obj)
        {
            return (Brush)obj.GetValue(CheckGlyphForegroundBrushIntermediateProperty);
        }

        /// <summary>
        /// Sets the glyph Foregorund for IsChecked = null.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        public static void SetCheckGlyphForegroundBrushIntermediate(DependencyObject obj, Brush value)
        {
            obj.SetValue(CheckGlyphForegroundBrushIntermediateProperty, value);
        }

        #endregion
    }
}