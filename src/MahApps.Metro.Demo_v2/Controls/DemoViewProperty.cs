using MahApps.Metro.Demo_v2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
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
            GetExampleXamlContent = GetExampleXamlContent_Default;
        }

        public DemoViewProperty(DependencyProperty dependencyProperty, DependencyObject bindingTarget, string groupName = null, DataTemplate dataTemplate = null) : this()
        {
            SetCurrentValue(PropertyNameProperty, GetDefaultPropertyName(dependencyProperty));

            SetCurrentValue(GroupNameProperty, groupName ?? GetDefaultGroupName());

            SetCurrentValue(DataTemplateProperty, dataTemplate ?? GetDefaultDataTemplate(dependencyProperty));

            // Create Binding to the Control
            var binding = new Binding()
            { 
                Path = new PropertyPath(dependencyProperty),
                Source = bindingTarget,
                Mode = BindingMode.TwoWay
            };
            BindingOperations.SetBinding(this, DemoViewProperty.ValueProperty, binding);

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
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
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
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }


        /// <summary>
        /// Gets or Sets the GroupName
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        private string GetDefaultGroupName()
        {
            if (string.IsNullOrWhiteSpace(PropertyName)) 
                return null;

            switch (this.PropertyName)
            {
                case string _ when PropertyName.EndsWith("Alignment"):
                case string _ when PropertyName.EndsWith("Height"):
                case string _ when PropertyName.EndsWith("Width"):
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
            get { return (DataTemplate)GetValue(DataTemplateProperty); }
            set { SetValue(DataTemplateProperty, value); }
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
                return App.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Numeric"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsAssignableFrom(typeof(string)))
            {
                return App.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.String"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType == typeof(bool))
            {
                return App.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Boolean"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsEnum)
            {
                return App.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Enum"] as DataTemplate;
            }

            if (dependencyProperty.PropertyType.IsAssignableFrom(typeof(Style)))
            {
                return App.Current.Resources["MahDemo.DataTemplates.PropertyPresenter.Styles"] as DataTemplate;
            }

            return null;
        }

        /// <summary>
        /// Gets or Sets the ItemSource used for selectable <see cref="Value"/>
        /// </summary>
        public IEnumerable ItemSource
        {
            get { return (IEnumerable)GetValue(ItemSourceProperty); }
            set { SetValue(ItemSourceProperty, value); }
        }


        #region XAML Replace Value
        public Func<string> GetExampleXamlContent { get; set; }

        private string GetExampleXamlContent_Default()
        {
            return Value?.ToString();
        }
        #endregion

    }
}
