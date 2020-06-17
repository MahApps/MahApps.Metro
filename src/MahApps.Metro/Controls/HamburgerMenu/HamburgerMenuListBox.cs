using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Converters;

namespace MahApps.Metro.Controls
{
    public class HamburgerMenuListBox : ListBox
    {
        private readonly BooleanToVisibilityConverter booleanToVisibilityConverter = new BooleanToVisibilityConverter();
        private readonly HamburgerMenuItemAccessibleConverter hamburgerMenuItemAccessibleConverter = new HamburgerMenuItemAccessibleConverter();

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

                    if (item is DependencyObject)
                    {
                        var helpTextPropertyMultiBinding = new MultiBinding
                                                           {
                                                               Converter = this.hamburgerMenuItemAccessibleConverter,
                                                               Bindings =
                                                               {
                                                                   new Binding
                                                                   {
                                                                       Source = hamburgerMenuItem,
                                                                       Path = new PropertyPath(nameof(IHamburgerMenuItem.ToolTip)),
                                                                       Mode = BindingMode.OneWay,
                                                                       FallbackValue = null
                                                                   },
                                                                   new Binding
                                                                   {
                                                                       Source = hamburgerMenuItem,
                                                                       Path = new PropertyPath(AutomationProperties.HelpTextProperty),
                                                                       Mode = BindingMode.OneWay,
                                                                       FallbackValue = null
                                                                   }
                                                               }
                                                           };
                        listBoxItem.SetBinding(AutomationProperties.HelpTextProperty, helpTextPropertyMultiBinding);

                        listBoxItem.SetBinding(AutomationProperties.LabeledByProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuItem,
                                                   Path = new PropertyPath(AutomationProperties.LabeledByProperty),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });

                        var namePropertyMultiBinding = new MultiBinding
                                                       {
                                                           Converter = this.hamburgerMenuItemAccessibleConverter,
                                                           Bindings =
                                                           {
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuItem,
                                                                   Path = new PropertyPath(nameof(IHamburgerMenuItem.Label)),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = null
                                                               },
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuItem,
                                                                   Path = new PropertyPath(AutomationProperties.NameProperty),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = null
                                                               }
                                                           }
                                                       };
                        listBoxItem.SetBinding(AutomationProperties.NameProperty, namePropertyMultiBinding);
                    }
                    else
                    {
                        listBoxItem.SetBinding(AutomationProperties.HelpTextProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuItem,
                                                   Path = new PropertyPath(nameof(IHamburgerMenuItem.ToolTip)),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });

                        listBoxItem.SetBinding(AutomationProperties.NameProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuItem,
                                                   Path = new PropertyPath(nameof(IHamburgerMenuItem.Label)),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });
                    }
                }

                if (item is IHamburgerMenuHeaderItem hamburgerMenuHeaderItem)
                {
                    if (item is DependencyObject)
                    {
                        listBoxItem.SetBinding(AutomationProperties.HelpTextProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuHeaderItem,
                                                   Path = new PropertyPath(AutomationProperties.HelpTextProperty),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });

                        listBoxItem.SetBinding(AutomationProperties.LabeledByProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuHeaderItem,
                                                   Path = new PropertyPath(AutomationProperties.LabeledByProperty),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });

                        var namePropertyMultiBinding = new MultiBinding
                                                       {
                                                           Converter = this.hamburgerMenuItemAccessibleConverter,
                                                           Bindings =
                                                           {
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuHeaderItem,
                                                                   Path = new PropertyPath(nameof(IHamburgerMenuHeaderItem.Label)),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = null
                                                               },
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuHeaderItem,
                                                                   Path = new PropertyPath(AutomationProperties.NameProperty),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = null
                                                               }
                                                           }
                                                       };
                        listBoxItem.SetBinding(AutomationProperties.NameProperty, namePropertyMultiBinding);
                    }
                    else
                    {
                        listBoxItem.SetBinding(AutomationProperties.NameProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuHeaderItem,
                                                   Path = new PropertyPath(nameof(IHamburgerMenuHeaderItem.Label)),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = null
                                               });
                    }
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
                BindingOperations.ClearBinding(listBoxItem, AutomationProperties.HelpTextProperty);
                BindingOperations.ClearBinding(listBoxItem, AutomationProperties.LabeledByProperty);
                BindingOperations.ClearBinding(listBoxItem, AutomationProperties.NameProperty);
            }
        }
    }
}