// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Tests.TestHelpers;
using MahApps.Metro.Tests.Views;
using NUnit.Framework;

namespace MahApps.Metro.Tests.Tests
{
    /// <summary>
    /// GH-4573: a registration was filed in a static dictionary that only ever lost an entry when the
    /// attached property changed. Closing a window does not change it, so a window that came up a few
    /// times left a view model and an element behind on every round.
    /// </summary>
    [TestFixture]
    public class DialogParticipationTests
    {
        private sealed class TheContext
        {
            public override string ToString() => "the context";
        }

        [Test]
        [Description("Nobody cleared the registration, and it is gone anyway once the context is.")]
        public void AForgottenRegistrationIsNotHeldOn()
        {
            var registration = Register();

            Collect();

            Assert.That(registration.Context.IsAlive, Is.False, "a registration should not keep the context it was made for");
            Assert.That(registration.Element.IsAlive, Is.False, "nor the element it was made on");
        }

        [Test]
        [Description("As long as somebody else holds the context, the registration is there to be used.")]
        public async Task ARegistrationOutlivesACollection()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var context = new TheContext();

            try
            {
                DialogParticipation.SetRegister(window, context);

                Collect();

                var dialog = (CustomDialog)window.Resources["CustomDialog"];
                await DialogCoordinator.Instance.ShowMetroDialogAsync(context, dialog).Within("the dialog to show");

                Assert.That(await DialogCoordinator.Instance.GetCurrentDialogAsync<CustomDialog>(context), Is.EqualTo(dialog), "the context should still lead to its window");

                await DialogCoordinator.Instance.HideMetroDialogAsync(context, dialog).Within("the dialog to hide");
            }
            finally
            {
                DialogParticipation.SetRegister(window, null);
                window.Close();
            }
        }

        [Test]
        [Description("Registered a second time, a context belongs to the window it was registered on last.")]
        public async Task ARegistrationMovesToWhereItWasMadeLast()
        {
            var first = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var second = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var context = new TheContext();

            try
            {
                DialogParticipation.SetRegister(first, context);
                DialogParticipation.SetRegister(second, context);

                var dialog = (CustomDialog)second.Resources["CustomDialog"];
                await DialogCoordinator.Instance.ShowMetroDialogAsync(context, dialog).Within("the dialog to show");

                Assert.That(await second.GetCurrentDialogAsync<CustomDialog>(), Is.EqualTo(dialog));
                Assert.That(await first.GetCurrentDialogAsync<CustomDialog>(), Is.Null, "the window it was registered on before should have nothing to show");

                await DialogCoordinator.Instance.HideMetroDialogAsync(context, dialog).Within("the dialog to hide");
            }
            finally
            {
                DialogParticipation.SetRegister(second, null);
                DialogParticipation.SetRegister(first, null);
                first.Close();
                second.Close();
            }
        }

        [Test]
        [Description("A context whose registration was cleared is turned away, the way an unknown one always was.")]
        public async Task AClearedRegistrationIsTurnedAway()
        {
            var window = await WindowHelpers.CreateInvisibleWindowAsync<DialogWindow>();
            var context = new TheContext();

            try
            {
                DialogParticipation.SetRegister(window, context);
                DialogParticipation.SetRegister(window, null);

                Assert.That(() => DialogCoordinator.Instance.GetCurrentDialogAsync<BaseMetroDialog>(context), Throws.InvalidOperationException);
            }
            finally
            {
                window.Close();
            }
        }

        /// <summary>
        /// Registers a context the way a window does, on an element that holds it as its DataContext,
        /// and keeps nothing but a weak reference to either of them.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (WeakReference Context, WeakReference Element) Register()
        {
            var context = new TheContext();
            var element = new ContentControl { DataContext = context };

            DialogParticipation.SetRegister(element, context);

            return (new WeakReference(context), new WeakReference(element));
        }

        /// <summary>
        /// Collects, which is the only way to ask whether something is still being held. Calling this
        /// is bad advice anywhere but in a test about lifetimes, which is why this file is left out
        /// of the analysis in .codacy.yml.
        /// </summary>
        private static void Collect()
        {
            for (var i = 0; i < 3; i++)
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
