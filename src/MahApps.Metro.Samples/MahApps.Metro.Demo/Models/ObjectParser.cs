using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MetroDemo.Models
{
    public class ObjectParser : IParseStringToObject
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IDialogCoordinator _dialogCoordinator;

        public ObjectParser (MainWindowViewModel mainWindowViewModel, IDialogCoordinator dialogCoordinator)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            this._dialogCoordinator = dialogCoordinator;
        }
        
        public object CreateObjectFromString(string input, CultureInfo culture, string stringFormat)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            MetroDialogSettings dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Yes",
                NegativeButtonText = "No",
                DefaultButtonFocus = MessageDialogResult.Affirmative
            };

            if (_dialogCoordinator.ShowModalMessageExternal(_mainWindowViewModel, "Add Animal", $"Do you want to add \"{input}\" to the animals list?",  MessageDialogStyle.AffirmativeAndNegative, dialogSettings) == MessageDialogResult.Affirmative)
            {
                return input.Trim();
            }
            else
            {
                return null;
            }
        }
    }
}
