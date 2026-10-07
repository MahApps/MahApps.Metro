// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A dialog with a title, content and up to three buttons, after the ContentDialog of WinUI. It is a
    /// <see cref="ChildWindow"/>, so it opens in the dialog container of a <see cref="MetroWindow"/>.
    /// </summary>
    [TemplatePart(Name = PART_PrimaryButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_SecondaryButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_CloseButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ContentTitleThumb, Type = typeof(IMetroThumb))]
    [TemplatePart(Name = PART_ContentTitleCloseButton, Type = typeof(Button))]
    [StyleTypedProperty(Property = nameof(PrimaryButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(SecondaryButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(CloseButtonStyle), StyleTargetType = typeof(Button))]
    public class ContentDialog : ChildWindow
    {
        private const string PART_PrimaryButton = "PART_PrimaryButton";
        private const string PART_SecondaryButton = "PART_SecondaryButton";
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_ContentTitleThumb = "PART_ContentTitleThumb";
        private const string PART_ContentTitleCloseButton = "PART_ContentTitleCloseButton";

        private Button? primaryButton;
        private Button? secondaryButton;
        private Button? closeButton;
        private IMetroThumb? contentTitleThumb;
        private Button? contentTitleCloseButton;

        /// <summary>Identifies the <see cref="PrimaryButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty PrimaryButtonTextProperty
            = DependencyProperty.Register(nameof(PrimaryButtonText),
                                          typeof(string),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null, OnButtonTextChanged));

        /// <summary>
        /// Gets or sets the text of the primary button. Without a text the button is not shown.
        /// </summary>
        public string? PrimaryButtonText
        {
            get => (string?)this.GetValue(PrimaryButtonTextProperty);
            set => this.SetValue(PrimaryButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="SecondaryButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty SecondaryButtonTextProperty
            = DependencyProperty.Register(nameof(SecondaryButtonText),
                                          typeof(string),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null, OnButtonTextChanged));

        /// <summary>
        /// Gets or sets the text of the secondary button. Without a text the button is not shown.
        /// </summary>
        public string? SecondaryButtonText
        {
            get => (string?)this.GetValue(SecondaryButtonTextProperty);
            set => this.SetValue(SecondaryButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonTextProperty
            = DependencyProperty.Register(nameof(CloseButtonText),
                                          typeof(string),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null, OnButtonTextChanged));

        /// <summary>
        /// Gets or sets the text of the close button. Without a text the button is not shown, while Escape still does what it would.
        /// </summary>
        public string? CloseButtonText
        {
            get => (string?)this.GetValue(CloseButtonTextProperty);
            set => this.SetValue(CloseButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="PrimaryButtonCommand"/> dependency property.</summary>
        public static readonly DependencyProperty PrimaryButtonCommandProperty
            = DependencyProperty.Register(nameof(PrimaryButtonCommand),
                                          typeof(ICommand),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that runs when the primary button is clicked. Its CanExecute can keep the dialog open.
        /// </summary>
        public ICommand? PrimaryButtonCommand
        {
            get => (ICommand?)this.GetValue(PrimaryButtonCommandProperty);
            set => this.SetValue(PrimaryButtonCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="PrimaryButtonCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty PrimaryButtonCommandParameterProperty
            = DependencyProperty.Register(nameof(PrimaryButtonCommandParameter),
                                          typeof(object),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter of <see cref="PrimaryButtonCommand"/>.
        /// </summary>
        public object? PrimaryButtonCommandParameter
        {
            get => this.GetValue(PrimaryButtonCommandParameterProperty);
            set => this.SetValue(PrimaryButtonCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="SecondaryButtonCommand"/> dependency property.</summary>
        public static readonly DependencyProperty SecondaryButtonCommandProperty
            = DependencyProperty.Register(nameof(SecondaryButtonCommand),
                                          typeof(ICommand),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that runs when the secondary button is clicked. Its CanExecute can keep the dialog open.
        /// </summary>
        public ICommand? SecondaryButtonCommand
        {
            get => (ICommand?)this.GetValue(SecondaryButtonCommandProperty);
            set => this.SetValue(SecondaryButtonCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="SecondaryButtonCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty SecondaryButtonCommandParameterProperty
            = DependencyProperty.Register(nameof(SecondaryButtonCommandParameter),
                                          typeof(object),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter of <see cref="SecondaryButtonCommand"/>.
        /// </summary>
        public object? SecondaryButtonCommandParameter
        {
            get => this.GetValue(SecondaryButtonCommandParameterProperty);
            set => this.SetValue(SecondaryButtonCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonCommand"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonCommandProperty
            = DependencyProperty.Register(nameof(CloseButtonCommand),
                                          typeof(ICommand),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command that runs for the close button, for Escape and for the close button in the title bar.
        /// Its CanExecute can keep the dialog open.
        /// </summary>
        public ICommand? CloseButtonCommand
        {
            get => (ICommand?)this.GetValue(CloseButtonCommandProperty);
            set => this.SetValue(CloseButtonCommandProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonCommandParameterProperty
            = DependencyProperty.Register(nameof(CloseButtonCommandParameter),
                                          typeof(object),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter of <see cref="CloseButtonCommand"/>.
        /// </summary>
        public object? CloseButtonCommandParameter
        {
            get => this.GetValue(CloseButtonCommandParameterProperty);
            set => this.SetValue(CloseButtonCommandParameterProperty, value);
        }

        /// <summary>Identifies the <see cref="IsPrimaryButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsPrimaryButtonEnabledProperty
            = DependencyProperty.Register(nameof(IsPrimaryButtonEnabled),
                                          typeof(bool),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the primary button can be clicked.
        /// </summary>
        public bool IsPrimaryButtonEnabled
        {
            get => (bool)this.GetValue(IsPrimaryButtonEnabledProperty);
            set => this.SetValue(IsPrimaryButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="IsSecondaryButtonEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsSecondaryButtonEnabledProperty
            = DependencyProperty.Register(nameof(IsSecondaryButtonEnabled),
                                          typeof(bool),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the secondary button can be clicked.
        /// </summary>
        public bool IsSecondaryButtonEnabled
        {
            get => (bool)this.GetValue(IsSecondaryButtonEnabledProperty);
            set => this.SetValue(IsSecondaryButtonEnabledProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="DefaultButton"/> dependency property.</summary>
        public static readonly DependencyProperty DefaultButtonProperty
            = DependencyProperty.Register(nameof(DefaultButton),
                                          typeof(ContentDialogButton),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(ContentDialogButton.None));

        /// <summary>
        /// Gets or sets the button that takes Enter, the accent look and, if the content has nothing to focus, the focus.
        /// </summary>
        public ContentDialogButton DefaultButton
        {
            get => (ContentDialogButton)this.GetValue(DefaultButtonProperty);
            set => this.SetValue(DefaultButtonProperty, value);
        }

        /// <summary>Identifies the <see cref="PrimaryButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty PrimaryButtonStyleProperty
            = DependencyProperty.Register(nameof(PrimaryButtonStyle),
                                          typeof(Style),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the primary button. As the default button it takes the accent style of the set instead.
        /// </summary>
        public Style? PrimaryButtonStyle
        {
            get => (Style?)this.GetValue(PrimaryButtonStyleProperty);
            set => this.SetValue(PrimaryButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="SecondaryButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty SecondaryButtonStyleProperty
            = DependencyProperty.Register(nameof(SecondaryButtonStyle),
                                          typeof(Style),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the secondary button. As the default button it takes the accent style of the set instead.
        /// </summary>
        public Style? SecondaryButtonStyle
        {
            get => (Style?)this.GetValue(SecondaryButtonStyleProperty);
            set => this.SetValue(SecondaryButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="CloseButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty CloseButtonStyleProperty
            = DependencyProperty.Register(nameof(CloseButtonStyle),
                                          typeof(Style),
                                          typeof(ContentDialog),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style of the close button. As the default button it takes the accent style of the set instead.
        /// </summary>
        public Style? CloseButtonStyle
        {
            get => (Style?)this.GetValue(CloseButtonStyleProperty);
            set => this.SetValue(CloseButtonStyleProperty, value);
        }

        /// <summary>
        /// Raised when the primary button is clicked. Setting <see cref="ContentDialogButtonClickEventArgs.Cancel"/> keeps the dialog open.
        /// </summary>
        public event EventHandler<ContentDialogButtonClickEventArgs>? PrimaryButtonClick;

        /// <summary>
        /// Raised when the secondary button is clicked. Setting <see cref="ContentDialogButtonClickEventArgs.Cancel"/> keeps the dialog open.
        /// </summary>
        public event EventHandler<ContentDialogButtonClickEventArgs>? SecondaryButtonClick;

        /// <summary>
        /// Raised when the close button is clicked, and for Escape and the close button in the title bar as well.
        /// Setting <see cref="ContentDialogButtonClickEventArgs.Cancel"/> keeps the dialog open.
        /// </summary>
        public event EventHandler<ContentDialogButtonClickEventArgs>? CloseButtonClick;

        /// <summary>
        /// Raised before the dialog closes, with what it closes with. Setting Cancel keeps it open.
        /// </summary>
        public new event EventHandler<ContentDialogClosingEventArgs>? Closing;

        static ContentDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentDialog), new FrameworkPropertyMetadata(typeof(ContentDialog)));
        }

        /// <summary>
        /// Shows the dialog in the active <see cref="MetroWindow"/> of the application, or in its main window.
        /// </summary>
        /// <exception cref="InvalidOperationException">There is no such window.</exception>
        public Task<ContentDialogResult> ShowAsync()
        {
            var application = Application.Current;
            var owner = application?.Windows.OfType<MetroWindow>().FirstOrDefault(w => w.IsActive)
                        ?? application?.MainWindow as MetroWindow;
            if (owner is null)
            {
                throw new InvalidOperationException("There is no MetroWindow to show the content dialog in.");
            }

            return this.ShowAsync(owner);
        }

        /// <summary>
        /// Shows the dialog in the given window.
        /// </summary>
        /// <param name="owner">The window to show the dialog in.</param>
        public Task<ContentDialogResult> ShowAsync(MetroWindow owner)
        {
            return owner.ShowContentDialogAsync(this);
        }

        /// <summary>
        /// Closes the dialog with <see cref="ContentDialogResult.None"/>.
        /// </summary>
        public void Hide()
        {
            this.Close(CloseReason.Close, ContentDialogResult.None);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.primaryButton is not null)
            {
                this.primaryButton.Click -= this.OnPrimaryButtonClick;
            }

            if (this.secondaryButton is not null)
            {
                this.secondaryButton.Click -= this.OnSecondaryButtonClick;
            }

            if (this.closeButton is not null)
            {
                this.closeButton.Click -= this.OnCloseButtonClick;
            }

            if (this.contentTitleCloseButton is not null)
            {
                this.contentTitleCloseButton.Click -= this.OnCloseButtonClick;
            }

            if (this.contentTitleThumb is not null)
            {
                this.contentTitleThumb.DragDelta -= this.OnTitleBarThumbDragDelta;
            }

            base.OnApplyTemplate();

            this.primaryButton = this.GetTemplateChild(PART_PrimaryButton) as Button;
            this.secondaryButton = this.GetTemplateChild(PART_SecondaryButton) as Button;
            this.closeButton = this.GetTemplateChild(PART_CloseButton) as Button;
            this.contentTitleCloseButton = this.GetTemplateChild(PART_ContentTitleCloseButton) as Button;
            this.contentTitleThumb = this.GetTemplateChild(PART_ContentTitleThumb) as IMetroThumb;

            if (this.primaryButton is not null)
            {
                this.primaryButton.Click += this.OnPrimaryButtonClick;
            }

            if (this.secondaryButton is not null)
            {
                this.secondaryButton.Click += this.OnSecondaryButtonClick;
            }

            if (this.closeButton is not null)
            {
                this.closeButton.Click += this.OnCloseButtonClick;
            }

            if (this.contentTitleCloseButton is not null)
            {
                this.contentTitleCloseButton.Click += this.OnCloseButtonClick;
            }

            if (this.contentTitleThumb is not null)
            {
                this.contentTitleThumb.DragDelta += this.OnTitleBarThumbDragDelta;
            }

            this.UpdateButtonsVisibilityState(false);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ContentDialogAutomationPeer(this);
        }

        /// <inheritdoc />
        protected override CancelEventArgs CreateClosingEventArgs(CloseReason closedBy, object? childWindowResult)
        {
            return new ContentDialogClosingEventArgs(childWindowResult is ContentDialogResult result ? result : ContentDialogResult.None);
        }

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (e is ContentDialogClosingEventArgs args)
            {
                this.Closing?.Invoke(this, args);
            }
        }

        /// <inheritdoc />
        protected override void OnTitleBarCloseButtonClick()
        {
            // the X is one more way to the close button, the command of the title bar is not used here
            this.Process(ContentDialogButton.Close, CloseReason.Close);
        }

        /// <inheritdoc />
        protected override UIElement? GetElementToFocus()
        {
            if (this.FocusedElement is not null)
            {
                return this.FocusedElement;
            }

            // the logical tree, since the overlay is still collapsed and nothing in it has been measured yet
            var inContent = FirstToTabTo(this.Content as DependencyObject);
            if (inContent is not null)
            {
                return inContent;
            }

            return this.DefaultButtonPart() is { IsEnabled: true, Visibility: Visibility.Visible } defaultButton ? defaultButton : null;
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled && e.Key == Key.Escape && this.CloseByEscape)
            {
                // Escape is the close button, whether or not it shows
                this.Process(ContentDialogButton.Close, CloseReason.Escape);
                e.Handled = true;
                return;
            }

            if (!e.Handled
                && e.Key == Key.Enter
                && e.OriginalSource is not ButtonBase
                && e.OriginalSource is not TextBoxBase { AcceptsReturn: true }
                && this.DefaultButtonPart() is { IsEnabled: true, Visibility: Visibility.Visible })
            {
                this.Process(this.DefaultButton, CloseReason.Close);
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// The first element Tab would stop at: one that is shown, enabled and a tab stop. A hidden branch is skipped whole.
        /// </summary>
        private static UIElement? FirstToTabTo(DependencyObject? element)
        {
            if (element is null)
            {
                return null;
            }

            if (element is UIElement uiElement)
            {
                if (uiElement.Visibility != Visibility.Visible)
                {
                    return null;
                }

                if (uiElement.Focusable && uiElement.IsEnabled && KeyboardNavigation.GetIsTabStop(uiElement))
                {
                    return uiElement;
                }
            }

            foreach (var child in LogicalTreeHelper.GetChildren(element).OfType<DependencyObject>())
            {
                var found = FirstToTabTo(child);
                if (found is not null)
                {
                    return found;
                }
            }

            return null;
        }

        private Button? DefaultButtonPart()
        {
            return this.DefaultButton switch
            {
                ContentDialogButton.Primary => this.primaryButton,
                ContentDialogButton.Secondary => this.secondaryButton,
                ContentDialogButton.Close => this.closeButton,
                _ => null
            };
        }

        private void OnPrimaryButtonClick(object sender, RoutedEventArgs e) => this.Process(ContentDialogButton.Primary, CloseReason.Close);

        private void OnSecondaryButtonClick(object sender, RoutedEventArgs e) => this.Process(ContentDialogButton.Secondary, CloseReason.Close);

        private void OnCloseButtonClick(object sender, RoutedEventArgs e) => this.Process(ContentDialogButton.Close, CloseReason.Close);

        /// <summary>
        /// The way every button goes: its click event, which can cancel, its command, which can refuse, and the close.
        /// </summary>
        private bool Process(ContentDialogButton button, CloseReason closedBy)
        {
            var args = new ContentDialogButtonClickEventArgs();
            ICommand? command;
            object? parameter;
            ContentDialogResult result;

            switch (button)
            {
                case ContentDialogButton.Primary:
                    this.PrimaryButtonClick?.Invoke(this, args);
                    command = this.PrimaryButtonCommand;
                    parameter = this.PrimaryButtonCommandParameter;
                    result = ContentDialogResult.Primary;
                    break;
                case ContentDialogButton.Secondary:
                    this.SecondaryButtonClick?.Invoke(this, args);
                    command = this.SecondaryButtonCommand;
                    parameter = this.SecondaryButtonCommandParameter;
                    result = ContentDialogResult.Secondary;
                    break;
                case ContentDialogButton.Close:
                    this.CloseButtonClick?.Invoke(this, args);
                    command = this.CloseButtonCommand;
                    parameter = this.CloseButtonCommandParameter;
                    result = ContentDialogResult.None;
                    break;
                default:
                    return false;
            }

            if (args.Cancel || (command is not null && !command.CanExecute(parameter)))
            {
                return false;
            }

            if (!this.Close(closedBy, result))
            {
                return false;
            }

            command?.Execute(parameter);
            return true;
        }

        private static void OnButtonTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentDialog)d).UpdateButtonsVisibilityState(true);
        }

        private void UpdateButtonsVisibilityState(bool useTransitions)
        {
            var primary = !string.IsNullOrEmpty(this.PrimaryButtonText);
            var secondary = !string.IsNullOrEmpty(this.SecondaryButtonText);
            var close = !string.IsNullOrEmpty(this.CloseButtonText);

            var state = (primary, secondary, close) switch
            {
                (true, true, true) => "AllVisible",
                (false, false, false) => "NoneVisible",
                (true, false, false) => "PrimaryVisible",
                (false, true, false) => "SecondaryVisible",
                (false, false, true) => "CloseVisible",
                (true, true, false) => "PrimaryAndSecondaryVisible",
                (true, false, true) => "PrimaryAndCloseVisible",
                _ => "SecondaryAndCloseVisible"
            };

            VisualStateManager.GoToState(this, state, useTransitions);
        }
    }
}
