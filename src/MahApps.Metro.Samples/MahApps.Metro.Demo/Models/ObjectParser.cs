// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Globalization;

namespace MetroDemo.Models
{
    public class ObjectParser : IParseStringToObject
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly IDialogCoordinator _dialogCoordinator;

        public ObjectParser(MainWindowViewModel mainWindowViewModel, IDialogCoordinator dialogCoordinator)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            this._dialogCoordinator = dialogCoordinator;
        }

        /// <inheritdoc />
        public bool TryCreateObjectFromString(string? input,
                                              out object? result,
                                              CultureInfo? culture = null,
                                              string? stringFormat = null,
                                              Type? elementType = null)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                result = null;
                return false;
            }

            MetroDialogSettings dialogSettings = new MetroDialogSettings
                                                 {
                                                     AffirmativeButtonText = "Yes",
                                                     NegativeButtonText = "No",
                                                     DefaultButtonFocus = MessageDialogResult.Affirmative
                                                 };

            if (this._dialogCoordinator.ShowModalMessageExternal(this._mainWindowViewModel, "Add Animal", $"Do you want to add \"{input}\" to the animals list?", MessageDialogStyle.AffirmativeAndNegative, dialogSettings) == MessageDialogResult.Affirmative)
            {
                result = input!.Trim();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}