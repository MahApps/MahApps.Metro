using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;
using MahApps.Metro.Tests.TestHelpers;
using Xunit;

namespace MahApps.Metro.Tests
{
    public class NumericUpDownFixture : IAsyncLifetime
    {
        public NumericUpDownWindow Window { get; private set; }

        public TextBox TextBox { get; private set; }

        public RepeatButton NumUp { get; private set; }

        public RepeatButton NumDown { get; private set; }

        /// <summary>
        /// Called immediately after the class has been created, before it is used.
        /// </summary>
        public async Task InitializeAsync()
        {
            await TestHost.SwitchToAppThread();

            this.Window = await WindowHelpers.CreateInvisibleWindowAsync<NumericUpDownWindow>().ConfigureAwait(false);
            this.TextBox = this.Window.TheNUD.FindChild<TextBox>();
            this.NumUp = this.Window.TheNUD.FindChild<RepeatButton>("PART_NumericUp");
            this.NumDown = this.Window.TheNUD.FindChild<RepeatButton>("PART_NumericDown");
        }

        public static IEnumerable<DependencyProperty> EnumerateDependencyProperties(DependencyObject obj)
        {
            if (obj != null)
            {
                LocalValueEnumerator lve = obj.GetLocalValueEnumerator();
                while (lve.MoveNext())
                {
                    yield return lve.Current.Property;
                }
            }
        }

        public async Task PrepareForTestAsync()
        {
            await TestHost.SwitchToAppThread();

            foreach (var property in EnumerateDependencyProperties(this.Window.TheNUD))
            {
                this.Window.TheNUD.ClearValue(property);
            }
        }

        /// <summary>
        /// Called when an object is no longer needed. Called just before <see cref="M:System.IDisposable.Dispose" />
        /// if the class also implements that.
        /// </summary>
        public Task DisposeAsync()
        {
            this.TextBox = null;
            this.NumUp = null;
            this.NumDown = null;
            this.Window = null;

#if NET452
            return Task.Delay(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}