// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class AutoWatermarkTest
    {
        [Test]
        public async Task TestAutoWatermark()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<AutoWatermarkTestWindow>();

            window.Invoke(() =>
                {
                    var autoWatermark = "AutoWatermark";

                    Assert.That(window.TestTextBox.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                    Assert.That(window.TestTextBoxSubModel.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                    Assert.That(window.TestComboBox.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                    Assert.That(window.TestNumericUpDown.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                    Assert.That(window.TestDatePicker.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                    Assert.That(window.TestHotKeyBox.GetValue(TextBoxHelper.WatermarkProperty), Is.EqualTo(autoWatermark));
                });

            window.Close();
        }
    }
}