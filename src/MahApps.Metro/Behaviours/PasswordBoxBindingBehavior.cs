//	--------------------------------------------------------------------
//		Obtained from: WPFSmartLibrary
//		For more information see : http://wpfsmartlibrary.codeplex.com/
//		(by DotNetMastermind)
//	--------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Threading;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Behaviours
{
    public class PasswordBoxBindingBehavior : Behavior<PasswordBox>
    {
        /// <summary>
        ///     Gets or sets the bindable Password property on the PasswordBox control. This is a dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty
            = DependencyProperty.RegisterAttached("Password", typeof(string),
                                                  typeof(PasswordBoxBindingBehavior),
                                                  new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnPasswordPropertyChanged)));

        [Category(AppName.MahApps)]
        public static string GetPassword(DependencyObject dpo)
        {
            return (string)dpo.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dpo, string value)
        {
            dpo.SetValue(PasswordProperty, value);
        }

        /// <summary>
        ///     Handles changes to the 'Password' attached property.
        /// </summary>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var targetPasswordBox = sender as PasswordBox;
            if (targetPasswordBox != null)
            {
                targetPasswordBox.PasswordChanged -= PasswordBoxPasswordChanged;
                if (!GetIsChanging(targetPasswordBox))
                {
                    targetPasswordBox.Password = (string)e.NewValue;
                }
                targetPasswordBox.PasswordChanged += PasswordBoxPasswordChanged;
            }
        }

        /// <summary>
        ///     Handle the 'PasswordChanged'-event on the PasswordBox
        /// </summary>
        private static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            SetIsChanging(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsChanging(passwordBox, false);
        }

        private static void SetRevealedPasswordCaretIndex(PasswordBox passwordBox)
        {
            var textBox = GetRevealedPasswordTextBox(passwordBox);
            if (textBox != null)
            {
                var caretPos = GetPasswordBoxCaretPosition(passwordBox);
                textBox.CaretIndex = caretPos;
            }
        }

        private static int GetPasswordBoxCaretPosition(PasswordBox passwordBox)
        {
            var selection = GetSelection(passwordBox);
            var tTextRange = selection?.GetType().GetInterfaces().FirstOrDefault(i => i.Name == "ITextRange");
            var oStart = tTextRange?.GetProperty("Start")?.GetGetMethod()?.Invoke(selection, null);
            var value = oStart?.GetType().GetProperty("Offset", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(oStart, null) as int?;
            var caretPosition = value.GetValueOrDefault(0);
            return caretPosition;
        }

        /// <summary>
        ///     Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        ///     Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.AssociatedObject.PasswordChanged += PasswordBoxPasswordChanged;
            this.AssociatedObject.Loaded += this.PasswordBoxLoaded;
            var selection = GetSelection(this.AssociatedObject);
            if (selection != null)
            {
                selection.Changed += this.PasswordBoxSelectionChanged;
            }
        }

        private void PasswordBoxLoaded(object sender, RoutedEventArgs e)
        {
            SetPassword(this.AssociatedObject, this.AssociatedObject.Password);

            var textBox = this.AssociatedObject.FindChild<TextBox>("RevealedPassword");
            if (textBox != null)
            {
                var selection = GetSelection(this.AssociatedObject);
                if (selection == null)
                {
                    var infos = this.AssociatedObject.GetType().GetProperty("Selection", BindingFlags.NonPublic | BindingFlags.Instance);
                    selection = infos?.GetValue(this.AssociatedObject, null) as TextSelection;
                    SetSelection(this.AssociatedObject, selection);
                    if (selection != null)
                    {
                        SetRevealedPasswordTextBox(this.AssociatedObject, textBox);
                        SetRevealedPasswordCaretIndex(this.AssociatedObject);

                        selection.Changed += this.PasswordBoxSelectionChanged;
                    }
                }
            }
        }

        private void PasswordBoxSelectionChanged(object sender, EventArgs e)
        {
            SetRevealedPasswordCaretIndex(this.AssociatedObject);
        }

        /// <summary>
        ///     Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        ///     Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            // it seems, it was already detached, or never attached
            if (this.AssociatedObject != null)
            {
                var selection = GetSelection(this.AssociatedObject);
                if (selection != null)
                {
                    selection.Changed -= this.PasswordBoxSelectionChanged;
                }
                this.AssociatedObject.Loaded -= this.PasswordBoxLoaded;
                this.AssociatedObject.PasswordChanged -= PasswordBoxPasswordChanged;
            }
            base.OnDetaching();
        }

        private static readonly DependencyProperty IsChangingProperty
            = DependencyProperty.RegisterAttached("IsChanging",
                                                  typeof(bool),
                                                  typeof(PasswordBoxBindingBehavior),
                                                  new UIPropertyMetadata(false));

        private static bool GetIsChanging(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsChangingProperty);
        }

        private static void SetIsChanging(DependencyObject obj, bool value)
        {
            obj.SetValue(IsChangingProperty, value);
        }

        private static readonly DependencyProperty SelectionProperty
            = DependencyProperty.RegisterAttached("Selection",
                                                  typeof(TextSelection),
                                                  typeof(PasswordBoxBindingBehavior),
                                                  new UIPropertyMetadata(default(TextSelection)));

        private static TextSelection GetSelection(DependencyObject obj)
        {
            return (TextSelection)obj.GetValue(SelectionProperty);
        }

        private static void SetSelection(DependencyObject obj, TextSelection value)
        {
            obj.SetValue(SelectionProperty, value);
        }

        private static readonly DependencyProperty RevealedPasswordTextBoxProperty
            = DependencyProperty.RegisterAttached("RevealedPasswordTextBox",
                                                  typeof(TextBox),
                                                  typeof(PasswordBoxBindingBehavior),
                                                  new UIPropertyMetadata(default(TextBox)));

        private static TextBox GetRevealedPasswordTextBox(DependencyObject obj)
        {
            return (TextBox)obj.GetValue(RevealedPasswordTextBoxProperty);
        }

        private static void SetRevealedPasswordTextBox(DependencyObject obj, TextBox value)
        {
            obj.SetValue(RevealedPasswordTextBoxProperty, value);
        }
    }
}