// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// Comparing two doubles for equality fails on values that only differ by the last bit or two, which
    /// is what arithmetic on them leaves behind. <see cref="DoubleUtil.AreClose"/> is the comparison the
    /// library uses instead.
    /// </summary>
    [TestFixture]
    public class DoubleUtilTests
    {
        [Test]
        public void TheSameValueShouldBeClose()
        {
            Assert.That(DoubleUtil.AreClose(1.5d, 1.5d), Is.True);
            Assert.That(DoubleUtil.AreClose(0d, 0d), Is.True);
            Assert.That(DoubleUtil.AreClose(-42.25d, -42.25d), Is.True);
        }

        [Test]
        public void WhatArithmeticLeavesBehindShouldBeClose()
        {
            // 0.1 + 0.2 is not 0.3 in binary, it is a few bits off, and every value that has been
            // through a calculation looks like this.
            var sum = 0.1d + 0.2d;
            Assert.That(sum, Is.Not.EqualTo(0.3d), "this is the comparison that fails");
            Assert.That(DoubleUtil.AreClose(sum, 0.3d), Is.True, "and this is the one that does not");
        }

        [Test]
        public void ValuesThatDifferShouldNotBeClose()
        {
            Assert.That(DoubleUtil.AreClose(1d, 1.0001d), Is.False);
            Assert.That(DoubleUtil.AreClose(0d, 0.0001d), Is.False);
            Assert.That(DoubleUtil.AreClose(-1d, 1d), Is.False);
        }

        [Test]
        public void TheToleranceShouldGrowWithTheValues()
        {
            // The comparison is relative, so the same absolute gap counts as close between two large
            // values and as a difference between two small ones.
            Assert.That(DoubleUtil.AreClose(1e12d, 1e12d + 0.0001d), Is.True, "next to a trillion this gap is noise");
            Assert.That(DoubleUtil.AreClose(1d, 1d + 0.0001d), Is.False, "next to one it is not");
        }

        [Test]
        public void InfinitiesShouldOnlyBeCloseToThemselves()
        {
            Assert.That(DoubleUtil.AreClose(double.PositiveInfinity, double.PositiveInfinity), Is.True, "the epsilon check cannot handle these, so they are compared directly");
            Assert.That(DoubleUtil.AreClose(double.NegativeInfinity, double.NegativeInfinity), Is.True);
            Assert.That(DoubleUtil.AreClose(double.PositiveInfinity, double.NegativeInfinity), Is.False);
            Assert.That(DoubleUtil.AreClose(double.PositiveInfinity, double.MaxValue), Is.False);
            Assert.That(DoubleUtil.AreClose(double.NegativeInfinity, double.MinValue), Is.False);
            Assert.That(DoubleUtil.AreClose(0d, double.NegativeInfinity), Is.False, "whichever side the infinity is on");
        }

        [Test]
        public void NotANumberShouldBeCloseToNothing()
        {
            Assert.That(DoubleUtil.AreClose(double.NaN, double.NaN), Is.False, "NaN equals nothing, itself included");
            Assert.That(DoubleUtil.AreClose(double.NaN, 1d), Is.False);
        }
    }
}
