// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            var key = property.IsReadOnly ? "MahApps.Gallery.Templates.Option.Readonly" : KeyFor(type);

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

            if (type == typeof(sbyte) || type == typeof(byte)
                                      || type == typeof(short) || type == typeof(ushort)
                                      || type == typeof(int) || type == typeof(uint)
                                      || type == typeof(long) || type == typeof(ulong)
                                      || type == typeof(float) || type == typeof(double)
                                      || type == typeof(decimal))
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
