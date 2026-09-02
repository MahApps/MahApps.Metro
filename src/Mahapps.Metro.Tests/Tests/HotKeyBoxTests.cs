// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class HotKeyBoxTests
    {
        private readonly IList<Action> detachHandlers = new List<Action>();

        private HotKeyBoxWindow? window;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<HotKeyBoxWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [SetUp]
        public void SetUp()
        {
            this.window?.TheHotKeyBox.ClearDependencyProperties();
        }

        [TearDown]
        public void TearDown()
        {
            // The window is shared by the whole fixture, so a recorder left attached would keep
            // collecting the events of every test that follows.
            foreach (var detach in this.detachHandlers)
            {
                detach();
            }

            this.detachHandlers.Clear();
        }

        [Test]
        public void ShouldNotRaiseHotKeyChangedForAnEqualHotKey()
        {
            Assert.That(this.window, Is.Not.Null);

            var recorded = this.RecordHotKeyChanges();

            this.window.TheHotKeyBox.HotKey = new HotKey(Key.S, ModifierKeys.Control);
            this.window.TheHotKeyBox.HotKey = new HotKey(Key.S, ModifierKeys.Control);

            Assert.That(recorded, Has.Count.EqualTo(1));
        }

        [Test]
        public void ShouldRaiseHotKeyChangedForADifferentHotKey()
        {
            Assert.That(this.window, Is.Not.Null);

            var recorded = this.RecordHotKeyChanges();

            this.window.TheHotKeyBox.HotKey = new HotKey(Key.S, ModifierKeys.Control);
            this.window.TheHotKeyBox.HotKey = new HotKey(Key.S, ModifierKeys.Control | ModifierKeys.Shift);
            this.window.TheHotKeyBox.HotKey = new HotKey(Key.O, ModifierKeys.Control | ModifierKeys.Shift);

            Assert.That(recorded, Has.Count.EqualTo(3));
        }

        [Test]
        public void ShouldRaiseHotKeyChangedWithTheOldAndTheNewHotKey()
        {
            Assert.That(this.window, Is.Not.Null);

            var recorded = this.RecordHotKeyChanges();

            this.window.TheHotKeyBox.HotKey = new HotKey(Key.S, ModifierKeys.Control);
            this.window.TheHotKeyBox.HotKey = new HotKey(Key.O, ModifierKeys.Control);
            this.window.TheHotKeyBox.HotKey = null;

            Assert.That(recorded, Has.Count.EqualTo(3));

            Assert.That(recorded[0].OldHotKey, Is.Null);
            Assert.That(recorded[0].NewHotKey, Is.EqualTo(new HotKey(Key.S, ModifierKeys.Control)));

            Assert.That(recorded[1].OldHotKey, Is.EqualTo(new HotKey(Key.S, ModifierKeys.Control)));
            Assert.That(recorded[1].NewHotKey, Is.EqualTo(new HotKey(Key.O, ModifierKeys.Control)));

            Assert.That(recorded[2].OldHotKey, Is.EqualTo(new HotKey(Key.O, ModifierKeys.Control)));
            Assert.That(recorded[2].NewHotKey, Is.Null);
        }

        [Test]
        public void ShouldUpdateTheTextWhenTheHotKeyChanges()
        {
            Assert.That(this.window, Is.Not.Null);

            var hotKey = new HotKey(Key.S, ModifierKeys.Control);

            this.window.TheHotKeyBox.HotKey = hotKey;
            Assert.That(this.window.TheHotKeyBox.Text, Is.EqualTo(hotKey.ToString()));

            this.window.TheHotKeyBox.HotKey = null;
            Assert.That(this.window.TheHotKeyBox.Text, Is.Empty);
        }

        private IList<(HotKey? OldHotKey, HotKey? NewHotKey)> RecordHotKeyChanges()
        {
            var recorded = new List<(HotKey? OldHotKey, HotKey? NewHotKey)>();
            this.window!.TheHotKeyBox.HotKeyChanged += OnHotKeyChanged;
            this.detachHandlers.Add(() => this.window!.TheHotKeyBox.HotKeyChanged -= OnHotKeyChanged);
            return recorded;

            void OnHotKeyChanged(object sender, RoutedPropertyChangedEventArgs<HotKey?> e)
            {
                recorded.Add((e.OldValue, e.NewValue));
            }
        }
    }
}
