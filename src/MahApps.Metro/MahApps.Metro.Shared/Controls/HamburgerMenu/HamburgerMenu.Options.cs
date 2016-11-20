namespace MahApps.Metro.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The HamburgerMenu is based on a SplitView control. By default it contains a HamburgerButton and a ListView to display menu items.
    /// </summary>
    public partial class HamburgerMenu
    {
        /// <summary>
        /// Identifies the <see cref="OptionsItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsItemsSourceProperty = DependencyProperty.Register(nameof(OptionsItemsSource), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OptionsItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsItemTemplateProperty = DependencyProperty.Register(nameof(OptionsItemTemplate), typeof(DataTemplate), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="OptionsVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OptionsVisibilityProperty = DependencyProperty.Register(nameof(OptionsVisibility), typeof(Visibility), typeof(HamburgerMenu), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        ///     Gets or sets an object source used to generate the content of the options.
        /// </summary>
        public object OptionsItemsSource
        {
            get { return GetValue(OptionsItemsSourceProperty); }
            set { SetValue(OptionsItemsSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in the options.
        /// </summary>
        public DataTemplate OptionsItemTemplate
        {
            get { return (DataTemplate)GetValue(OptionsItemTemplateProperty); }
            set { SetValue(OptionsItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets the collection used to generate the content of the option list.
        /// </summary>
        /// <exception cref="Exception">
        /// Exception thrown if OptionsListView is not yet defined.
        /// </exception>
        public ItemCollection OptionsItems
        {
            get
            {
                if (_optionsListView == null)
                {
                    throw new Exception("OptionsListView is not defined yet. Please use OptionsItemsSource instead.");
                }

                return _optionsListView?.Items;
            }
        }

        /// <summary>
        /// Gets or sets options' visibility.
        /// </summary>
        public Visibility OptionsVisibility
        {
            get { return (Visibility)GetValue(OptionsVisibilityProperty); }
            set { SetValue(OptionsVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected options menu item.
        /// </summary>
        public object SelectedOptionsItem
        {
            get { return _optionsListView.SelectedItem; }
            set { _optionsListView.SelectedItem = value; }
        }

        /// <summary>
        /// Gets or sets the selected options menu index.
        /// </summary>
        public int SelectedOptionsIndex
        {
            get { return _optionsListView.SelectedIndex; }
            set { _optionsListView.SelectedIndex = value; }
        }
    }
}
