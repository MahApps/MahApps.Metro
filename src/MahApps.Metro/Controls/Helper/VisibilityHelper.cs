﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public static class VisibilityHelper
    {
        public static readonly DependencyProperty IsVisibleProperty
            = DependencyProperty.RegisterAttached(
                "IsVisible",
                typeof(bool?),
                typeof(VisibilityHelper),
                new FrameworkPropertyMetadata(default(bool?),
                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                              IsVisibleChangedCallback));

        private static void IsVisibleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            else if (d is DataGridColumn dataGridColumn)
            {
                dataGridColumn.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public static void SetIsVisible(DependencyObject element, bool? value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        [Category(AppName.MahApps)]
        public static bool? GetIsVisible(DependencyObject element)
        {
            return (bool?)element.GetValue(IsVisibleProperty);
        }

        public static readonly DependencyProperty IsCollapsedProperty
            = DependencyProperty.RegisterAttached(
                "IsCollapsed",
                typeof(bool?),
                typeof(VisibilityHelper),
                new FrameworkPropertyMetadata(default(bool?),
                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                              IsCollapsedChangedCallback));

        private static void IsCollapsedChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
            else if (d is DataGridColumn dataGridColumn)
            {
                dataGridColumn.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        public static void SetIsCollapsed(DependencyObject element, bool? value)
        {
            element.SetValue(IsCollapsedProperty, value);
        }

        [Category(AppName.MahApps)]
        public static bool? GetIsCollapsed(DependencyObject element)
        {
            return (bool?)element.GetValue(IsCollapsedProperty);
        }

        public static readonly DependencyProperty IsHiddenProperty
            = DependencyProperty.RegisterAttached(
                "IsHidden",
                typeof(bool?),
                typeof(VisibilityHelper),
                new FrameworkPropertyMetadata(default(bool?),
                                              FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                                              IsHiddenChangedCallback));

        private static void IsHiddenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
            else if (d is DataGridColumn dataGridColumn)
            {
                dataGridColumn.Visibility = (bool?)e.NewValue == true
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
        }

        public static void SetIsHidden(DependencyObject element, bool? value)
        {
            element.SetValue(IsHiddenProperty, value);
        }

        [Category(AppName.MahApps)]
        public static bool? GetIsHidden(DependencyObject element)
        {
            return (bool?)element.GetValue(IsHiddenProperty);
        }
    }
}