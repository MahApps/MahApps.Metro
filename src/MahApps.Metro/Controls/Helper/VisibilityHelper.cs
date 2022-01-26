// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.ValueBoxes;

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
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender,
                                              (d, e) => SetVisibility(d, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Visible : Visibility.Collapsed)));

        [Category(AppName.MahApps)]
        public static void SetIsVisible(DependencyObject element, bool? value)
        {
            element.SetValue(IsVisibleProperty, BooleanBoxes.Box(value));
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
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender,
                                              (d, e) => SetVisibility(d, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Collapsed : Visibility.Visible)));

        [Category(AppName.MahApps)]
        public static void SetIsCollapsed(DependencyObject element, bool? value)
        {
            element.SetValue(IsCollapsedProperty, BooleanBoxes.Box(value));
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
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender,
                                              (d, e) => SetVisibility(d, (e.NewValue as bool?).GetValueOrDefault() ? Visibility.Hidden : Visibility.Visible)));

        [Category(AppName.MahApps)]
        public static void SetIsHidden(DependencyObject element, bool? value)
        {
            element.SetValue(IsHiddenProperty, BooleanBoxes.Box(value));
        }

        [Category(AppName.MahApps)]
        public static bool? GetIsHidden(DependencyObject element)
        {
            return (bool?)element.GetValue(IsHiddenProperty);
        }

        private static void SetVisibility(DependencyObject depObject, Visibility visibility)
        {
            switch (depObject)
            {
                case FrameworkElement fe:
                    fe.Visibility = visibility;
                    break;
                case DataGridColumn dataGridColumn:
                    dataGridColumn.Visibility = visibility;
                    break;
            }
        }
    }
}