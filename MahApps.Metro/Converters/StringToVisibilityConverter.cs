//	--------------------------------------------------------------------
//		Obtained from: WPFSmartLibrary
//		For more information see : http://wpfsmartlibrary.codeplex.com/
//		(by DotNetMastermind)
//	--------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    /// <summary>
    /// Converts a String into a Visibility enumeration (and back)
    /// The FalseEquivalent can be declared with the "FalseEquivalent" property
    /// </summary>
    [ValueConversion(typeof(string), typeof(Visibility))]
    [MarkupExtensionReturnType(typeof(StringToVisibilityConverter))]
    public class StringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// FalseEquivalent (default : Visibility.Collapsed => see Constructor)
        /// </summary>
        public Visibility FalseEquivalent { get; set; }

        /// <summary>
        /// Define whether the opposite boolean value is crucial (default : false)
        /// </summary>
        public bool OppositeStringValue { get; set; }

        /// <summary>
        /// Initialize the properties with standard values
        /// </summary>
        public StringToVisibilityConverter()
        {
            this.FalseEquivalent = Visibility.Collapsed;
            this.OppositeStringValue = false;
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && targetType == typeof(Visibility))
            {
                if (this.OppositeStringValue == true)
                {
                    return ((value as string).ToLower().Equals(String.Empty)) ? Visibility.Visible : this.FalseEquivalent;
                }
                return ((value as string).ToLower().Equals(String.Empty)) ? this.FalseEquivalent : Visibility.Visible;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (this.OppositeStringValue == true)
                {
                    return ((Visibility)value == Visibility.Visible) ? String.Empty : "visible";
                }
                return ((Visibility)value == Visibility.Visible) ? "visible" : String.Empty;
            }
            return value;
        }

        #endregion

        #region MarkupExtension "overrides"

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new StringToVisibilityConverter
            {
                FalseEquivalent = this.FalseEquivalent,
                OppositeStringValue = this.OppositeStringValue
            };
        }

        #endregion
    }
}
