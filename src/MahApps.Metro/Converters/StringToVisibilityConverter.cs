//	--------------------------------------------------------------------
//		Obtained from: WPFSmartLibrary
//		For more information see : http://wpfsmartlibrary.codeplex.com/
//		(by DotNetMastermind)
//	--------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    /// <summary>
    ///     Converts a String into a Visibility enumeration (and back)
    ///     The FalseEquivalent can be declared with the "FalseEquivalent" property
    /// </summary>
    [ValueConversion(typeof (string), typeof (Visibility))]
    [MarkupExtensionReturnType(typeof (StringToVisibilityConverter))]
    public class StringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        ///     Initialize the properties with standard values
        /// </summary>
        public StringToVisibilityConverter()
        {
            FalseEquivalent = Visibility.Collapsed;
            OppositeStringValue = false;
        }

        /// <summary>
        ///     FalseEquivalent (default : Visibility.Collapsed => see Constructor)
        /// </summary>
        public Visibility FalseEquivalent { get; set; }

        /// <summary>
        ///     Define whether the opposite boolean value is crucial (default : false)
        /// </summary>
        public bool OppositeStringValue { get; set; }

        #region MarkupExtension "overrides"

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new StringToVisibilityConverter
            {
                FalseEquivalent = FalseEquivalent,
                OppositeStringValue = OppositeStringValue
            };
        }

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null || value is string) && targetType == typeof (Visibility))
            {
                if (OppositeStringValue)
                {
                    return string.IsNullOrEmpty((string)value) ? Visibility.Visible : FalseEquivalent;
                }
                return string.IsNullOrEmpty((string)value) ? FalseEquivalent : Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (OppositeStringValue)
                {
                    return ((Visibility) value == Visibility.Visible) ? String.Empty : "visible";
                }
                return ((Visibility) value == Visibility.Visible) ? "visible" : String.Empty;
            }
            return value;
        }

        #endregion
    }
}