using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    public partial class HamburgerMenu
    {
        /// <summary>Identifies the <see cref="OptionsItemsSource"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemsSourceProperty = DependencyProperty.Register(nameof(OptionsItemsSource), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OptionsItemContainerStyle"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemContainerStyleProperty = DependencyProperty.Register(nameof(OptionsItemContainerStyle), typeof(Style), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OptionsItemTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemTemplateProperty = DependencyProperty.Register(nameof(OptionsItemTemplate), typeof(DataTemplate), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OptionsItemTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemTemplateSelectorProperty = DependencyProperty.Register(nameof(OptionsItemTemplateSelector), typeof(DataTemplateSelector), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OptionsVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsVisibilityProperty = DependencyProperty.Register(nameof(OptionsVisibility), typeof(Visibility), typeof(HamburgerMenu), new PropertyMetadata(Visibility.Visible));

        /// <summary>Identifies the <see cref="SelectedOptionsItem"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedOptionsItemProperty = DependencyProperty.Register(nameof(SelectedOptionsItem), typeof(object), typeof(HamburgerMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Identifies the <see cref="SelectedOptionsIndex"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedOptionsIndexProperty = DependencyProperty.Register(nameof(SelectedOptionsIndex), typeof(int), typeof(HamburgerMenu), new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal));

        /// <summary>Identifies the <see cref="OptionsItemCommand"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemCommandProperty = DependencyProperty.Register(nameof(OptionsItemCommand), typeof(ICommand), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OptionsItemCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty OptionsItemCommandParameterProperty = DependencyProperty.Register(nameof(OptionsItemCommandParameter), typeof(object), typeof(HamburgerMenu), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets an object source used to generate the content of the options.
        /// </summary>
        public object OptionsItemsSource
        {
            get => this.GetValue(OptionsItemsSourceProperty);
            set => this.SetValue(OptionsItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the Style used for each item in the options.
        /// </summary>
        public Style OptionsItemContainerStyle
        {
            get => (Style)this.GetValue(OptionsItemContainerStyleProperty);
            set => this.SetValue(OptionsItemContainerStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item in the options.
        /// </summary>
        public DataTemplate OptionsItemTemplate
        {
            get => (DataTemplate)this.GetValue(OptionsItemTemplateProperty);
            set => this.SetValue(OptionsItemTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the DataTemplateSelector used to display each item in the options.
        /// </summary>
        public DataTemplateSelector OptionsItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(OptionsItemTemplateSelectorProperty);
            set => this.SetValue(OptionsItemTemplateSelectorProperty, value);
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
                if (this.optionsListView == null)
                {
                    throw new Exception("OptionsListView is not defined yet. Please use OptionsItemsSource instead.");
                }

                return this.optionsListView?.Items;
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the options menu.
        /// </summary>
        public Visibility OptionsVisibility
        {
            get => (Visibility)this.GetValue(OptionsVisibilityProperty);
            set => this.SetValue(OptionsVisibilityProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected options menu item.
        /// </summary>
        public object SelectedOptionsItem
        {
            get => this.GetValue(SelectedOptionsItemProperty);
            set => this.SetValue(SelectedOptionsItemProperty, value);
        }

        /// <summary>
        /// Gets or sets the selected options menu index.
        /// </summary>
        public int SelectedOptionsIndex
        {
            get => (int)this.GetValue(SelectedOptionsIndexProperty);
            set => this.SetValue(SelectedOptionsIndexProperty, value);
        }

        /// <summary>
        /// Gets or sets a command which will be executed if an options item is clicked by the user.
        /// </summary>
        public ICommand OptionsItemCommand
        {
            get => (ICommand)this.GetValue(OptionsItemCommandProperty);
            set => this.SetValue(OptionsItemCommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command parameter which will be passed by the OptionsItemCommand.
        /// </summary>
        public object OptionsItemCommandParameter
        {
            get => (object)this.GetValue(OptionsItemCommandParameterProperty);
            set => this.SetValue(OptionsItemCommandParameterProperty, value);
        }

        /// <summary>
        /// Executes the options item command which can be set by the user.
        /// </summary>
        public void RaiseOptionsItemCommand()
        {
            var command = this.OptionsItemCommand;
            var commandParameter = this.OptionsItemCommandParameter ?? this;
            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }
}