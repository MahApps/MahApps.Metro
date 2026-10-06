// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Shows a <see cref="ChildWindow"/> in the dialog container of a window, or in a panel of your own.
    /// </summary>
    public static class ChildWindowManager
    {
        private const string PART_MetroActiveDialogContainer = "PART_MetroActiveDialogContainer";
        private const string PART_MetroInactiveDialogsContainer = "PART_MetroInactiveDialogsContainer";

        /// <summary>
        /// How far the overlay of a child window reaches.
        /// </summary>
        public enum OverlayFillBehavior
        {
            /// <summary>
            /// The overlay covers the full window.
            /// </summary>
            FullWindow,

            /// <summary>
            /// The overlay covers only the window content, so the title bar stays usable.
            /// </summary>
            WindowContent
        }

        /// <summary>
        /// Counts, per container, the open child windows that keep it to the content row. The row
        /// goes back only when the last of them closes.
        /// </summary>
        private static readonly ConditionalWeakTable<Panel, ContentRowUse> ContentRowUses = new();

        private sealed class ContentRowUse
        {
            public int Count;
        }

        /// <summary>
        /// Shows the given child window in the dialog container of the control.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="overlayFillBehavior">The overlay fill behavior.</param>
        /// <returns>A task that completes when the child window is closed.</returns>
        /// <exception cref="InvalidOperationException">The control has no dialog container, or the child window is already in it.</exception>
        public static Task ShowChildWindowAsync(this Control control, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            return control.ShowChildWindowAsync<object>(dialog, overlayFillBehavior);
        }

        /// <summary>
        /// Shows the given child window in the dialog container of the control and returns its result once it is closed.
        /// </summary>
        /// <param name="control">The owning control with a container for the child window.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="overlayFillBehavior">The overlay fill behavior.</param>
        /// <returns>
        /// The <see cref="ChildWindow.ChildWindowResult"/> if it is a <typeparamref name="TResult"/>,
        /// else the <see cref="ChildWindow.ClosedBy"/> if that is one, else the default.
        /// </returns>
        /// <exception cref="InvalidOperationException">The control has no dialog container, or the child window is already in it.</exception>
        public static async Task<TResult?> ShowChildWindowAsync<TResult>(this Control control, ChildWindow dialog, OverlayFillBehavior overlayFillBehavior = OverlayFillBehavior.WindowContent)
        {
            control.Dispatcher.VerifyAccess();

            var container = control.Template?.FindName(PART_MetroActiveDialogContainer, control) as Grid
                            ?? control.Template?.FindName(PART_MetroInactiveDialogsContainer, control) as Grid;
            if (container is null)
            {
                throw new InvalidOperationException("The provided child window can not add, there is no container defined.");
            }

            if (container.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided child window is already visible in the specified window.");
            }

            var keepsToTheContentRow = overlayFillBehavior == OverlayFillBehavior.WindowContent;
            if (keepsToTheContentRow)
            {
                TakeContentRow(control, container);
            }

            try
            {
                return await OpenDialogAsync<TResult>(control, dialog, container).ConfigureAwait(true);
            }
            finally
            {
                if (keepsToTheContentRow)
                {
                    GiveBackContentRow(container);
                }
            }
        }

        /// <summary>
        /// Shows the given child window in the given container.
        /// </summary>
        /// <param name="control">The owning control.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns>A task that completes when the child window is closed.</returns>
        /// <exception cref="InvalidOperationException">There is no container, or the child window is already in it.</exception>
        public static Task ShowChildWindowAsync(this Control control, ChildWindow dialog, Panel container)
        {
            return control.ShowChildWindowAsync<object>(dialog, container);
        }

        /// <summary>
        /// Shows the given child window in the given container and returns its result once it is closed.
        /// </summary>
        /// <param name="control">The owning control.</param>
        /// <param name="dialog">A child window instance.</param>
        /// <param name="container">The container.</param>
        /// <returns>
        /// The <see cref="ChildWindow.ChildWindowResult"/> if it is a <typeparamref name="TResult"/>,
        /// else the <see cref="ChildWindow.ClosedBy"/> if that is one, else the default.
        /// </returns>
        /// <exception cref="InvalidOperationException">There is no container, or the child window is already in it.</exception>
        public static async Task<TResult?> ShowChildWindowAsync<TResult>(this Control control, ChildWindow dialog, Panel container)
        {
            control.Dispatcher.VerifyAccess();

            if (container is null)
            {
                throw new InvalidOperationException("The provided child window can not add, there is no container defined.");
            }

            if (container.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided child window is already visible in the specified window.");
            }

            return await OpenDialogAsync<TResult>(control, dialog, container).ConfigureAwait(true);
        }

        private static async Task<TResult?> OpenDialogAsync<TResult>(Control control, ChildWindow dialog, Panel container)
        {
            var tcs = new TaskCompletionSource<TResult?>();

            var hasParent = dialog.TryFindParent<Panel>() is not null;

            if (!dialog.IsOpen || !hasParent)
            {
                if (!hasParent)
                {
                    container.Children.Add(dialog);
                }

                void OnDialogClosingFinished(object sender, RoutedEventArgs args)
                {
                    // the event bubbles, so a child window shown inside this one passes its close up here
                    if (!ReferenceEquals(args.OriginalSource, dialog))
                    {
                        return;
                    }

                    dialog.ClosingFinished -= OnDialogClosingFinished;
                    container.Children.Remove(dialog);
                    UpdateIsAnyDialogOpen(control);
                    tcs.TrySetResult(dialog.ChildWindowResult is TResult result ? result : (dialog.ClosedBy is TResult closedBy ? closedBy : default));
                }

                dialog.ClosingFinished += OnDialogClosingFinished;

                UpdateIsAnyDialogOpen(control);

                dialog.SetCurrentValue(ChildWindow.IsOpenProperty, BooleanBoxes.TrueBox);
            }

            return await tcs.Task.ConfigureAwait(true);
        }

        /// <summary>
        /// Keeps <see cref="MetroWindow.IsAnyDialogOpen"/> right, by the same rule the
        /// <see cref="Dialogs.DialogManager"/> uses: open is whatever is in the active container.
        /// </summary>
        private static void UpdateIsAnyDialogOpen(Control control)
        {
            if (control is MetroWindow { metroActiveDialogContainer: { } activeContainer } window)
            {
                window.SetValue(MetroWindow.IsAnyDialogOpenPropertyKey, BooleanBoxes.Box(activeContainer.Children.Count > 0));

                // a WebBrowser or another hosted handle would be drawn over the child window
                window.RefreshHwndHosts();
            }
        }

        /// <summary>
        /// The z-index that puts the element over everything else in the container. The dialog
        /// manager and the child windows share these containers, so both go by this one rule.
        /// </summary>
        internal static int ZIndexOnTop(this Panel container, UIElement element)
        {
            return container.Children.OfType<UIElement>()
                            .Where(child => !ReferenceEquals(child, element))
                            .Select(Panel.GetZIndex)
                            .DefaultIfEmpty(-1)
                            .Max() + 1;
        }

        private static void TakeContentRow(Control control, Panel container)
        {
            var use = ContentRowUses.GetOrCreateValue(container);
            if (use.Count++ > 0)
            {
                return;
            }

            // the content of a MetroWindow is row 1 whether the dialogs go over the title bar or not
            var row = control is MetroWindow ? 1 : Grid.GetRow(container) + 1;
            container.SetCurrentValue(Grid.RowProperty, row);
            container.SetCurrentValue(Grid.RowSpanProperty, 1);
        }

        private static void GiveBackContentRow(Panel container)
        {
            if (!ContentRowUses.TryGetValue(container, out var use) || --use.Count > 0)
            {
                return;
            }

            // back to what the template and its triggers say, rather than to a value remembered
            // from before, which would stop following ShowDialogsOverTitleBar
            container.InvalidateProperty(Grid.RowProperty);
            container.InvalidateProperty(Grid.RowSpanProperty);
        }
    }
}
