using MahApps.Metro;
using MetroDemo.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MetroDemo.Models
{
    public class CustomAccentCreator: ViewModelBase, IDataErrorInfo
    {
        private string _AccentColorName;
        private string _HighLightColorName;
        private string _AccentName;

        public CustomAccentCreator(Action<CustomAccentCreator> closeHandler)
        {
            this.CloseCommand = new SimpleCommand(o => true, o => closeHandler(this));
        }

        public string AccentName
        {
            get => this._AccentName;
            set
            {
                this._AccentName = value;
                this.OnPropertyChanged();
            }
        }

        public string AccentColorName
        {
            get => this._AccentColorName;
            set
            {
                this._AccentColorName = value;
                this.OnPropertyChanged();
            }
        }


        public string HighlightColorName
        {
            get => this._HighLightColorName;
            set
            {
                this._HighLightColorName = value;
                this.OnPropertyChanged();
            }
        }

        public void CreateTheme()
        {
            var accentColor = TryGetColor(AccentColorName) ?? Colors.Magenta;
            var highlightColor = TryGetColor(_HighLightColorName);

            var name = ThemeManager.AddUserDefinedTheme(accentColor, highlightColor, AccentName);

            var exisitingAccentMenuItem = MainWindowViewModel.CustomAccentColors.FirstOrDefault(x => x.Name == AccentName);
            var newAccentMenuItem = new AccentColorMenuData() { Name = name, ColorBrush = new SolidColorBrush(accentColor) };

            if (exisitingAccentMenuItem != null)
            {
                MainWindowViewModel.CustomAccentColors.Remove(exisitingAccentMenuItem);
            }

            MainWindowViewModel.CustomAccentColors.Add(newAccentMenuItem);
            newAccentMenuItem.ChangeAccentCommand.Execute(null);
        }

        private Color? TryGetColor (string Name)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(Name);
            }
            catch 
            {
                return null;
            }
        }

        public ICommand CloseCommand { get; }

        #region IDataErrorInfo
        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "AccentColorName":
                        try
                        {
                            ColorConverter.ConvertFromString(AccentColorName);
                            return null;
                        }
                        catch (Exception)
                        {
                            return "This is not a valid color.";
                        }
                    default:
                        return null;
                }
            }
        }
        #endregion

    }
}
