// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Caliburn.Micro;
using MahApps.Metro.Controls;

//using MetroDemo.Models;

namespace Caliburn.Metro.Demo.ViewModels.Flyouts
{
    public class Flyout3ViewModel : FlyoutBaseViewModel
    {
        private readonly IObservableCollection<object> artists =
            new BindableCollection<object>();

        public IObservableCollection<object> Artists
        {
            get { return this.artists; }
        }

        public Flyout3ViewModel()
        {
            //SampleData.Seed();
            //this.Artists.AddRange(SampleData.Artists);
            this.Header = "third";
            this.Position = Position.Right;
        }
    }
}