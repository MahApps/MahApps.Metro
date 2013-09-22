using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Interaction logic for MessegeBox.xaml
    /// </summary>
    public partial class MetroMessageBox : Window
    {
        public MetroMessageBox()
        {
            InitializeComponent();
            
        }
        protected override void OnActivated(EventArgs e)
        {   
            if (Owner != null)
            {                
                if (Owner.WindowState == WindowState.Maximized)
                {
                    Left = 0;
                    Top = (System.Windows.SystemParameters.PrimaryScreenHeight-200)/2;
                    Width = System.Windows.SystemParameters.PrimaryScreenWidth;
                }
                else
                {
                    Left = Owner.Left + 1;
                    Top = Owner.Top + ((Owner.Height - 200) / 2);
                    Width = Owner.Width - 2;
                }
            }

            base.OnActivated(e);
        }

        private MessageBoxButton _Buttons = MessageBoxButton.OK;
        private MessageBoxResult _Result = MessageBoxResult.None;


        #region internal Properties
        internal MessageBoxButton Buttons
        {
            get { return _Buttons; }
            set
            {
                _Buttons = value;
                // Set all Buttons Visibility Properties
                SetButtonsVisibility();
            }
        }

        internal MessageBoxResult Result
        {
            get { return _Result; }
            set { _Result = value; }
        }
        #endregion

        #region SetButtonsVisibility Method
        internal void SetButtonsVisibility()
        {
            switch (_Buttons)
            {
                case MessageBoxButton.OK:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Visible;
                    btnYes.Visibility = Visibility.Collapsed;
                    btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    btnOk.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnOk.Visibility = Visibility.Collapsed;
                    btnCancel.Visibility = Visibility.Visible;
                    btnYes.Visibility = Visibility.Visible;
                    btnNo.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region Button Click Events
        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            this.Close();
        }
        #endregion

        #region Windows Drag Event
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }
        #endregion

        #region Deactivated Event
        private void Window_Deactivated(object sender, EventArgs e)
        {
            // If only an OK button is displayed, 
            // allow the user to just move away from this dialog box
            if (Buttons == MessageBoxButton.OK)
                this.Close();
        }
        #endregion


        public MessageBoxResult Show(string message,Window owner)
        {
            this.Owner = owner;
            return Show(message, string.Empty, MessageBoxButton.OK, owner);
        }

        public MessageBoxResult Show(string message, string caption,Window owner)
        {
            this.Owner = owner;
            return Show(message, caption, MessageBoxButton.OK, owner);
        }
        public MessageBoxResult Show(string message, MessageBoxButton buttons, Window owner)
        {
            this.Owner = owner;
            return Show(message,string.Empty, buttons, owner);
        }

        public class DarkenAdorner : Adorner
        {
            public Brush DarkenBrush { get; set; }
            public DarkenAdorner(UIElement adornedElement): base(adornedElement)
                {
                    Brush darkenBrush = new SolidColorBrush(new Color() { R = 0, G = 0, B = 0, A = 100 });
                    darkenBrush.Freeze();
                    DarkenBrush = darkenBrush;
                }

            protected override void OnRender(DrawingContext drawingContext)
            {
                drawingContext.DrawRectangle(DarkenBrush, null, new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
            }       
        }

    
        public MessageBoxResult Show(string message, string caption, MessageBoxButton buttons,Window owner)
        {

            this.Owner = owner;
            MessageBoxResult result = MessageBoxResult.None;
            UIElement rootVisual = this.Owner.Content as UIElement;
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(rootVisual);
            if ( rootVisual!=null && adornerLayer!=null)
            {
                DarkenAdorner darkenAdorner = new DarkenAdorner(rootVisual);
                adornerLayer.Add(darkenAdorner);

                title.Text = caption;
                tbMessage.Text = message;
                Buttons = buttons;
                if (buttons == MessageBoxButton.OK)
                    this.ShowDialog();
                else
                {
                    this.ShowDialog();
                    result = this.Result;
                }
                adornerLayer.Remove(darkenAdorner);
            }

            return result;
         }
    
    }
}
