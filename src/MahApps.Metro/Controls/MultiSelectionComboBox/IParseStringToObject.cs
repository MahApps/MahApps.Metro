// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// This interfaces is used to parse a string to any object. 
    /// </summary>
    public interface IParseStringToObject
    {
        /// <summary>
        /// Parses the given input to an object of the given type.
        /// </summary>
        /// <param name="input">The input string to parse</param>
        /// <param name="culture">The culture which should be used to parse</param>
        /// <param name="stringFormat">The string format to applay</param>
        /// <returns>The object if successful, otherwise null</returns>
        object CreateObjectFromString(string input, CultureInfo culture, string stringFormat);
    }
}
