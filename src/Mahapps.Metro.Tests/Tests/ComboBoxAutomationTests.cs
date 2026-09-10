// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-3998: the box an editable combo box is typed into is a focusable element of its own in the
    /// automation tree, and one without a name is what an accessibility check trips over. WPF's own
    /// template hands it the name of the combo box, and so should ours.
    /// </summary>
    [TestFixture]
    public class ComboBoxAutomationTests
    {
        private TestWindow? window;
        private ResourceDictionary? dictionary;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
            this.dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.ComboBox.xaml", UriKind.Absolute) };
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("An editable combo box gives its name to the box that is typed into.")]
        public void TheEditableBoxOfAComboBoxCarriesTheNameOfTheComboBox()
        {
            Assert.That(this.window, Is.Not.Null);

            var comboBox = new ComboBox { Width = 200, IsEditable = true };
            comboBox.SetValue(FrameworkElement.StyleProperty, this.dictionary!["MahApps.Styles.ComboBox"]);
            comboBox.Items.Add("Beam me up...");
            AutomationProperties.SetName(comboBox, "where to");

            this.Show(comboBox);

            Assert.That(NameOfTheEditableBox(comboBox), Is.EqualTo("where to"));
        }

        [Test]
        [Description("The same holds for the multi selection combo box, which has a box of its own.")]
        public void TheEditableBoxOfAMultiSelectionComboBoxCarriesTheNameOfTheComboBox()
        {
            Assert.That(this.window, Is.Not.Null);

            var comboBox = new MultiSelectionComboBox { Width = 200, IsEditable = true };
            comboBox.Items.Add("Beam me up...");
            AutomationProperties.SetName(comboBox, "where to");

            this.Show(comboBox);

            Assert.That(NameOfTheEditableBox(comboBox), Is.EqualTo("where to"));
        }

        private void Show(Control control)
        {
            this.window!.Content = control;
            this.window.UpdateLayout();
            control.UpdateLayout();

            // Let the box finish loading before a test reaches into its template.
            ClipAssert.Pump();
            Assert.That(control.IsLoaded, Is.True, "the box should be loaded before a test looks at it");
        }

        /// <summary>What an accessibility check reads off the box that is typed into.</summary>
        private static string? NameOfTheEditableBox(Control comboBox)
        {
            var editableBox = comboBox.Template?.FindName("PART_EditableTextBox", comboBox) as TextBox;
            Assert.That(editableBox, Is.Not.Null, "an editable combo box should have a box to type into");

            var peer = UIElementAutomationPeer.CreatePeerForElement(editableBox!);
            Assert.That(peer, Is.Not.Null, "and that box should be in the automation tree");

            return peer!.GetName();
        }
    }
}
