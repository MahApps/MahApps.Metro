// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Tests.Views
{
    /// <summary>
    /// Interaction logic for MultiSelectorHelperTestWindow.xaml
    /// </summary>
    public partial class MultiSelectorHelperTestWindow : MetroWindow
    {
        public static readonly DependencyProperty ItemsProperty
            = DependencyProperty.Register(
                nameof(Items),
                typeof(ObservableCollection<string>),
                typeof(MultiSelectorHelperTestWindow),
                new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> Items
        {
            get => (ObservableCollection<string>)this.GetValue(ItemsProperty);
            set => this.SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty SecondItemsProperty
            = DependencyProperty.Register(
                nameof(SecondItems),
                typeof(ObservableCollection<string>),
                typeof(MultiSelectorHelperTestWindow),
                new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> SecondItems
        {
            get => (ObservableCollection<string>)this.GetValue(SecondItemsProperty);
            set => this.SetValue(SecondItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsProperty
            = DependencyProperty.Register(
                nameof(SelectedItems),
                typeof(ObservableCollection<string>),
                typeof(MultiSelectorHelperTestWindow),
                new PropertyMetadata(default(ObservableCollection<string>)));

        public ObservableCollection<string> SelectedItems
        {
            get => (ObservableCollection<string>)this.GetValue(SelectedItemsProperty);
            set => this.SetValue(SelectedItemsProperty, value);
        }

        public MultiSelectorHelperTestWindow()
        {
            this.InitializeComponent();

            this.Items = new ObservableCollection<string>(new[] { "Item1", "Item2", "Item3", "Item4", "Item5" });
            this.SecondItems = new ObservableCollection<string>(this.Items);
            this.SelectedItems = new ObservableCollection<string>();
        }
    }
}