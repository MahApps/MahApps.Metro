// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using Windows.Win32;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class HotKeyBox : Control
    {
        private const string PART_TextBox = "PART_TextBox";

        /// <summary>Identifies the <see cref="HotKey"/> dependency property.</summary>
        public static readonly DependencyProperty HotKeyProperty
            = DependencyProperty.Register(nameof(HotKey),
                                          typeof(HotKey),
                                          typeof(HotKeyBox),
                                          new FrameworkPropertyMetadata(default(HotKey?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHotKeyPropertyChanged));

        private static void OnHotKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HotKeyBox hotKeyBox)
            {
                if (e.OldValue != e.NewValue)
                {
                    hotKeyBox.RaiseEvent(new RoutedPropertyChangedEventArgs<HotKey?>(e.OldValue as HotKey, e.NewValue as HotKey, HotKeyChangedEvent));
                }

                hotKeyBox.UpdateText();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="HotKey"/> for this <see cref="HotKeyBox"/>.
        /// </summary>
        public HotKey? HotKey
        {
            get => (HotKey?)this.GetValue(HotKeyProperty);
            set => this.SetValue(HotKeyProperty, value);
        }

        /// <summary>Identifies the <see cref="AreModifierKeysRequired"/> dependency property.</summary>
        public static readonly DependencyProperty AreModifierKeysRequiredProperty
            = DependencyProperty.Register(nameof(AreModifierKeysRequired),
                                          typeof(bool),
                                          typeof(HotKeyBox),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets if the modifier keys are required.
        /// </summary>
        public bool AreModifierKeysRequired
        {
            get => (bool)this.GetValue(AreModifierKeysRequiredProperty);
            set => this.SetValue(AreModifierKeysRequiredProperty, BooleanBoxes.Box(value));
        }

        private static readonly DependencyPropertyKey TextPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Text),
                                                  typeof(string),
                                                  typeof(HotKeyBox),
                                                  new PropertyMetadata(default(string)));

        /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the text of the <see cref="HotKey"/>.
        /// </summary>
        public string? Text
        {
            get => (string?)this.GetValue(TextProperty);
            protected set => this.SetValue(TextPropertyKey, value);
        }

        /// <summary>Identifies the <see cref="HotKeyChanged"/> routed event.</summary>
        public static readonly RoutedEvent HotKeyChangedEvent
            = EventManager.RegisterRoutedEvent(nameof(HotKeyChanged),
                                               RoutingStrategy.Bubble,
                                               typeof(RoutedPropertyChangedEventHandler<HotKey?>),
                                               typeof(HotKeyBox));

        /// <summary>
        /// Add / Remove HotKeyChangedEvent handler
        /// Event which will be fired from this HotKeyBox when its value has been changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<HotKey?> HotKeyChanged
        {
            add => this.AddHandler(HotKeyChangedEvent, value);
            remove => this.RemoveHandler(HotKeyChangedEvent, value);
        }

        private TextBox? _textBox;

        static HotKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotKeyBox), new FrameworkPropertyMetadata(typeof(HotKeyBox)));
            EventManager.RegisterClassHandler(typeof(HotKeyBox), GotFocusEvent, new RoutedEventHandler(OnGotFocus));
        }

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // If we're an editable HotKeyBox, forward focus to the TextBox or previous element
            if (!e.Handled)
            {
                HotKeyBox hotKeyBox = (HotKeyBox)sender;
                if (hotKeyBox.Focusable && e.OriginalSource == hotKeyBox)
                {
                    // MoveFocus takes a TraversalRequest as its argument.
                    var request = new TraversalRequest((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next);
                    // Gets the element with keyboard focus.
                    // Change keyboard focus.
                    if (Keyboard.FocusedElement is UIElement elementWithFocus)
                    {
                        elementWithFocus.MoveFocus(request);
                    }
                    else
                    {
                        hotKeyBox.Focus();
                    }

                    e.Handled = true;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            if (this._textBox != null)
            {
                this._textBox.PreviewKeyDown -= this.TextBoxOnPreviewKeyDown2;
                this._textBox.GotFocus -= this.TextBoxOnGotFocus;
                this._textBox.LostFocus -= this.TextBoxOnLostFocus;
                this._textBox.TextChanged -= this.TextBoxOnTextChanged;
            }

            base.OnApplyTemplate();

            this._textBox = this.Template.FindName(PART_TextBox, this) as TextBox;
            if (this._textBox != null)
            {
                this._textBox.PreviewKeyDown += this.TextBoxOnPreviewKeyDown2;
                this._textBox.GotFocus += this.TextBoxOnGotFocus;
                this._textBox.LostFocus += this.TextBoxOnLostFocus;
                this._textBox.TextChanged += this.TextBoxOnTextChanged;
            }

            this.UpdateText();
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs args)
        {
            this._textBox!.SelectionStart = this._textBox.Text.Length;
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage += this.ComponentDispatcherOnThreadPreprocessMessage;
        }

        private void ComponentDispatcherOnThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == PInvoke.WM_HOTKEY)
            {
                // swallow all hotkeys, so our control can catch the key strokes
                handled = true;
            }
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage -= this.ComponentDispatcherOnThreadPreprocessMessage;
        }

        private void TextBoxOnPreviewKeyDown2(object sender, KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            switch (key)
            {
                case Key.Tab:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.RWin:
                case Key.LWin:
                    return;
            }

            e.Handled = true;

            var currentModifierKeys = GetCurrentModifierKeys();
            if (currentModifierKeys == ModifierKeys.None && key == Key.Back)
            {
                this.SetCurrentValue(HotKeyProperty, null);
            }
            else if (currentModifierKeys != ModifierKeys.None || !this.AreModifierKeysRequired)
            {
                this.SetCurrentValue(HotKeyProperty, new HotKey(key, currentModifierKeys));
            }

            this.UpdateText();
        }

        private static ModifierKeys GetCurrentModifierKeys()
        {
            var modifier = ModifierKeys.None;
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                modifier |= ModifierKeys.Control;
            }

            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                modifier |= ModifierKeys.Alt;
            }

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                modifier |= ModifierKeys.Shift;
            }

            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
            {
                modifier |= ModifierKeys.Windows;
            }

            return modifier;
        }

        private void UpdateText()
        {
            var hotKey = this.HotKey;
            this.SetValue(HotKeyBox.TextPropertyKey, hotKey is null || hotKey.Key == Key.None ? string.Empty : hotKey.ToString());
        }
    }
}