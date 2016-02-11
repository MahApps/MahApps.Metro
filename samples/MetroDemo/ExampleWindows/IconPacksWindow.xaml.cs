using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using MahApps.Metro;
using MahApps.Metro.Controls;

namespace MetroDemo.ExampleWindows
{
    /// <summary>
    /// Interaction logic for IconPacksWindow.xaml
    /// </summary>
    public partial class IconPacksWindow : MetroWindow
    {
        public IconPacksWindow()
        {
            DataContext = new IconPacksViewModel();
            InitializeComponent();
            this.Loaded += (sender, args) => ((IconPacksViewModel)this.DataContext).Initialize();
        }

        private void HyperlinkMaterialOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://materialdesignicons.com/");
        }

        private void HyperlinkModernOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://modernuiicons.com/");
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(textBox.SelectAll));
        }
    }

    public class IconPacksViewModel : INotifyPropertyChanged
    {
        public IconPacksViewModel()
        {
        }

        public void Initialize()
        {
            ModernKinds = new Lazy<IEnumerable<PackIconModernKind>>(
                () => Enum.GetValues(typeof(PackIconModernKind)).OfType<PackIconModernKind>()
                          .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList()
                ).Value;

            MaterialKinds = new Lazy<IEnumerable<PackIconMaterialKind>>(
                () => Enum.GetValues(typeof(PackIconMaterialKind)).OfType<PackIconMaterialKind>()
                          .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList()
                ).Value;
        }

        private IEnumerable<PackIconModernKind> _modernKinds;

        public IEnumerable<PackIconModernKind> ModernKinds
        {
            get { return _modernKinds; }
            set
            {
                if (Equals(value, _modernKinds)) return;
                _modernKinds = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<PackIconMaterialKind> _materialKinds;

        public IEnumerable<PackIconMaterialKind> MaterialKinds
        {
            get { return _materialKinds; }
            set
            {
                if (Equals(value, _materialKinds)) return;
                _materialKinds = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}