// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MahApps.Metro.Tests.Views
{
    public partial class TypedUpDownWindow : TestWindow
    {
        public TypedUpDownWindow()
        {
            this.InitializeComponent();
            this.DataContext = new TypedUpDownViewModel();
        }
    }

    public class TypedUpDownViewModel : INotifyPropertyChanged
    {
        private decimal? price;

        public decimal? Price
        {
            get => this.price;
            set
            {
                if (this.price == value)
                {
                    return;
                }

                this.price = value;
                this.OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
