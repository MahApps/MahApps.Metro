using System;
using System.Windows.Markup;

namespace MetroDemo.Markup
{

    /// <summary>
    /// Markup extension for Enum values.
    /// </summary>
    public class EnumBindingSourceExtension
        : MarkupExtension
    {
        private Type _EnumType;

        /// <summary>
        /// Gets or sets the type of the Enum.
        /// </summary>
        /// <exception cref="ArgumentException">Value is not an Enum type.</exception>
        public Type EnumType
        {
            get { return this._EnumType; }
            set
            {
                if (!Object.Equals(value, this.EnumType))
                {
                    if (!Object.Equals(value, null) && !(Nullable.GetUnderlyingType(value) ?? value).IsEnum)
                        throw new ArgumentException("Type must be an Enum.");

                    this._EnumType = value;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of EnumBindingSourceExtension.
        /// </summary>
        public EnumBindingSourceExtension() { }

        /// <summary>
        /// Initializes a new instance of EnumBindingSourceExtension.
        /// </summary>
        /// <param name="enumType">The type of the Enum.</param>
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>The values of the Enum.</returns>
        /// <exception cref="InvalidOperationException">The type of the Enum is undefined.</exception>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Object.Equals(this.EnumType, null))
                throw new InvalidOperationException("The type of the Enum is undefined.");

            var underlyingEnumType = Nullable.GetUnderlyingType(this.EnumType) ?? this.EnumType;
            var enumValues = Enum.GetValues(underlyingEnumType);
            if (underlyingEnumType.Equals(this.EnumType))
                return enumValues;

            var nullableEnumValues = Array.CreateInstance(underlyingEnumType, enumValues.Length);
            enumValues.CopyTo(nullableEnumValues, 1);
            return nullableEnumValues;
        }
    }
}
