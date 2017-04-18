using System;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuItem provides an implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuItem : Freezable, ICommandSource
    {
        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(nameof(Label), typeof(string), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TargetPageType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetPageTypeProperty = DependencyProperty.Register(nameof(TargetPageType), typeof(Type), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Tag"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(nameof(Tag), typeof(object), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(HamburgerMenuItem), new PropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(HamburgerMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(HamburgerMenuItem), new PropertyMetadata(true, null, IsEnabledCoerceValueCallback));

        /// <summary>
        /// Gets or sets a value that specifies label to display.
        /// </summary>
        public string Label
        {
            get
            {
                return (string)GetValue(LabelProperty);
            }

            set
            {
                SetValue(LabelProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the page to navigate to (if you use the HamburgerMenu with a Frame content)
        /// </summary>
        public Type TargetPageType
        {
            get
            {
                return (Type)GetValue(TargetPageTypeProperty);
            }

            set
            {
                SetValue(TargetPageTypeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies an user specific value.
        /// </summary>
        public object Tag
        {
            get
            {
                return GetValue(TagProperty);
            }

            set
            {
                SetValue(TagProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a command which will be executed if an item is clicked by the user.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command parameter which will be passed by the Command.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the element on which to raise the specified command.
        /// </summary>
        /// <returns>
        /// Element on which to raise a command.
        /// </returns>
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(CommandTargetProperty);
            }

            set
            {
                this.SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is enabled in the user interface (UI). This is a dependency property.
        /// </summary>
        /// <returns>
        /// true if the item is enabled; otherwise, false. The default value is true.
        /// </returns>
        public bool IsEnabled
        {
            get
            {
                return (bool)this.GetValue(IsEnabledProperty);
            }

            set
            {
                this.SetValue(IsEnabledProperty, value);
            }
        }

        /// <summary>
        /// Executes the command which can be set by the user.
        /// </summary>
        public void RaiseCommand()
        {
            CommandHelpers.ExecuteCommandSource((ICommandSource)this);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HamburgerMenuItem)d).OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        private void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
        {
            if (oldCommand != null)
            {
                this.UnhookCommand(oldCommand);
            }
            if (newCommand != null)
            {
                this.HookCommand(newCommand);
            }
        }

        private void UnhookCommand(ICommand command)
        {
#if NET4
            var handler = CommandHelpers.GetCanExecuteChangedHandler(this);
            if (handler != null)
            {
                command.CanExecuteChanged -= handler;
                CommandHelpers.SetCanExecuteChangedHandler(this, null);
            }
#else
            CanExecuteChangedEventManager.RemoveHandler(command, new EventHandler<EventArgs>(this.OnCanExecuteChanged));
#endif
            this.UpdateCanExecute();
        }

        private void HookCommand(ICommand command)
        {
#if NET4
            EventHandler handler = new EventHandler(OnCanExecuteChanged);
            CommandHelpers.SetCanExecuteChangedHandler(this, handler);
            command.CanExecuteChanged += handler;
#else            
            CanExecuteChangedEventManager.AddHandler(command, new EventHandler<EventArgs>(this.OnCanExecuteChanged));
#endif
            this.UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            if (this.Command != null)
            {
                this.CanExecute = CommandHelpers.CanExecuteCommandSource(this);
            }
            else
            {
                this.CanExecute = true;
            }
        }

        private static object IsEnabledCoerceValueCallback(DependencyObject d, object value)
        {
            if (!(bool)value)
            {
                return false;
            }
            return ((HamburgerMenuItem)d).CanExecute;
        }

        private bool canExecute;

        private bool CanExecute
        {
            get
            {
                return this.canExecute;
            }
            set
            {
                if (value == this.canExecute)
                {
                    return;
                }
                this.canExecute = value;
                this.CoerceValue(IsEnabledProperty);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new HamburgerMenuItem();
        }
    }
}
