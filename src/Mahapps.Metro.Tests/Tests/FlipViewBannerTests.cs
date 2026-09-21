// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4447: the banner of a FlipView showed the text it starts out with instead of the one the
    /// item asked for, as soon as the items came from an ItemsSource rather than being written out.
    /// </summary>
    [TestFixture]
    public class FlipViewBannerTests
    {
        private TestWindow? window;

        public sealed class Slide
        {
            public Slide(string caption)
            {
                this.Caption = caption;
            }

            public string Caption { get; }
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            this.window = await WindowHelpers.CreateInvisibleWindowAsync<TestWindow>().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.window?.Close();
            this.window = null;
        }

        [Test]
        [Description("Items written out one by one carry their own banner, and the one that is showing says what the banner reads.")]
        public void ItemsWrittenOutCarryTheirOwnBanner()
        {
            var flipView = this.Show();

            flipView.Items.Add(new FlipViewItem { BannerText = "first", Content = "one" });
            flipView.Items.Add(new FlipViewItem { BannerText = "second", Content = "two" });
            this.Settle();

            flipView.SelectedIndex = 0;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("first"), "the first item should have named the banner");

            flipView.SelectedIndex = 1;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("second"), "and flipping on should hand it over to the second");
        }

        [Test]
        [Description("A FlipViewItem in the item template is the only place an item out of a source can name a banner, since the control shows the selected item in a presenter and never wraps anything in a container of its own.")]
        public void ItemsOutOfASourceCarryTheBannerTheirTemplateNames()
        {
            var flipView = this.Show();

            flipView.ItemTemplate = TemplateNaming("{Binding Caption}");
            flipView.ItemsSource = new[] { new Slide("first"), new Slide("second") };
            this.Settle();

            flipView.SelectedIndex = 0;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("first"), "the first item should have named the banner");

            flipView.SelectedIndex = 1;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("second"), "and flipping on should hand it over to the second");
        }

        [Test]
        [Description("An empty banner is a banner somebody asked for, so it wins over the text a FlipViewItem starts out with rather than being read as nothing having been said.")]
        public void AnEmptyBannerIsStillABannerSomebodyAskedFor()
        {
            var flipView = this.Show();

            flipView.ItemTemplate = TemplateNaming(string.Empty);
            flipView.ItemsSource = new[] { new Slide("first") };
            this.Settle();

            flipView.SelectedIndex = 0;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo(string.Empty));
        }

        /// <summary>The template out of the report: a FlipViewItem inside it, naming a banner.</summary>
        private static DataTemplate TemplateNaming(string banner)
        {
            return (DataTemplate)XamlReader.Parse(
                "<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""
                + " xmlns:mah=\"http://metro.mahapps.com/winfx/xaml/controls\">"
                + $"<mah:FlipViewItem BannerText=\"{banner}\">"
                + "<TextBlock Text=\"{Binding Caption}\" />"
                + "</mah:FlipViewItem></DataTemplate>");
        }

        [Test]
        [Description("A banner named on the control itself belongs to the whole view, so items that say nothing about a banner leave it alone rather than handing over the text a FlipViewItem starts out with.")]
        public void ItemsThatNameNoBannerLeaveTheOneOnTheControl()
        {
            var flipView = this.Show();

            flipView.BannerText = "the whole thing";
            flipView.ItemsSource = new[] { new Slide("first"), new Slide("second") };
            this.Settle();

            flipView.SelectedIndex = 0;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("the whole thing"), "nobody named a banner for the item");

            flipView.SelectedIndex = 1;
            this.Settle();

            Assert.That(flipView.BannerText, Is.EqualTo("the whole thing"), "and flipping on changes nothing about that");
        }

        private FlipView Show()
        {
            Assert.That(this.window, Is.Not.Null);

            var flipView = new FlipView { Width = 400, Height = 300 };

            this.window!.Content = flipView;
            this.Settle();

            Assert.That(flipView.IsLoaded, Is.True, "the control should be up before a test looks at it");

            return flipView;
        }

        private void Settle()
        {
            this.window!.UpdateLayout();
            ClipAssert.Pump();
        }
    }
}
