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
using JetBrains.Annotations;

namespace MahApps.Metro.Controls
{
    public interface ISpellCheckMenuItem
    {
    }

    public class SpellCheckMenuItem : MenuItem, ISpellCheckMenuItem
    {
        public SpellCheckMenuItem()
        {
            this.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
        }
    }

    public class SpellCheckSeparator : Separator, ISpellCheckMenuItem
    {
    }

    /// <summary>
    /// A helper class that provides various attached properties for the TextBox control.
    /// </summary>
    /// <remarks>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </remarks>
    public class TextBoxHelper
    {
        public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextBoxHelper), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty UseFloatingWatermarkProperty = DependencyProperty.RegisterAttached("UseFloatingWatermark", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextBoxHelper), new UIPropertyMetadata(0));
        public static readonly DependencyProperty ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty TextButtonProperty = DependencyProperty.RegisterAttached("TextButton", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonsAlignmentProperty = DependencyProperty.RegisterAttached("ButtonsAlignment", typeof(ButtonsAlignment), typeof(TextBoxHelper), new FrameworkPropertyMetadata(ButtonsAlignment.Right, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        /// <summary>
        /// The clear text button behavior property. It sets a click event to the button if the value is true.
        /// </summary>
        public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty = DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

        /// <summary>
        /// This property can be used to set the button width (PART_ClearText) of TextBox, PasswordBox, ComboBox, NumericUpDown
        /// </summary>
        public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.RegisterAttached("ButtonWidth", typeof(double), typeof(TextBoxHelper), new FrameworkPropertyMetadata(22d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.RegisterAttached("ButtonCommand", typeof(ICommand), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null, ButtonCommandOrClearTextChanged));
        public static readonly DependencyProperty ButtonCommandParameterProperty = DependencyProperty.RegisterAttached("ButtonCommandParameter", typeof(object), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.RegisterAttached("ButtonContent", typeof(object), typeof(TextBoxHelper), new FrameworkPropertyMetadata("r"));
        public static readonly DependencyProperty ButtonContentTemplateProperty = DependencyProperty.RegisterAttached("ButtonContentTemplate", typeof(DataTemplate), typeof(TextBoxHelper), new FrameworkPropertyMetadata((DataTemplate)null));
        public static readonly DependencyProperty ButtonTemplateProperty = DependencyProperty.RegisterAttached("ButtonTemplate", typeof(ControlTemplate), typeof(TextBoxHelper), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ButtonFontFamilyProperty = DependencyProperty.RegisterAttached("ButtonFontFamily", typeof(FontFamily), typeof(TextBoxHelper), new FrameworkPropertyMetadata(new FontFamilyConverter().ConvertFromString("Marlett")));
        public static readonly DependencyProperty ButtonFontSizeProperty = DependencyProperty.RegisterAttached("ButtonFontSize", typeof(double), typeof(TextBoxHelper), new FrameworkPropertyMetadata(SystemFonts.MessageFontSize));
        
        public static readonly DependencyProperty SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsWaitingForDataProperty = DependencyProperty.RegisterAttached("IsWaitingForData", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof (bool), typeof (TextBoxHelper), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsSpellCheckContextMenuEnabledProperty = DependencyProperty.RegisterAttached("IsSpellCheckContextMenuEnabled", typeof(bool), typeof(TextBoxHelper), new FrameworkPropertyMetadata(false, UseSpellCheckContextMenuChanged));

        /// <summary>
        /// This property can be used to retrieve the watermark using the <see cref="DisplayAttribute"/> of bound property.
        /// </summary>
        /// <remarks>
        /// Setting this property to true will uses reflection.
        /// </remarks>
        public static readonly DependencyProperty AutoWatermarkProperty = DependencyProperty.RegisterAttached("AutoWatermark", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(default(bool), OnAutoWatermarkChanged));

        private static readonly Dictionary<Type, DependencyProperty> AutoWatermarkPropertyMapping = new Dictionary<Type, DependencyProperty>
                                                                                                    {
                                                                                                        { typeof(TextBox), TextBox.TextProperty },
                                                                                                        { typeof(ComboBox), Selector.SelectedItemProperty },
                                                                                                        { typeof(NumericUpDown), NumericUpDown.ValueProperty },
                                                                                                        { typeof(DatePicker), DatePicker.SelectedDateProperty },
                                                                                                        { typeof(TimePicker), TimePickerBase.SelectedTimeProperty },
                                                                                                        { typeof(DateTimePicker), DateTimePicker.SelectedDateProperty }
                                                                                                    };

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
            element.SetValue(IsSpellCheckContextMenuEnabledProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static bool GetAutoWatermark(DependencyObject element)
        {
            return (bool)element.GetValue(AutoWatermarkProperty);
        }

        ///  <summary>
        ///  Indicates if the watermark is automatically retrieved by using the <see cref="DisplayAttribute"/> of the bound property.
        ///  </summary>
        /// <remarks>This attached property uses reflection; thus it might reduce the performance of the application.
        /// The auto-watermak does work for the following controls:
        /// In the following case no custom watermark is shown
        /// <list type="bullet">
        /// <item>There is no binding</item>
        /// <item>Binding path errors</item>
        /// <item>Binding to a element of a collection without using a property of that element <c>Binding Path=Collection[0]</c> use: <c>Binding Path=Collection[0].SubProperty</c></item>
        /// <item>The bound property does not have a <see cref="DisplayAttribute"/></item>
        /// </list></remarks>
        public static void SetAutoWatermark(DependencyObject element, bool value)
        {
            element.SetValue(AutoWatermarkProperty, value);
        }

        private static void OnAutoWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            bool? enable = e.NewValue as bool?;
            if (element != null)
            {
                if (enable.GetValueOrDefault())
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
            FrameworkElement obj = (FrameworkElement)o;
            obj.Loaded -= OnControlWithAutoWatermarkSupportLoaded;

            DependencyProperty dependencyProperty;

            if (!AutoWatermarkPropertyMapping.TryGetValue(obj.GetType(), out dependencyProperty))
            {
                throw new NotSupportedException($"{nameof(AutoWatermarkProperty)} is not supported for {obj.GetType()}");
            }

            var resolvedProperty = ResolvePropertyFromBindingExpression(obj.GetBindingExpression(dependencyProperty));
            if (resolvedProperty != null)
            {
#if NET4
                var attribute = resolvedProperty.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;
#else
                var attribute = resolvedProperty.GetCustomAttribute<DisplayAttribute>();
#endif
                if (attribute != null)
                {
                    obj.SetValue(WatermarkProperty, attribute.GetPrompt());
                }
            }
        }

        [CanBeNull]
        private static PropertyInfo ResolvePropertyFromBindingExpression(BindingExpression bindingExpression)
        {
            if (bindingExpression != null)
            {
                if (bindingExpression.Status == BindingStatus.PathError)
                {
                    return null;
                }
#if NET4
                var propertyName = bindingExpression.ParentBinding.Path.Path;
                if (propertyName != null && propertyName.Contains('.'))
                {
                    propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
                }
#else
                var propertyName = bindingExpression.ResolvedSourcePropertyName;
#endif
                if (!string.IsNullOrEmpty(propertyName))
                {
#if NET4
                    var resolvedType = ResolveBinding(bindingExpression.DataItem.GetType(), bindingExpression.ParentBinding.Path.Path.Split('.'));
#elif NET4_5
                    var resolvedType = bindingExpression.ResolvedSource?.GetType();
#endif
                    if (resolvedType != null)
                    {
                        return resolvedType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                    }
                }

            }
            return null;
        }

#if NET4
        [CanBeNull]
        private static Type ResolveBinding(Type type, string[] paths)
        {
            if (type != null && paths != null)
            {
                if (paths.Length == 1)
                {
                    return type;
                }
                var propertyName = paths[0];

                if (propertyName.Contains('[') && propertyName.EndsWith("]"))
                {
                    var indexOf = propertyName.IndexOf('[');
                    propertyName = propertyName.Substring(0, indexOf);
                }

                var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.PropertyType;
                if (property != null)
                {
                    var remainingPath = paths.Skip(1).ToArray();
                    if (property.IsArray)
                    {
                        return ResolveBinding(property.GetElementType(), remainingPath);
                    }
                    return ResolveBinding(property, remainingPath);
                }
            }
            return null;
        }
#endif

        private static void UseSpellCheckContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tb = d as TextBoxBase;
            if (null == tb)
            {
                throw new InvalidOperationException("The property 'IsSpellCheckContextMenuEnabled' may only be set on TextBoxBase elements.");
            }

            if (e.OldValue != e.NewValue)
            {
                if ((bool)e.NewValue)
                {
                    tb.SetValue(SpellCheck.IsEnabledProperty, true);
                    tb.ContextMenuOpening += TextBoxBaseContextMenuOpening;
                    tb.LostFocus += TextBoxBaseLostFocus;
                    tb.ContextMenuClosing += TextBoxBaseContextMenuClosing;
                }
                else
                {
                    tb.SetValue(SpellCheck.IsEnabledProperty, false);
                    tb.ContextMenuOpening -= TextBoxBaseContextMenuOpening;
                    tb.LostFocus -= TextBoxBaseLostFocus;
                    tb.ContextMenuClosing -= TextBoxBaseContextMenuClosing;
                }
            }
        }

        private static void TextBoxBaseLostFocus(object sender, RoutedEventArgs e)
        {
            RemoveSpellCheckMenuItems((FrameworkElement)sender);
        }

        private static void TextBoxBaseContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            RemoveSpellCheckMenuItems((FrameworkElement)sender);
        }

        private static void TextBoxBaseContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var tbBase = (TextBoxBase)sender;
            var textBox = tbBase as TextBox;
            var richTextBox = tbBase as RichTextBox;

            RemoveSpellCheckMenuItems((FrameworkElement)sender);

            // the default item comes normally through the styles, so I think we don't need to do this
            /*if (tbBase.ContextMenu == null)
            {
                tbBase.ContextMenu = GetDefaultTextBoxBaseContextMenu();
            }*/

            var cmdIndex = 0;
            var spellingError = textBox != null
                ? textBox.GetSpellingError(textBox.CaretIndex)
                : richTextBox?.GetSpellingError(richTextBox.CaretPosition);
            if (spellingError != null) {
                var suggestions = spellingError.Suggestions.ToList();
                if (suggestions.Any()) {
                    foreach (var suggestion in suggestions) {
                        var mi = new SpellCheckMenuItem();
                        mi.Header = suggestion;
                        mi.FontWeight = FontWeights.Bold;
                        mi.Command = EditingCommands.CorrectSpellingError;
                        mi.CommandParameter = suggestion;
                        mi.CommandTarget = tbBase;
                        tbBase.ContextMenu.Items.Insert(cmdIndex, mi);
                        cmdIndex++;
                    }
                    // add a separator
                    tbBase.ContextMenu.Items.Insert(cmdIndex, new SpellCheckSeparator());
                    cmdIndex++;
                }
                var ignoreAllMI = new SpellCheckMenuItem();
                ignoreAllMI.Header = "Ignore All";
                ignoreAllMI.Command = EditingCommands.IgnoreSpellingError;
                ignoreAllMI.CommandTarget = tbBase;
                tbBase.ContextMenu.Items.Insert(cmdIndex, ignoreAllMI);
                cmdIndex++;
                // add another separator
                tbBase.ContextMenu.Items.Insert(cmdIndex, new SpellCheckSeparator());
            }
        }

        private static void RemoveSpellCheckMenuItems([CanBeNull] FrameworkElement tbBase)
        {
            if (tbBase?.ContextMenu == null)
            {
                return;
            }
            var spellCheckItems = tbBase.ContextMenu.Items.OfType<ISpellCheckMenuItem>().ToList();
            foreach (var item in spellCheckItems)
            {
                tbBase.ContextMenu.Items.Remove(item);
            }
        }

        /*private static ContextMenu GetDefaultTextBoxBaseContextMenu()
        {
            var defaultMenu = new ContextMenu();

            var m1 = new MenuItem { Command = ApplicationCommands.Cut };
            m1.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
            var m2 = new MenuItem { Command = ApplicationCommands.Copy };
            m2.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");
            var m3 = new MenuItem { Command = ApplicationCommands.Paste };
            m3.SetResourceReference(FrameworkElement.StyleProperty, "MetroMenuItem");

            defaultMenu.Items.Add(m1);
            defaultMenu.Items.Add(m2);
            defaultMenu.Items.Add(m3);

            return defaultMenu;
        }*/

        public static void SetIsWaitingForData(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWaitingForDataProperty, value);
        }
        
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        public static bool GetIsWaitingForData(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWaitingForDataProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(TimePickerBase))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static bool GetUseFloatingWatermark(DependencyObject obj)
        {
            return (bool)obj.GetValue(UseFloatingWatermarkProperty);
        }

        public static void SetUseFloatingWatermark(DependencyObject obj, bool value)
        {
            obj.SetValue(UseFloatingWatermarkProperty, value);
        }
        
        /// <summary>
        /// Gets if the attached TextBox has text.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        [AttachedPropertyBrowsableForType(typeof(PasswordBox))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(NumericUpDown))]
        public static bool GetHasText(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasTextProperty);
        }

        public static void SetHasText(DependencyObject obj, bool value)
        {
            obj.SetValue(HasTextProperty, value);
        }

        private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = (TextBox)d;
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    txtBox.BeginInvoke(() => TextChanged(txtBox, new TextChangedEventArgs(TextBox.TextChangedEvent, UndoAction.None)));

                    txtBox.TextChanged += TextChanged;
                    txtBox.GotFocus += TextBoxGotFocus;
                }
                else
                {
                    txtBox.TextChanged -= TextChanged;
                    txtBox.GotFocus -= TextBoxGotFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = (PasswordBox)d;
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    passBox.BeginInvoke(() => PasswordChanged(passBox, new RoutedEventArgs(PasswordBox.PasswordChangedEvent, passBox)));

                    passBox.PasswordChanged += PasswordChanged;
                    passBox.GotFocus += PasswordGotFocus;
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
            else if (d is NumericUpDown)
            {
                var numericUpDown = (NumericUpDown)d;
                if ((bool)e.NewValue)
                {
                    // Fixes #1343 and #2514: also triggers the show of the floating watermark if necessary
                    numericUpDown.BeginInvoke(() => OnNumericUpDownValueChaged(numericUpDown, new RoutedEventArgs(NumericUpDown.ValueChangedEvent, numericUpDown)));

                    numericUpDown.ValueChanged += OnNumericUpDownValueChaged;
                    //numericUpDown.GotFocus += NumericUpDownGotFocus;
                }
                else
                {
                    numericUpDown.ValueChanged -= OnNumericUpDownValueChaged;
                    //numericUpDown.GotFocus -= NumericUpDownGotFocus;
                }
            }
            else if (d is TimePickerBase)
            {
                var timePicker = (TimePickerBase)d;
                if ((bool)e.NewValue)
                {
                    timePicker.SelectedTimeChanged += OnTimePickerBaseSelectedTimeChanged;
                }
                else
                {
                    timePicker.SelectedTimeChanged -= OnTimePickerBaseSelectedTimeChanged;
                }
            }
            else if (d is DatePicker)
            {
                var timePicker = (DatePicker)d;
                if ((bool)e.NewValue)
                {
                    timePicker.SelectedDateChanged += OnDatePickerBaseSelectedDateChanged;
                }
                else
                {
                    timePicker.SelectedDateChanged -= OnDatePickerBaseSelectedDateChanged;
                }
            }
        }
        
        private static void SetTextLength<TDependencyObject>(TDependencyObject sender, Func<TDependencyObject, int> funcTextLength) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                var value = funcTextLength(sender);
                sender.SetValue(TextLengthProperty, value);
                sender.SetValue(HasTextProperty, value >= 1);
            }
        }

        private static void TextChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TextBox, textBox => textBox.Text.Length);
        }

        private static void OnNumericUpDownValueChaged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as NumericUpDown, numericUpDown => numericUpDown.Value.HasValue ? 1 : 0);
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as PasswordBox, passwordBox => passwordBox.Password.Length);
        }

        private static void OnDatePickerBaseSelectedDateChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as DatePicker, timePickerBase => timePickerBase.SelectedDate.HasValue ? 1 : 0);
        }

        private static void OnTimePickerBaseSelectedTimeChanged(object sender, RoutedEventArgs e)
        {
            SetTextLength(sender as TimePickerBase, timePickerBase => timePickerBase.SelectedTime.HasValue ? 1 : 0);
        }

        private static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as TextBox, textBox => textBox.SelectAll());
        }

        private static void NumericUpDownGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as NumericUpDown, numericUpDown => numericUpDown.SelectAll());
        }

        private static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            ControlGotFocus(sender as PasswordBox, passwordBox => passwordBox.SelectAll());
        }

        private static void ControlGotFocus<TDependencyObject>(TDependencyObject sender, Action<TDependencyObject> action) where TDependencyObject : DependencyObject
        {
            if (sender != null)
            {
                if (GetSelectAllOnFocus(sender))
                {
                    sender.Dispatcher.BeginInvoke(action, sender);
                }
            }
        }

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
            obj.SetValue(ClearTextButtonProperty, value);
        }

        /// <summary>
        /// Gets the text button visibility.
        /// </summary>
        [Category(AppName.MahApps)]
        public static bool GetTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(TextButtonProperty);
        }

        /// <summary>
        /// Sets the text button visibility.
        /// </summary>
        public static void SetTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(TextButtonProperty, value);
        }

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
        /// Gets the clear text button behavior.
        /// </summary>
        [Category(AppName.MahApps)]
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static bool GetIsClearTextButtonBehaviorEnabled(Button d)
        {
            return (bool)d.GetValue(IsClearTextButtonBehaviorEnabledProperty);
        }

        /// <summary>
        /// Sets the clear text button behavior.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(ButtonBase))]
        public static void SetIsClearTextButtonBehaviorEnabled(Button obj, bool value)
        {
            obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
        }

        [Category(AppName.MahApps)]
        public static double GetButtonWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ButtonWidthProperty);
        }

        public static void SetButtonWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonWidthProperty, value);
        }

        [Category(AppName.MahApps)]
        public static ICommand GetButtonCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(ButtonCommandProperty);
        }

        public static void SetButtonCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ButtonCommandProperty, value);
        }

        [Category(AppName.MahApps)]
        public static object GetButtonCommandParameter(DependencyObject d)
        {
            return (object)d.GetValue(ButtonCommandParameterProperty);
        }

        public static void SetButtonCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonCommandParameterProperty, value);
        }

        [Category(AppName.MahApps)]
        public static object GetButtonContent(DependencyObject d)
        {
            return (object)d.GetValue(ButtonContentProperty);
        }

        public static void SetButtonContent(DependencyObject obj, object value)
        {
            obj.SetValue(ButtonContentProperty, value);
        }

        /// <summary> 
        /// ButtonContentTemplate is the template used to display the content of the ClearText button. 
        /// </summary>
        [Category(AppName.MahApps)]
        public static DataTemplate GetButtonContentTemplate(DependencyObject d)
        {
            return (DataTemplate)d.GetValue(ButtonContentTemplateProperty);
        }

        public static void SetButtonContentTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(ButtonContentTemplateProperty, value);
        }

        [Category(AppName.MahApps)]
        public static ControlTemplate GetButtonTemplate(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(ButtonTemplateProperty);
        }

        public static void SetButtonTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(ButtonTemplateProperty, value);
        }

        [Category(AppName.MahApps)]
        public static FontFamily GetButtonFontFamily(DependencyObject d)
        {
            return (FontFamily)d.GetValue(ButtonFontFamilyProperty);
        }

        public static void SetButtonFontFamily(DependencyObject obj, FontFamily value)
        {
            obj.SetValue(ButtonFontFamilyProperty, value);
        }

        [Category(AppName.MahApps)]
        public static double GetButtonFontSize(DependencyObject d)
        {
            return (double)d.GetValue(ButtonFontSizeProperty);
        }

        public static void SetButtonFontSize(DependencyObject obj, double value)
        {
            obj.SetValue(ButtonFontSizeProperty, value);
        }

        private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue && button != null)
            {
                button.Click -= ButtonClicked;
                if ((bool)e.NewValue)
                {
                    button.Click += ButtonClicked;
                }
            }
        }

        public static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = VisualTreeHelper.GetParent(button);
            while (!(parent is TextBox || parent is PasswordBox || parent is ComboBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var command = GetButtonCommand(parent);
            var commandParameter = GetButtonCommandParameter(parent) ?? parent;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }

            if (GetClearTextButton(parent))
            {
                if (parent is TextBox)
                {
                    ((TextBox)parent).Clear();
                }
                else if (parent is PasswordBox)
                {
                    ((PasswordBox)parent).Clear();
                }
                else if (parent is ComboBox)
                {
                    if (((ComboBox)parent).IsEditable)
                    {
                        ((ComboBox)parent).Text = string.Empty;
                    }
                    ((ComboBox)parent).SelectedItem = null;
                }
            }
        }

        private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
            {
                // only one loaded event
                textbox.Loaded -= TextChanged;
                textbox.Loaded += TextChanged;
                if (textbox.IsLoaded)
                {
                    TextChanged(textbox, new RoutedEventArgs());
                }
            }
            var passbox = d as PasswordBox;
            if (passbox != null)
            {
                // only one loaded event
                passbox.Loaded -= PasswordChanged;
                passbox.Loaded += PasswordChanged;
                if (passbox.IsLoaded)
                {
                    PasswordChanged(passbox, new RoutedEventArgs());
                }
            }
            var combobox = d as ComboBox;
            if (combobox != null)
            {
                // only one loaded event
                combobox.Loaded -= ComboBoxLoaded;
                combobox.Loaded += ComboBoxLoaded;
                if (combobox.IsLoaded)
                {
                    ComboBoxLoaded(combobox, new RoutedEventArgs());
                }
            }
        }

        static void ComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                comboBox.SetValue(HasTextProperty, !string.IsNullOrWhiteSpace(comboBox.Text) || comboBox.SelectedItem != null);
            }
        }
    }
}
