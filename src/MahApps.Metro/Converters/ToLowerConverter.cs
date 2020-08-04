// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Converters
{
    [MarkupExtensionReturnType(typeof(ToLowerConverter))]
    public class ToLowerConverter : MarkupConverter
    {
        private static ToLowerConverter _instance;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        [SuppressMessage("ReSharper", "EmptyConstructor")]
        static ToLowerConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new ToLowerConverter());
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string s ? s.ToLower(culture) : value;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}