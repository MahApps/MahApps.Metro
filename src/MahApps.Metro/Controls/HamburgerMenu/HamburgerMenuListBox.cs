// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
                                                               Converter = HamburgerMenuItemAccessibleConverter.Default,
                                                               Mode = BindingMode.OneWay,
                                                               FallbackValue = string.Empty,
                                                               Bindings =
                                                               {
                                                                   new Binding
                                                                   {
                                                                       Source = hamburgerMenuItem,
                                                                       Path = new PropertyPath(nameof(IHamburgerMenuItem.ToolTip)),
                                                                       Mode = BindingMode.OneWay,
                                                                       FallbackValue = string.Empty
                                                                   },
                                                                   new Binding
                                                                   {
                                                                       Source = hamburgerMenuItem,
                                                                       Path = new PropertyPath(AutomationProperties.HelpTextProperty),
                                                                       Mode = BindingMode.OneWay,
                                                                       FallbackValue = string.Empty
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
                                                           Converter = HamburgerMenuItemAccessibleConverter.Default,
                                                           Mode = BindingMode.OneWay,
                                                           FallbackValue = string.Empty,
                                                           Bindings =
                                                           {
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuItem,
                                                                   Path = new PropertyPath(nameof(IHamburgerMenuItem.Label)),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = string.Empty
                                                               },
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuItem,
                                                                   Path = new PropertyPath(AutomationProperties.NameProperty),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = string.Empty
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
                                                   FallbackValue = string.Empty
                                               });

                        listBoxItem.SetBinding(AutomationProperties.NameProperty,
                                               new Binding
                                               {
                                                   Source = hamburgerMenuItem,
                                                   Path = new PropertyPath(nameof(IHamburgerMenuItem.Label)),
                                                   Mode = BindingMode.OneWay,
                                                   FallbackValue = string.Empty
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
                                                   FallbackValue = string.Empty
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
                                                           Converter = HamburgerMenuItemAccessibleConverter.Default,
                                                           Mode = BindingMode.OneWay,
                                                           FallbackValue = string.Empty,
                                                           Bindings =
                                                           {
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuHeaderItem,
                                                                   Path = new PropertyPath(nameof(IHamburgerMenuHeaderItem.Label)),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = string.Empty
                                                               },
                                                               new Binding
                                                               {
                                                                   Source = hamburgerMenuHeaderItem,
                                                                   Path = new PropertyPath(AutomationProperties.NameProperty),
                                                                   Mode = BindingMode.OneWay,
                                                                   FallbackValue = string.Empty
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
                                                   FallbackValue = string.Empty
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