// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Converters;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MathConverterTests
    {
        private readonly MathAddConverter mathAddConverter = new MathAddConverter();
        private readonly MathSubtractConverter mathSubtractConverter = new MathSubtractConverter();
        private readonly MathMultiplyConverter mathMultiplyConverter = new MathMultiplyConverter();
        private readonly MathDivideConverter mathDivideConverter = new MathDivideConverter();

        [Theory]
        [TestCase(10, 32, 42d)]
        [TestCase(10d, 32d, 42d)]
        [TestCase(10.5d, 31.5d, 42d)]
        [TestCase(52, -10, 42d)]
        public void MathAddConverter_should_add_values(object value, object parameter, object expectedValue)
        {
            Assert.That(this.mathAddConverter.Convert(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase(52, 10, 42d)]
        [TestCase(52d, 10d, 42d)]
        [TestCase(52.5d, 10.5d, 42d)]
        [TestCase(-32, 10, -42d)]
        public void MathSubtractConverter_should_substract_values(object value, object parameter, object expectedValue)
        {
            Assert.That(this.mathSubtractConverter.Convert(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase(7, 6, 42d)]
        [TestCase(7d, 6d, 42d)]
        [TestCase(0, 0, 0d)]
        [TestCase(42d, 0d, 0d)]
        public void MathMultiplyConverter_should_multiply_values(object value, object parameter, object expectedValue)
        {
            Assert.That(this.mathMultiplyConverter.Convert(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(expectedValue));
        }

        [Theory]
        [TestCase(42, 6, 7d)]
        [TestCase(42d, 7d, 6d)]
        [TestCase(0, 10, 0d)]
        [TestCase(42d, 0d, null)]
        [TestCase(42d, 0, null)]
        public void MathDivideConverter_should_multiply_values(object value, object parameter, object? expectedValue)
        {
            Assert.That(this.mathDivideConverter.Convert(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(expectedValue ?? Binding.DoNothing));
        }

        [Theory]
        [TestCase(10, 32)]
        [TestCase(10d, 32d)]
        [TestCase(10.5d, 31.5d)]
        [TestCase(52, -10)]
        public void MathConverter_should_not_convert_back(object value, object parameter)
        {
            Assert.That(this.mathAddConverter.ConvertBack(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(DependencyProperty.UnsetValue));
            Assert.That(this.mathSubtractConverter.ConvertBack(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(DependencyProperty.UnsetValue));
            Assert.That(this.mathMultiplyConverter.ConvertBack(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(DependencyProperty.UnsetValue));
            Assert.That(this.mathDivideConverter.ConvertBack(value, typeof(object), parameter, CultureInfo.InvariantCulture), Is.EqualTo(DependencyProperty.UnsetValue));
        }
    }
}