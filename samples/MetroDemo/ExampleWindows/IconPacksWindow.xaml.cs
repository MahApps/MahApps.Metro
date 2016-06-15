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
using MahApps.Metro.Controls;
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
            this.InitializeComponent();
            this.DataContext = new IconPacksViewModel();
            this.Loaded += (sender, args) => ((IconPacksViewModel)this.DataContext).Initialize();
        }

        private void HyperlinkMaterialOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://materialdesignicons.com");
        }

        private void HyperlinkModernOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://modernuiicons.com");
        }

        private void HyperlinkFontAwesomeOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://fontawesome.io");
        }

        private void HyperlinkEntypoOnClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.entypo.com");
        }

        private void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.Dispatcher.BeginInvoke(new Action(textBox.SelectAll));
        }
    }

    public class IconPacksEnumModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public object Value { get; set; }
    }

    public class IconPacksViewModel : INotifyPropertyChanged
    {
        private ICollectionView materialCVS;
        private ICollectionView modernCVS;
        private ICollectionView faCVS;
        private ICollectionView entypoCVS;

        public IconPacksViewModel()
        {
            this.CopyToClipboard =
                new SimpleCommand
                {
                    CanExecuteDelegate = x => (x is TextBox),
                    ExecuteDelegate = x => ((TextBox)x).Dispatcher.BeginInvoke(new Action(() => Clipboard.SetDataObject(((TextBox)x).Text)))
                };
        }

        public void Initialize()
        {
            this.MaterialKinds = GetIconPackKinds(typeof(PackIconMaterialKind));
            this.materialCVS = CollectionViewSource.GetDefaultView(this.MaterialKinds);
            this.materialCVS.Filter = o => this.FilterKinds(this.MaterialFilterTerm, (IconPacksEnumModel)o);
            this.SelectedMaterialKind = this.MaterialKinds.First();

            this.ModernKinds = GetIconPackKinds(typeof(PackIconModernKind));
            this.modernCVS = CollectionViewSource.GetDefaultView(this.ModernKinds);
            this.modernCVS.Filter = o => this.FilterKinds(this.ModernFilterTerm, (IconPacksEnumModel)o);
            this.SelectedModernKind = this.ModernKinds.First();

            this.FontAwesomeKinds = GetIconPackKinds(typeof(PackIconFontAwesomeKind));
            this.faCVS = CollectionViewSource.GetDefaultView(this.FontAwesomeKinds);
            this.faCVS.Filter = o => this.FilterKinds(this.FontAwesomeFilterTerm, (IconPacksEnumModel)o);
            this.SelectedFontAwesomeKind = this.FontAwesomeKinds.First();

            this.EntypoKinds = GetIconPackKinds(typeof(PackIconEntypoKind));
            this.entypoCVS = CollectionViewSource.GetDefaultView(this.EntypoKinds);
            this.entypoCVS.Filter = o => this.FilterKinds(this.EntypoFilterTerm, (IconPacksEnumModel)o);
            this.SelectedEntypoKind = this.EntypoKinds.First();
        }

        private static string GetDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return attribute != null ? attribute.Description : value.ToString();
        }

        private static IEnumerable<IconPacksEnumModel> GetIconPackKinds(Type enumType)
        {
            return new ObservableCollection<IconPacksEnumModel>(
                Enum.GetValues(enumType).OfType<Enum>()
                    .Select(k => new IconPacksEnumModel() { Name = k.ToString(), Description = GetDescription(k), Value = k })
                    .OrderBy(m => m.Name, StringComparer.InvariantCultureIgnoreCase));
        }

        private bool FilterKinds(string filterTerm, IconPacksEnumModel enumModel)
        {
            return string.IsNullOrWhiteSpace(filterTerm) || enumModel.Name.IndexOf(filterTerm, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private string materialFilterTerm;

        public string MaterialFilterTerm
        {
            get { return this.materialFilterTerm; }
            set
            {
                if (Equals(value, this.materialFilterTerm))
                {
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
                if (Equals(value, this.modernFilterTerm))
                {
                    return;
                }
                this.modernFilterTerm = value;
                this.OnPropertyChanged();
                this.modernCVS.Refresh();
            }
        }

        private string fontAwesomeFilterTerm;

        public string FontAwesomeFilterTerm
        {
            get { return this.fontAwesomeFilterTerm; }
            set
            {
                if (Equals(value, this.fontAwesomeFilterTerm))
                {
                    return;
                }
                this.fontAwesomeFilterTerm = value;
                this.OnPropertyChanged();
                this.faCVS.Refresh();
            }
        }

        private string entypoFilterTerm;

        public string EntypoFilterTerm
        {
            get { return this.entypoFilterTerm; }
            set
            {
                if (Equals(value, this.entypoFilterTerm))
                {
                    return;
                }
                this.entypoFilterTerm = value;
                this.OnPropertyChanged();
                this.entypoCVS.Refresh();
            }
        }

        private IEnumerable<IconPacksEnumModel> modernKinds;

        public IEnumerable<IconPacksEnumModel> ModernKinds
        {
            get { return this.modernKinds; }
            set
            {
                if (Equals(value, this.modernKinds))
                {
                    return;
                }
                this.modernKinds = value;
                this.OnPropertyChanged();
            }
        }

        private IconPacksEnumModel selectedModernKind;

        public IconPacksEnumModel SelectedModernKind
        {
            get { return this.selectedModernKind; }
            set
            {
                if (Equals(value, this.selectedModernKind))
                {
                    return;
                }
                this.selectedModernKind = value;
                this.OnPropertyChanged();
            }
        }

        private IEnumerable<IconPacksEnumModel> materialKinds;

        public IEnumerable<IconPacksEnumModel> MaterialKinds
        {
            get { return this.materialKinds; }
            set
            {
                if (Equals(value, this.materialKinds))
                {
                    return;
                }
                this.materialKinds = value;
                this.OnPropertyChanged();
            }
        }

        private IconPacksEnumModel selectedMaterialKind;

        public IconPacksEnumModel SelectedMaterialKind
        {
            get { return this.selectedMaterialKind; }
            set
            {
                if (Equals(value, this.selectedMaterialKind))
                {
                    return;
                }
                this.selectedMaterialKind = value;
                this.OnPropertyChanged();
            }
        }

        private IEnumerable<IconPacksEnumModel> fontAwesomeKinds;

        public IEnumerable<IconPacksEnumModel> FontAwesomeKinds
        {
            get { return this.fontAwesomeKinds; }
            set
            {
                if (Equals(value, this.fontAwesomeKinds))
                {
                    return;
                }
                this.fontAwesomeKinds = value;
                this.OnPropertyChanged();
            }
        }

        private IconPacksEnumModel selectedFontAwesomeKind;

        public IconPacksEnumModel SelectedFontAwesomeKind
        {
            get { return this.selectedFontAwesomeKind; }
            set
            {
                if (Equals(value, this.selectedFontAwesomeKind))
                {
                    return;
                }
                this.selectedFontAwesomeKind = value;
                this.OnPropertyChanged();
            }
        }

        private IEnumerable<IconPacksEnumModel> entypoKinds;

        public IEnumerable<IconPacksEnumModel> EntypoKinds
        {
            get { return this.entypoKinds; }
            set
            {
                if (Equals(value, this.entypoKinds))
                {
                    return;
                }
                this.entypoKinds = value;
                this.OnPropertyChanged();
            }
        }

        private IconPacksEnumModel selectedEntypoKind;

        public IconPacksEnumModel SelectedEntypoKind
        {
            get { return this.selectedEntypoKind; }
            set
            {
                if (Equals(value, this.selectedEntypoKind))
                {
                    return;
                }
                this.selectedEntypoKind = value;
                this.OnPropertyChanged();
            }
        }

        private ICommand copyToClipboard;

        public ICommand CopyToClipboard
        {
            get { return this.copyToClipboard; }
            set
            {
                if (Equals(value, this.copyToClipboard))
                {
                    return;
                }
                this.copyToClipboard = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}