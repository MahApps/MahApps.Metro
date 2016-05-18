using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ControlzEx
{
    /// <summary>
    /// Helper class for a common focusing problem.
    /// The focus itself isn't the problem. If we use the common focusing methods the control get the focus
    /// but it doesn't get the focus visual style.
    /// The KeyboardNavigation class handles the visual style only if the control get the focus from a keyboard
    /// device or if the SystemParameters.KeyboardCues is true.
    /// </summary>
    public sealed class KeyboardNavigationEx
    {
        private static KeyboardNavigationEx _instance;
        //internal static bool AlwaysShowFocusVisual
        private readonly PropertyInfo _alwaysShowFocusVisual;
        //internal static void ShowFocusVisual()
        private readonly MethodInfo _showFocusVisual;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static KeyboardNavigationEx()
        {
        }

        private KeyboardNavigationEx()
        {
            var type = typeof(KeyboardNavigation);
            _alwaysShowFocusVisual = type.GetProperty("AlwaysShowFocusVisual", BindingFlags.NonPublic | BindingFlags.Static);
            _showFocusVisual = type.GetMethod("ShowFocusVisual", BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// Gets the KeyboardNavigationEx singleton instance.
        /// </summary>
        internal static KeyboardNavigationEx Instance => _instance ?? (_instance = new KeyboardNavigationEx());

        /// <summary>
        /// Shows the focus visual of the current focused UI element.
        /// Works only together with AlwaysShowFocusVisual property.
        /// </summary>
        internal void ShowFocusVisualInternal()
        {
            _showFocusVisual.Invoke(null, null);
        }

        internal bool AlwaysShowFocusVisualInternal
        {
            get { return (bool) _alwaysShowFocusVisual.GetValue(null, null); }
            set { _alwaysShowFocusVisual.SetValue(null, value, null); }
        }

        /// <summary>
        /// Focuses the specified element and shows the focus visual style.
        /// </summary>
        /// <param name="element">The element which will be focused.</param>
        public static void Focus(UIElement element)
        {
            element?.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                var keybHack = KeyboardNavigationEx.Instance;
                var alwaysShowFocusVisual = keybHack.AlwaysShowFocusVisualInternal;
                keybHack.AlwaysShowFocusVisualInternal = true;
                try
                {
                    Keyboard.Focus(element);
                    keybHack.ShowFocusVisualInternal();
                }
                finally
                {
                    keybHack.AlwaysShowFocusVisualInternal = alwaysShowFocusVisual;
                }
            }));
        }

        /// <summary>
        /// Attached DependencyProperty for setting AlwaysShowFocusVisual for a UI element.
        /// </summary>
        public static readonly DependencyProperty AlwaysShowFocusVisualProperty
            = DependencyProperty.RegisterAttached("AlwaysShowFocusVisual",
                typeof(bool),
                typeof(KeyboardNavigationEx),
                new FrameworkPropertyMetadata(default(bool), AlwaysShowFocusVisualPropertyChangedCallback));

        private static void AlwaysShowFocusVisualPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var fe = dependencyObject as UIElement;
            if (fe != null && args.NewValue != args.OldValue)
            {
                fe.GotFocus -= FrameworkElementGotFocus;
                if ((bool)args.NewValue)
                {
                    fe.GotFocus += FrameworkElementGotFocus;
                }
            }
        }

        private static void FrameworkElementGotFocus(object sender, RoutedEventArgs e)
        {
            KeyboardNavigationEx.Focus(sender as UIElement);
        }

        /// <summary>
        /// Gets a the value which indicates if the UI element always show the focus visual style.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static bool GetAlwaysShowFocusVisual(UIElement element)
        {
            return (bool)element.GetValue(AlwaysShowFocusVisualProperty);
        }

        /// <summary>
        /// Sets a the value which indicates if the UI element always show the focus visual style.
        /// </summary>
        public static void SetAlwaysShowFocusVisual(UIElement element, bool value)
        {
            element.SetValue(AlwaysShowFocusVisualProperty, value);
        }
    }
}