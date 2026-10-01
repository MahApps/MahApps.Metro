// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MahApps.Metro.Gallery.Controls
{
    /// <summary>
    /// Picks the editor for a property from what the property takes, so that a page saying which
    /// properties to watch does not also have to say how to edit them.
    /// </summary>
    public class ExamplePropertyTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc />
        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ExampleProperty property || container is not FrameworkElement element)
            {
                return null;
            }

            // a nullable property takes the editor of the type it wraps, or Value on a
            // NumericUpDown would end up in a plain text box
            var wrapped = Nullable.GetUnderlyingType(property.PropertyType);
            var type = wrapped ?? property.PropertyType;

            // except a nullable bool, where the third state is the point of the property rather than
            // a detail of it: IsChecked on a three state check box is one, and a switch has no way
            // to say neither
            var key = property.IsReadOnly
                ? "MahApps.Gallery.Templates.Option.Readonly"
                : wrapped == typeof(bool)
                    ? "MahApps.Gallery.Templates.Option.ThreeState"
                    : KeyFor(type);

            return element.TryFindResource(key) as DataTemplate;
        }

        private static string KeyFor(Type type)
        {
            const string Prefix = "MahApps.Gallery.Templates.Option.";

            if (type.IsEnum)
            {
                return Prefix + "Choice";
            }

            if (type == typeof(bool))
            {
                return Prefix + "Boolean";
            }

            if (typeof(Brush).IsAssignableFrom(type))
            {
                return Prefix + "Brush";
            }

            // the three the library has an up-down of its own for, so that what the editor writes is
            // already what the property holds
            if (type == typeof(int))
            {
                return Prefix + "WholeNumber";
            }

            if (type == typeof(long))
            {
                return Prefix + "LongNumber";
            }

            if (type == typeof(decimal))
            {
                return Prefix + "DecimalNumber";
            }

            // and the rest, which go through the one that counts in doubles and are brought into
            // their own type on the way to the property
            if (type == typeof(sbyte) || type == typeof(byte)
                                      || type == typeof(short) || type == typeof(ushort)
                                      || type == typeof(uint) || type == typeof(ulong)
                                      || type == typeof(float) || type == typeof(double))
            {
                return Prefix + "Number";
            }

            // anything the reader could not have typed in the first place is shown rather than
            // offered for editing, and that takes both directions: the HotKey of a HotKeyBox says
            // "Ctrl + S" with no way back from that text to the object, while the TextDecorations
            // of a Hyperlink can be read from "Underline" but come back out as the name of their
            // own type.
            if (type != typeof(object))
            {
                var converter = TypeDescriptor.GetConverter(type);

                if (!converter.CanConvertFrom(typeof(string)) || !converter.CanConvertTo(typeof(string)))
                {
                    return Prefix + "Readonly";
                }
            }

            return Prefix + "Text";
        }
    }
}
