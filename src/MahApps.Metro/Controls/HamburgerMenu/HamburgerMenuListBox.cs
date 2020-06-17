using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuListBox : ListBox
    {
        private readonly BooleanToVisibilityConverter booleanToVisibilityConverter = new BooleanToVisibilityConverter();

        static HamburgerMenuListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HamburgerMenuListBox), new FrameworkPropertyMetadata(typeof(HamburgerMenuListBox)));
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element is ListBoxItem listBoxItem)
            {
                if (item is IHamburgerMenuItemBase baseMenuItem)
                {
                    listBoxItem.SetBinding(VisibilityProperty,
                                           new Binding
                                           {
                                               Source = baseMenuItem,
                                               Path = new PropertyPath(nameof(IHamburgerMenuItemBase.IsVisible)),
                                               Converter = this.booleanToVisibilityConverter,
                                               FallbackValue = Visibility.Visible
                                           });
                }

                if (item is IHamburgerMenuItem hamburgerMenuItem)
                {
                    listBoxItem.SetBinding(IsEnabledProperty,
                                           new Binding
                                           {
                                               Source = hamburgerMenuItem,
                                               Path = new PropertyPath(nameof(IHamburgerMenuItem.IsEnabled)),
                                               FallbackValue = true
                                           });
                }
            }
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);

            if (element is ListBoxItem listBoxItem)
            {
                BindingOperations.ClearBinding(listBoxItem, VisibilityProperty);
                BindingOperations.ClearBinding(listBoxItem, IsEnabledProperty);
            }
        }
    }
}