using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahApps.Metro.Controls
{
    /// <summary>
    ///   This class is a helper class for the <see cref="MultiSelectionComboBox"/>. 
    ///   It uses the <see cref="TypeConverter"/> for the elements <see cref="Type"/>. If you need more control
    ///   over the conversion you should create your own class which implements <see cref="IParseStringToObject"/>
    /// </summary>
    public class BuiltInStringToObjectParser : IParseStringToObject
    {
        private static BuiltInStringToObjectParser instance;
        public static BuiltInStringToObjectParser Instance => instance ??= new BuiltInStringToObjectParser();

        public bool TryCreateObjectFromString(string input, out object result,  CultureInfo culture = null, string stringFormat = null, Type targetType = null)
        {
            try
            {
                // If the input is null the result is also null
                if (input is null)
                {
                    result = null;
                    return true;
                }

                // If we don't know the target type we cannot convert in this class. Either provide the target type or roll your own class implementing IParseStringToObject
                if (targetType is null)
                {
                    result = null;
                    return false;
                }

                var typeConverter = TypeDescriptor.GetConverter(targetType);

                if (!(typeConverter is null))
                {
                    result = typeConverter.ConvertFromString(null, culture ?? CultureInfo.InvariantCulture, input);
                    return true;
                }
                else 
                {
                    result = null;
                    return false;
                }
            }
            catch
            {
                result = null;
                return false;
            }
        }


        /// <summary>
        /// Tries to get the elemts <see cref="Type"/> for a given <see cref="IEnumerable"/>
        /// </summary>
        /// <param name="list">Any collection of elements</param>
        /// <returns>the elements <see cref="Type"/></returns>
        public Type GetElementType(IEnumerable list)
        {
            if (list is null)
            {
                return null;
            }

            var listType = list.GetType();

            if (listType.IsGenericType)
            {
                return listType.GetGenericArguments().FirstOrDefault();
            }
            else
            {
                return listType.GetElementType();
            }
        }
    }
}
