// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Converters;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class MathConverterTests
    {
        private readonly MathAddConverter mathAddConverter = new MathAddConverter();
        private readonly MathSubtractConverter mathSubtractConverter = new MathSubtractConverter();
        private readonly MathMultiplyConverter mathMultiplyConverter = new MathMultiplyConverter();
        private readonly MathDivideConverter mathDivideConverter = new MathDivideConverter();

        [Theory]
        [InlineData(10, 32, 42d)]
        [InlineData(10d, 32d, 42d)]
        [InlineData(10.5d, 31.5d, 42d)]
        [InlineData(52, -10, 42d)]
        public void MathAddConverter_should_add_values(object value1, object value2, object expectedValue)
        {
            Assert.Equal(expectedValue, this.mathAddConverter.Convert(value1, null, value2, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(52, 10, 42d)]
        [InlineData(52d, 10d, 42d)]
        [InlineData(52.5d, 10.5d, 42d)]
        [InlineData(-32, 10, -42d)]
        public void MathSubtractConverter_should_substract_values(object value1, object value2, object expectedValue)
        {
            Assert.Equal(expectedValue, this.mathSubtractConverter.Convert(value1, null, value2, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(7, 6, 42d)]
        [InlineData(7d, 6d, 42d)]
        [InlineData(0, 0, 0d)]
        [InlineData(42d, 0d, 0d)]
        public void MathMultiplyConverter_should_multiply_values(object value1, object value2, object expectedValue)
        {
            Assert.Equal(expectedValue, this.mathMultiplyConverter.Convert(value1, null, value2, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(42, 6, 7d)]
        [InlineData(42d, 7d, 6d)]
        [InlineData(0, 10, 0d)]
        [InlineData(42d, 0d, null)]
        [InlineData(42d, 0, null)]
        public void MathDivideConverter_should_multiply_values(object value1, object value2, object expectedValue)
        {
            Assert.Equal(expectedValue ?? Binding.DoNothing, this.mathDivideConverter.Convert(value1, null, value2, CultureInfo.InvariantCulture));
        }

        [Theory]
        [InlineData(10, 32)]
        [InlineData(10d, 32d)]
        [InlineData(10.5d, 31.5d)]
        [InlineData(52, -10)]
        public void MathConverter_should_not_convert_back(object value1, object value2)
        {
            Assert.Equal(DependencyProperty.UnsetValue, this.mathAddConverter.ConvertBack(value1, (Type)null, value2, CultureInfo.InvariantCulture));
            Assert.Equal(DependencyProperty.UnsetValue, this.mathSubtractConverter.ConvertBack(value1, (Type)null, value2, CultureInfo.InvariantCulture));
            Assert.Equal(DependencyProperty.UnsetValue, this.mathMultiplyConverter.ConvertBack(value1, (Type)null, value2, CultureInfo.InvariantCulture));
            Assert.Equal(DependencyProperty.UnsetValue, this.mathDivideConverter.ConvertBack(value1, (Type)null, value2, CultureInfo.InvariantCulture));
        }
    }
}