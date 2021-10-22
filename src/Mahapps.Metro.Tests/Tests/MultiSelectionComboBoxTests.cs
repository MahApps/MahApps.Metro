// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests.Tests
{
    public class MultiSelectionComboBoxTests : AutomationTestBase
    {
        [Fact]
        [DisplayTestMethodName]
        public void ShouldGetElementTypeFromList()
        {
            var list = new List<MyTestClass>();
            var elementType = DefaultStringToObjectParser.Instance.GetElementType(list);

            Assert.Equal(typeof(MyTestClass), elementType);

            list.Add(new MyOtherTestClass());
            elementType = DefaultStringToObjectParser.Instance.GetElementType(list);

            Assert.Equal(typeof(MyTestClass), elementType);
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