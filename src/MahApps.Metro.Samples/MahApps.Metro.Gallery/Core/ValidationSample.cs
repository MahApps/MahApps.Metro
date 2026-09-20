// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// What the validation samples are bound to. WPF draws the error adornment for a binding whose
    /// source says the value is wrong, so a page about that adornment needs something to be wrong
    /// about, and this is the smallest thing that can be.
    /// <para>
    /// It reports through INotifyDataErrorInfo, which is the one of the three ways that can hand
    /// back more than one message for the same property. IDataErrorInfo has one string per
    /// property, and a ValidationRule in the binding stops at the first rule that turns the value
    /// down.
    /// </para>
    /// </summary>
    public sealed class ValidationSample : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string age = "142";
        private string secret = "42";
        private double amount = 142d;

        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// A number between one and ninety-nine, which the one it starts at is not.
        /// </summary>
        public string Age
        {
            get => this.age;
            set => this.Set(ref this.age, value);
        }

        /// <summary>
        /// A word that has two rules to answer to and answers to neither.
        /// </summary>
        public string Secret
        {
            get => this.secret;
            set => this.Set(ref this.secret, value);
        }

        /// <summary>
        /// A number for the controls that carry no error template of their own.
        /// </summary>
        public double Amount
        {
            get => this.amount;
            set => this.Set(ref this.amount, value);
        }

        /// <inheritdoc />
        public bool HasErrors => this.ErrorsOf(nameof(this.Age)).Any()
                                 || this.ErrorsOf(nameof(this.Secret)).Any()
                                 || this.ErrorsOf(nameof(this.Amount)).Any();

        /// <inheritdoc />
        public IEnumerable GetErrors(string? propertyName)
        {
            return this.ErrorsOf(propertyName);
        }

        private IEnumerable<string> ErrorsOf(string? propertyName)
        {
            switch (propertyName)
            {
                case nameof(this.Age):
                    if (!int.TryParse(this.age, NumberStyles.Integer, CultureInfo.CurrentCulture, out var years))
                    {
                        yield return "Whole numbers only.";
                    }
                    else if (years < 1 || years > 99)
                    {
                        yield return "Between 1 and 99.";
                    }

                    break;

                // both of them at once, which is what the popup shows one under the other
                case nameof(this.Secret):
                    if (this.secret.Length < 8)
                    {
                        yield return "At least eight characters.";
                    }

                    if (!this.secret.Any(char.IsLetter))
                    {
                        yield return "At least one letter.";
                    }

                    break;

                case nameof(this.Amount):
                    if (this.amount > 100d)
                    {
                        yield return "At most 100.";
                    }

                    break;
            }
        }

        private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
