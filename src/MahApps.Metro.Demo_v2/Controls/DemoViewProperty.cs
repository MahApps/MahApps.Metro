// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Demo_v2;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace MahApps.Demo.Controls
{
    public class DemoViewProperty : DependencyObject, INotifyPropertyChanged
    {
        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string PropertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DemoViewProperty demoViewProperty)
            {
                demoViewProperty.RaisePropertyChanged(e.Property.Name);
            }
        }

        #endregion

        #region Constructors

        public DemoViewProperty()
        {
            this.GetExampleXamlContent = this.GetExampleXamlContent_Default;
        }

        public DemoViewProperty(DependencyProperty dependencyProperty, DependencyObject bindingTarget, string groupName = null, DataTemplate dataTemplate = null)
            : this()
        {
            this.SetCurrentValue(PropertyNameProperty, this.GetDefaultPropertyName(dependencyProperty));

            this.SetCurrentValue(GroupNameProperty, groupName ?? this.GetDefaultGroupName());

            this.SetCurrentValue(DataTemplateProperty, dataTemplate ?? this.GetDefaultDataTemplate(dependencyProperty));

            // Create Binding to the Control
            var binding = new Binding()
                          {
                              Path = new PropertyPath(dependencyProperty),
                              Source = bindingTarget,
                              Mode = BindingMode.TwoWay
                          };
            BindingOperations.SetBinding(this, ValueProperty, binding);
        }

        #endregion

        /// <summary>Identifies the <see cref="PropertyName"/> dependency property.</summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(DemoViewProperty), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(DemoViewProperty), new PropertyMetadata(null, OnValueChanged));

        /// <summary>Identifies the <see cref="DataTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty DataTemplateProperty = DependencyProperty.Register(nameof(DataTemplate), typeof(DataTemplate), typeof(DemoViewProperty), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="ItemSource"/> dependency property.</summary>
        public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(nameof(ItemSource), typeof(IEnumerable), typeof(DemoViewProperty), new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="GroupName"/> dependency property.</summary>
        public static readonly DependencyProperty GroupNameProperty = DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(DemoViewProperty), new PropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the PropertyName
        /// </summary>
        public string PropertyName
        {
            get => (string)this.GetValue(PropertyNameProperty);
            set => this.SetValue(PropertyNameProperty, value);
        }

        private string GetDefaultPropertyName(DependencyProperty dependencyProperty)
        {
            if (typeof(UIElement).IsAssignableFrom(dependencyProperty.OwnerType))
            {
                return dependencyProperty.Name;
            }
            else
            {
                return $"{dependencyProperty.OwnerType.Name}.{dependencyProperty.Name}";
            }
        }

        /// <summary>
        /// Gets or Sets the Value
        /// </summary>
        public object Value
        {
            get => (object)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Gets or Sets the GroupName
        /// </summary>
        public string GroupName
        {
            get => (string)this.GetValue(GroupNameProperty);
            set => this.SetValue(GroupNameProperty, value);
        }

        private string GetDefaultGroupName()
        {
            if (string.IsNullOrWhiteSpace(this.PropertyName))
                return null;

            switch (this.PropertyName)
            {
                case string _ when this.PropertyName.EndsWith("Alignment"):
                case string _ when this.PropertyName.EndsWith("Height"):
                case string _ when this.PropertyName.EndsWith("Width"):
                    return "Layout";

                default:
                    return "Misc";
            }
        }

        /// <summary>
        /// Gets or Sets the DataTemplate
        /// </summary>
        public DataTemplate DataTemplate
        {
            get => (DataTemplate)this.GetValue(DataTemplateProperty);
            set => this.SetValue(DataTemplateProperty, value);
        }

        private DataTemplate GetDefaultDataTemplate(DependencyProperty dependencyProperty)
        {
            // Any Object 
            if (dependencyProperty.PropertyType == typeof(object)) return null;

            // Numeric 
            if (dependencyProperty.PropertyType.IsAssignableFrom(typeof(sbyte)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(byte)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(short)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(ushort)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(int)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(uint)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(long)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(ulong)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(float)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(double)) ||
                dependencyProperty.PropertyType.IsAssignableFrom(typeof(decimal)))
            {
                return Application.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Numeric"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsAssignableFrom(typeof(string)))
            {
                return Application.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.String"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType == typeof(bool))
            {
                return Application.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Boolean"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsEnum)
            {
                return Application.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Enum"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsAssignableFrom(typeof(Style)))
            {
                return Application.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Styles"] as DataTemplate;
            }

            return null;
        }

        /// <summary>
        /// Gets or Sets the ItemSource used for selectable <see cref="Value"/>
        /// </summary>
        public IEnumerable ItemSource
        {
            get => (IEnumerable)this.GetValue(ItemSourceProperty);
            set => this.SetValue(ItemSourceProperty, value);
        }

        #region XAML Replace Value

        public Func<string> GetExampleXamlContent { get; set; }

        private string GetExampleXamlContent_Default()
        {
            return this.Value?.ToString();
        }

        #endregion
    }
}