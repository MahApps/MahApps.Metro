using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class WindowCommands : ItemsControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ShowSeparatorsProperty
            = DependencyProperty.Register("ShowSeparators",
                                          typeof(bool),
                                          typeof(WindowCommands),
                                          new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender |
                                                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                                                              FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets the value indicating whether to show the separators.
        /// </summary>
        public bool ShowSeparators
        {
            get { return (bool)this.GetValue(ShowSeparatorsProperty); }
            set { this.SetValue(ShowSeparatorsProperty, value); }
        }

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        public WindowCommands()
        {
            this.Loaded += WindowCommands_Loaded;
        }

        private void WindowCommands_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= WindowCommands_Loaded;
            var parentWindow = this.ParentWindow;
            if (null == parentWindow)
            {
                this.ParentWindow = this.TryFindParent<Window>();
            }
        }

        private Window _parentWindow;

        public Window ParentWindow
        {
            get { return _parentWindow; }
            set
            {
                if (Equals(_parentWindow, value))
                {
                    return;
                }
                _parentWindow = value;
                this.RaisePropertyChanged("ParentWindow");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
