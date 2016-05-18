using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    internal class MetroTabItemCloseCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> executeAction;

        public MetroTabItemCloseCommand(Func<object, bool> canExecute, Action<object> executeAction)
        {
            this.canExecute = canExecute;
            this.executeAction = executeAction;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            this.executeAction?.Invoke(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

    /// <summary>
    /// An extended TabItem with a metro style.
    /// </summary>
    public class MetroTabItem : TabItem
    {
        public MetroTabItem()
        {
            DefaultStyleKey = typeof(MetroTabItem);
            this.InternalCloseTabCommand = new MetroTabItemCloseCommand(InternalCloseTabCommandCanExecute, InternalCloseTabCommandExecuteAction);
        }

        private void InternalCloseTabCommandExecuteAction(object o)
        {
            var closeTabCommand = this.CloseTabCommand;
            if (closeTabCommand != null)
            {
                var closeTabCommandParameter = this.CloseTabCommandParameter ?? this;
                if (closeTabCommand.CanExecute(closeTabCommandParameter))
                {
                    // force the command handler to run
                    closeTabCommand.Execute(closeTabCommandParameter);
                }
            }

            var owningTabControl = this.TryFindParent<BaseMetroTabControl>();
            // run the command handler for the TabControl
            // see #555
            owningTabControl?.BeginInvoke(() => owningTabControl.CloseThisTabItem(this));
        }

        private bool InternalCloseTabCommandCanExecute(object o)
        {
            var closeTabCommand = this.CloseTabCommand;
            return closeTabCommand == null || closeTabCommand.CanExecute(this.CloseTabCommandParameter ?? this);
        }

        internal Button closeButton;
        internal Thickness newButtonMargin;
        internal ContentPresenter contentSite;
        private bool closeButtonClickUnloaded;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.AdjustCloseButton();

            contentSite = GetTemplateChild("ContentSite") as ContentPresenter;
        }

        private void AdjustCloseButton()
        {
            closeButton = closeButton ?? GetTemplateChild("PART_CloseButton") as Button;
            if (closeButton != null)
            {
                closeButton.Margin = newButtonMargin;
            }
        }

        public static readonly DependencyProperty CloseButtonEnabledProperty =
            DependencyProperty.Register("CloseButtonEnabled",
                                        typeof(bool),
                                        typeof(MetroTabItem),
                                        new FrameworkPropertyMetadata(false,
                                                                      FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits,
                                                                      OnCloseButtonEnabledPropertyChangedCallback));

        private static void OnCloseButtonEnabledPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var item = dependencyObject as MetroTabItem;
            item?.AdjustCloseButton();
        }

        /// <summary>
        /// Gets/sets whether the Close Button is visible.
        /// </summary>
        public bool CloseButtonEnabled
        {
            get { return (bool)GetValue(CloseButtonEnabledProperty); }
            set { SetValue(CloseButtonEnabledProperty, value); }
        }

        internal static readonly DependencyProperty InternalCloseTabCommandProperty =
            DependencyProperty.Register("InternalCloseTabCommand",
                                        typeof(ICommand),
                                        typeof(MetroTabItem));

        /// <summary>
        /// Gets/sets the command that is executed when the Close Button is clicked.
        /// </summary>
        internal ICommand InternalCloseTabCommand
        { 
            get { return (ICommand)GetValue(InternalCloseTabCommandProperty); } 
            set { SetValue(InternalCloseTabCommandProperty, value); } 
        }

        public static readonly DependencyProperty CloseTabCommandProperty =
            DependencyProperty.Register("CloseTabCommand",
                                        typeof(ICommand),
                                        typeof(MetroTabItem));

        /// <summary>
        /// Gets/sets the command that is executed when the Close Button is clicked.
        /// </summary>
        public ICommand CloseTabCommand 
        { 
            get { return (ICommand)GetValue(CloseTabCommandProperty); } 
            set { SetValue(CloseTabCommandProperty, value); } 
        }

        public static readonly DependencyProperty CloseTabCommandParameterProperty =
            DependencyProperty.Register("CloseTabCommandParameter",
                                        typeof(object),
                                        typeof(MetroTabItem),
                                        new PropertyMetadata(null));

        /// <summary>
        /// Gets/sets the command parameter which is passed to the close button command.
        /// </summary>
        public object CloseTabCommandParameter
        {
            get { return GetValue(CloseTabCommandParameterProperty); }
            set { SetValue(CloseTabCommandParameterProperty, value); }
        }
    }
}