// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Provides data for the <see cref="MultiSelectionComboBox.AddingItemEvent"/>
    /// </summary>
    public class AddingItemEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingItemEventArgs"/> class.
        /// </summary>
        /// <param name="routedEvent">The AddingItemEvent</param>
        /// <param name="source">The source object</param>
        /// <param name="input">The input string to parse</param>
        /// <param name="parsedObject">The parsed object</param>
        /// <param name="accepted"><see langword="true"/> if the <see cref="ParsedObject"/> is accepted otherwise <see langword="false"/></param>
        /// <param name="targetList">The target <see cref="IList"/> where the <see cref="ParsedObject"/> should be added</param>
        /// <param name="targetType">the target <see cref="Type"/> to which the <see cref="Input"/> should be converted to</param>
        /// <param name="stringFormat">the string format which can be used to control the <see cref="Parser"/></param>
        /// <param name="culture">the culture which can be used to control the <see cref="Parser"/></param>
        /// <param name="parser">The used parser</param>
        public AddingItemEventArgs(RoutedEvent routedEvent,
                                   object source,
                                   string? input,
                                   object? parsedObject,
                                   bool accepted,
                                   IList? targetList,
                                   Type? targetType,
                                   string? stringFormat,
                                   CultureInfo culture,
                                   IParseStringToObject? parser)
            : base(routedEvent, source)
        {
            this.Input = input;
            this.ParsedObject = parsedObject;
            this.Accepted = accepted;
            this.TargetList = targetList;
            this.TargetType = targetType;
            this.StringFormat = stringFormat;
            this.Culture = culture;
            this.Parser = parser;
        }

        /// <summary>
        /// The text input to parse 
        /// </summary>
        public string? Input { get; }

        /// <summary>
        /// Gets or sets the parsed object to add. You can override it
        /// </summary>
        public object? ParsedObject { get; set; }

        /// <summary>
        /// Gets the string format which can be used to control the <see cref="Parser"/>
        /// </summary>
        public string? StringFormat { get; }

        /// <summary>
        /// Gets the culture which can be used to control the <see cref="Parser"/>
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the <see cref="IParseStringToObject"/>-Instance which was used to parse the <see cref="Input"/> to the <see cref="ParsedObject"/>
        /// </summary>
        public IParseStringToObject? Parser { get; }

        /// <summary>
        /// Gets the target <see cref="Type"/> to which the <see cref="Input"/> should be converted to
        /// </summary>
        public Type? TargetType { get; }

        /// <summary>
        /// Gets the <see cref="IList"/> where the <see cref="ParsedObject"/> should be added
        /// </summary>
        public IList? TargetList { get; }

        /// <summary>
        /// Gets or sets whether the <see cref="ParsedObject"/> is accepted and can be added
        /// </summary>
        public bool Accepted { get; set; }
    }

    /// <summary>
    /// RoutedEventHandler used for the <see cref="MultiSelectionComboBox.AddingItemEvent"/>.
    /// </summary>
    public delegate void AddingItemEventArgsHandler(object? sender, AddingItemEventArgs args);
}