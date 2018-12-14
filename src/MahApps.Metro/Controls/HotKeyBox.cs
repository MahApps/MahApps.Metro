using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using ControlzEx.Native;
using ControlzEx.Standard;

namespace MahApps.Metro.Controls {
    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    public class HotKeyBox : Control
    {
        private const string PART_TextBox = "PART_TextBox";

        public static readonly DependencyProperty HotKeyProperty = DependencyProperty.Register(
            "HotKey", typeof(HotKey), typeof(HotKeyBox),
            new FrameworkPropertyMetadata(default(HotKey), OnHotKeyChanged) { BindsTwoWayByDefault = true });

        public HotKey HotKey
        {
            get { return (HotKey) GetValue(HotKeyProperty); }
            set { SetValue(HotKeyProperty, value); }
        }

        private static void OnHotKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (HotKeyBox)d;
            ctrl.UpdateText();
        }

        public static readonly DependencyProperty AreModifierKeysRequiredProperty = DependencyProperty.Register(
            "AreModifierKeysRequired", typeof(bool), typeof(HotKeyBox), new PropertyMetadata(default(bool)));

        public bool AreModifierKeysRequired
        {
            get { return (bool) GetValue(AreModifierKeysRequiredProperty); }
            set { SetValue(AreModifierKeysRequiredProperty, value); }
        }

        private static readonly DependencyPropertyKey TextPropertyKey = DependencyProperty.RegisterReadOnly(
            "Text", typeof(string), typeof(HotKeyBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            private set { SetValue(TextPropertyKey, value); }
        }

        private TextBox _textBox;

        static HotKeyBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HotKeyBox), new FrameworkPropertyMetadata(typeof(HotKeyBox)));
            EventManager.RegisterClassHandler(typeof(HotKeyBox), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
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
                    var elementWithFocus = Keyboard.FocusedElement as UIElement;
                    // Change keyboard focus.
                    if (elementWithFocus != null)
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
            if (_textBox != null)
            {
                _textBox.PreviewKeyDown -= TextBoxOnPreviewKeyDown2;
                _textBox.GotFocus -= TextBoxOnGotFocus;
                _textBox.LostFocus -= TextBoxOnLostFocus;
                _textBox.TextChanged -= TextBoxOnTextChanged;
            }

            base.OnApplyTemplate();

            _textBox = Template.FindName(PART_TextBox, this) as TextBox;
            if (_textBox != null)
            {
                _textBox.PreviewKeyDown += TextBoxOnPreviewKeyDown2;
                _textBox.GotFocus += TextBoxOnGotFocus;
                _textBox.LostFocus += TextBoxOnLostFocus;
                _textBox.TextChanged += TextBoxOnTextChanged;
                UpdateText();
            }
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs args)
        {
            _textBox.SelectionStart = _textBox.Text.Length;
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcherOnThreadPreprocessMessage;
        }

#pragma warning disable 618
        private void ComponentDispatcherOnThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message == (int)WM.HOTKEY)
            {
                // swallow all hotkeys, so our control can catch the key strokes
                handled = true;
            }
        }
#pragma warning restore 618

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcherOnThreadPreprocessMessage;
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
                HotKey = null;
            }
            else if (currentModifierKeys != ModifierKeys.None || !AreModifierKeysRequired)
            {
                HotKey = new HotKey(key, currentModifierKeys);
            }

            UpdateText();
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
            var hotkey = HotKey;
            Text = hotkey == null || hotkey.Key == Key.None ? string.Empty : hotkey.ToString();
        }
    }

    public class HotKey : IEquatable<HotKey>
    {
        private readonly Key _key;
        private readonly ModifierKeys _modifierKeys;

        public HotKey(Key key, ModifierKeys modifierKeys = ModifierKeys.None)
        {
            _key = key;
            _modifierKeys = modifierKeys;
        }

        public Key Key
        {
            get { return _key; }
        }

        public ModifierKeys ModifierKeys
        {
            get { return _modifierKeys; }
        }

        public override bool Equals(object obj)
        {
            return obj is HotKey && Equals((HotKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) _key*397) ^ (int) _modifierKeys;
            }
        }

        public bool Equals(HotKey other)
        {
            return _key == other._key && _modifierKeys == other._modifierKeys;
        }

#pragma warning disable 618
        public override string ToString()
        {
            var sb = new StringBuilder();
            if ((_modifierKeys & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_MENU));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_CONTROL));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                sb.Append(GetLocalizedKeyStringUnsafe(Constants.VK_SHIFT));
                sb.Append("+");
            }
            if ((_modifierKeys & ModifierKeys.Windows) == ModifierKeys.Windows)
            {
                sb.Append("Windows+");
            }
            sb.Append(GetLocalizedKeyString(_key));
            return sb.ToString();
        }
#pragma warning restore 618

        private static string GetLocalizedKeyString(Key key)
        {
            if (key >= Key.BrowserBack && key <= Key.LaunchApplication2)
            {
                return key.ToString();
            }

            var vkey = KeyInterop.VirtualKeyFromKey(key);
            return GetLocalizedKeyStringUnsafe(vkey) ?? key.ToString();
        }

#pragma warning disable 618
        private static string GetLocalizedKeyStringUnsafe(int key)
        {
            // strip any modifier keys
            long keyCode = key & 0xffff;

            var sb = new StringBuilder(256);

            long scanCode = NativeMethods.MapVirtualKey((uint)keyCode, NativeMethods.MapType.MAPVK_VK_TO_VSC);

            // shift the scancode to the high word
            scanCode = (scanCode << 16);
            if (keyCode == 45 ||
                keyCode == 46 ||
                keyCode == 144 ||
                (33 <= keyCode && keyCode <= 40))
            {
                // add the extended key flag
                scanCode |= 0x1000000;
            }

            var resultLength = UnsafeNativeMethods.GetKeyNameText((int)scanCode, sb, 256);
            return resultLength > 0 ? sb.ToString() : null;
        }
#pragma warning restore 618
    }
}