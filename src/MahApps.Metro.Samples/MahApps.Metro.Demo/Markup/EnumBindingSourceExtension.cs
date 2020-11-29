// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Markup;

namespace MetroDemo.Markup
{
    /// <summary>
    /// Markup extension for Enum values.
    /// </summary>
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type enumType;

        /// <summary>
        /// Gets or sets the type of the Enum.
        /// </summary>
        /// <exception cref="ArgumentException">Value is not an Enum type.</exception>
        public Type EnumType
        {
            get => this.enumType;
            set
            {
                if (value != this.enumType)
                {
                    if (null != value)
                    {
                        var type = Nullable.GetUnderlyingType(value) ?? value;

                        if (!type.IsEnum)
                        {
                            throw new ArgumentException("Type must be for an Enum.");
                        }
                    }

                    this.enumType = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of EnumBindingSourceExtension.
        /// </summary>
        public EnumBindingSourceExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of EnumBindingSourceExtension.
        /// </summary>
        /// <param name="enumType">The type of the Enum.</param>
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.EnumType is null)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }

            var underlyingEnumType = Nullable.GetUnderlyingType(this.EnumType) ?? this.EnumType;
            var enumValues = Enum.GetValues(underlyingEnumType);

            if (underlyingEnumType == this.EnumType)
            {
                return enumValues;
            }

            var nullableEnumValues = Array.CreateInstance(underlyingEnumType, enumValues.Length + 1);
            enumValues.CopyTo(nullableEnumValues, 1);
            return nullableEnumValues;
        }
    }
}