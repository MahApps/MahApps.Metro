// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace MahApps.Demo.Core
{
    public class BaseClass : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        #region INotifyPropertyChanged

        // This event tells the UI to update
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string PropertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        #endregion INotifyPropertyChanged

        #region INotifyDataErrorInfo

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => this._errorsByPropertyName.Count > 0;

        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }
            else
            {
                return this._errorsByPropertyName.ContainsKey(propertyName)
                    ? this._errorsByPropertyName[propertyName]
                    : null;
            }
        }

        public bool GetHasError(string PropertyName)
        {
            return this._errorsByPropertyName.ContainsKey(PropertyName);
        }

        public void AddError(string propertyName, string error)
        {
            if (!this._errorsByPropertyName.ContainsKey(propertyName)) this._errorsByPropertyName[propertyName] = new List<string>();

            if (!this._errorsByPropertyName[propertyName].Contains(error))
            {
                this._errorsByPropertyName[propertyName].Add(error);
                this.OnErrorsChanged(propertyName);
            }
        }

        public void ClearErrors(string propertyName)
        {
            if (this._errorsByPropertyName.ContainsKey(propertyName))
            {
                this._errorsByPropertyName.Remove(propertyName);
                this.OnErrorsChanged(propertyName);
            }
        }

        public void OnErrorsChanged(string propertyName)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion INotifyDataErrorInfo
    }
}