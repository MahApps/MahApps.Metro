using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>The object if successful, otherwise null</returns>
        object CreateObjectFromString(string input, CultureInfo culture);
    }
}
