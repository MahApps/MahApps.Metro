using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(MessageBoxButton))]
    [TemplatePart(Name = "PART_NoButton", Type = typeof(MessageBoxButton))]
    [TemplatePart(Name = "PART_YesButton", Type = typeof(MessageBoxButton))]
    [TemplatePart(Name = "PART_OkButton", Type = typeof(MessageBoxButton))]
    [TemplatePart(Name = "PART_CancelButton", Type = typeof(MessageBoxButton))]
    public class MessageBox : Window
    {
        private MessageBox()
        {
            DefaultStyleKey = typeof(MessageBox);
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PreviewKeyDown += MessageBox_PreviewKeyDown;
            SizeToContent = SizeToContent.WidthAndHeight;
        }

        public override void OnApplyTemplate()
        {
            var templateChild = GetTemplateChild("PART_CloseButton");
            if (templateChild is Button)
            {
                _closeButton = templateChild as Button;
                _closeButton.Click += CloseButton_Click;
            }

            templateChild = GetTemplateChild("PART_NoButton");
            if (templateChild is Button)
            {
                _noButton = templateChild as Button;
                _noButton.Content = NoButtonContent;
                _noButton.Click += NoButton_Click;
            }

            templateChild = GetTemplateChild("PART_OkButton");
            if (templateChild is Button)
            {
                _okButton = templateChild as Button;
                _okButton.Content = OkButtonContent;
                _okButton.Click += OkButton_Click;
            }

            templateChild = GetTemplateChild("PART_YesButton");
            if (templateChild is Button)
            {
                _yesButton = templateChild as Button;
                _yesButton.Content = YesButtonContent;
                _yesButton.Click += YesButton_Click;
            }

            templateChild = GetTemplateChild("PART_CancelButton");
            if (templateChild is Button)
            {
                _cancelButton = templateChild as Button;
                _cancelButton.Content = CancelButtonContent;
                _cancelButton.Click += CancelButton_Click;
            }
        }

        #region Fields

        private MessageBoxResult _result = MessageBoxResult.Cancel;
        private Button _closeButton;
        private Button _okButton;
        private Button _yesButton;
        private Button _noButton;
        private Button _cancelButton;

        #endregion

        #region DependencyProperties
        public static readonly DependencyProperty CloseButtonStyleProperty = DependencyProperty.Register("CloseButtonStyle", typeof(Style), typeof(MessageBox));
        public Style CloseButtonStyle
        {
            get { return (Style)GetValue(CloseButtonStyleProperty); }
            set { SetValue(CloseButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty OkButtonStyleProperty = DependencyProperty.Register("OkButtonStyle", typeof(Style), typeof(MessageBox));
        public Style OkButtonStyle
        {
            get { return (Style)GetValue(OkButtonStyleProperty); }
            set { SetValue(OkButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty YesButtonStyleProperty = DependencyProperty.Register("YesButtonStyle", typeof(Style), typeof(MessageBox));
        public Style YesButtonStyle
        {
            get { return (Style)GetValue(YesButtonStyleProperty); }
            set { SetValue(YesButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty NoButtonStyleProperty = DependencyProperty.Register("NoButtonStyle", typeof(Style), typeof(MessageBox));
        public Style NoButtonStyle
        {
            get { return (Style)GetValue(NoButtonStyleProperty); }
            set { SetValue(NoButtonStyleProperty, value); }
        }


        public static readonly DependencyProperty CancelButtonStyleProperty = DependencyProperty.Register("CancelButtonStyle", typeof(Style), typeof(MessageBox));
        public Style CancelButtonStyle
        {
            get { return (Style)GetValue(CancelButtonStyleProperty); }
            set { SetValue(CancelButtonStyleProperty, value); }
        }

        public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register("WindowTitleBrush", typeof(Brush), typeof(MessageBox));
        public Brush WindowTitleBrush
        {
            get { return (Brush)GetValue(WindowTitleBrushProperty); }
            set { SetValue(WindowTitleBrushProperty, value); }
        }

        public static readonly DependencyProperty WindowContentBrushProperty = DependencyProperty.Register("WindowContentBrush", typeof(Brush), typeof(MessageBox));
        public Brush WindowContentBrush
        {
            get { return (Brush)GetValue(WindowContentBrushProperty); }
            set { SetValue(WindowContentBrushProperty, value); }
        }

        public static readonly DependencyProperty MessageBoxButtonBackgroundProperty = DependencyProperty.Register("MessageBoxButtonBackground", typeof(Brush), typeof(MessageBox));
        public Brush MessageBoxButtonBackground
        {
            get { return (Brush)GetValue(MessageBoxButtonBackgroundProperty); }
            set { SetValue(MessageBoxButtonBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MessageBoxButtonProperty = DependencyProperty.Register("MessageBoxButton", typeof(MessageBoxButton), typeof(MessageBox), new PropertyMetadata(MessageBoxButton.OK));
        public MessageBoxButton MessageBoxButton
        {
            get { return (MessageBoxButton)GetValue(MessageBoxButtonProperty); }
            set { SetValue(MessageBoxButtonProperty, value); }
        }

        public static readonly DependencyProperty OkButtonContentProperty = DependencyProperty.Register("OkButtonContent", typeof(object), typeof(MessageBox), new PropertyMetadata("Ok"));
        public object OkButtonContent
        {
            get { return GetValue(OkButtonContentProperty); }
            set { SetValue(OkButtonContentProperty, value); }
        }

        public static readonly DependencyProperty YesButtonContentProperty = DependencyProperty.Register("YesButtonContent", typeof(object), typeof(MessageBox), new PropertyMetadata("Yes"));
        public object YesButtonContent
        {
            get { return GetValue(YesButtonContentProperty); }
            set { SetValue(YesButtonContentProperty, value); }
        }

        public static readonly DependencyProperty NoButtonContentProperty = DependencyProperty.Register("NoButtonContent", typeof(object), typeof(MessageBox), new PropertyMetadata("No"));
        public object NoButtonContent
        {
            get { return GetValue(NoButtonContentProperty); }
            set { SetValue(NoButtonContentProperty, value); }
        }

        public static readonly DependencyProperty CancelButtonContentProperty = DependencyProperty.Register("CancelButtonContent", typeof(object), typeof(MessageBox), new PropertyMetadata("Cancel"));
        public object CancelButtonContent
        {
            get { return GetValue(CancelButtonContentProperty); }
            set { SetValue(CancelButtonContentProperty, value); }
        }
        #endregion

        #region Public Methods
        private MessageBoxResult ShowMessageBox(Window owner, string content, string caption, MessageBoxButton boxButton)
        {
            Content = content;
            Title = caption;
            if (owner != null)
                Owner = owner;
            else
            {
                var activeWindow = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                if (activeWindow != null)
                    Owner = activeWindow;
            }
            MessageBoxButton = boxButton;
            ShowDialog();
            return _result;
        }

        public static MessageBoxResult Show(Window owner, string content, string caption, MessageBoxButton boxButton)
        {
            var messageBoa = new MessageBox();
            return messageBoa.ShowMessageBox(owner, content, caption, boxButton);
        }

        public static MessageBoxResult Show(string content, string caption, MessageBoxButton boxButton)
        {
            return Show(null, content, caption, boxButton);
        }

        public static MessageBoxResult Show(Window owner, string content, string caption)
        {
            return Show(owner, content, caption, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string content, string caption)
        {
            return Show(content, caption, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string content)
        {
            return Show(owner, content, "");
        }

        public static MessageBoxResult Show(string content)
        {
            return Show(owner: null, content: content);
        }

        #endregion

        #region Private Methods

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            switch (MessageBoxButton)
            {
                case MessageBoxButton.OK:
                    _result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    _result = MessageBoxResult.Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    _result = MessageBoxResult.No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    _result = MessageBoxResult.Cancel;
                    break;
                default:
                    _result = MessageBoxResult.None;
                    break;
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Cancel;
            Close();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.Yes;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.OK;
            Close();
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _result = MessageBoxResult.No;
            Close();
        }



        private void MessageBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                CancelButton_Click(sender, new RoutedEventArgs());
        }
        #endregion
    }
}
