using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using JetBrains.Annotations;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.Models;

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
        private ICollectionView materialCVS;
        private ICollectionView modernCVS;

        public IconPacksViewModel()
        {
        }

        public void Initialize()
        {
            MaterialKinds = new ObservableCollection<PackIconMaterialKind>(
                Enum.GetValues(typeof(PackIconMaterialKind)).OfType<PackIconMaterialKind>()
                    .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList());
            materialCVS = CollectionViewSource.GetDefaultView(MaterialKinds);
            materialCVS.Filter = o => FilterMaterialKinds((PackIconMaterialKind)o);

            ModernKinds = new ObservableCollection<PackIconModernKind>(
                Enum.GetValues(typeof(PackIconModernKind)).OfType<PackIconModernKind>()
                    .OrderBy(k => k.ToString(), StringComparer.InvariantCultureIgnoreCase).ToList());
            modernCVS = CollectionViewSource.GetDefaultView(ModernKinds);
            modernCVS.Filter = o => FilterModernKindss((PackIconModernKind)o);
        }

        private bool FilterMaterialKinds(PackIconMaterialKind packIconMaterialKind)
        {
            return string.IsNullOrWhiteSpace(MaterialFilterTerm) || packIconMaterialKind.ToString().IndexOf(MaterialFilterTerm, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private bool FilterModernKindss(PackIconModernKind packIconModernKind)
        {
            return string.IsNullOrWhiteSpace(ModernFilterTerm) || packIconModernKind.ToString().IndexOf(ModernFilterTerm, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private string materialFilterTerm;

        public string MaterialFilterTerm
        {
            get { return this.materialFilterTerm; }
            set
            {
                if (Equals(value, this.MaterialFilterTerm)) {
                    return;
                }
                this.materialFilterTerm = value;
                this.OnPropertyChanged();
                this.materialCVS.Refresh();
            }
        }

        private string modernFilterTerm;

        public string ModernFilterTerm
        {
            get { return this.modernFilterTerm; }
            set
            {
                if (Equals(value, this.ModernFilterTerm)) {
                    return;
                }
                this.modernFilterTerm = value;
                this.OnPropertyChanged();
                this.modernCVS.Refresh();
            }
        }

        private IEnumerable<PackIconModernKind> _modernKinds;

        public IEnumerable<PackIconModernKind> ModernKinds
        {
            get { return _modernKinds; }
            set
            {
                if (Equals(value, _modernKinds)) {
                    return;
                }
                _modernKinds = value;
                OnPropertyChanged();
            }
        }

        private PackIconModernKind selectedModernKind;

        public PackIconModernKind SelectedModernKind
        {
            get { return this.selectedModernKind; }
            set
            {
                if (Equals(value, this.SelectedModernKind)) {
                    return;
                }
                this.selectedModernKind = value;
                this.OnPropertyChanged();
            }
        }

        private IEnumerable<PackIconMaterialKind> _materialKinds;

        public IEnumerable<PackIconMaterialKind> MaterialKinds
        {
            get { return _materialKinds; }
            set
            {
                if (Equals(value, _materialKinds)) {
                    return;
                }
                _materialKinds = value;
                OnPropertyChanged();
            }
        }

        private PackIconMaterialKind selectedMaterialKind;

        public PackIconMaterialKind SelectedMaterialKind
        {
            get { return this.selectedMaterialKind; }
            set
            {
                if (Equals(value, this.SelectedMaterialKind)) {
                    return;
                }
                this.selectedMaterialKind = value;
                this.OnPropertyChanged();
            }
        }

        private ICommand copyToClipboard;

        public ICommand CopyToClipboard
        {
            get
            {
                return this.copyToClipboard ?? (this.copyToClipboard =
                    new SimpleCommand
                    {
                        CanExecuteDelegate = x => (x is TextBox),
                        ExecuteDelegate = x => ((TextBox)x).Dispatcher.BeginInvoke(new Action(() => Clipboard.SetDataObject(((TextBox)x).Text)))
                    });
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