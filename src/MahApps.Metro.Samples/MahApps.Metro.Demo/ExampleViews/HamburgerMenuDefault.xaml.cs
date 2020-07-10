// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace MetroDemo.ExampleViews
{
    public sealed partial class HamburgerMenuDefault : UserControl
    {
        public HamburgerMenuDefault()
        {
            this.InitializeComponent();
        }

        [UsedImplicitly]
        // Another option to handle the menu item click
        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // instead using binding Content="{Binding RelativeSource={RelativeSource Self}, Mode=OneWay, Path=SelectedItem}"
            // we can do this
            this.HamburgerMenuControl.Content = e.ClickedItem;

            // close the menu if a item was selected
            if (this.HamburgerMenuControl.IsPaneOpen)
            {
                this.HamburgerMenuControl.IsPaneOpen = false;
            }
        }

        [UsedImplicitly]
        // Another option to handle the options menu item click
        private async void HamburgerMenuControl_OnOptionsItemClick(object sender, ItemClickEventArgs e)
        {
            var menuItem = e.ClickedItem as HamburgerMenuItem;
            await this.TryFindParent<MetroWindow>().ShowMessageAsync("", $"You clicked on {menuItem.Label} button");
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            this.HamburgerMenuControl.Content = e.InvokedItem;

            if (!e.IsItemOptions && this.HamburgerMenuControl.IsPaneOpen)
            {
                // close the menu if a item was selected
                this.HamburgerMenuControl.IsPaneOpen = false;
            }
        }
    }

    // This class can be used to avoid the following error message
    // System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=
    // WPF doesn’t know which FrameworkElement to use to get the DataContext, because the HamburgerMenuItem doesn’t belong to the visual or logical tree of the HamburgerMenu.
    public class BindingProxy : Freezable
    {
        // Using a DependencyProperty as the backing store for Data. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get { return (object)this.GetValue(DataProperty); }
            set { this.SetValue(DataProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }

    public static class ShowAboutCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand(nameof(Command), typeof(ShowAboutCommand));

        static ShowAboutCommand()
        {
            Application.Current.MainWindow?.CommandBindings.Add(new CommandBinding(Command, Execute, CanExecute));
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var mainViewModel = ((MainWindow)sender).DataContext as MainWindowViewModel;
            e.CanExecute = mainViewModel != null && mainViewModel.CanShowHamburgerAboutCommand;
        }

        private static async void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var menuItem = e.Parameter as HamburgerMenuItem;
            await ((MainWindow)sender).ShowMessageAsync("", $"You clicked on {menuItem?.Label} button");
        }
    }
}