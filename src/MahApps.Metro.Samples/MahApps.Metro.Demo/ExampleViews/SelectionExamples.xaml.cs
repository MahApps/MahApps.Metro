﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Data;

namespace MetroDemo.ExampleViews
{
    /// <summary>
    /// Interaction logic for SelectionExamples.xaml
    /// </summary>
    public partial class SelectionExamples : UserControl
    {
        public SelectionExamples()
        {
            this.InitializeComponent();

            this.DataContextChanged += (sender, args) =>
                {
                    var vm = args.NewValue as MainWindowViewModel;
                    if (vm != null)
                    {
                        CollectionViewSource.GetDefaultView(vm.Albums).GroupDescriptions.Clear();
                        CollectionViewSource.GetDefaultView(vm.Albums).GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
                    }
                };
        }
    }
}