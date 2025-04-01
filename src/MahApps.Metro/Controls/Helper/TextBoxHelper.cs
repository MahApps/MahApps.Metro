// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    internal enum SpellingResourceKeyId
    {
        SuggestionMenuItemStyle,
        IgnoreAllMenuItemStyle,
        NoSuggestionsMenuItemStyle,
        SeparatorStyle,
    }

    public static class Spelling
    {
        public static ResourceKey SuggestionMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), SpellingResourceKeyId.SuggestionMenuItemStyle);

        public static ResourceKey IgnoreAllMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), SpellingResourceKeyId.IgnoreAllMenuItemStyle);

        public static ResourceKey NoSuggestionsMenuItemStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), SpellingResourceKeyId.NoSuggestionsMenuItemStyle);

        public static ResourceKey SeparatorStyleKey { get; } = new ComponentResourceKey(typeof(Spelling), SpellingResourceKeyId.SeparatorStyle);
    }

    /// <summary>
    /// A helper class that provides various attached properties for the TextBox control.
    /// </summary>
    /// <remarks>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </remarks>
    public class TextBoxHelper
    {
        public static readonly DependencyProperty IsMonitoringProperty
            = DependencyProperty.RegisterAttached(
                "IsMonitoring",
                typeof(bool),
                typeof(TextBoxHelper),
                new UIPropertyMetadata(BooleanBoxes.FalseBox, OnIsMonitoringChanged));

        [Category(AppName.MahApps)]
        public static bool GetIsMonitoring(UIElement element)
        {
            return (bool)element.GetValue(IsMonitoringProperty);
        }

        [Category(AppName.MahApps)]
        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty WatermarkProperty
            = DependencyProperty.RegisterAttached(
                "Watermark",
                typeof(string),
                typeof(TextBoxHelper),
                new UIPropertyMetadata(string.Empty));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkAlignmentProperty
            = DependencyProperty.RegisterAttached(
                "WatermarkAlignment",
                typeof(TextAlignment),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(TextAlignment.Left, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets a value that indicates the horizontal alignment of the watermark.
        /// </summary>
        /// <returns>
        /// One of the <see cref="System.Windows.TextAlignment" /> values that specifies the desired alignment. The default is <see cref="System.Windows.TextAlignment.Left" />.
        /// </returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static TextAlignment GetWatermarkAlignment(DependencyObject obj)
        {
            return (TextAlignment)obj.GetValue(WatermarkAlignmentProperty);
        }

        /// <summary>
        /// Sets a value that indicates the horizontal alignment of the watermark.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetWatermarkAlignment(DependencyObject obj, TextAlignment value)
        {
            obj.SetValue(WatermarkAlignmentProperty, value);
        }

        public static readonly DependencyProperty WatermarkTrimmingProperty
            = DependencyProperty.RegisterAttached(
                "WatermarkTrimming",
                typeof(TextTrimming),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(TextTrimming.None, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets the text trimming behavior to employ when watermark overflows the content area.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Windows.TextTrimming" /> values that specifies the text trimming behavior to employ. The default is <see cref="F:System.Windows.TextTrimming.None" />.
        /// </returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static TextTrimming GetWatermarkTrimming(DependencyObject obj)
        {
            return (TextTrimming)obj.GetValue(WatermarkTrimmingProperty);
        }

        /// <summary>
        /// Sets the text trimming behavior to employ when watermark overflows the content area.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetWatermarkTrimming(DependencyObject obj, TextTrimming value)
        {
            obj.SetValue(WatermarkTrimmingProperty, value);
        }

        public static readonly DependencyProperty WatermarkWrappingProperty
            = DependencyProperty.RegisterAttached(
                "WatermarkWrapping",
                typeof(TextWrapping),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets how the watermark should wrap text.
        /// </summary>
        /// <returns>One of the <see cref="T:System.Windows.TextWrapping" /> values. The default is <see cref="F:System.Windows.TextWrapping.NoWrap" />.
        /// </returns>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static TextWrapping GetWatermarkWrapping(DependencyObject obj)
        {
            return (TextWrapping)obj.GetValue(WatermarkWrappingProperty);
        }

        /// <summary>
        /// Sets how the watermark should wrap text.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static void SetWatermarkWrapping(DependencyObject obj, TextWrapping value)
        {
            obj.SetValue(WatermarkWrappingProperty, value);
        }

        public static readonly DependencyProperty UseFloatingWatermarkProperty
            = DependencyProperty.RegisterAttached(
                "UseFloatingWatermark",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, ButtonCommandOrClearTextChanged));

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static bool GetUseFloatingWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseFloatingWatermarkProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetUseFloatingWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(UseFloatingWatermarkProperty, BooleanBoxes.Box(value));
        }

        /// <summary>
        /// This property can be used to retrieve the watermark using the <see cref="DisplayAttribute"/> of bound property.
        /// </summary>
        /// <remarks>
        /// Setting this property to true will uses reflection.
        /// </remarks>
        public static readonly DependencyProperty AutoWatermarkProperty
            = DependencyProperty.RegisterAttached(
                "AutoWatermark",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(BooleanBoxes.FalseBox, OnAutoWatermarkChanged));

        private static void OnAutoWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && e.OldValue != e.NewValue)
            {
                if ((e.NewValue as bool?).GetValueOrDefault())
                {
                    if (element.IsLoaded)
                    {
                        OnControlWithAutoWatermarkSupportLoaded(element, new RoutedEventArgs());
                    }
                    else
                    {
                        element.Loaded += OnControlWithAutoWatermarkSupportLoaded;
                    }
                }
                else
                {
                    element.Loaded -= OnControlWithAutoWatermarkSupportLoaded;
                }
            }
        }

        private static void OnControlWithAutoWatermarkSupportLoaded(object o, RoutedEventArgs routedEventArgs)
        {
            if (o is FrameworkElement fe)
            {
                fe.Loaded -= OnControlWithAutoWatermarkSupportLoaded;

                if (!AutoWatermarkPropertyMapping.TryGetValue(fe.GetType(), out var dependencyProperty))
                {
                    throw new NotSupportedException($"{nameof(AutoWatermarkProperty)} is not supported for {fe.GetType()}");
                }

                var resolvedProperty = ResolvePropertyFromBindingExpression(fe.GetBindingExpression(dependencyProperty));
                if (resolvedProperty != null)
                {
                    var attribute = resolvedProperty.GetCustomAttribute<DisplayAttribute>();
                    if (attribute != null)
                    {
                        fe.SetCurrentValue(WatermarkProperty, attribute.GetPrompt());
                    }
                }
            }
        }

        private static PropertyInfo? ResolvePropertyFromBindingExpression(BindingExpression? bindingExpression)
        {
            if (bindingExpression != null && bindingExpression.Status != BindingStatus.PathError)
            {
                var propertyName = bindingExpression.ResolvedSourcePropertyName;
                if (!string.IsNullOrEmpty(propertyName))
                {
                    var resolvedType = bindingExpression.ResolvedSource?.GetType();
                    if (resolvedType != null)
                    {
                        return resolvedType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    }
                }
            }

            return null;
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static bool GetAutoWatermark(DependencyObject element)
        {
            return (bool)element.GetValue(AutoWatermarkProperty);
        }

        /// <summary>
        /// Indicates whether if the watermark is automatically retrieved by using the <see cref="DisplayAttribute"/> of the bound property.
        /// </summary>
        /// <remarks>
        /// This attached property uses reflection; thus it might reduce the performance of the application.
        /// The auto-watermark does work for the following controls:
        /// In the following case no custom watermark is shown
        /// <list type="bullet">
        /// <item>There is no binding</item>
        /// <item>Binding path errors</item>
        /// <item>Binding to a element of a collection without using a property of that element <c>Binding Path=Collection[0]</c> use: <c>Binding Path=Collection[0].SubProperty</c></item>
        /// <item>The bound property does not have a <see cref="DisplayAttribute"/></item>
        /// </list>
        /// </remarks>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        [AttachedPropertyBrowsableForType(typeof(HotKeyBox))]
        [AttachedPropertyBrowsableForType(typeof(ColorPicker))]
        public static void SetAutoWatermark(DependencyObject element, bool value)
        {
            element.SetValue(AutoWatermarkProperty, BooleanBoxes.Box(value));
        }

        private static readonly Dictionary<Type, DependencyProperty> AutoWatermarkPropertyMapping
            = new()
              {
                  { typeof(TextBox), TextBox.TextProperty },
                  { typeof(ComboBox), Selector.SelectedItemProperty },
                  { typeof(NumericUpDown), NumericUpDown.ValueProperty },
                  { typeof(HotKeyBox), HotKeyBox.HotKeyProperty },
                  { typeof(DatePicker), DatePicker.SelectedDateProperty },
                  { typeof(TimePicker), TimePickerBase.SelectedDateTimeProperty },
                  { typeof(DateTimePicker), TimePickerBase.SelectedDateTimeProperty },
                  { typeof(ColorPicker), ColorPickerBase.SelectedColorProperty },
                  { typeof(ColorCanvas), ColorPickerBase.SelectedColorProperty },
                  { typeof(MultiSelectionComboBox), MultiSelectionComboBox.SelectedItemProperty }
              };

        internal static readonly DependencyPropertyKey TextLengthPropertyKey
            = DependencyProperty.RegisterAttachedReadOnly(
                "TextLength",
                typeof(int),
                typeof(TextBoxHelper),
                new PropertyMetadata(0));

        public static readonly DependencyProperty TextLengthProperty = TextLengthPropertyKey.DependencyProperty;

        [Category(AppName.MahApps)]
        public static int GetTextLength(UIElement element)
        {
            return (int)element.GetValue(TextLengthProperty);
        }

        public static readonly DependencyProperty HasTextProperty
            = DependencyProperty.RegisterAttached(
                "HasText",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets if the attached TextBox has text.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(MultiSelectionComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty ClearTextButtonProperty
            = DependencyProperty.RegisterAttached(
                "ClearTextButton",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, ButtonCommandOrClearTextChanged));

        /// <summary>
        /// Gets the clear text button visibility / feature. Can be used to enable text deletion.
        /// </summary>
        [Category(AppName.MahApps)]
        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        /// <summary>
        /// Sets the clear text button visibility / feature. Can be used to enable text deletion.
        /// </summary>
        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty ButtonsAlignmentProperty
            = DependencyProperty.RegisterAttached(
                "ButtonsAlignment",
                typeof(ButtonsAlignment),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets the buttons placement variant.
        /// </summary>
        [Category(AppName.MahApps)]
        public static ButtonsAlignment GetButtonsAlignment(DependencyObject d)
        {
            return (ButtonsAlignment)d.GetValue(ButtonsAlignmentProperty);
        }

        /// <summary>
        /// Sets the buttons placement variant.
        /// </summary>
        public static void SetButtonsAlignment(DependencyObject obj, ButtonsAlignment value)
        {
            obj.SetValue(ButtonsAlignmentProperty, value);
        }

        /// <summary>
        /// This property can be used to set the button width (PART_ClearText) of TextBox, PasswordBox, ComboBox, NumericUpDown
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty
            = DependencyProperty.RegisterAttached(
                "ButtonWidth",
                typeof(double),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(22d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        [Category(AppName.MahApps)]
        public static double GetButtonWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandProperty
            = DependencyProperty.RegisterAttached(
                "ButtonCommand",
                typeof(ICommand),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));

        [Category(AppName.MahApps)]
        public static ICommand? GetButtonCommand(DependencyObject d)
        {
            return (ICommand?)d.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(DependencyObject obj, ICommand? value)
        {
            obj.SetValue(ButtonCommandProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandParameterProperty
            = DependencyProperty.RegisterAttached(
                "ButtonCommandParameter",
                typeof(object),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        [Category(AppName.MahApps)]
        public static object? GetButtonCommandParameter(DependencyObject d)
        {
            return (object?)d.GetValue(ButtonCommandParameterProperty);
        }

        public static void SetButtonCommandParameter(DependencyObject obj, object? value)
        {
            obj.SetValue(ButtonCommandParameterProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandTargetProperty
            = DependencyProperty.RegisterAttached(
                "ButtonCommandTarget",
                typeof(IInputElement),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        [Category(AppName.MahApps)]
        public static IInputElement? GetButtonCommandTarget(DependencyObject d)
        {
            return (IInputElement?)d.GetValue(ButtonCommandTargetProperty);
        }

        public static void SetButtonCommandTarget(DependencyObject obj, IInputElement? value)
        {
            obj.SetValue(ButtonCommandTargetProperty, value);
        }

        public static readonly DependencyProperty ButtonContentProperty
            = DependencyProperty.RegisterAttached(
                "ButtonContent",
                typeof(object),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata("r"));

        [Category(AppName.MahApps)]
        public static object GetButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(ButtonContentProperty);
        }

        public static void SetButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonContentProperty, value);
        }

        public static readonly DependencyProperty ButtonContentTemplateProperty
            = DependencyProperty.RegisterAttached(
                "ButtonContentTemplate",
                typeof(DataTemplate),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        /// <summary> 
        /// ButtonContentTemplate is the template used to display the content of the ClearText button. 
        /// </summary>
        [Category(AppName.MahApps)]
        public static DataTemplate? GetButtonContentTemplate(DependencyObject d)
        {
            return (DataTemplate?)d.GetValue(ButtonContentTemplateProperty);
        }

        public static void SetButtonContentTemplate(DependencyObject obj, DataTemplate? value)
        {
            obj.SetValue(ButtonContentTemplateProperty, value);
        }

        public static readonly DependencyProperty ButtonTemplateProperty
            = DependencyProperty.RegisterAttached(
                "ButtonTemplate",
                typeof(ControlTemplate),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(null));

        [Category(AppName.MahApps)]
        public static ControlTemplate? GetButtonTemplate(DependencyObject d)
        {
            return (ControlTemplate?)d.GetValue(ButtonTemplateProperty);
        }

        public static void SetButtonTemplate(DependencyObject obj, ControlTemplate? value)
        {
            obj.SetValue(ButtonTemplateProperty, value);
        }

        public static readonly DependencyProperty ButtonFontFamilyProperty
            = DependencyProperty.RegisterAttached(
                "ButtonFontFamily",
                typeof(FontFamily),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(new FontFamilyConverter().ConvertFromString("Marlett")));

        [Category(AppName.MahApps)]
        public static FontFamily GetButtonFontFamily(DependencyObject d)
        {
            return (FontFamily)d.GetValue(ButtonFontFamilyProperty);
        }

        public static void SetButtonFontFamily(DependencyObject obj, FontFamily value)
        {
            obj.SetValue(ButtonFontFamilyProperty, value);
        }

        public static readonly DependencyProperty ButtonFontSizeProperty
            = DependencyProperty.RegisterAttached(
                "ButtonFontSize",
                typeof(double),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize));

        [Category(AppName.MahApps)]
        public static double GetButtonFontSize(DependencyObject d)
        {
            return (double)d.GetValue(ButtonFontSizeProperty);
        }

        public static void SetButtonFontSize(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonFontSizeProperty, value);
        }

        public static readonly DependencyProperty SelectAllOnFocusProperty
            = DependencyProperty.RegisterAttached(
                "SelectAllOnFocus",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, BooleanBoxes.Box(value));
        }

        public static readonly DependencyProperty IsSpellCheckContextMenuEnabledProperty
            = DependencyProperty.RegisterAttached(
                "IsSpellCheckContextMenuEnabled",
                typeof(bool),
                typeof(TextBoxHelper),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, IsSpellCheckContextMenuEnabledChanged));

        /// <summary>
        /// Indicates if a TextBox or RichTextBox should use SpellCheck context menu
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static bool GetIsSpellCheckContextMenuEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsSpellCheckContextMenuEnabledProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static void SetIsSpellCheckContextMenuEnabled(UIElement element, bool value)
        {
            element.SetValue(IsSpellCheckContextMenuEnabledProperty, BooleanBoxes.Box(value));
        }

        private static void IsSpellCheckContextMenuEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBoxBase textBox))
            {
                throw new InvalidOperationException("The property 'IsSpellCheckContextMenuEnabled' may only be set on TextBoxBase elements.");
            }

            if (e.OldValue != e.NewValue)
            {
                textBox.SetCurrentValue(SpellCheck.IsEnabledProperty, BooleanBoxes.Box((bool)e.NewValue));
                if ((bool)e.NewValue)
                {
                    textBox.ContextMenuOpening += TextBoxBaseContextMenuOpening;
                    textBox.LostFocus += TextBoxBaseLostFocus;
                    textBox.ContextMenuClosing += TextBoxBaseContextMenuClosing;
                }
                else
                {
                    textBox.ContextMenuOpening -= TextBoxBaseContextMenuOpening;
                    textBox.LostFocus -= TextBoxBaseLostFocus;
                    textBox.ContextMenuClosing -= TextBoxBaseContextMenuClosing;
                }
            }
        }

        private static void TextBoxBaseLostFocus(object sender, RoutedEventArgs e)
        {
            RemoveSpellCheckMenuItems((sender as FrameworkElement)?.ContextMenu);
        }

        private static void TextBoxBaseContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            RemoveSpellCheckMenuItems((sender as FrameworkElement)?.ContextMenu);
        }

        private static void TextBoxBaseContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var tbBase = (TextBoxBase)sender;
            var contextMenu = tbBase.ContextMenu;
            if (contextMenu is null)
            {
                return;
            }

            RemoveSpellCheckMenuItems(contextMenu);

            if (!SpellCheck.GetIsEnabled(tbBase))
            {
                return;
            }

            var spellingError = tbBase switch
            {
                TextBox textBox => textBox.GetSpellingError(textBox.CaretIndex),
                RichTextBox richTextBox => richTextBox.GetSpellingError(richTextBox.CaretPosition),
                _ => null
            };

            if (spellingError != null)
            {
                var itemIndex = 0;

                var spellingSuggestionStyle = contextMenu.TryFindResource(Spelling.SuggestionMenuItemStyleKey) as Style;
                var suggestions = spellingError.Suggestions.ToList();
                if (suggestions.Any())
                {
                    foreach (var suggestion in suggestions)
                    {
                        contextMenu.Items.Insert(itemIndex++, new MenuItem
                                                              {
                                                                  Command = EditingCommands.CorrectSpellingError,
                                                                  CommandParameter = suggestion,
                                                                  CommandTarget = tbBase,
                                                                  Style = spellingSuggestionStyle,
                                                                  Tag = typeof(Spelling)
                                                              });
                    }
                }
                else
                {
                    contextMenu.Items.Insert(itemIndex++, new MenuItem
                                                          {
                                                              Style = contextMenu.TryFindResource(Spelling.NoSuggestionsMenuItemStyleKey) as Style,
                                                              Tag = typeof(Spelling)
                                                          });
                }

                // add a separator
                contextMenu.Items.Insert(itemIndex++, new Separator
                                                      {
                                                          Style = contextMenu.TryFindResource(Spelling.SeparatorStyleKey) as Style,
                                                          Tag = typeof(Spelling)
                                                      });

                // ignore all
                contextMenu.Items.Insert(itemIndex++, new MenuItem
                                                      {
                                                          Command = EditingCommands.IgnoreSpellingError,
                                                          CommandTarget = tbBase,
                                                          Style = contextMenu.TryFindResource(Spelling.IgnoreAllMenuItemStyleKey) as Style,
                                                          Tag = typeof(Spelling)
                                                      });

                // add another separator
                contextMenu.Items.Insert(itemIndex, new Separator
                                                    {
                                                        Style = contextMenu.TryFindResource(Spelling.SeparatorStyleKey) as Style,
                                                        Tag = typeof(Spelling)
                                                    });
            }
        }

        private static void RemoveSpellCheckMenuItems(ContextMenu? contextMenu)
        {
            if (contextMenu != null)
            {
                var spellCheckItems = contextMenu.Items.OfType<FrameworkElement>().Where(item => ReferenceEquals(item.Tag, typeof(Spelling))).ToList();
                foreach (var item in spellCheckItems)
                {
                    contextMenu.Items.Remove(item);
                }
            }
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    richTextBox.BeginInvoke(() => RichTextBox_TextChanged(richTextBox, new TextChangedEventArgs(TextBoxBase.TextChangedEvent, UndoAction.None)));

                    richTextBox.TextChanged += RichTextBox_TextChanged;
                    richTextBox.GotFocus += TextBoxGotFocus;
                    richTextBox.PreviewMouseLeftButtonDown += UIElementPreviewMouseLeftButtonDown;
                }
                else
                {
                    richTextBox.TextChanged -= RichTextBox_TextChanged;
                    richTextBox.GotFocus -= TextBoxGotFocus;
                    richTextBox.PreviewMouseLeftButtonDown -= UIElementPreviewMouseLeftButtonDown;
                }
            }
            else if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    textBox.BeginInvoke(() => TextChanged(textBox, new TextChangedEventArgs(TextBoxBase.TextChangedEvent, UndoAction.None)));

                    textBox.TextChanged += TextChanged;
                    textBox.GotFocus += TextBoxGotFocus;
                    textBox.PreviewMouseLeftButtonDown += UIElementPreviewMouseLeftButtonDown;
                }
                else
                {
                    textBox.TextChanged -= TextChanged;
                    textBox.GotFocus -= TextBoxGotFocus;
                    textBox.PreviewMouseLeftButtonDown -= UIElementPreviewMouseLeftButtonDown;
                }
            }
            else if (d is PasswordBox passBox)
            {
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    passBox.BeginInvoke(() => PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox)));

                    passBox.PasswordChanged += PasswordChanged;
                    passBox.GotFocus += PasswordGotFocus;
                    passBox.PreviewMouseLeftButtonDown += UIElementPreviewMouseLeftButtonDown;
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                    passBox.PreviewMouseLeftButtonDown -= UIElementPreviewMouseLeftButtonDown;
                }
            }
            else if (d is HotKeyBox hotKeyBox)
            {
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    hotKeyBox.BeginInvoke(() => OnHotKeyBoxHotKeyChanged(hotKeyBox, new RoutedEventArgs(HotKeyBox.HotKeyChangedEvent, hotKeyBox)));

                    hotKeyBox.HotKeyChanged += OnHotKeyBoxHotKeyChanged;
                }
                else
                {
                    hotKeyBox.HotKeyChanged -= OnHotKeyBoxHotKeyChanged;
                }
            }
            else if (d is NumericUpDown numericUpDown)
            {
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    numericUpDown.BeginInvoke(() => OnNumericUpDownValueChanged(numericUpDown, new RoutedEventArgs(NumericUpDown.ValueChangedEvent, numericUpDown)));

                    numericUpDown.ValueChanged += OnNumericUpDownValueChanged;
                }
                else
                {
                    numericUpDown.ValueChanged -= OnNumericUpDownValueChanged;
                }
            }
            else if (d is TimePickerBase timePicker)
            {
                if ((bool)e.NewValue)
                {
                    timePicker.SelectedDateTimeChanged += OnTimePickerBaseSelectedDateTimeChanged;
                }
                else
                {
                    timePicker.SelectedDateTimeChanged -= OnTimePickerBaseSelectedDateTimeChanged;
                }
            }
            else if (d is DatePicker datePicker)
            {
                if ((bool)e.NewValue)
                {
                    datePicker.SelectedDateChanged += OnDatePickerBaseSelectedDateChanged;
                }
                else
                {
                    datePicker.SelectedDateChanged -= OnDatePickerBaseSelectedDateChanged;
                }
            }
        }

        private static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                SetRichTextBoxTextLength(richTextBox);
            }
        }

        private static void SetTextLength<TDependencyObject>(TDependencyObject? sender, Func<TDependencyObject, int> funcTextLength)
            where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                var value = funcTextLength(sender);
                sender.SetValue(TextLengthPropertyKey, value);
                sender.SetCurrentValue(HasTextProperty, BooleanBoxes.Box(value > 0));
            }
        }

        private static void TextChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
        }

        private static void OnHotKeyBoxHotKeyChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as HotKeyBox, hotKeyBox => hotKeyBox.Text?.Length ?? (hotKeyBox.HotKey is not null ? 1 : 0));
        }

        private static void OnNumericUpDownValueChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as NumericUpDown, numericUpDown => numericUpDown.Value.HasValue ? 1 : 0);
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
        }

        private static void OnDatePickerBaseSelectedDateChanged(object? sender, RoutedEventArgs e)
        {
            SetTextLength(sender as DatePicker, timePickerBase => timePickerBase.SelectedDate.HasValue ? 1 : 0);
        }

        private static void OnTimePickerBaseSelectedDateTimeChanged(object? sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TimePickerBase, timePickerBase => timePickerBase.SelectedDateTime.HasValue ? 1 : 0);
        }

        private static void TextBoxGotFocus(object? sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as TextBoxBase, textBox => textBox.SelectAll());
        }

        private static void UIElementPreviewMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
        {
            if (sender is UIElement uiElement && !uiElement.IsKeyboardFocusWithin && GetSelectAllOnFocus(uiElement))
            {
                // always SelectAllOnFocus
                uiElement.Focus();
                e.Handled = true;
            }
        }

        private static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox?.SelectAll());
        }

        private static void ControlGotFocus<TDependencyObject>(TDependencyObject? sender, Action<TDependencyObject> action)
            where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                if (GetSelectAllOnFocus(sender))
                {
                    sender.Dispatcher.BeginInvoke(action, sender);
                }
            }
        }

        private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox richTextBox)
            {
                richTextBox.Loaded -= RichTextBoxLoaded;
                richTextBox.Loaded += RichTextBoxLoaded;
                if (richTextBox.IsLoaded)
                {
                    RichTextBoxLoaded(richTextBox, new RoutedEventArgs());
                }
            }
            else if (d is TextBox textBox)
            {
                // only one loaded event
                textBox.Loaded -= TextChanged;
                textBox.Loaded += TextChanged;
                if (textBox.IsLoaded)
                {
                    TextChanged(textBox, new RoutedEventArgs());
                }
            }
            else if (d is PasswordBox passwordBox)
            {
                // only one loaded event
                passwordBox.Loaded -= PasswordChanged;
                passwordBox.Loaded += PasswordChanged;
                if (passwordBox.IsLoaded)
                {
                    PasswordChanged(passwordBox, new RoutedEventArgs());
                }
            }
            else if (d is ComboBox comboBox)
            {
                // only one loaded event
                comboBox.Loaded -= ComboBoxLoaded;
                comboBox.Loaded += ComboBoxLoaded;
                if (comboBox.IsLoaded)
                {
                    ComboBoxLoaded(comboBox, new RoutedEventArgs());
                }
            }
        }

        private static void RichTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is RichTextBox richTextBox)
            {
                SetRichTextBoxTextLength(richTextBox);
            }
        }

        private static void SetRichTextBoxTextLength(RichTextBox richTextBox)
        {
            SetTextLength(richTextBox, rtb =>
                {
                    var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    var text = textRange.Text;
                    var lastIndexOfNewLine = text.LastIndexOf(Environment.NewLine, StringComparison.InvariantCulture);
                    if (lastIndexOfNewLine >= 0)
                    {
                        text = text.Remove(lastIndexOfNewLine);
                    }

                    return text.Length;
                });
        }

        private static void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.SetCurrentValue(HasTextProperty, BooleanBoxes.Box(!string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null));
            }
        }
    }
}