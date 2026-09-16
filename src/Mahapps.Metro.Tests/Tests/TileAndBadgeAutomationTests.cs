// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Automation.Peers;
using MahApps.Metro.Controls;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4454: what a screen reader is told about a tile and about a badge. A tile carries its caption
    /// in a title of its own, and a badge is a mark on whatever it wraps, so neither of them reaches a
    /// client through the content alone.
    /// </summary>
    [TestFixture]
    public class TileAndBadgeAutomationTests
    {
        [Test]
        [Description("A tile with nothing but a title is named after it, instead of being read out as a button with no caption.")]
        public void ATileIsNamedByItsTitle()
        {
            var tile = new Tile { Title = "Messages" };

            Assert.That(PeerOf(tile).GetName(), Is.EqualTo("Messages"));
        }

        [Test]
        [Description("Content is what a button is named after, so it keeps the upper hand over the title.")]
        public void TheContentOfATileComesFirst()
        {
            var tile = new Tile { Title = "Photos", Content = "42 of them" };

            Assert.That(PeerOf(tile).GetName(), Is.EqualTo("42 of them"));
        }

        [Test]
        [Description("A name handed over by hand beats both of them.")]
        public void ANameOfItsOwnBeatsTheTitle()
        {
            var tile = new Tile { Title = "Messages" };
            System.Windows.Automation.AutomationProperties.SetName(tile, "Unread messages");

            Assert.That(PeerOf(tile).GetName(), Is.EqualTo("Unread messages"));
        }

        [Test]
        [Description("A tile says what it is, rather than passing itself off as a plain button.")]
        public void ATileSaysWhatItIs()
        {
            Assert.That(PeerOf(new Tile()).GetClassName(), Is.EqualTo("Tile"));
        }

        [Test]
        [Description("The badge is a mark on the thing it wraps, which is what an item status is for.")]
        public void TheBadgeIsToldAsTheStatusOfWhatItMarks()
        {
            var badged = new Badged { Badge = "3" };

            Assert.That(PeerOf(badged).GetItemStatus(), Is.EqualTo("3"));
        }

        [Test]
        [Description("Nothing on the badge, nothing to say.")]
        public void WithoutABadgeThereIsNothingToSay()
        {
            var badged = new Badged();

            Assert.That(PeerOf(badged).GetItemStatus(), Is.Empty);
        }

        [Test]
        [Description("A badge holds whatever it is given, and a client is told the plain text of it.")]
        public void ABadgeOfAnythingElseIsToldAsText()
        {
            var badged = new Badged { Badge = 12 };

            Assert.That(PeerOf(badged).GetItemStatus(), Is.EqualTo("12"));
        }

        [Test]
        [Description("What wraps something else is a group, and it says what it is.")]
        public void ABadgeSaysWhatItIs()
        {
            var peer = PeerOf(new Badged());

            Assert.That(peer.GetClassName(), Is.EqualTo("Badged"));
            Assert.That(peer.GetAutomationControlType(), Is.EqualTo(AutomationControlType.Group));
        }

        private static AutomationPeer PeerOf(System.Windows.UIElement element)
        {
            var peer = UIElementAutomationPeer.CreatePeerForElement(element);

            Assert.That(peer, Is.Not.Null, "the control should make a peer of its own");

            return peer!;
        }
    }
}
