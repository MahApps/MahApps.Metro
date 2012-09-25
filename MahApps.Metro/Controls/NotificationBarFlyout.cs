using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_Header", Type = typeof(ContentPresenter))]
    public class MessageBoxBarFlyout : FlyoutBase
    {
        private static FrameworkElement _rootElement;
        private Window _Shell;
        private static AdornerLayer _myAdorner;

        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(bool), typeof(MessageBoxBarFlyout), new PropertyMetadata(default(bool)));
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(MessageBoxBarFlyout));
        //public static readonly DependencyProperty CommandsProperty = DependencyProperty.Register("Commands", typeof(ObservableCollection<CommandViewModel>), typeof(NotificationBarFlyout), new PropertyMetadata(default(ObservableCollection<CommandViewModel>), CommandsPropertyChanged));
        //public static readonly DependencyPropertyKey WrappedCommandsPropertyKey = DependencyProperty.RegisterReadOnly("WrappedCommands", typeof(ReadOnlyCollection<CommandViewModel>), typeof(NotificationBarFlyout), new PropertyMetadata(default(ReadOnlyCollection<CommandViewModel>)));
        
        //private static void CommandsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        //{
        //    NotificationBarFlyout targetObject = dependencyObject as NotificationBarFlyout;
        //    if ((targetObject.Commands == null) || (!targetObject.Commands.Any())) return;

        //    List<CommandViewModel> sourceCommands = targetObject.Commands.Select(commandViewModel => new CommandViewModel(commandViewModel.DisplayName,
        //                                                                                                                  new RelayCommand(o =>
        //                                                                                                                                       {
        //                                                                                                                                           targetObject.IsOpen = false;
        //                                                                                                                                           commandViewModel.Command.Execute(o);
        //                                                                                                                                       },
        //                                                                                                                                   o => commandViewModel.Command.CanExecute(o)))).ToList();
        //    targetObject.WrappedCommands = new ReadOnlyCollection<CommandViewModel>(sourceCommands);
        //}

        public bool IsClosable
        {
            get { return (bool)GetValue(IsClosableProperty); }
            set { SetValue(IsClosableProperty, value); }
        }

        //public ReadOnlyCollection<CommandViewModel> WrappedCommands
        //{
        //    get { return (ReadOnlyCollection<CommandViewModel>)GetValue(WrappedCommandsPropertyKey.DependencyProperty); }
        //    protected set { SetValue(WrappedCommandsPropertyKey, value); }
        //}

        //public ObservableCollection<CommandViewModel> Commands
        //{
        //    get { return (ObservableCollection<CommandViewModel>)GetValue(CommandsProperty); }
        //    set { SetValue(CommandsProperty, value); }
        //}

        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        static MessageBoxBarFlyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageBoxBarFlyout), new FrameworkPropertyMetadata(typeof(MessageBoxBarFlyout)));
        }

        public MessageBoxBarFlyout()
        {
            Loaded += NotificationBarFlyout_Loaded;
            SizeChanged += OnSizeChanged;
        }

        protected override void IsOpenedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.IsOpenedChanged(e);
            if ((bool)e.NewValue)
                _myAdorner.Visibility = Visibility.Visible;
            else
                _myAdorner.Visibility = Visibility.Hidden;
        }

        protected override void PositionChanged(DependencyPropertyChangedEventArgs e)
        {
            base.PositionChanged(e);
            ApplyAnimation((Position)e.NewValue, IsOpen);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            ApplyAnimation(Position, IsOpen);
        }

        public bool IsDesignMode
        {
            get
            {
                return DesignerProperties.GetIsInDesignMode(this);
            }
        }

        private void NotificationBarFlyout_Loaded(object sender, RoutedEventArgs e)
        {
            EnsureRootElement();
            if (!IsDesignMode)
            {
                if (_Shell == null)
                {
                    _Shell = ((Window)_rootElement.GetTopLevelControl());
                    //_Shell.SizeChanged += OnSizeChanged;
                    Binding parentWidthBinding = new Binding("Width") { Source = _Shell };
                    SetBinding(WidthProperty, parentWidthBinding);
                }

                if (_myAdorner == null)
                {
                    if (_Shell is MetroWindow)
                    {
                        Grid contentGrid = _Shell.FindChildren<Grid>().First();
                        _myAdorner = AdornerLayer.GetAdornerLayer(contentGrid);
                        _myAdorner.Visibility = Visibility.Hidden;
                        _myAdorner.Add(new Shader(contentGrid));
                    }
                }
            }
            ApplyAnimation(Position, IsOpen);
        }

        private void ApplyAnimation(Position position, bool isOpen)
        {
            var root = (Grid)GetTemplateChild("root");
            if (root == null)
                return;

            var hideFrame = (EasingDoubleKeyFrame)GetTemplateChild("hideFrame");
            var showFrame = (EasingDoubleKeyFrame)GetTemplateChild("showFrame");

            if (hideFrame == null || showFrame == null)
                return;

            showFrame.Value = 0;
            Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            if (position == Position.Right)
                HorizontalAlignment = HorizontalAlignment.Right;

            if (position == Position.Right)
            {
                hideFrame.Value = DesiredSize.Width;
                if (!isOpen) root.RenderTransform = new TranslateTransform(DesiredSize.Width, 0);
            }
            else
            {
                hideFrame.Value = -DesiredSize.Width;
                if (!isOpen) root.RenderTransform = new TranslateTransform(-DesiredSize.Width, 0);
            }
        }

        private void EnsureRootElement()
        {
            if (_rootElement != null) return;

            _rootElement = this.GetParentControlOffsetFromTop(1) as FrameworkElement;
        }
    }

    /// <summary>
    /// Viewmodel to represent a command with a display name for associated buttons.
    /// </summary>
    public class CommandViewModel
    {
        /// <summary>
        /// Creates a new CommandViewModel.
        /// </summary>
        /// <param name="displayName">The display name to show when the command is bound to a button or other suitable UI element that supports it.</param>
        /// <param name="command">The command to bind to the button or other UI element.</param>
        /// <remarks>Currently only supported or used by the ModalDialogPopup buttons.</remarks>
        public CommandViewModel(string displayName, ICommand command)
        {
            DisplayName = displayName;
            Command = command;
        }

        /// <summary>
        /// The display name to show when the command is bound to a button or other suitable UI element that supports it.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// The command to bind to the button or other UI element.
        /// </summary>
        public ICommand Command { get; set; }
    }


    /// <summary>
    /// ICommand implementation supporting the providing of commands by wrapping ACtion delegates.
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _Execute;
        readonly Predicate<object> _CanExecute;

        #endregion // Fields

        #region Constructors

        /// <summary>
        /// Creates a RelayCommand.
        /// </summary>
        /// <param name="execute">The action delegate implementing the commands behaviour.</param>
        /// <param name="canExecute">The action delegate implementing the check for the commands CanExecute behaviour.</param>
        /// <exception cref="ArgumentNullException">execute delegate is null.</exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _Execute = execute;
            _CanExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute(parameter);
        }

        /// <summary>
        /// Delegated to CommandManager RequerySuggested, to ensure that RelayCommand's check their CanExecute values if conditions change.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _Execute(parameter);
        }

        #endregion // ICommand Members
    }
}