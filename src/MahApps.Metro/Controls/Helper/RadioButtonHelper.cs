using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class RadioButtonHelper
    {
        public static readonly DependencyProperty RadioStrokeThicknessProperty
            = DependencyProperty.RegisterAttached(
                "RadioStrokeThickness",
                typeof(double),
                typeof(RadioButtonHelper),
                new FrameworkPropertyMetadata(1.0));

        /// <summary>
        /// Gets the StrokeThickness of the RadioButton itself.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static double GetRadioStrokeThickness(DependencyObject obj)
        {
            return (double)obj.GetValue(RadioStrokeThicknessProperty);
        }

        /// <summary>
        /// Sets the StrokeThickness of the RadioButton itself.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        public static void SetRadioStrokeThickness(DependencyObject obj, double value)
        {
            obj.SetValue(RadioStrokeThicknessProperty, value);
        }
    }
}