﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    public class DialogCoordinator : IDialogCoordinator
    {
        /// <summary>
        /// Gets the default instance if the dialog coordinator, which can be injected into a view model.
        /// </summary>
        public static readonly IDialogCoordinator Instance = new DialogCoordinator();

        public Task<string?> ShowInputAsync(object context, string title, string message, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.ShowInputAsync(title, message, settings));
        }

        public string? ShowModalInputExternal(object context, string title, string message, MetroDialogSettings? metroDialogSettings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.ShowModalInputExternal(title, message, metroDialogSettings);
        }

        public Task<LoginDialogData?> ShowLoginAsync(object context, string title, string message, LoginDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.ShowLoginAsync(title, message, settings));
        }

        public LoginDialogData? ShowModalLoginExternal(object context, string title, string message, LoginDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.ShowModalLoginExternal(title, message, settings);
        }

        public Task<MessageDialogResult> ShowMessageAsync(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.ShowMessageAsync(title, message, style, settings));
        }

        public MessageDialogResult ShowModalMessageExternal(object context, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.ShowModalMessageExternal(title, message, style, settings);
        }

        public Task<ProgressDialogController> ShowProgressAsync(object context, string title, string message, bool isCancelable = false, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.ShowProgressAsync(title, message, isCancelable, settings));
        }

        public Task ShowMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.ShowMetroDialogAsync(dialog, settings));
        }

        public Task HideMetroDialogAsync(object context, BaseMetroDialog dialog, MetroDialogSettings? settings = null)
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.HideMetroDialogAsync(dialog, settings));
        }

        public Task<TDialog?> GetCurrentDialogAsync<TDialog>(object context)
            where TDialog : BaseMetroDialog
        {
            var metroWindow = GetMetroWindow(context);
            return metroWindow.Invoke(() => metroWindow.GetCurrentDialogAsync<TDialog>());
        }

        private static MetroWindow GetMetroWindow(object context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (DialogParticipation.IsRegistered(context) == false)
            {
                throw new InvalidOperationException($"The context `{context}` is not registered. Consider using the DialogParticipation.Register property in XAML to bind in the DataContext.");
            }

            var association = DialogParticipation.GetAssociation(context);
            var metroWindow = association.Invoke(() => Window.GetWindow(association) as MetroWindow);
            if (metroWindow is null)
            {
                throw new InvalidOperationException($"The context `{context}` is not inside a MetroWindow.");
            }

            return metroWindow;
        }
    }
}