// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    public static class DatePickerHelper
    {
        public static readonly DependencyProperty DropDownButtonContentProperty
            = DependencyProperty.RegisterAttached(
                "DropDownButtonContent",
                typeof(object),
                typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets the content of the DropDown Button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static object GetDropDownButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(DropDownButtonContentProperty);
        }

        /// <summary>
        /// Sets the content of the DropDown Button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static void SetDropDownButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(DropDownButtonContentProperty, value);
        }

        public static readonly DependencyProperty DropDownButtonContentTemplateProperty
            = DependencyProperty.RegisterAttached(
                "DropDownButtonContentTemplate",
                typeof(DataTemplate),
                typeof(DatePickerHelper),
                new FrameworkPropertyMetadata((DataTemplate)null));

        /// <summary> 
        /// Gets the data template used to display the content of the DropDown Button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static DataTemplate GetDropDownButtonContentTemplate(DependencyObject d)
        {
            return (DataTemplate)d.GetValue(DropDownButtonContentTemplateProperty);
        }

        /// <summary> 
        /// Sets the data template used to display the content of the DropDown Button.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static void SetDropDownButtonContentTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DropDownButtonContentTemplateProperty, value);
        }

        public static readonly DependencyProperty DropDownButtonFontFamilyProperty
            = DependencyProperty.RegisterAttached(
                "DropDownButtonFontFamily",
                typeof(FontFamily),
                typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the font family of the desired font.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static FontFamily GetDropDownButtonFontFamily(DependencyObject d)
        {
            return (FontFamily)d.GetValue(DropDownButtonFontFamilyProperty);
        }

        /// <summary>
        /// Sets the font family of the desired font.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static void SetDropDownButtonFontFamily(DependencyObject obj, FontFamily value)
        {
            obj.SetValue(DropDownButtonFontFamilyProperty, value);
        }

        public static readonly DependencyProperty DropDownButtonFontSizeProperty
            = DependencyProperty.RegisterAttached(
                "DropDownButtonFontSize",
                typeof(double),
                typeof(DatePickerHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the size of the desired font.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static double GetDropDownButtonFontSize(DependencyObject d)
        {
            return (double)d.GetValue(DropDownButtonFontSizeProperty);
        }

        /// <summary>
        /// Sets the size of the desired font.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        public static void SetDropDownButtonFontSize(DependencyObject obj, double value)
        {
            obj.SetValue(DropDownButtonFontSizeProperty, value);
        }
    }
}