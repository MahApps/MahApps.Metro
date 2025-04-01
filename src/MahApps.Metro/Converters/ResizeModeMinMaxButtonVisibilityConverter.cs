﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Metro.Converters
{
    public sealed class ResizeModeMinMaxButtonVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets a static default instance of <see cref="ResizeModeMinMaxButtonVisibilityConverter"/>.
        /// </summary>
        public static readonly ResizeModeMinMaxButtonVisibilityConverter Instance = new();

        public object? Convert(object[]? values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values != null && parameter is ResizeModeButtonType whichButton)
            {
                var showButton = (values.ElementAtOrDefault(0) as bool?).GetValueOrDefault(true);

                if (whichButton == ResizeModeButtonType.Close)
                {
                    return !showButton ? Visibility.Collapsed : Visibility.Visible;
                }

                var windowResizeMode = (values.ElementAtOrDefault(1) as ResizeMode?).GetValueOrDefault(ResizeMode.CanResize);

                switch (windowResizeMode)
                {
                    case ResizeMode.NoResize:
                        return Visibility.Collapsed;
                    case ResizeMode.CanMinimize:
                        if (whichButton == ResizeModeButtonType.Min)
                        {
                            return !showButton ? Visibility.Collapsed : Visibility.Visible;
                        }

                        return Visibility.Collapsed;
                    case ResizeMode.CanResize:
                    case ResizeMode.CanResizeWithGrip:
                    default:
                        return !showButton ? Visibility.Collapsed : Visibility.Visible;
                }
            }

            return Visibility.Visible;
        }

        public object[]? ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
        {
            return targetTypes.Select(t => DependencyProperty.UnsetValue).ToArray();
        }
    }
}