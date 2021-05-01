// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ControlzEx;
using MahApps.Metro.Controls.Helper;
using MahApps.Metro.ValueBoxes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = nameof(PART_PopupListBox), Type = typeof(ListBox))]
    [TemplatePart(Name = nameof(PART_Popup), Type = typeof(Popup))]
    [TemplatePart(Name = nameof(PART_SelectedItemsPresenter), Type = typeof(ListBox))]
    [StyleTypedProperty(Property = nameof(SelectedItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    public class MultiSelectionComboBox : ComboBox
    {
        #region Constructors

        static MultiSelectionComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(typeof(MultiSelectionComboBox)));
            TextProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnTextChanged));
            CommandManager.RegisterClassCommandBinding(typeof(MultiSelectionComboBox), new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandManager.RegisterClassCommandBinding(typeof(MultiSelectionComboBox), new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Members
        // 
        //-------------------------------------------------------------------

        #region private Members

        private Popup PART_Popup;
        private ListBox PART_PopupListBox;
        private TextBox PART_EditableTextBox;
        private ListBox PART_SelectedItemsPresenter;

        private bool isUserdefinedTextInputPending;
        private DispatcherTimer _updateSelectedItemsFromTextTimer;

        #endregion

        //-------------------------------------------------------------------
        //
        //  Public Properties
        // 
        //-------------------------------------------------------------------

        #region Public Properties

        /// <summary>Identifies the <see cref="SelectionMode"/> dependency property.</summary>
        public static readonly DependencyProperty SelectionModeProperty
            = DependencyProperty.Register(nameof(SelectionMode),
                                          typeof(SelectionMode),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(SelectionMode.Single),
                                          IsValidSelectionMode);

        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single
                   || value == SelectionMode.Multiple
                   || value == SelectionMode.Extended;
        }

        /// <summary>
        ///     Indicates the selection behavior for the ListBox.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get => (SelectionMode)this.GetValue(SelectionModeProperty);
            set => this.SetValue(SelectionModeProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItems"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey SelectedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(SelectedItems),
                                                  typeof(IList),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata((IList)null));

        /// <summary>Identifies the <see cref="SelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// The currently selected items.
        /// </summary>
        [Bindable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems
        {
            get => (IList)this.GetValue(SelectedItemsProperty);
            protected set => this.SetValue(SelectedItemsPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="DisplaySelectedItems"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey DisplaySelectedItemsPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(DisplaySelectedItems),
                                                  typeof(IEnumerable),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata((IEnumerable)null));

        /// <summary>Identifies the <see cref="DisplaySelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty DisplaySelectedItemsProperty = DisplaySelectedItemsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the <see cref="SelectedItems"/> in the specified order which was set via <see cref="OrderSelectedItemsBy"/>
        /// </summary>
        public IEnumerable DisplaySelectedItems
        {
            get => (IEnumerable)this.GetValue(DisplaySelectedItemsProperty);
            protected set => this.SetValue(DisplaySelectedItemsPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="OrderSelectedItemsBy"/> dependency property.</summary>
        public static readonly DependencyProperty OrderSelectedItemsByProperty
            = DependencyProperty.Register(nameof(OrderSelectedItemsBy),
                                          typeof(SelectedItemsOrderType),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(SelectedItemsOrderType.SelectedOrder, OnOrderSelectedItemsByChanged));

        /// <summary>
        /// Gets or sets how the <see cref="SelectedItems"/> should be sorted
        /// </summary>
        public SelectedItemsOrderType OrderSelectedItemsBy
        {
            get => (SelectedItemsOrderType)this.GetValue(OrderSelectedItemsByProperty);
            set => this.SetValue(OrderSelectedItemsByProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleProperty
            = DependencyProperty.Register(nameof(SelectedItemContainerStyle),
                                          typeof(Style),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Style"/> for the <see cref="SelectedItems"/>
        /// </summary>
        public Style SelectedItemContainerStyle
        {
            get => (Style)this.GetValue(SelectedItemContainerStyleProperty);
            set => this.SetValue(SelectedItemContainerStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemContainerStyleSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleSelectorProperty
            = DependencyProperty.Register(nameof(SelectedItemContainerStyleSelector),
                                          typeof(StyleSelector),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> for the <see cref="SelectedItemContainerStyle"/>
        /// </summary>
        public StyleSelector SelectedItemContainerStyleSelector
        {
            get => (StyleSelector)this.GetValue(SelectedItemContainerStyleSelectorProperty);
            set => this.SetValue(SelectedItemContainerStyleSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="Separator"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorProperty
            = DependencyProperty.Register(nameof(Separator),
                                          typeof(string),
                                          typeof(MultiSelectionComboBox),
                                          new FrameworkPropertyMetadata(null, UpdateText));

        /// <summary>
        /// Gets or Sets the Separator which will be used if the ComboBox is editable.
        /// </summary>
        public string Separator
        {
            get => (string)this.GetValue(SeparatorProperty);
            set => this.SetValue(SeparatorProperty, value);
        }

        /// <summary>Identifies the <see cref="HasCustomText"/> dependency property.</summary>
        internal static readonly DependencyPropertyKey HasCustomTextPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(HasCustomText),
                                                  typeof(bool),
                                                  typeof(MultiSelectionComboBox),
                                                  new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="HasCustomText"/> dependency property.</summary>
        public static readonly DependencyProperty HasCustomTextProperty = HasCustomTextPropertyKey.DependencyProperty;

        /// <summary>
        /// Indicates if the text is user defined
        /// </summary>
        public bool HasCustomText
        {
            get => (bool)this.GetValue(HasCustomTextProperty);
            protected set => this.SetValue(HasCustomTextPropertyKey, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="TextWrapping"/> dependency property.</summary>
        public static readonly DependencyProperty TextWrappingProperty
            = TextBlock.TextWrappingProperty.AddOwner(typeof(MultiSelectionComboBox),
                                                      new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)this.GetValue(TextWrappingProperty);
            set => this.SetValue(TextWrappingProperty, value);
        }

        /// <summary>Identifies the <see cref="AcceptsReturn"/> dependency property.</summary>
        public static readonly DependencyProperty AcceptsReturnProperty
            = TextBoxBase.AcceptsReturnProperty.AddOwner(typeof(MultiSelectionComboBox));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public bool AcceptsReturn
        {
            get => (bool)this.GetValue(AcceptsReturnProperty);
            set => this.SetValue(AcceptsReturnProperty, value);
        }

        /// <summary>Identifies the <see cref="ObjectToStringComparer"/> dependency property.</summary>
        public static readonly DependencyProperty ObjectToStringComparerProperty
            = DependencyProperty.Register(nameof(ObjectToStringComparer),
                                          typeof(ICompareObjectToString),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a function that is used to check if the entered Text is an object that should be selected.
        /// </summary>
        public ICompareObjectToString ObjectToStringComparer
        {
            get => (ICompareObjectToString)this.GetValue(ObjectToStringComparerProperty);
            set => this.SetValue(ObjectToStringComparerProperty, value);
        }

        /// <summary>Identifies the <see cref="EditableTextStringComparision"/> dependency property.</summary>
        public static readonly DependencyProperty EditableTextStringComparisionProperty
            = DependencyProperty.Register(nameof(EditableTextStringComparision),
                                          typeof(StringComparison),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(StringComparison.Ordinal));

        /// <summary>
        ///  Gets or Sets the <see cref="StringComparison"/> that is used to check if the entered <see cref="ComboBox.Text"/> matches to the <see cref="SelectedItems"/>
        /// </summary>
        public StringComparison EditableTextStringComparision
        {
            get => (StringComparison)this.GetValue(EditableTextStringComparisionProperty);
            set => this.SetValue(EditableTextStringComparisionProperty, value);
        }

        /// <summary>Identifies the <see cref="StringToObjectParser"/> dependency property.</summary>
        public static readonly DependencyProperty StringToObjectParserProperty
            = DependencyProperty.Register(nameof(StringToObjectParser),
                                          typeof(IParseStringToObject),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a parser-class that implements <see cref="IParseStringToObject"/> 
        /// </summary>
        public IParseStringToObject StringToObjectParser
        {
            get => (IParseStringToObject)this.GetValue(StringToObjectParserProperty);
            set => this.SetValue(StringToObjectParserProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContent"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentProperty
            = DependencyProperty.Register(nameof(DisabledPopupOverlayContent),
                                          typeof(object),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContent
        /// </summary>
        public object DisabledPopupOverlayContent
        {
            get => (object)this.GetValue(DisabledPopupOverlayContentProperty);
            set => this.SetValue(DisabledPopupOverlayContentProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentTemplateProperty
            = DependencyProperty.Register(nameof(DisabledPopupOverlayContentTemplate),
                                          typeof(DataTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContentTemplate
        /// </summary>
        public DataTemplate DisabledPopupOverlayContentTemplate
        {
            get => (DataTemplate)this.GetValue(DisabledPopupOverlayContentTemplateProperty);
            set => this.SetValue(DisabledPopupOverlayContentTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty
            = DependencyProperty.Register(nameof(SelectedItemTemplate),
                                          typeof(DataTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplate
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get => (DataTemplate)this.GetValue(SelectedItemTemplateProperty);
            set => this.SetValue(SelectedItemTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty
            = DependencyProperty.Register(nameof(SelectedItemTemplateSelector),
                                          typeof(DataTemplateSelector),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplateSelector
        /// </summary>
        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(SelectedItemTemplateSelectorProperty);
            set => this.SetValue(SelectedItemTemplateSelectorProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemStringFormatProperty
            = DependencyProperty.Register(nameof(SelectedItemStringFormat),
                                          typeof(string),
                                          typeof(MultiSelectionComboBox),
                                          new FrameworkPropertyMetadata(null, UpdateText));

        /// <summary>
        /// Gets or Sets the string format for the selected items
        /// </summary>
        public string SelectedItemStringFormat
        {
            get => (string)this.GetValue(SelectedItemStringFormatProperty);
            set => this.SetValue(SelectedItemStringFormatProperty, value);
        }

        /// <summary>Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(VerticalScrollBarVisibility),
                                          typeof(ScrollBarVisibility),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the vertical scrollbar is visible
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => (ScrollBarVisibility)this.GetValue(VerticalScrollBarVisibilityProperty);
            set => this.SetValue(VerticalScrollBarVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty
            = DependencyProperty.Register(nameof(HorizontalScrollBarVisibility),
                                          typeof(ScrollBarVisibility),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the horizontal scrollbar is visible
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => (ScrollBarVisibility)this.GetValue(HorizontalScrollBarVisibilityProperty);
            set => this.SetValue(HorizontalScrollBarVisibilityProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectedItemsPanelTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsPanelTemplateProperty
            = DependencyProperty.Register(nameof(SelectedItemsPanelTemplate),
                                          typeof(ItemsPanelTemplate),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ItemsPanelTemplate"/> for the selected items.
        /// </summary>
        public ItemsPanelTemplate SelectedItemsPanelTemplate
        {
            get => (ItemsPanelTemplate)this.GetValue(SelectedItemsPanelTemplateProperty);
            set => this.SetValue(SelectedItemsPanelTemplateProperty, value);
        }

        /// <summary>Identifies the <see cref="SelectItemsFromTextInputDelay"/> dependency property.</summary>
        public static readonly DependencyProperty SelectItemsFromTextInputDelayProperty
            = DependencyProperty.Register(nameof(SelectItemsFromTextInputDelay),
                                          typeof(int),
                                          typeof(MultiSelectionComboBox),
                                          new PropertyMetadata(-1));

        /// <summary>
        /// Gets or Sets the delay in milliseconds to wait before the selection is updated during text input.
        /// If this value is -1 the selection will not be updated during text input. 
        /// Note: You also need to set <see cref="ObjectToStringComparer"/> to get this to work. 
        /// </summary>
        public int SelectItemsFromTextInputDelay
        {
            get => (int)this.GetValue(SelectItemsFromTextInputDelayProperty);
            set => this.SetValue(SelectItemsFromTextInputDelayProperty, value);
        }

        /// <summary>
        /// Resets the custom Text to the selected Items text 
        /// </summary>
        public void ResetEditableText()
        {
            var oldSelectionStart = this.PART_EditableTextBox.SelectionStart;
            var oldSelectionLength = this.PART_EditableTextBox.SelectionLength;

            this.SetValue(HasCustomTextPropertyKey, false);
            this.UpdateEditableText();

            this.PART_EditableTextBox.SelectionStart = oldSelectionStart;
            this.PART_EditableTextBox.SelectionLength = oldSelectionLength;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the Text of the editable Textbox.
        /// Sets the custom Text if any otherwise the concatenated string.
        /// </summary>
        private void UpdateEditableText()
        {
            if (this.PART_EditableTextBox is null || this.SelectedItems is null)
            {
                return;
            }

            var selectedItemsText = this.GetSelectedItemsText();

            if (!this.HasCustomText)
            {
                this.SetCurrentValue(TextProperty, selectedItemsText);
            }

            this.UpdateHasCustomText(selectedItemsText);
        }

        private void UpdateDisplaySelectedItems()
        {
            this.UpdateDisplaySelectedItems(this.OrderSelectedItemsBy);
        }

        public string GetSelectedItemsText()
        {
            switch (this.SelectionMode)
            {
                case SelectionMode.Single:
                    if (this.ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue
                        || this.ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        return BindingHelper.Eval(this.SelectedItem, this.DisplayMemberPath ?? string.Empty, this.SelectedItemStringFormat)?.ToString();
                    }
                    else
                    {
                        return this.SelectedItem?.ToString();
                    }

                case SelectionMode.Multiple:
                case SelectionMode.Extended:
                    IEnumerable<object> values;

                    if (this.ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue
                        || this.ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        values = ((IEnumerable<object>)this.DisplaySelectedItems)?.Select(o => BindingHelper.Eval(o, this.DisplayMemberPath ?? string.Empty, this.SelectedItemStringFormat));
                    }
                    else
                    {
                        values = (IEnumerable<object>)this.DisplaySelectedItems;
                    }

                    return values is null ? null : string.Join(this.Separator ?? string.Empty, values);

                default:
                    return null;
            }
        }

        private void UpdateHasCustomText(string selectedItemsText)
        {
            // if the parameter was null lets get the text on our own.
            selectedItemsText ??= this.GetSelectedItemsText();

            bool hasCustomText = !((string.IsNullOrEmpty(selectedItemsText) && string.IsNullOrEmpty(this.Text))
                                   || string.Equals(this.Text, selectedItemsText, this.EditableTextStringComparision));

            this.HasCustomText = hasCustomText;
        }

        private void UpdateDisplaySelectedItems(SelectedItemsOrderType selectedItemsOrderType)
        {
            var displaySelectedItems = selectedItemsOrderType switch
            {
                SelectedItemsOrderType.SelectedOrder => this.SelectedItems,
                SelectedItemsOrderType.ItemsSourceOrder => ((IEnumerable<object>)this.SelectedItems).OrderBy(o => this.Items.IndexOf(o)),
                _ => this.DisplaySelectedItems
            };

            this.SetValue(DisplaySelectedItemsPropertyKey, displaySelectedItems);
        }

        private void SelectItemsFromText(int millisecondsToWait)
        {
            if (!this.isUserdefinedTextInputPending)
            {
                return;
            }

            if (this._updateSelectedItemsFromTextTimer is null)
            {
                this._updateSelectedItemsFromTextTimer = new DispatcherTimer(DispatcherPriority.Background);
                this._updateSelectedItemsFromTextTimer.Tick += this.UpdateSelectedItemsFromTextTimer_Tick;
            }

            if (this._updateSelectedItemsFromTextTimer.IsEnabled)
            {
                this._updateSelectedItemsFromTextTimer.Stop();
            }

            if (this.HasCustomText && !(this.ObjectToStringComparer is null) && !string.IsNullOrEmpty(this.Separator))
            {
                this._updateSelectedItemsFromTextTimer.Interval = TimeSpan.FromMilliseconds(millisecondsToWait > 0 ? millisecondsToWait : 0);
                this._updateSelectedItemsFromTextTimer.Start();
            }
        }

        private void UpdateSelectedItemsFromTextTimer_Tick(object sender, EventArgs e)
        {
            this._updateSelectedItemsFromTextTimer.Stop();

            // We clear the selection if there is no text available. 
            if (string.IsNullOrEmpty(this.Text))
            {
                switch (this.SelectionMode)
                {
                    case SelectionMode.Single:
                        this.SetCurrentValue(SelectedItemProperty, null);
                        break;
                    case SelectionMode.Multiple:
                    case SelectionMode.Extended:
                        this.SelectedItems?.Clear();
                        break;
                    default:
                        throw new NotSupportedException("Unknown SelectionMode");
                }

                return;
            }

            object item;
            switch (this.SelectionMode)
            {
                case SelectionMode.Single:
                    item = this.Items
                               .OfType<object>()
                               .FirstOrDefault(x => this.ObjectToStringComparer.CheckIfStringMatchesObject(this.Text, x, this.EditableTextStringComparision, this.SelectedItemStringFormat));

                    if (item is null)
                    {
                        item = this.TryAddObjectFromString(this.Text);
                    }

                    this.SetCurrentValue(SelectedItemProperty, item);

                    break;
                case SelectionMode.Multiple:
                case SelectionMode.Extended:

                    this.SelectedItems.Clear();

                    var strings = this.Text.Split(new[] { this.Separator }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in strings)
                    {
                        item = this.Items
                                   .OfType<object>()
                                   .FirstOrDefault(x => this.ObjectToStringComparer.CheckIfStringMatchesObject(s, x, this.EditableTextStringComparision, this.SelectedItemStringFormat));

                        if (item is null)
                        {
                            item = this.TryAddObjectFromString(s);
                        }

                        if (!(item is null))
                        {
                            this.SelectedItems.Add(item);
                        }
                    }

                    break;
                default:
                    throw new NotSupportedException("Unknown SelectionMode");
            }

            // First we need to check if the string matches completely to the selected items. Therefore we need to display the items in the selected order first
            this.UpdateDisplaySelectedItems(SelectedItemsOrderType.SelectedOrder);
            this.UpdateHasCustomText(null);

            // If the items should be ordered differntly than above we need to reoder them accordingly.
            if (this.OrderSelectedItemsBy != SelectedItemsOrderType.SelectedOrder)
            {
                this.UpdateDisplaySelectedItems();
            }

            var oldCaretPos = this.PART_EditableTextBox.CaretIndex;
            this.UpdateEditableText();
            this.PART_EditableTextBox.CaretIndex = oldCaretPos;

            this.isUserdefinedTextInputPending = false;
        }

        private object TryAddObjectFromString(string input)
        {
            var item = this.StringToObjectParser?.CreateObjectFromString(input, this.Language.GetEquivalentCulture(), this.SelectedItemStringFormat);

            if (item is null)
            {
                return null;
            }

            if (this.ReadLocalValue(ItemsSourceProperty) == DependencyProperty.UnsetValue)
            {
                this.Items.Add(item);
            }
            else if (this.ItemsSource is IList list)
            {
                list.Add(item);
            }

            return item;
        }

        #endregion

        #region Commands

        // Clear Text Command
        public static RoutedUICommand ClearContentCommand { get; } = new RoutedUICommand("ClearContent", nameof(ClearContentCommand), typeof(MultiSelectionComboBox));

        private static void ExecutedClearContentCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo)
            {
                if (multiSelectionCombo.HasCustomText)
                {
                    multiSelectionCombo.ResetEditableText();
                }
                else
                {
                    switch (multiSelectionCombo.SelectionMode)
                    {
                        case SelectionMode.Single:
                            multiSelectionCombo.SetCurrentValue(SelectedItemProperty, null);
                            break;
                        case SelectionMode.Multiple:
                        case SelectionMode.Extended:
                            multiSelectionCombo.SelectedItems?.Clear();
                            break;
                        default:
                            throw new NotSupportedException("Unknown SelectionMode");
                    }
                }
            }
        }

        private static void CanExecuteClearContentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox multiSelectionComboBox)
            {
                e.CanExecute = !string.IsNullOrEmpty(multiSelectionComboBox.Text) || multiSelectionComboBox.SelectedItems?.Count > 0;
            }
        }

        public static RoutedUICommand RemoveItemCommand { get; } = new RoutedUICommand("Remove item", nameof(RemoveItemCommand), typeof(MultiSelectionComboBox));

        private static void RemoveItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo)
            {
                if (multiSelectionCombo.SelectionMode == SelectionMode.Single)
                {
                    multiSelectionCombo.SetCurrentValue(SelectedItemProperty, null);
                    return;
                }

                if (multiSelectionCombo.SelectedItems != null && multiSelectionCombo.SelectedItems.Contains(e.Parameter))
                {
                    multiSelectionCombo.SelectedItems.Remove(e.Parameter);
                }
            }
        }

        private static void RemoveItemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox)
            {
                e.CanExecute = e.Parameter != null;
            }
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Init SelectedItemsPresenter
            this.PART_SelectedItemsPresenter = this.GetTemplateChild(nameof(this.PART_SelectedItemsPresenter)) as ListBox;

            if (!(this.PART_SelectedItemsPresenter is null))
            {
                this.PART_SelectedItemsPresenter.MouseLeftButtonUp -= this.PART_SelectedItemsPresenter_MouseLeftButtonUp;
                this.PART_SelectedItemsPresenter.SelectionChanged -= this.PART_SelectedItemsPresenter_SelectionChanged;

                this.PART_SelectedItemsPresenter.MouseLeftButtonUp += this.PART_SelectedItemsPresenter_MouseLeftButtonUp;
                this.PART_SelectedItemsPresenter.SelectionChanged += this.PART_SelectedItemsPresenter_SelectionChanged;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(this.PART_SelectedItemsPresenter)}\" could not be found.");
            }

            // Init EditableTextBox
            this.PART_EditableTextBox = this.GetTemplateChild(nameof(this.PART_EditableTextBox)) as TextBox;

            if (!(this.PART_EditableTextBox is null))
            {
                this.PART_EditableTextBox.LostFocus -= this.PART_EditableTextBox_LostFocus;
                this.PART_EditableTextBox.LostFocus += this.PART_EditableTextBox_LostFocus;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(this.PART_EditableTextBox)}\" could not be found.");
            }

            // Init Popup
            this.PART_Popup = this.GetTemplateChild(nameof(this.PART_Popup)) as Popup;

            if (this.PART_Popup is null)
            {
                throw new MahAppsException($"The template part \"{nameof(this.PART_Popup)}\" could not be found.");
            }

            this.PART_PopupListBox = this.GetTemplateChild(nameof(this.PART_PopupListBox)) as ListBox;

            if (!(this.PART_PopupListBox is null))
            {
                this.PART_PopupListBox.SelectionChanged -= this.PART_PopupListBox_SelectionChanged;
                this.PART_PopupListBox.SelectionChanged += this.PART_PopupListBox_SelectionChanged;
                this.SelectedItems = this.PART_PopupListBox.SelectedItems;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(this.PART_PopupListBox)}\" could not be found.");
            }

            // Do update the text 
            this.UpdateEditableText();
            this.UpdateDisplaySelectedItems();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.UpdateEditableText();
            this.UpdateDisplaySelectedItems();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (!this.IsLoaded)
            {
                this.Loaded += this.MultiSelectionComboBox_Loaded;
                return;
            }

            // If we have the ItemsSource set, we need to exit here. 
            if ((this.PART_PopupListBox?.Items as IList)?.IsReadOnly ?? true)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        this.PART_PopupListBox.Items.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.PART_PopupListBox.Items.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    this.PART_PopupListBox.Items.Clear();
                    foreach (var item in this.Items)
                    {
                        this.PART_PopupListBox.Items.Add(item);
                    }

                    break;
                default:
                    throw new NotSupportedException("Unsupported NotifyCollectionChangedAction");
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // For now we only want to update our poition if the height changed. Else we will get a flickering in SharedGridColumns
            if (this.IsDropDownOpen && sizeInfo.HeightChanged && !(this.PART_Popup is null))
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                            (DispatcherOperationCallback)((object arg) =>
                                                {
                                                    MultiSelectionComboBox mscb = (MultiSelectionComboBox)arg;
                                                    mscb.PART_Popup.HorizontalOffset++;
                                                    mscb.PART_Popup.HorizontalOffset--;

                                                    return null;
                                                }), this);
            }
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            this.PART_PopupListBox.Focus();

            if (this.PART_PopupListBox.Items.Count == 0)
            {
                return;
            }

            var index = this.PART_PopupListBox.SelectedIndex;
            if (index < 0)
            {
                index = 0;
            }

            Action action = () =>
                {
                    this.PART_PopupListBox.ScrollIntoView(this.PART_PopupListBox.SelectedItem);

                    if (this.PART_PopupListBox.ItemContainerGenerator.ContainerFromIndex(index) is ListBoxItem item)
                    {
                        item.Focus();
                        KeyboardNavigationEx.Focus(item);
                    }
                };

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, action);

            this.SelectItemsFromText(0);
        }

        /// <summary>
        /// Return true if the item is (or is eligible to be) its own ItemUI
        /// </summary>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ListBoxItem);
        }

        /// <summary> Create or identify the element used to display the given item. </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListBoxItem();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (this.IsEditable && !this.IsDropDownOpen && !(this.PART_EditableTextBox is null) && !ComboBoxHelper.GetInterceptMouseWheelSelection(this))
            {
                if (this.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled && ScrollViewerHelper.GetIsHorizontalScrollWheelEnabled(this))
                {
                    if (e.Delta > 0)
                    {
                        this.PART_EditableTextBox.LineLeft();
                    }
                    else
                    {
                        this.PART_EditableTextBox.LineRight();
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        this.PART_EditableTextBox.LineUp();
                    }
                    else
                    {
                        this.PART_EditableTextBox.LineDown();
                    }
                }
            }
            else if (!this.IsDropDownOpen && !ComboBoxHelper.GetInterceptMouseWheelSelection(this))
            {
                base.OnPreviewMouseWheel(e);
                return;
            }
            // ListBox eats the selection so we need to handle this event here if we want to select the next item.
            else if (!this.IsDropDownOpen && ComboBoxHelper.GetInterceptMouseWheelSelection(this) && this.SelectionMode == SelectionMode.Single)
            {
                if (e.Delta > 0 && this.PART_PopupListBox.SelectedIndex > 0)
                {
                    this.PART_PopupListBox.SelectedIndex--;
                }
                else if (e.Delta < 0 && this.PART_PopupListBox.SelectedIndex < this.PART_PopupListBox.Items.Count - 1)
                {
                    this.PART_PopupListBox.SelectedIndex++;
                }
            }

            // The event is handled if the drop down is not open. 
            e.Handled = !this.IsDropDownOpen;
            base.OnPreviewMouseWheel(e);
        }

        #endregion

        #region Events

        private void PART_PopupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.UpdateDisplaySelectedItems();
            this.UpdateEditableText();
            this.isUserdefinedTextInputPending = false;
        }

        private void MultiSelectionComboBox_Loaded(object sender, EventArgs e)
        {
            this.Loaded -= this.MultiSelectionComboBox_Loaded;

            // If we have the ItemsSource set, we need to exit here. 
            if ((this.PART_PopupListBox?.Items as IList)?.IsReadOnly ?? true)
            {
                return;
            }

            this.PART_PopupListBox.Items.Clear();
            foreach (var item in this.Items)
            {
                this.PART_PopupListBox.Items.Add(item);
            }
        }

        private void PART_EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SelectItemsFromText(0);
        }

        private void PART_SelectedItemsPresenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we have a ScrollViewer (ListBox has) we need to handle this event here as it will not be forwarded to the ToggleButton
            this.SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.Box(!this.IsDropDownOpen));
        }

        private void PART_SelectedItemsPresenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We don't want the SelctedItems to be selectable. So anytime the selection will be changed we will reset it. 
            this.PART_SelectedItemsPresenter.SelectedItem = null;
        }

        private static void UpdateText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                multiSelectionComboBox.UpdateEditableText();
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox)
            {
                multiSelectionComboBox.UpdateHasCustomText(null);
                multiSelectionComboBox.isUserdefinedTextInputPending = true;

                // Select the items during typing if enabled
                if (multiSelectionComboBox.SelectItemsFromTextInputDelay >= 0)
                {
                    multiSelectionComboBox.SelectItemsFromText(multiSelectionComboBox.SelectItemsFromTextInputDelay);
                }
            }
        }

        private static void OnOrderSelectedItemsByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiSelectionComboBox multiSelectionComboBox && !multiSelectionComboBox.HasCustomText)
            {
                multiSelectionComboBox.UpdateDisplaySelectedItems();
                multiSelectionComboBox.UpdateEditableText();
            }
        }

        #endregion
    }
}