// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class MultiSelectionComboBoxTests
    {
        [Test]
        public void ShouldGetElementTypeFromList()
        {
            var list = new List<MyTestClass>();
            var elementType = DefaultStringToObjectParser.Instance.GetElementType(list);

            Assert.That(elementType, Is.EqualTo(typeof(MyTestClass)));

            list.Add(new MyOtherTestClass());
            elementType = DefaultStringToObjectParser.Instance.GetElementType(list);

            Assert.That(elementType, Is.EqualTo(typeof(MyTestClass)));
        }
    }

    public class MyTestClass
    {
        public string TestField { get; set; } = "Test";
    }

    public class MyOtherTestClass : MyTestClass
    {
    }
}