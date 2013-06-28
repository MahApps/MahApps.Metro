using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace MahApps.Metro.Controls
{
    [TemplateVisualState(Name = VisualStates.NoneButton, GroupName = VisualStates.PopupButtonsGroup)]
    [TemplateVisualState(Name = VisualStates.OK, GroupName = VisualStates.PopupButtonsGroup)]
    [TemplateVisualState(Name = VisualStates.OKCancel, GroupName = VisualStates.PopupButtonsGroup)]
    [TemplateVisualState(Name = VisualStates.YesNo, GroupName = VisualStates.PopupButtonsGroup)]
    [TemplateVisualState(Name = VisualStates.YesNoCancel, GroupName = VisualStates.PopupButtonsGroup)]

    [TemplateVisualState(Name = VisualStates.NoneImage, GroupName = VisualStates.PopupImagesGroup)]
    [TemplateVisualState(Name = VisualStates.Error, GroupName = VisualStates.PopupImagesGroup)]
    [TemplateVisualState(Name = VisualStates.Question, GroupName = VisualStates.PopupImagesGroup)]
    [TemplateVisualState(Name = VisualStates.Warning, GroupName = VisualStates.PopupImagesGroup)]
    [TemplateVisualState(Name = VisualStates.Information, GroupName = VisualStates.PopupImagesGroup)]

    [TemplatePart(Name = PART_CancelButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_NoButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_OkButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_YesButton, Type = typeof(Button))]
    public class MetroPopup : HeaderedContentControl
    {
        private const string PART_CancelButton = "PART_CancelButton";

        private const string PART_NoButton = "PART_NoButton";

        private const string PART_OkButton = "PART_OkButton";

        private const string PART_YesButton = "PART_YesButton";

        #region Private Members
        /// <summary>
        /// Tracks the PopupResult to set as the default and focused button
        /// </summary>
        private PopupResult _defaultResult = PopupResult.None;

        private bool? _result;

        private Button _okButton;

        private Button _yesButton;

        private Button _noButton;

        private Button _cancelButton;


        private EventHandler _canExecuteChangedHandler;
        #endregion //Private Members

        #region Constructors
        static MetroPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroPopup), new FrameworkPropertyMetadata(typeof(MetroPopup)));
        }

        public MetroPopup()
        {
            DefaultStyleKey = typeof(MetroPopup);

            IsOpen = false;
            InitHandlers();
        }
        #endregion //Constructors

        #region Events
        public event CancelEventHandler Closing;

        public delegate void PopupClosedEventHandler(object sender, PopupClosedEventArgs eventArgs);

        public event PopupClosedEventHandler Closed;
        #endregion // Events

        #region Properties

        #region Dependency Properties

        #region OkButtonContent
        public static readonly DependencyProperty OkButtonContentProperty =
            DependencyProperty.Register("OkButtonContent", typeof(object), typeof(MetroPopup), new UIPropertyMetadata(SystemStrings.OK));

        public object OkButtonContent
        {
            get { return GetValue(OkButtonContentProperty); }
            set { SetValue(OkButtonContentProperty, value); }
        }
        #endregion //OkButtonContent

        #region YesButtonContent
        public static readonly DependencyProperty YesButtonContentProperty =
            DependencyProperty.Register("YesButtonContent", typeof(object), typeof(MetroPopup), new UIPropertyMetadata(SystemStrings.Yes));

        public object YesButtonContent
        {
            get { return GetValue(YesButtonContentProperty); }
            set { SetValue(YesButtonContentProperty, value); }
        }
        #endregion //YesButtonContent

        #region NoButtonContent
        public static readonly DependencyProperty NoButtonContentProperty =
            DependencyProperty.Register("NoButtonContent", typeof(object), typeof(MetroPopup), new UIPropertyMetadata(SystemStrings.No));

        public object NoButtonContent
        {
            get { return GetValue(NoButtonContentProperty); }
            set { SetValue(NoButtonContentProperty, value); }
        }
        #endregion //NoButtonContent

        #region CancelButtonContent
        public static readonly DependencyProperty CancelButtonContentProperty =
            DependencyProperty.Register("CancelButtonContent", typeof(object), typeof(MetroPopup), new UIPropertyMetadata(SystemStrings.Cancel));

        public object CancelButtonContent
        {
            get { return GetValue(CancelButtonContentProperty); }
            set { SetValue(CancelButtonContentProperty, value); }
        }
        #endregion //CancelButtonContent


        #region Commands

        #region OKCommand
        public static readonly DependencyProperty OKCommandProperty = DependencyProperty.Register(
            "OKCommand", typeof(ICommand), typeof(MetroPopup), new PropertyMetadata(default(ICommand), CommandChanged));

        public ICommand OKCommand
        {
            get { return (ICommand)GetValue(OKCommandProperty); }
            set { SetValue(OKCommandProperty, value); }
        }
        #endregion // OKCommand

        #region YesCommand
        public static readonly DependencyProperty YesCommandProperty = DependencyProperty.Register(
            "YesCommand", typeof(ICommand), typeof(MetroPopup), new PropertyMetadata(default(ICommand), CommandChanged));

        public ICommand YesCommand
        {
            get { return (ICommand)GetValue(YesCommandProperty); }
            set { SetValue(YesCommandProperty, value); }
        }
        #endregion // YesCommand

        #region NoCommand
        public static readonly DependencyProperty NoCommandProperty = DependencyProperty.Register(
            "NoCommand", typeof(ICommand), typeof(MetroPopup), new PropertyMetadata(default(ICommand), CommandChanged));

        public ICommand NoCommand
        {
            get { return (ICommand)GetValue(NoCommandProperty); }
            set { SetValue(NoCommandProperty, value); }
        }
        #endregion // NoCommand

        #region CancelCommand
        public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
            "CancelCommand", typeof(ICommand), typeof(MetroPopup), new PropertyMetadata(default(ICommand), CommandChanged));

        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }
        #endregion // CancelCommand


        #region OKCommandParameter
        public static readonly DependencyProperty OKCommandParameterProperty = DependencyProperty.Register(
            "OKCommandParameter", typeof(object), typeof(MetroPopup), new PropertyMetadata(default(object)));

        public object OKCommandParameter
        {
            get { return GetValue(OKCommandParameterProperty); }
            set { SetValue(OKCommandParameterProperty, value); }
        }
        #endregion // OKCommandParameter

        #region YesCommandParameter
        public static readonly DependencyProperty YesCommandParameterProperty = DependencyProperty.Register(
            "YesCommandParameter", typeof(object), typeof(MetroPopup), new PropertyMetadata(default(object)));

        public object YesCommandParameter
        {
            get { return GetValue(YesCommandParameterProperty); }
            set { SetValue(YesCommandParameterProperty, value); }
        }
        #endregion // YesCommandParameter

        #region NoCommandParameter
        public static readonly DependencyProperty NoCommandParameterProperty = DependencyProperty.Register(
            "NoCommandParameter", typeof(object), typeof(MetroPopup), new PropertyMetadata(default(object)));

        public object NoCommandParameter
        {
            get { return GetValue(NoCommandParameterProperty); }
            set { SetValue(NoCommandParameterProperty, value); }
        }
        #endregion // NoCommandParameter

        #region CancelCommandParameter
        public static readonly DependencyProperty CancelCommandParameterProperty = DependencyProperty.Register(
            "CancelCommandParameter", typeof(object), typeof(MetroPopup), new PropertyMetadata(default(object)));

        public object CancelCommandParameter
        {
            get { return GetValue(CancelCommandParameterProperty); }
            set { SetValue(CancelCommandParameterProperty, value); }
        }
        #endregion // CancelCommandParameter

        #endregion // Commands


        #region PopupImage
        public static readonly DependencyProperty PopupImageProperty =
            DependencyProperty.Register("PopupImage", typeof(PopupImage), typeof(MetroPopup), new PropertyMetadata(PopupImage.None, PropertyChangedApplyTemplate));

        public PopupImage PopupImage
        {
            get { return (PopupImage)GetValue(PopupImageProperty); }
            set { SetValue(PopupImageProperty, value); }
        }
        #endregion // PopupImage

        #region PopupButton
        public static readonly DependencyProperty PopupButtonProperty =
            DependencyProperty.Register("PopupButton", typeof(PopupButton), typeof(MetroPopup), new PropertyMetadata(PopupButton.OK, PropertyChangedApplyTemplate));

        private static void PropertyChangedApplyTemplate(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var messageBox = dependencyObject as MetroPopup;

            if (messageBox == null)
                return;

            messageBox.OnApplyTemplate();
        }

        public PopupButton PopupButton
        {
            get { return (PopupButton)GetValue(PopupButtonProperty); }
            set { SetValue(PopupButtonProperty, value); }
        }
        #endregion //PopupButton

        #region PopupResult
        /// <summary>
        /// Gets the MessageBox result, which is set when the "Closed" event is raised.
        /// </summary>
        public PopupResult PopupResult { get; private set; }
        #endregion //PopupResult


        #region IsOpen
        public static readonly DependencyProperty IsOpenProperty =
                DependencyProperty.Register(
                        "IsOpen",
                        typeof(bool),
                        typeof(MetroPopup),
                        new FrameworkPropertyMetadata(false,
                                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault) { CoerceValueCallback = CoerceIsOpen });

        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            var metroPopup = d as MetroPopup;
            var value = (bool)basevalue;

            if (metroPopup != null)
                metroPopup.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

            return basevalue;
        }

        /// <summary>
        /// Indicates whether the Popup is visible.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        #endregion // IsOpen

        #region ButtonOrientation
        public static readonly DependencyProperty ButtonOrientationProperty =
            DependencyProperty.Register("ButtonOrientation", typeof(Orientation), typeof(MetroPopup), new PropertyMetadata(Orientation.Vertical));

        public Orientation ButtonOrientation
        {
            get { return (Orientation)GetValue(ButtonOrientationProperty); }
            set { SetValue(ButtonOrientationProperty, value); }
        }
        #endregion // ButtonOrientation

        #endregion //Dependency Properties

        #endregion //Properties

        #region Methods

        #region Public Static
        /// <summary>
        /// Displays a message box that has a message and that returns a result.
        /// </summary>
        /// <param name="messageText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A System.Windows.PopupButton value that specifies which button or buttons to display.</param>
        /// <param name="icon"> A System.Windows.PopupImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A System.Windows.PopupResult value that specifies the default result of the MessageBox.</param>
        /// <param name="messageBoxStyle">A Style that will be applied to the MessageBox instance.</param>
        /// <returns>A System.Windows.PopupResult value that specifies which message box button is clicked by the user.</returns>
        public static PopupResult Show(string messageText,
                                       string caption = "",
                                       PopupButton button = PopupButton.OK,
                                       PopupImage icon = PopupImage.None,
                                       PopupResult defaultResult = PopupResult.None,
                                       Style messageBoxStyle = null)
        {
            return ShowCore(messageText, caption, button, icon, defaultResult, messageBoxStyle);
        }

        public static PopupResult Show(Window owner,
                                       string messageText,
                                       string caption = "",
                                       PopupButton button = PopupButton.OK,
                                       PopupImage icon = PopupImage.None,
                                       PopupResult defaultResult = PopupResult.None,
                                       Style messageBoxStyle = null)
        {
            return ShowCore(messageText, caption, button, icon, defaultResult, messageBoxStyle);
        }
        #endregion //Public Static

        #region Public Methods
        /// <summary>
        /// Display the MessageBox window and returns only when this MessageBox closes.
        /// </summary>
        public bool? ShowDialog()
        {
            IsOpen = true;
            while (IsOpen)
            {
                // HACK: Stop the thread if the application is about to close
                if (Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
                    break;

                // HACK: Simulate "DoEvents"
                Dispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }

            return _result;
        }
        #endregion

        #region Protected
        /// <summary>
        /// Initializes the MessageBox.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="image">The image.</param>
        /// <param name="defaultResult">The default result.</param>
        protected void InitializeMessageBox(string text, string caption, PopupButton button, PopupImage image, PopupResult defaultResult)
        {
            Header = caption;
            Content = text;
            PopupButton = button;
            PopupImage = image;
            _defaultResult = defaultResult;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _okButton = GetTemplateChild(PART_OkButton) as Button;
            _yesButton = GetTemplateChild(PART_YesButton) as Button;
            _noButton = GetTemplateChild(PART_NoButton) as Button;
            _cancelButton = GetTemplateChild(PART_CancelButton) as Button;

            ChangeVisualState(PopupButton == PopupButton.None ? VisualStates.NoneButton : PopupButton.ToString(), true);
            ChangeVisualState(PopupImage == PopupImage.None ? VisualStates.NoneImage : PopupImage.ToString(), true);

            SetDefaultResult();
        }

        /// <summary>
        /// Changes the control's visual state(s).
        /// </summary>
        /// <param name="name">name of the state</param>
        /// <param name="useTransitions">True if state transitions should be used.</param>
        protected void ChangeVisualState(string name, bool useTransitions)
        {
            VisualStateManager.GoToState(this, name, useTransitions);
        }

        #endregion //Protected

        #region Private
        /// <summary>
        /// Sets the button that represents the _defaultResult to the default button and gives it focus.
        /// </summary>
        private void SetDefaultResult()
        {
            var defaultButton = GetDefaultButtonFromDefaultResult();
            if (defaultButton != null)
            {
                defaultButton.IsDefault = true;
                defaultButton.Focus();
            }
        }


        /// <summary>
        /// Gets the default button from the _defaultResult.
        /// </summary>
        /// <returns>The default button that represents the defaultResult</returns>
        private Button GetDefaultButtonFromDefaultResult()
        {
            switch (_defaultResult)
            {
                case PopupResult.OK:
                    return _okButton;
                case PopupResult.Yes:
                    return _yesButton;
                case PopupResult.No:
                    return _noButton;
                case PopupResult.Cancel:
                    return _cancelButton;
                case PopupResult.None:
                    return GetDefaultButton();
            }

            return null;
        }

        /// <summary>
        /// Gets the default button.
        /// </summary>
        /// <remarks>Used when the _defaultResult is set to None</remarks>
        /// <returns>The button to use as the default</returns>
        private Button GetDefaultButton()
        {
            switch (PopupButton)
            {
                case PopupButton.OK:
                case PopupButton.OKCancel:
                    return _okButton;
                case PopupButton.YesNo:
                case PopupButton.YesNoCancel:
                    return _yesButton;
            }

            return null;
        }

        /// <summary>
        /// Shows the MessageBox.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="button">The button.</param>
        /// <param name="icon">The icon.</param>
        /// <param name="defaultResult">The default result.</param>
        /// <param name="messageBoxStyle">The style to be set.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Static methods for MessageBoxes are not available in XBAP.
        /// Use the instance ShowMessageBox methods instead.</exception>
        private static PopupResult ShowCore(string messageText, string caption,
                                            PopupButton button, PopupImage icon, PopupResult defaultResult,
                                            Style messageBoxStyle)
        {
            if (System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted)
                throw new InvalidOperationException("Static methods for MessageBoxes are not available in XBAP. Use the instance ShowMessageBox methods instead.");

            var messageBox = new MetroPopup();
            messageBox.InitializeMessageBox(messageText, caption, button, icon, defaultResult);

            // Setting the style to null will inhibit any implicit styles      
            if (messageBoxStyle != null)
                messageBox.Style = messageBoxStyle;

            if (Application.Current.MainWindow.Content as Visual == null)
                return PopupResult.None;

            var layer = AdornerLayer.GetAdornerLayer(Application.Current.MainWindow.Content as Visual);
            var contentAd = new ControlAdorner(Application.Current.MainWindow.Content as UIElement) { Child = messageBox };

            layer.Add(contentAd);

            // Disable Closing of window while dialog is shown
            Application.Current.MainWindow.Closing += MainWindow_Closing;

            messageBox.ShowDialog();

            Application.Current.MainWindow.Closing -= MainWindow_Closing;

            layer.Remove(contentAd);

            return messageBox.PopupResult;
        }

        /// <summary>
        /// Closes the MessageBox.
        /// </summary>
        private void Close()
        {
            var eventArgs = new CancelEventArgs();

            if (Closing != null)
                Closing(this, eventArgs);

            if (eventArgs.Cancel)
                return;

            IsOpen = false;

            if (Closed != null)
                Closed(this, new PopupClosedEventArgs(PopupResult));
        }

        /// <summary>
        /// Add Handler to containing buttons
        /// </summary>
        private void InitHandlers()
        {
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Button_Click));
        }

        /// <summary>
        /// Command dependency property change callback. 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var messageBox = d as MetroPopup;

            if (messageBox == null)
                return;

            messageBox.HookUpCommand((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        /// <summary>
        /// Add a new command to the Command Property.
        /// </summary>
        /// <param name="oldCommand">Old command to be removed.</param>
        /// <param name="newCommand">New command to be added.</param>
        private void HookUpCommand(ICommand oldCommand, ICommand newCommand)
        {
            // If oldCommand is not null, then we need to remove the handlers. 
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= CanExecuteChanged;
            }

            _canExecuteChangedHandler = CanExecuteChanged;

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += _canExecuteChangedHandler;
            }
        }

        private void CanExecuteChanged(object sender, EventArgs e)
        {
            if (sender == OKCommand)
            {
                if (_okButton != null)
                    _okButton.IsEnabled = OKCommand.CanExecute(OKCommandParameter);
            }
            else if (sender == YesCommand)
            {
                if (_yesButton != null)
                    _yesButton.IsEnabled = YesCommand.CanExecute(YesCommandParameter);
            }
            else if (sender == NoCommand)
            {
                if (_noButton != null)
                    _noButton.IsEnabled = NoCommand.CanExecute(NoCommandParameter);
            }
            else if (sender == CancelCommand)
            {
                if (_cancelButton != null)
                    _cancelButton.IsEnabled = CancelCommand.CanExecute(CancelCommandParameter);
            }
        }
        #endregion //Private

        #endregion //Methods

        #region Handler
        /// <summary>
        /// Sets the PopupResult according to the button pressed and then closes the MessageBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;

            if (button == null)
                return;

            _result = null;

            switch (button.Name)
            {
                case PART_OkButton:
                    _result = true;
                    PopupResult = PopupResult.OK;

                    if (OKCommand != null)
                        OKCommand.Execute(OKCommandParameter);
                    break;
                case PART_YesButton:
                    _result = true;
                    PopupResult = PopupResult.Yes;

                    if (YesCommand != null)
                        YesCommand.Execute(YesCommandParameter);
                    break;
                case PART_NoButton:
                    _result = false;
                    PopupResult = PopupResult.No;

                    if (NoCommand != null)
                        NoCommand.Execute(NoCommandParameter);
                    break;
                case PART_CancelButton:
                    PopupResult = PopupResult.Cancel;

                    if (CancelCommand != null)
                        CancelCommand.Execute(CancelCommandParameter);
                    break;
                default:
                    return;
            }

            e.Handled = true;
            Close();
        }

        private static void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
        }
        #endregion //Handler
    }

    internal class ControlAdorner : Adorner
    {
        private Control _child;

        public ControlAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0) throw new ArgumentOutOfRangeException();
            return _child;
        }

        public Control Child
        {
            get { return _child; }
            set
            {
                if (_child != null)
                {
                    RemoveVisualChild(_child);
                }
                _child = value;
                if (_child != null)
                {
                    AddVisualChild(_child);
                }
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            //return _child.DesiredSize;
            return AdornedElement.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(new Point(0, 0), finalSize));
            //return new Size(_child.ActualWidth, _child.ActualHeight);

            return AdornedElement.RenderSize;
        }
    }

    public enum PopupImage
    {
        None = 0,
        Error = 16,
        Question = 32,
        Warning = 48,
        Information = 64
    }

    public enum PopupButton
    {
        None = 0,
        OK = 1,
        OKCancel = 2,
        YesNoCancel = 3,
        YesNo = 4,
    }

    public enum PopupResult
    {
        None = 0,
        OK = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public class PopupClosedEventArgs : EventArgs
    {
        public PopupResult PopupResult { get; set; }

        public PopupClosedEventArgs(PopupResult popupResult)
        {
            PopupResult = popupResult;
        }
    }

    internal static class SystemStrings
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int LoadString(IntPtr hInstance, uint uID, StringBuilder lpBuffer, int nBufferMax);
        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string lpFileName);

        private const uint OK_CAPTION = 800;
        private const uint CANCEL_CAPTION = 801;
        private const uint ABORT_CAPTION = 802;
        private const uint RETRY_CAPTION = 803;
        private const uint IGNORE_CAPTION = 804;
        private const uint YES_CAPTION = 805;
        private const uint NO_CAPTION = 806;
        private const uint CLOSE_CAPTION = 807;
        private const uint HELP_CAPTION = 808;
        private const uint TRYAGAIN_CAPTION = 809;
        private const uint CONTINUE_CAPTION = 810;

        private static readonly StringBuilder StringBuilder;

        private static readonly IntPtr User32;

        static SystemStrings()
        {
            StringBuilder = new StringBuilder(256);

            User32 = LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
        }

        public static string OK
        {
            get { return GetString(OK_CAPTION); }
        }

        public static string Cancel
        {
            get { return GetString(CANCEL_CAPTION); }
        }

        public static string Abort
        {
            get { return GetString(ABORT_CAPTION); }
        }

        public static string Retry
        {
            get { return GetString(RETRY_CAPTION); }
        }

        public static string Ignore
        {
            get { return GetString(IGNORE_CAPTION); }
        }

        public static string Yes
        {
            get { return GetString(YES_CAPTION); }
        }

        public static string No
        {
            get { return GetString(NO_CAPTION); }
        }

        public static string Close
        {
            get { return GetString(CLOSE_CAPTION); }
        }

        public static string Help
        {
            get { return GetString(HELP_CAPTION); }
        }

        public static string TryAgain
        {
            get { return GetString(TRYAGAIN_CAPTION); }
        }

        public static string Continue
        {
            get { return GetString(CONTINUE_CAPTION); }
        }

        private static string GetString(uint code)
        {
            LoadString(User32, code, StringBuilder, StringBuilder.Capacity);
            return StringBuilder.ToString().Trim('&');
        }
    }
}
