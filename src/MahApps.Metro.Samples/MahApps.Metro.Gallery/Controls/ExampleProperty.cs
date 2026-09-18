// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace MahApps.Metro.Gallery.Controls
{
    /// <summary>
    /// One property of a sample, turned by the options of a <see cref="ControlExample"/>.
    /// <para>
    /// It holds a two way binding to the property on the sample, so whatever the editor writes here
    /// arrives at the control, and whatever the control does to the property arrives back. The
    /// example also reads it to put the current value into the XAML it shows, which is what the
    /// card hands a callback in for: it owns its properties and wants to know when one moves.
    /// </para>
    /// </summary>
    public sealed class ExampleProperty : DependencyObject
    {
        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty
            = DependencyProperty.Register(nameof(Value),
                                          typeof(object),
                                          typeof(ExampleProperty),
                                          new PropertyMetadata(null, OnValueChanged, CoerceValue));

        private readonly Action? valueChanged;

        public ExampleProperty(DependencyObject target,
                               DependencyProperty property,
                               string? group = null,
                               Action? valueChanged = null)
        {
            this.valueChanged = valueChanged;

            this.Target = target ?? throw new ArgumentNullException(nameof(target));
            this.Property = property ?? throw new ArgumentNullException(nameof(property));

            // a property whose owner the sample is not is an attached one, and those are written
            // with their owner in front of them
            this.IsAttached = !property.OwnerType.IsInstanceOfType(target);
            this.Name = this.IsAttached ? property.OwnerType.Name + "." + property.Name : property.Name;
            this.XamlNamespace = this.IsAttached ? XamlNamespaceOf(property.OwnerType) : null;

            this.TargetName = target is FrameworkElement element && !string.IsNullOrEmpty(element.Name)
                ? element.Name
                : null;

            this.Group = group ?? DefaultGroup(this.Name);

            if (property.PropertyType.IsEnum)
            {
                this.Values = Enum.GetValues(property.PropertyType);
            }

            BindingOperations.SetBinding(this,
                                         ValueProperty,
                                         new Binding
                                         {
                                             Path = new PropertyPath(property),
                                             Source = target,
                                             Mode = BindingMode.TwoWay
                                         });
        }

        /// <summary>
        /// The sample this property belongs to.
        /// </summary>
        public DependencyObject Target { get; }

        /// <summary>
        /// The property being turned.
        /// </summary>
        public DependencyProperty Property { get; }

        /// <summary>
        /// How the property is written in XAML, which is also what the options list calls it.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Whether this is an attached property.
        /// </summary>
        public bool IsAttached { get; }

        /// <summary>
        /// The XAML namespace an attached property has to be written in, so that it can be put into
        /// the shown markup even when the sample does not carry it.
        /// </summary>
        public string? XamlNamespace { get; }

        /// <summary>
        /// The x:Name of the sample, which is how the right element is found again in the shown XAML
        /// when an example holds more than one control.
        /// </summary>
        public string? TargetName { get; }

        /// <summary>
        /// The heading the options list files this property under.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// What the property takes, which decides the editor the options list puts in front of it.
        /// </summary>
        public Type PropertyType => this.Property.PropertyType;

        /// <summary>
        /// What there is to choose from, for a property that takes one of a set.
        /// </summary>
        public IEnumerable? Values { get; }

        /// <summary>
        /// The value, bound both ways to the sample.
        /// </summary>
        public object? Value
        {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        /// <summary>
        /// The value as it would be written in XAML, or null where the attribute should not be there
        /// at all.
        /// </summary>
        public string? ToXamlValue()
        {
            return this.Value switch
                   {
                       null => null,
                       bool flag => flag ? "True" : "False",
                       Enum value => value.ToString(),
                       IFormattable value => value.ToString(null, CultureInfo.InvariantCulture),
                       var value => value.ToString()
                   };
        }

        /// <summary>
        /// The XAML namespace the type is exported under, which is what its XmlnsDefinition says.
        /// </summary>
        private static string? XamlNamespaceOf(Type type)
        {
            return type.Assembly
                       .GetCustomAttributes(typeof(XmlnsDefinitionAttribute), false)
                       .OfType<XmlnsDefinitionAttribute>()
                       .FirstOrDefault(definition => definition.ClrNamespace == type.Namespace)
                       ?.XmlNamespace;
        }

        /// <summary>
        /// Whether the property still holds what it holds without anybody saying so, which is what
        /// keeps Height="NaN" and the like out of the shown markup.
        /// </summary>
        public bool IsAtDefaultValue()
        {
            return Equals(this.Value, this.Property.GetMetadata(this.Target).DefaultValue);
        }

        private static string DefaultGroup(string name)
        {
            // the handful of properties that say where a control sits rather than what it is
            return name.EndsWith("Alignment", StringComparison.Ordinal)
                   || name.EndsWith("Width", StringComparison.Ordinal)
                   || name.EndsWith("Height", StringComparison.Ordinal)
                   || name == "Margin"
                   || name == "Padding"
                ? "Layout"
                : "Properties";
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ExampleProperty)?.valueChanged?.Invoke();
        }

        /// <summary>
        /// Brings the value into the type the watched property holds. The editor for a number only
        /// ever writes a double, and handing that to a property that wants an int would be dropped
        /// by the binding with nothing but a line in the debug output.
        /// </summary>
        private static object? CoerceValue(DependencyObject d, object? value)
        {
            if (d is not ExampleProperty property || value is null)
            {
                return value;
            }

            var wanted = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (value.GetType() == wanted || !typeof(IConvertible).IsAssignableFrom(wanted) || wanted.IsEnum)
            {
                return value;
            }

            try
            {
                return System.Convert.ChangeType(value, wanted, CultureInfo.InvariantCulture);
            }
            catch (Exception exception) when (exception is InvalidCastException or FormatException or OverflowException)
            {
                return value;
            }
        }
    }
}
