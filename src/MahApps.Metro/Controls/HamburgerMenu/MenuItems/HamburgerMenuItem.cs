// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenuItem provides an implementation for HamburgerMenu entries.
    /// </summary>
    public class HamburgerMenuItem : HamburgerMenuItemBase, IHamburgerMenuItem, ICommandSource
    {
        /// <summary>
        /// Identifies the <see cref="Label"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty
            = DependencyProperty.Register(nameof(Label),
                                          typeof(string),
                                          typeof(HamburgerMenuItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies label to display.
        /// </summary>
        public string? Label
        {
            get => (string?)this.GetValue(LabelProperty);
            set => this.SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TargetPageType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetPageTypeProperty
            = DependencyProperty.Register(nameof(TargetPageType),
                                          typeof(Type),
                                          typeof(HamburgerMenuItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that specifies the page to navigate to (if you use the HamburgerMenu with a Frame content)
        /// </summary>
        public Type? TargetPageType
        {
            get => (Type?)this.GetValue(TargetPageTypeProperty);
            set => this.SetValue(TargetPageTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(nameof(Command),
                                          typeof(ICommand),
                                          typeof(HamburgerMenuItem),
                                          new PropertyMetadata(null, OnCommandPropertyChanged));

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HamburgerMenuItem)d).OnCommandChanged(e.OldValue as ICommand, e.NewValue as ICommand);
        }

        /// <summary>
        /// Gets or sets a command which will be executed if an item is clicked by the user.
        /// </summary>
        public ICommand? Command
        {
            get => (ICommand?)this.GetValue(CommandProperty);
            set => this.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty
            = DependencyProperty.Register(nameof(CommandParameter),
                                          typeof(object),
                                          typeof(HamburgerMenuItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter which will be passed by the Command.
        /// </summary>
        public object? CommandParameter
        {
            get => this.GetValue(CommandParameterProperty);
            set => this.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CommandTarget"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty
            = DependencyProperty.Register(nameof(CommandTarget),
                                          typeof(IInputElement),
                                          typeof(HamburgerMenuItem),
                                          new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the element on which to raise the specified command.
        /// </summary>
        /// <returns>
        /// Element on which to raise a command.
        /// </returns>
        public IInputElement? CommandTarget
        {
            get => (IInputElement?)this.GetValue(CommandTargetProperty);
            set => this.SetValue(CommandTargetProperty, value);
        }

        /// <summary>
        /// Gets whether this item can be used. A command that says it cannot run right now turns its
        /// item off, on top of whatever the menu above it already says.
        /// </summary>
        protected override bool IsEnabledCore => base.IsEnabledCore && this.canExecute;

        /// <summary>
        /// Executes the command which can be set by the user.
        /// </summary>
        public void RaiseCommand()
        {
            CommandHelpers.ExecuteCommandSource(this);
        }

        private void OnCommandChanged(ICommand? oldCommand, ICommand? newCommand)
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
            CanExecuteChangedEventManager.RemoveHandler(command, this.OnCanExecuteChanged);
            this.UpdateCanExecute();
        }

        private void HookCommand(ICommand command)
        {
            CanExecuteChangedEventManager.AddHandler(command, this.OnCanExecuteChanged);
            this.UpdateCanExecute();
        }

        private void OnCanExecuteChanged(object? sender, EventArgs e)
        {
            this.UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            this.CanExecute = this.Command is null || CommandHelpers.CanExecuteCommandSource(this);
        }

        private bool canExecute = true;

        private bool CanExecute
        {
            get => this.canExecute;
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
    }
}