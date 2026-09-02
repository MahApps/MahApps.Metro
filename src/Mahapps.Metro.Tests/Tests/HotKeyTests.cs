// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    [TestFixture]
    public class HotKeyTests
    {
        [Test]
        public void ShouldTreatSameKeyAndModifierKeysAsEqual()
        {
            var first = new HotKey(Key.S, ModifierKeys.Control);
            var second = new HotKey(Key.S, ModifierKeys.Control);

            var areEqual = first == second;
            var areNotEqual = first != second;

            Assert.That(areEqual, Is.True);
            Assert.That(areNotEqual, Is.False);
        }

        [Test]
        public void ShouldNotTreatDifferentKeysAsEqual()
        {
            var first = new HotKey(Key.S, ModifierKeys.Control);
            var second = new HotKey(Key.O, ModifierKeys.Control);

            var areEqual = first == second;
            var areNotEqual = first != second;

            Assert.That(areEqual, Is.False);
            Assert.That(areNotEqual, Is.True);
        }

        [Test]
        public void ShouldNotTreatDifferentModifierKeysAsEqual()
        {
            var first = new HotKey(Key.S, ModifierKeys.Control);
            var second = new HotKey(Key.S, ModifierKeys.Control | ModifierKeys.Shift);

            var areEqual = first == second;
            var areNotEqual = first != second;

            Assert.That(areEqual, Is.False);
            Assert.That(areNotEqual, Is.True);
        }

        [Test]
        public void ShouldCompareAgainstNullWithoutThrowing()
        {
            var hotKey = new HotKey(Key.S, ModifierKeys.Control);
            HotKey? nothing = null;
            HotKey? alsoNothing = null;

            var bothNullAreEqual = nothing == alsoNothing;
            var nullOnTheRightIsEqual = hotKey == nothing;
            var nullOnTheLeftIsEqual = nothing == hotKey;

            Assert.That(bothNullAreEqual, Is.True);
            Assert.That(nullOnTheRightIsEqual, Is.False);
            Assert.That(nullOnTheLeftIsEqual, Is.False);
            Assert.That(hotKey.Equals(null), Is.False);
        }

        [Test]
        public void ShouldTreatTheSameInstanceAsEqual()
        {
            var hotKey = new HotKey(Key.S, ModifierKeys.Control);
            var sameInstance = hotKey;

            var areEqual = hotKey == sameInstance;

            Assert.That(areEqual, Is.True);
            Assert.That(hotKey.Equals(sameInstance), Is.True);
        }

        [Test]
        public void ShouldReturnTheSameHashCodeForEqualHotKeys()
        {
            var first = new HotKey(Key.S, ModifierKeys.Control);
            var second = new HotKey(Key.S, ModifierKeys.Control);

            Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
        }
    }
}
