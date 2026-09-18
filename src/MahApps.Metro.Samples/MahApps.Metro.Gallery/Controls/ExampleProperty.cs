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

            // What decides whether a property is written with its owner in front of it is not who
            // registered it but whether the sample has it as a property of its own. FontSize is
            // registered by TextElement and handed on to Control, so a FontIcon carries it and it
            // is written plainly; ControlsHelper.CornerRadius is nowhere on a Button, so it needs
            // the owner. Going by the owner alone had FontSize written twice, once plainly and once
            // as TextElement.FontSize.
            var onTarget = target.GetType().GetProperty(property.Name);

            this.IsAttached = onTarget is null;

            // a property that can be read but not written takes a one way binding and is shown
            // rather than offered for editing. IsDragging on a MetroThumbContentControl is one:
            // asking for two way there throws rather than simply not working.
            this.IsReadOnly = onTarget is not null && onTarget.GetSetMethod() is null;
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
                                             Mode = this.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
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
        /// Whether the property can only be read, which the sample decides rather than the reader.
        /// </summary>
        public bool IsReadOnly { get; }

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
                       // a size nobody has set is NaN, and NaN is not something anybody writes into
                       // XAML, so the attribute stays away even once the box has been touched
                       double number when double.IsNaN(number) => null,
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
        /// Whether somebody actually said this value, either by writing it into the sample or by
        /// turning it here. A value that comes from a style or from the default is not written into
        /// the shown markup, because nobody would have written it themselves: the underline brush
        /// of a tab control and the corner radius of a button both arrive from their style, and
        /// Height arrives as NaN from nowhere at all.
        /// </summary>
        public bool IsSetHere()
        {
            return DependencyPropertyHelper.GetValueSource(this.Target, this.Property).BaseValueSource
                   == BaseValueSource.Local;
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
            if (d is not ExampleProperty property)
            {
                return value;
            }

            if (value is null)
            {
                // an emptied editor means the property goes back to holding what it holds without
                // anybody saying so, and for Height that is NaN rather than nothing at all, which a
                // double could not take anyway
                return Nullable.GetUnderlyingType(property.PropertyType) is null && property.PropertyType.IsValueType
                    ? property.Property.GetMetadata(property.Target).DefaultValue
                    : null;
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
