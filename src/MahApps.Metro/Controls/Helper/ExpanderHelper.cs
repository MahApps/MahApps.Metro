// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A helper class that provides various attached properties for the Expander control.
    /// <see cref="Expander"/>
    /// </summary>
    public static class ExpanderHelper
    {
        public static readonly DependencyProperty HeaderUpStyleProperty = DependencyProperty.RegisterAttached("HeaderUpStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderUpStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderUpStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Up.
        /// </summary>
        public static void SetHeaderUpStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderUpStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderDownStyleProperty = DependencyProperty.RegisterAttached("HeaderDownStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderDownStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderDownStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Down.
        /// </summary>
        public static void SetHeaderDownStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderDownStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderLeftStyleProperty = DependencyProperty.RegisterAttached("HeaderLeftStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderLeftStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderLeftStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Left.
        /// </summary>
        public static void SetHeaderLeftStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderLeftStyleProperty, value);
        }

        public static readonly DependencyProperty HeaderRightStyleProperty = DependencyProperty.RegisterAttached("HeaderRightStyle", typeof(Style), typeof(ExpanderHelper), new FrameworkPropertyMetadata((Style)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Style GetHeaderRightStyle(UIElement element)
        {
            return (Style)element.GetValue(HeaderRightStyleProperty);
        }

        /// <summary>
        /// Sets the toggle button style used for the ExpandDirection Right.
        /// </summary>
        public static void SetHeaderRightStyle(UIElement element, Style value)
        {
            element.SetValue(HeaderRightStyleProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the <see cref="Expander"/>' ExpandStoryboard property.
        ///
        /// If the Storyboard is set, the expanded event applies this to the inner grid.
        /// </summary>
        public static readonly DependencyProperty ExpandStoryboardProperty
            = DependencyProperty.RegisterAttached("ExpandStoryboard",
                                                  typeof(Storyboard),
                                                  typeof(ExpanderHelper),
                                                  new FrameworkPropertyMetadata((Storyboard)null, OnExpandStoryboardPropertyChangedCallback));

        private static void OnExpandStoryboardPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && dependencyObject is Expander expander)
            {
                expander.Expanded -= Expander_Expanded;
                expander.ExecuteWhenLoaded(() =>
                    {
                        if (expander.IsExpanded)
                        {
                            var expandSite = GetExpandSite(expander);
                            if (expandSite is null)
                            {
                                return;
                            }

                            expandSite.SetCurrentValue(UIElement.OpacityProperty, 1d);
                            expandSite.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
                        }
                    });

                if (e.NewValue is Storyboard)
                {
                    expander.Expanded += Expander_Expanded;
                }
            }
        }

        private static void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Expander expander && Equals(sender, e.OriginalSource))
            {
                var expandSite = GetExpandSite(expander);
                if (expandSite is null)
                {
                    return;
                }

                var storyBoard = GetExpandStoryboard(expander);
                if (storyBoard is null)
                {
                    expandSite.SetCurrentValue(UIElement.OpacityProperty, 1d);
                    return;
                }

                try
                {
                    storyBoard.Begin(expandSite);
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"The storyboard {storyBoard} could not be applied: {exception}");
                }
            }
        }

        /// <summary>
        /// Helper for getting <see cref="ExpandStoryboardProperty"/> from <paramref name="element"/>.
        ///
        /// If the Storyboard is set, the expanded event applies this to the inner grid.
        /// </summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="ExpandStoryboardProperty"/> from.</param>
        /// <returns>ExpandStoryboard property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Storyboard GetExpandStoryboard(UIElement element)
        {
            return (Storyboard)element.GetValue(ExpandStoryboardProperty);
        }

        /// <summary>
        /// Helper for setting <see cref="ExpandStoryboardProperty"/> on <paramref name="element"/>.
        ///
        /// If the Storyboard is set, the expanded event applies this to the inner grid.
        /// </summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="ExpandStoryboardProperty"/> on.</param>
        /// <param name="value">ExpandStoryboard property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetExpandStoryboard(UIElement element, Storyboard value)
        {
            element.SetValue(ExpandStoryboardProperty, value);
        }

        /// <summary>
        /// The DependencyProperty for the <see cref="Expander"/>' CollapseStoryboard property.
        ///
        /// If the Storyboard is set, the collapsed event applies this to the inner grid.
        /// </summary>
        public static readonly DependencyProperty CollapseStoryboardProperty
            = DependencyProperty.RegisterAttached("CollapseStoryboard",
                                                  typeof(Storyboard),
                                                  typeof(ExpanderHelper),
                                                  new FrameworkPropertyMetadata((Storyboard)null, OnCollapseStoryboardPropertyChangedCallback));

        private static void OnCollapseStoryboardPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue && dependencyObject is Expander expander)
            {
                expander.Collapsed -= Expander_Collapsed;
                expander.ExecuteWhenLoaded(() =>
                    {
                        if (!expander.IsExpanded)
                        {
                            var expandSite = GetExpandSite(expander);
                            if (expandSite is null)
                            {
                                return;
                            }

                            expandSite.SetCurrentValue(UIElement.OpacityProperty, 0d);
                            expandSite.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Collapsed);
                        }
                    });

                if (e.NewValue is Storyboard)
                {
                    expander.Collapsed += Expander_Collapsed;
                }
            }
        }

        private static void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Expander expander && Equals(sender, e.OriginalSource))
            {
                var expandSite = GetExpandSite(expander);
                if (expandSite is null)
                {
                    return;
                }

                var storyBoard = GetCollapseStoryboard(expander);
                if (storyBoard is null)
                {
                    expandSite.SetCurrentValue(UIElement.OpacityProperty, 0d);
                    return;
                }

                try
                {
                    storyBoard.Begin(expandSite);
                }
                catch (Exception exception)
                {
                    Trace.TraceError($"The storyboard {storyBoard} could not be applied: {exception}");
                }
            }
        }

        /// <summary>
        /// Helper for getting <see cref="CollapseStoryboardProperty"/> from <paramref name="element"/>.
        ///
        /// If the Storyboard is set, the collapsed event applies this to the inner grid.
        /// </summary>
        /// <param name="element"><see cref="UIElement"/> to read <see cref="CollapseStoryboardProperty"/> from.</param>
        /// <returns>CollapseStoryboard property value.</returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static Storyboard GetCollapseStoryboard(UIElement element)
        {
            return (Storyboard)element.GetValue(CollapseStoryboardProperty);
        }

        /// <summary>
        /// Helper for setting <see cref="CollapseStoryboardProperty"/> on <paramref name="element"/>.
        ///
        /// If the Storyboard is set, the collapsed event applies this to the inner grid.
        /// </summary>
        /// <param name="element"><see cref="UIElement"/> to set <see cref="CollapseStoryboardProperty"/> on.</param>
        /// <param name="value">CollapseStoryboard property value.</param>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(Expander))]
        public static void SetCollapseStoryboard(UIElement element, Storyboard value)
        {
            element.SetValue(CollapseStoryboardProperty, value);
        }

        internal static FrameworkElement GetExpandSite(Expander expander)
        {
            if (expander is null)
            {
                return null;
            }

            var expandSite = GetExpandSiteControl(expander);
            if (expandSite is null)
            {
                expander.ApplyTemplate();
                expandSite = expander.Template?.FindName("ExpandSite", expander) as FrameworkElement;

                SetExpandSiteControl(expander, expandSite);
            }

            return expandSite;
        }

        internal static readonly DependencyProperty ExpandSiteControlProperty
            = DependencyProperty.RegisterAttached("ExpandSiteControl",
                                                  typeof(FrameworkElement),
                                                  typeof(ExpanderHelper),
                                                  new PropertyMetadata(default(FrameworkElement)));

        internal static FrameworkElement GetExpandSiteControl(UIElement element)
        {
            return (FrameworkElement)element.GetValue(ExpandSiteControlProperty);
        }

        internal static void SetExpandSiteControl(UIElement element, FrameworkElement value)
        {
            element.SetValue(ExpandSiteControlProperty, value);
        }
    }
}