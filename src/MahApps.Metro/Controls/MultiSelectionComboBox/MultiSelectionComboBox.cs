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
            TextProperty.OverrideMetadata(typeof(MultiSelectionComboBox), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(OnTextChanged)));
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
        public static readonly DependencyProperty SelectionModeProperty =
                DependencyProperty.Register(
                        nameof(SelectionMode),
                        typeof(SelectionMode),
                        typeof(MultiSelectionComboBox),
                        new PropertyMetadata(SelectionMode.Single),
                        new ValidateValueCallback(IsValidSelectionMode));

        /// <summary>
        ///     Indicates the selection behavior for the ListBox.
        /// </summary>
        public SelectionMode SelectionMode
        {
            get { return (SelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }


        /// <summary>Identifies the <see cref="SelectedItem"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedItemProperty =
             DependencyProperty.Register(
                 nameof(SelectedItem),
                 typeof(object),
                 typeof(MultiSelectionComboBox),
                 new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the selectedItem
        /// </summary>
        public new object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedIndex"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register(
                nameof(SelectedIndex), typeof(int), typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the SelectedIndex
        /// </summary>
        public new int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedValue"/> dependency property.</summary>
        public static new readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue),
                typeof(object),
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Gets or Sets the SelectedValue
        /// </summary>
        public new object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        private static bool IsValidSelectionMode(object o)
        {
            SelectionMode value = (SelectionMode)o;
            return value == SelectionMode.Single
                || value == SelectionMode.Multiple
                || value == SelectionMode.Extended;
        }

        /// <summary>Identifies the <see cref="SelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsProperty = 
            DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(IList),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata((IList)null));

        /// <summary>
        /// The currently selected items.
        /// </summary>
        [Bindable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList SelectedItems
        {
            get
            {
                return PART_PopupListBox?.SelectedItems;
            }
        }

        /// <summary>Identifies the <see cref="DisplaySelectedItems"/> dependency property.</summary>
        public static readonly DependencyProperty DisplaySelectedItemsProperty =
            DependencyProperty.Register(
                nameof(DisplaySelectedItems),
                typeof(IEnumerable),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata((IEnumerable)null));

        /// <summary>
        /// Gets the <see cref="SelectedItems"/> in the specified order which was set via <see cref="OrderSelectedItemsBy"/>
        /// </summary>
        public IEnumerable DisplaySelectedItems
        {
            get { return (IEnumerable)GetValue(DisplaySelectedItemsProperty); }
        }

        /// <summary>Identifies the <see cref="OrderSelectedItemsBy"/> dependency property.</summary>
        public static readonly DependencyProperty OrderSelectedItemsByProperty =
            DependencyProperty.Register(
                nameof(OrderSelectedItemsBy),
                typeof(OrderSelectedItemsBy),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(OrderSelectedItemsBy.SelectedOrder, new PropertyChangedCallback(OnOrderSelectedItemsByChanged)));

        /// <summary>
        /// Gets or sets how the <see cref="SelectedItems"/> should be sorted
        /// </summary>
        public OrderSelectedItemsBy OrderSelectedItemsBy
        {
            get { return (OrderSelectedItemsBy)GetValue(OrderSelectedItemsByProperty); }
            set { SetValue(OrderSelectedItemsByProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemContainerStyle),
                typeof(Style),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="Style"/> for the <see cref="SelectedItems"/>
        /// </summary>
        public Style SelectedItemContainerStyle
        {
            get { return (Style)GetValue(SelectedItemContainerStyleProperty); }
            set { SetValue(SelectedItemContainerStyleProperty, value); }
        }



        /// <summary>Identifies the <see cref="SelectedItemContainerStyleSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemContainerStyleSelectorProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemContainerStyleSelector),
                typeof(StyleSelector),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="StyleSelector"/> for the <see cref="SelectedItemContainerStyle"/>
        /// </summary>
        public StyleSelector SelectedItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(SelectedItemContainerStyleSelectorProperty); }
            set { SetValue(SelectedItemContainerStyleSelectorProperty, value); }
        }



        /// <summary>Identifies the <see cref="Separator"/> dependency property.</summary>
        public static readonly DependencyProperty SeparatorProperty = 
            DependencyProperty.Register(
                nameof(Separator),
                typeof(string),
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(UpdateText)));

        /// <summary>
        /// Gets or Sets the Separator which will be used if the ComboBox is editable.
        /// </summary>
        public string Separator
        {
            get { return (string)GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        /// <summary>Identifies the <see cref="HasCustomText"/> dependency property.</summary>
        public static readonly DependencyProperty HasCustomTextProperty = 
            DependencyProperty.Register(
                nameof(HasCustomText),
                typeof(bool),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(false));

        /// <summary>
        /// Indicates if the text is userdefined
        /// </summary>
        public bool HasCustomText
        {
            get { return (bool)GetValue(HasCustomTextProperty); }
        }

        /// <summary>Identifies the <see cref="TextWrapping"/> dependency property.</summary>
        public static readonly DependencyProperty TextWrappingProperty = 
            TextBlock.TextWrappingProperty.AddOwner(
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(TextWrapping.NoWrap, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        /// <summary>Identifies the <see cref="AcceptsReturn"/> dependency property.</summary>
        public static readonly DependencyProperty AcceptsReturnProperty = 
            TextBoxBase.AcceptsReturnProperty.AddOwner(typeof(MultiSelectionComboBox));

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps
        /// when it reaches the flow edge of its containing block box.
        /// </summary>
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }


        /// <summary>Identifies the <see cref="ObjectToStringComparer"/> dependency property.</summary>
        public static readonly DependencyProperty ObjectToStringComparerProperty = 
            DependencyProperty.Register(
                nameof(ObjectToStringComparer),
                typeof(ICompareObjectToString),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a function that is used to check if the entered Text is an object that should be selected.
        /// </summary>
        public ICompareObjectToString ObjectToStringComparer
        {
            get { return (ICompareObjectToString)GetValue(ObjectToStringComparerProperty); }
            set { SetValue(ObjectToStringComparerProperty, value); }
        }


        /// <summary>Identifies the <see cref="EditableTextStringComparision"/> dependency property.</summary>
        public static readonly DependencyProperty EditableTextStringComparisionProperty = 
            DependencyProperty.Register(
                nameof(EditableTextStringComparision),
                typeof(StringComparison),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(StringComparison.Ordinal));

        /// <summary>
        ///  Gets or Sets the <see cref="StringComparison"/> that is used to check if the entered <see cref="ComboBox.Text"/> matches to the <see cref="SelectedItems"/>
        /// </summary>
        public StringComparison EditableTextStringComparision
        {
            get { return (StringComparison)GetValue(EditableTextStringComparisionProperty); }
            set { SetValue(EditableTextStringComparisionProperty, value); }
        }


        /// <summary>Identifies the <see cref="StringToObjectParser"/> dependency property.</summary>
        public static readonly DependencyProperty StringToObjectParserProperty = 
            DependencyProperty.Register(
                nameof(StringToObjectParser),
                typeof(IParseStringToObject),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a parser-class that implements <see cref="IParseStringToObject"/> 
        /// </summary>
        public IParseStringToObject StringToObjectParser
        {
            get { return (IParseStringToObject)GetValue(StringToObjectParserProperty); }
            set { SetValue(StringToObjectParserProperty, value); }
        }
       
        /// <summary>
        /// Resets the custom Text to the selected Items text 
        /// </summary>
        public void ResetEditableText()
        {
            SetCurrentValue(HasCustomTextProperty, BooleanBoxes.FalseBox);
            UpdateEditableText();
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContent"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentProperty = 
            DependencyProperty.Register(
                nameof(DisabledPopupOverlayContent),
                typeof(object),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContent
        /// </summary>
        public object DisabledPopupOverlayContent
        {
            get { return (object)GetValue(DisabledPopupOverlayContentProperty); }
            set { SetValue(DisabledPopupOverlayContentProperty, value); }
        }

        /// <summary>Identifies the <see cref="DisabledPopupOverlayContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledPopupOverlayContentTemplateProperty = 
            DependencyProperty.Register(
                nameof(DisabledPopupOverlayContentTemplate),
                typeof(DataTemplate),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the DisabledPopupOverlayContentTemplate
        /// </summary>
        public DataTemplate DisabledPopupOverlayContentTemplate
        {
            get { return (DataTemplate)GetValue(DisabledPopupOverlayContentTemplateProperty); }
            set { SetValue(DisabledPopupOverlayContentTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemTemplate),
                typeof(DataTemplate),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplate
        /// </summary>
        public DataTemplate SelectedItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectedItemTemplateProperty); }
            set { SetValue(SelectedItemTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemTemplateSelectorProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemTemplateSelector),
                typeof(DataTemplateSelector),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the SelectedItemTemplateSelector
        /// </summary>
        public DataTemplateSelector SelectedItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedItemTemplateSelectorProperty); }
            set { SetValue(SelectedItemTemplateSelectorProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedItemStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemStringFormatProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemStringFormat),
                typeof(string),
                typeof(MultiSelectionComboBox),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(UpdateText)));

        /// <summary>
        /// Gets or Sets the string format for the selected items
        /// </summary>
        public string SelectedItemStringFormat
        {
            get { return (string)GetValue(SelectedItemStringFormatProperty); }
            set { SetValue(SelectedItemStringFormatProperty, value); }
        }


        /// <summary>Identifies the <see cref="VerticalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(MultiSelectionComboBox), new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the vertical scrollbar is visible
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }


        /// <summary>Identifies the <see cref="HorizontalScrollBarVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = 
            DependencyProperty.Register(
                nameof(HorizontalScrollBarVisibility),
                typeof(ScrollBarVisibility),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(ScrollBarVisibility.Auto));

        /// <summary>
        /// Gets or Sets if the horizontal scrollbar is visible
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }


        /// <summary>Identifies the <see cref="SelectedItemsPanelTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedItemsPanelTemplateProperty = 
            DependencyProperty.Register(
                nameof(SelectedItemsPanelTemplate),
                typeof(ItemsPanelTemplate),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="ItemsPanelTemplate"/> for the selected items.
        /// </summary>
        public ItemsPanelTemplate SelectedItemsPanelTemplate
        {
            get { return (ItemsPanelTemplate)GetValue(SelectedItemsPanelTemplateProperty); }
            set { SetValue(SelectedItemsPanelTemplateProperty, value); }
        }


        /// <summary>Identifies the <see cref="SelectItemsFromTextInputDelay"/> dependency property.</summary>
        public static readonly DependencyProperty SelectItemsFromTextInputDelayProperty = 
            DependencyProperty.Register(
                nameof(SelectItemsFromTextInputDelay),
                typeof(int),
                typeof(MultiSelectionComboBox),
                new PropertyMetadata(-1));

        /// <summary>
        /// Gets or Sets the delay in miliseconds to wait before the selection is updated during text input. If this value is -1 the selection will not be updated during text input. 
        /// Note: You also need to set <see cref="ObjectToStringComparer"/> to get this to work. 
        /// </summary>
        public int SelectItemsFromTextInputDelay
        {
            get { return (int)GetValue(SelectItemsFromTextInputDelayProperty); }
            set { SetValue(SelectItemsFromTextInputDelayProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the Text of the editable Textbox.
        /// Sets the custom Text if any otherwise the concatenated string.
        /// </summary>
        private void UpdateEditableText()
        {
            if (PART_EditableTextBox is null || SelectedItems is null)
                return;

            var selectedItemsText = GetSelectedItemsText();

            if (!HasCustomText)
            {
                SetCurrentValue(TextProperty, selectedItemsText);
            }

            UpdateHasCustomText(selectedItemsText);
        }

        private void UpdateDisplaySelectedItems()
        {
            UpdateDisplaySelectedItems(OrderSelectedItemsBy);
        }


        public string GetSelectedItemsText()
        {
            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    if (ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue || ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        return BindingHelper.Eval(SelectedItem, DisplayMemberPath ?? "", SelectedItemStringFormat)?.ToString();
                    }
                    else
                    {
                        return SelectedItem?.ToString();
                    }

                case SelectionMode.Multiple:
                case SelectionMode.Extended:
                    IEnumerable<object> values;

                    if (ReadLocalValue(DisplayMemberPathProperty) != DependencyProperty.UnsetValue || ReadLocalValue(SelectedItemStringFormatProperty) != DependencyProperty.UnsetValue)
                    {
                        values = ((IEnumerable<object>)DisplaySelectedItems)?.Select(o => BindingHelper.Eval(o, DisplayMemberPath ?? string.Empty, SelectedItemStringFormat));
                    }
                    else
                    {
                        values = (IEnumerable<object>)DisplaySelectedItems;
                    }

                    return values is null ? null : string.Join(Separator ?? string.Empty, values);

                default:
                    return null;
            }
        }

        private void UpdateHasCustomText(string selectedItemsText)
        {
            // if the parameter was null lets get the text on our own.
            selectedItemsText ??= GetSelectedItemsText();

            bool hasCustomText = !((string.IsNullOrEmpty(selectedItemsText) && string.IsNullOrEmpty(Text)) || string.Equals(Text, selectedItemsText, EditableTextStringComparision));

            SetCurrentValue(HasCustomTextProperty, BooleanBoxes.Box(hasCustomText));
        }

        private void UpdateDisplaySelectedItems(OrderSelectedItemsBy orderBy)
        {
            switch (orderBy)
            {
                case OrderSelectedItemsBy.SelectedOrder:
                    SetCurrentValue(DisplaySelectedItemsProperty, SelectedItems);
                    break;
                case OrderSelectedItemsBy.ItemsSourceOrder:
                    SetCurrentValue(DisplaySelectedItemsProperty, ((IEnumerable<object>)PART_PopupListBox.SelectedItems).OrderBy(o => Items.IndexOf(o)));
                    break;
            }
        }

        private void SelectItemsFromText(int miliSecondsToWait)
        {
            if (!isUserdefinedTextInputPending)
            {
                return;
            }

            if (_updateSelectedItemsFromTextTimer is null)
            {
                _updateSelectedItemsFromTextTimer = new DispatcherTimer(DispatcherPriority.Background);
                _updateSelectedItemsFromTextTimer.Tick += UpdateSelectedItemsFromTextTimer_Tick;
            }

            if (_updateSelectedItemsFromTextTimer.IsEnabled)
            {
                _updateSelectedItemsFromTextTimer.Stop();
            }

            if (HasCustomText && !(ObjectToStringComparer is null) && !string.IsNullOrEmpty(Separator))
            {
                _updateSelectedItemsFromTextTimer.Interval = TimeSpan.FromMilliseconds(miliSecondsToWait > 0 ? miliSecondsToWait : 0);
                _updateSelectedItemsFromTextTimer.Start();
            }
        }

        private void UpdateSelectedItemsFromTextTimer_Tick(object sender, EventArgs e)
        {
            _updateSelectedItemsFromTextTimer.Stop();

            bool foundItem;

            // We clear the selection if there is no text available. 
            if (string.IsNullOrEmpty(Text))
            {
                switch (SelectionMode)
                {
                    case SelectionMode.Single:
                        SelectedItem = null;
                        break;
                    case SelectionMode.Multiple:
                    case SelectionMode.Extended:
                        SelectedItems.Clear();
                        break;
                    default:
                        break;
                }
                return;
            }

            switch (SelectionMode)
            {
                case SelectionMode.Single:
                    foundItem = false;
                    SelectedItem = null;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (ObjectToStringComparer.CheckIfStringMatchesObject(Text, Items[i], EditableTextStringComparision, SelectedItemStringFormat))
                        {
                            SetCurrentValue(SelectedItemProperty, Items[i]);
                            foundItem = true;
                            break;
                        }
                    }

                    if (!foundItem)
                    {
                        var result = TryAddObjectFromString(Text);
                        if (!(result is null))
                        {
                            SelectedItem = result;
                        }
                    }

                    break;
                case SelectionMode.Multiple:
                case SelectionMode.Extended:

                    var strings = Text.Split(new string[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

                    SelectedItems.Clear();

                    for (int i = 0; i < strings.Length; i++)
                    {
                        foundItem = false;
                        for (int j = 0; j < Items.Count; j++)
                        {
                            if (ObjectToStringComparer.CheckIfStringMatchesObject(strings[i], Items[j], EditableTextStringComparision, SelectedItemStringFormat))
                            {
                                SelectedItems.Add(Items[j]);
                                foundItem = true;
                            }
                        }

                        if (!foundItem)
                        {
                            var result = TryAddObjectFromString(strings[i]);
                            if (!(result is null))
                            {
                                SelectedItems.Add(result);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }


            // First we need to check if the string matches completely to the selected items. Therefore we need to display the items in the selected order first
            UpdateDisplaySelectedItems(OrderSelectedItemsBy.SelectedOrder);
            UpdateHasCustomText(null);

            // If the items should be ordered differntly than above we need to reoder them accordingly.
            if (OrderSelectedItemsBy != OrderSelectedItemsBy.SelectedOrder)
            {
                UpdateDisplaySelectedItems();
            }

            var oldCaretPos = PART_EditableTextBox.CaretIndex;
            UpdateEditableText();
            PART_EditableTextBox.CaretIndex = oldCaretPos;

            isUserdefinedTextInputPending = false;
        }

        private object TryAddObjectFromString(string input)
        {
            if (!(StringToObjectParser is null))
            {
                object item = StringToObjectParser.CreateObjectFromString(input, Language.GetEquivalentCulture(), SelectedItemStringFormat);

                if (item is null)
                {
                    return null;
                }
                else if (ReadLocalValue(ItemsSourceProperty) == DependencyProperty.UnsetValue)
                {
                    Items.Add(item);
                }
                else if (ItemsSource is IList list)
                {
                    list.Add(item);
                }

                return item;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Commands

        // Clear Text Command
        public static RoutedUICommand ClearContentCommand { get; } = new RoutedUICommand("ClearContent", nameof(ClearContentCommand), typeof(MultiSelectionComboBox));

        private void ExecutedClearContentCommand(object sender, ExecutedRoutedEventArgs e)
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
                            multiSelectionCombo.SelectedItem = null;
                            break;
                        case SelectionMode.Multiple:
                        case SelectionMode.Extended:
                            multiSelectionCombo.SelectedItems.Clear();
                            break;
                    }
                }
            }
        }

        private void CanExecuteClearContentCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (sender is MultiSelectionComboBox multiSelectionComboBox)
            {
                e.CanExecute = multiSelectionComboBox.Text != null || multiSelectionComboBox.SelectedItems?.Count > 0;
            }
        }

        public static RoutedUICommand RemoveItemCommand { get; } = new RoutedUICommand("Remove item", nameof(RemoveItemCommand), typeof(MultiSelectionComboBox));

        private void RemoveItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is MultiSelectionComboBox multiSelectionCombo && multiSelectionCombo.SelectedItems.Contains(e.Parameter))
            {
                if (multiSelectionCombo.SelectionMode == SelectionMode.Single)
                {
                    multiSelectionCombo.SelectedItem = null;
                    return;
                }
                multiSelectionCombo.SelectedItems.Remove(e.Parameter);
            }
        }

        private void RemoveItemCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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
            PART_SelectedItemsPresenter = GetTemplateChild(nameof(PART_SelectedItemsPresenter)) as ListBox;
            if (!(PART_SelectedItemsPresenter is null))
            {
                PART_SelectedItemsPresenter.MouseLeftButtonUp -= PART_SelectedItemsPresenter_MouseLeftButtonUp;
                PART_SelectedItemsPresenter.SelectionChanged -= PART_SelectedItemsPresenter_SelectionChanged;
                
                PART_SelectedItemsPresenter.MouseLeftButtonUp += PART_SelectedItemsPresenter_MouseLeftButtonUp;
                PART_SelectedItemsPresenter.SelectionChanged += PART_SelectedItemsPresenter_SelectionChanged;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(PART_SelectedItemsPresenter)}\" could not be found.");
            }
            // Init EditableTextBox
            PART_EditableTextBox = GetTemplateChild(nameof(PART_EditableTextBox)) as TextBox;

            if (!(PART_EditableTextBox is null))
            {
                PART_EditableTextBox.LostFocus -= PART_EditableTextBox_LostFocus;
                PART_EditableTextBox.LostFocus += PART_EditableTextBox_LostFocus;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(PART_EditableTextBox)}\" could not be found.");
            }

            // Init Popup
            PART_Popup = GetTemplateChild(nameof(PART_Popup)) as Popup;

            if (PART_Popup is null)
            {
                throw new MahAppsException($"The template part \"{nameof(PART_Popup)}\" could not be found.");
            }

            PART_PopupListBox = GetTemplateChild(nameof(PART_PopupListBox)) as ListBox;
            if (!(PART_PopupListBox is null))
            {
                PART_PopupListBox.SelectionChanged -= PART_PopupListBox_SelectionChanged;
                PART_PopupListBox.SelectionChanged += PART_PopupListBox_SelectionChanged;
            }
            else
            {
                throw new MahAppsException($"The template part \"{nameof(PART_PopupListBox)}\" could not be found.");
            }

            CommandBindings.Add(new CommandBinding(ClearContentCommand, ExecutedClearContentCommand, CanExecuteClearContentCommand));
            CommandBindings.Add(new CommandBinding(RemoveItemCommand, RemoveItemCommand_Executed, RemoveItemCommand_CanExecute));

            // Do update the text 
            UpdateEditableText();
            UpdateDisplaySelectedItems();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateEditableText();
            UpdateDisplaySelectedItems();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (!IsLoaded)
            {
                Loaded += MultiSelectionComboBox_Loaded;
                return;
            }

            // If we have the ItemsSource set, we need to exit here. 
            if (ReadLocalValue(ItemsSourceProperty) != DependencyProperty.UnsetValue) return;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        PART_PopupListBox.Items.Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        PART_PopupListBox.Items.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    // TODO Add Handler
                    break;
                case NotifyCollectionChangedAction.Move:
                    // TODO Add Handler
                    break;
                case NotifyCollectionChangedAction.Reset:
                    PART_PopupListBox.Items.Clear();
                    foreach (var item in Items)
                    {
                        PART_PopupListBox.Items.Add(item);
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // For now we only want to update our poition if the height changed. Else we will get a flickering in SharedGridColumns
            if (IsDropDownOpen && sizeInfo.HeightChanged && !(PART_Popup is null))
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


            PART_PopupListBox.Focus();

            if (PART_PopupListBox.Items.Count == 0) return;

            var index = PART_PopupListBox.SelectedIndex;
            if (index < 0) index = 0;

            Action action = () =>
            {
                PART_PopupListBox.ScrollIntoView(PART_PopupListBox.SelectedItem);

                if (PART_PopupListBox.ItemContainerGenerator.ContainerFromIndex(index) is ListBoxItem item)
                {
                    item.Focus();
                    KeyboardNavigationEx.Focus(item);
                }
            };

            Dispatcher.BeginInvoke(DispatcherPriority.Background, action);

            SelectItemsFromText(0);
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
            if (IsEditable && !IsDropDownOpen && !(PART_EditableTextBox is null) && !ComboBoxHelper.GetInterceptMouseWheelSelection(this))
            {
                if (HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled && ScrollViewerHelper.GetIsHorizontalScrollWheelEnabled(this))
                {
                    if (e.Delta > 0)
                    {
                        PART_EditableTextBox.LineLeft();
                    }
                    else
                    {
                        PART_EditableTextBox.LineRight();
                    }
                }
                else
                {
                    if (e.Delta > 0)
                    {
                        PART_EditableTextBox.LineUp();
                    }
                    else
                    {
                        PART_EditableTextBox.LineDown();
                    }
                }
            }
            else if (!IsDropDownOpen && !ComboBoxHelper.GetInterceptMouseWheelSelection(this))
            {
                base.OnPreviewMouseWheel(e);
                return;
            }
            // ListBox eats the selection so we need to handle this event here if we want to select the next item.
            else if (!IsDropDownOpen && ComboBoxHelper.GetInterceptMouseWheelSelection(this) && SelectionMode == SelectionMode.Single)
            { 
                if (e.Delta > 0 && PART_PopupListBox.SelectedIndex > 0)
                {
                    PART_PopupListBox.SelectedIndex--;
                }
                else if(e.Delta < 0 && PART_PopupListBox.SelectedIndex < PART_PopupListBox.Items.Count-1)
                {
                    PART_PopupListBox.SelectedIndex++;
                }
            }

            // The event is handled if the drop down is not open. 
            e.Handled = !IsDropDownOpen;
            base.OnPreviewMouseWheel(e);
        }
        #endregion

        #region Events

        private void PART_PopupListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDisplaySelectedItems();
            UpdateEditableText();
            isUserdefinedTextInputPending = false;
        }

        private void MultiSelectionComboBox_Loaded(object sender, EventArgs e)
        {
            Loaded -= MultiSelectionComboBox_Loaded;

            // If we have the ItemsSource set, we need to exit here. 
            if (ReadLocalValue(ItemsSourceProperty) != DependencyProperty.UnsetValue) return;

            PART_PopupListBox.Items.Clear();
            foreach (var item in Items)
            {
                PART_PopupListBox.Items.Add(item);
            }
        }

        private void PART_EditableTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SelectItemsFromText(0);
        }

        private void PART_SelectedItemsPresenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we have a ScrollViewer (ListBox has) we need to handle this event here as it will not be forwarded to the ToggleButton
            SetCurrentValue(IsDropDownOpenProperty, BooleanBoxes.Box(!IsDropDownOpen));
        }

        private void PART_SelectedItemsPresenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // We don't want the SelctedItems to be selectable. So anytime the selection will be changed we will reset it. 
            PART_SelectedItemsPresenter.SelectedItem = null;
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
