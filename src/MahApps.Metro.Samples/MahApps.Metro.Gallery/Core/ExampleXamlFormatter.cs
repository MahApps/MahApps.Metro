// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MahApps.Metro.Gallery.Controls;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Core
{
    /// <summary>
    /// The XAML of a sample, with the values the reader has just set.
    /// <para>
    /// ShowMeTheXAML hands over the markup the build wrote down, which is the markup as it stands in
    /// the file. That is the right thing to show until somebody turns one of the options, and from
    /// then on the code and the control above it disagree. So the attributes of the watched
    /// properties are set to whatever is currently on, before the markup is laid out.
    /// </para>
    /// <para>
    /// The values go in first and the layout comes second, because at this point the markup still
    /// carries every namespace declaration the build wrote and can be read as XML. Afterwards the
    /// declarations of the default namespaces are gone, and x:Name without xmlns:x is not XML.
    /// </para>
    /// </summary>
    internal sealed class ExampleXamlFormatter : IXamlFormatter
    {
        private static readonly XName NameInXaml = XName.Get("Name", "http://schemas.microsoft.com/winfx/2006/xaml");

        // what the prefixes are called in this repository, for the case where an attached property
        // has to be written into a sample whose markup never mentioned that namespace. Without a
        // prefix of its own XLinq would invent p1, which is valid and reads badly.
        private static readonly Dictionary<string, string> Prefixes = new Dictionary<string, string>
                                                                      {
                                                                          { "http://metro.mahapps.com/winfx/xaml/controls", "mah" },
                                                                          { "http://metro.mahapps.com/winfx/xaml/shared", "mah" },
                                                                          { "http://metro.mahapps.com/winfx/xaml/iconpacks", "iconPacks" }
                                                                      };

        private readonly IXamlFormatter formatter = new XamlFormatter { NewLineOnAttributes = true };
        private readonly IEnumerable<ExampleProperty> properties;

        public ExampleXamlFormatter(IEnumerable<ExampleProperty> properties)
        {
            this.properties = properties;
        }

        /// <inheritdoc />
        public string FormatXaml(string xaml)
        {
            return this.formatter.FormatXaml(this.WithCurrentValues(xaml));
        }

        private string WithCurrentValues(string xaml)
        {
            if (string.IsNullOrWhiteSpace(xaml))
            {
                return xaml;
            }

            XDocument document;
            try
            {
                document = XDocument.Parse(xaml);
            }
            catch (XmlException)
            {
                // not something that can be read, so it goes on untouched and the reader still gets
                // the markup as the build wrote it
                return xaml;
            }

            var root = document.Root;
            if (root is null)
            {
                return xaml;
            }

            foreach (var property in this.properties)
            {
                var element = FindTarget(root, property.TargetName);
                if (element is null)
                {
                    continue;
                }

                var attribute = FindAttribute(element, property.Name);
                var value = property.ToXamlValue();

                if (attribute is not null)
                {
                    if (value is null)
                    {
                        attribute.Remove();
                    }
                    else
                    {
                        attribute.Value = value;
                    }
                }
                else if (value is not null && !property.IsAtDefaultValue())
                {
                    // only what somebody would have written down themselves: a property still
                    // sitting at its default value says nothing and would only make the sample longer
                    element.SetAttributeValue(NameFor(element, property), value);
                }
            }

            return document.ToString();
        }

        /// <summary>
        /// The name to write a property that the sample does not carry yet under. An attached one
        /// belongs in the namespace of its owner, unless that is the default namespace of the
        /// document, where an attribute is written without a prefix the way XAML does it.
        /// </summary>
        private static XName NameFor(XElement element, ExampleProperty property)
        {
            if (!property.IsAttached
                || property.XamlNamespace is null
                || property.XamlNamespace == element.GetDefaultNamespace().NamespaceName)
            {
                return XName.Get(property.Name);
            }

            var space = XNamespace.Get(property.XamlNamespace);

            // the declaration goes on the sample rather than on the display around it, because the
            // formatter that runs after this one throws every attribute of that one away, and a
            // namespace nobody declared would come out as p3 instead of mah
            if (element.GetPrefixOfNamespace(space) is null
                && Prefixes.TryGetValue(property.XamlNamespace, out var prefix))
            {
                element.SetAttributeValue(XNamespace.Xmlns + prefix, property.XamlNamespace);
            }

            return space + property.Name;
        }

        private static XElement? FindTarget(XElement root, string? name)
        {
            // no name to go by means the sample is whatever sits inside the display, and a local
            // name with a dot in it is a property element rather than a control
            if (name is null)
            {
                return root.Elements().FirstOrDefault(element => !element.Name.LocalName.Contains("."));
            }

            return root.DescendantsAndSelf()
                       .FirstOrDefault(element => (string?)element.Attribute(NameInXaml) == name
                                                  || (string?)element.Attribute("Name") == name);
        }

        private static XAttribute? FindAttribute(XElement element, string name)
        {
            // the prefix of an attached property depends on the file it was written in, so the
            // local name is what counts
            return element.Attributes().FirstOrDefault(attribute => attribute.Name.LocalName == name);
        }
    }
}
