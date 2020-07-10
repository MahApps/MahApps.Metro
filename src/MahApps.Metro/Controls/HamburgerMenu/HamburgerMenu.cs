// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using ControlzEx;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The HamburgerMenu is based on a <see cref="SplitView"/> control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    [TemplatePart(Name = "HamburgerButton", Type = typeof(Button))]
    [TemplatePart(Name = "ButtonsListView", Type = typeof(ListBox))]
    [TemplatePart(Name = "OptionsListView", Type = typeof(ListBox))]
    public partial class HamburgerMenu : ContentControl
    {
        private Button hamburgerButton;
        private ListBox buttonsListView;
        private ListBox optionsListView;
        private readonly PropertyChangeNotifier actualWidthPropertyChangeNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="HamburgerMenu"/> class.
        /// </summary>
        public HamburgerMenu()
        {
            this.DefaultStyleKey = typeof(HamburgerMenu);

            this.actualWidthPropertyChangeNotifier = new PropertyChangeNotifier(this, ActualWidthProperty);
            this.actualWidthPropertyChangeNotifier.ValueChanged += (s, e) => this.CoerceValue(OpenPaneLengthProperty);
        }

        /// <summary>
        /// Override default OnApplyTemplate to capture children controls
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.hamburgerButton != null)
            {
                this.hamburgerButton.Click -= this.OnHamburgerButtonClick;
            }

            if (this.buttonsListView != null)
            {
                this.buttonsListView.SelectionChanged -= this.ButtonsListView_SelectionChanged;
            }

            if (this.optionsListView != null)
            {
                this.optionsListView.SelectionChanged -= this.OptionsListView_SelectionChanged;
            }

            this.hamburgerButton = this.GetTemplateChild("HamburgerButton") as Button;
            this.buttonsListView = this.GetTemplateChild("ButtonsListView") as ListBox;
            this.optionsListView = this.GetTemplateChild("OptionsListView") as ListBox;

            if (this.hamburgerButton != null)
            {
                this.hamburgerButton.Click += this.OnHamburgerButtonClick;
            }

            if (this.buttonsListView != null)
            {
                this.buttonsListView.SelectionChanged += this.ButtonsListView_SelectionChanged;
            }

            if (this.optionsListView != null)
            {
                this.optionsListView.SelectionChanged += this.OptionsListView_SelectionChanged;
            }

            this.ChangeItemFocusVisualStyle();

            this.Loaded -= this.HamburgerMenu_Loaded;
            this.Loaded += this.HamburgerMenu_Loaded;

            base.OnApplyTemplate();
        }

        private void HamburgerMenu_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.GetValue(ContentProperty) != null)
            {
                return;
            }

            var item = this.buttonsListView?.SelectedItem;
            var canRaiseItemEvents = this.CanRaiseItemEvents(item);
            if (canRaiseItemEvents && this.RaiseItemEvents(item))
            {
                return;
            }

            var optionItem = this.optionsListView?.SelectedItem;
            var canRaiseOptionsItemEvents = this.CanRaiseOptionsItemEvents(optionItem);
            if (canRaiseOptionsItemEvents && this.RaiseOptionsItemEvents(optionItem))
            {
                return;
            }

            if (canRaiseItemEvents || canRaiseOptionsItemEvents)
            {
                var selectedItem = item ?? optionItem;
                if (selectedItem != null)
                {
                    this.SetCurrentValue(ContentProperty, selectedItem);
                }
            }
        }
    }
}