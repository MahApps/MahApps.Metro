// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        private const string DefaultAffirmativeButtonText = "OK";
        private const string DefaultNegativeButtonText = "Cancel";

        public MetroDialogSettings()
        {
        }

        public MetroDialogSettings(MetroDialogSettings? source)
            : this()
        {
            if (source is null)
            {
                return;
            }

            this.OwnerCanCloseWithDialog = source.OwnerCanCloseWithDialog;

            this.AffirmativeButtonText = source.AffirmativeButtonText;
            this.NegativeButtonText = source.NegativeButtonText;
            this.DefaultText = source.DefaultText;
            this.FirstAuxiliaryButtonText = source.FirstAuxiliaryButtonText;
            this.SecondAuxiliaryButtonText = source.SecondAuxiliaryButtonText;

            this.ColorScheme = source.ColorScheme;
            this.CustomResourceDictionary = source.CustomResourceDictionary;

            this.AnimateShow = source.AnimateShow;
            this.AnimateHide = source.AnimateHide;

            this.MaximumBodyHeight = source.MaximumBodyHeight;

            this.DefaultButtonFocus = source.DefaultButtonFocus;
            this.CancellationToken = source.CancellationToken;
            this.DialogTitleFontSize = source.DialogTitleFontSize;
            this.DialogMessageFontSize = source.DialogMessageFontSize;
            this.DialogButtonFontSize = source.DialogButtonFontSize;
            this.DialogResultOnCancel = source.DialogResultOnCancel;

            this.Icon = source.Icon;
            this.IconTemplate = source.IconTemplate;
        }

        /// <summary>
        /// Gets or sets whether the owner of the dialog can be closed.
        /// </summary>
        public bool OwnerCanCloseWithDialog { get; set; }

        /// <summary>
        /// Gets or sets the text used for the Affirmative button. For example: "OK" or "Yes".
        /// </summary>
        public string AffirmativeButtonText { get; set; } = DefaultAffirmativeButtonText;

        /// <summary>
        /// Enable or disable dialog hiding animation
        /// "True" - play hiding animation.
        /// "False" - skip hiding animation.
        /// </summary>
        public bool AnimateHide { get; set; } = true;

        /// <summary>
        /// Enable or disable dialog showing animation.
        /// "True" - play showing animation.
        /// "False" - skip showing animation.
        /// </summary>
        public bool AnimateShow { get; set; } = true;

        /// <summary>
        /// Gets or sets a token to cancel the dialog.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        /// Gets or sets whether the metro dialog should use the default black/white appearance (theme) or try to use the current accent.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; } = MetroDialogColorScheme.Theme;

        /// <summary>
        /// Gets or sets a custom resource dictionary which can contains custom styles, brushes or something else.
        /// </summary>
        public ResourceDictionary? CustomResourceDictionary { get; set; }

        /// <summary>
        /// Gets or sets which button should be focused by default
        /// </summary>
        public MessageDialogResult DefaultButtonFocus { get; set; } = MessageDialogResult.Negative;

        /// <summary>
        /// Gets or sets the default text for <see cref="InputDialog"/>.
        /// </summary>
        public string DefaultText { get; set; } = "";

        /// <summary>
        /// Gets or sets the size of the dialog message font.
        /// </summary>
        /// <value>
        /// The size of the dialog message font.
        /// </value>
        public double DialogMessageFontSize { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the size of the dialog button font.
        /// </summary>
        /// <value>
        /// The size of the dialog button font.
        /// </value>
        public double DialogButtonFontSize { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the message dialog result when the user cancelled the dialog with 'ESC' key
        /// </summary>
        /// <remarks>
        /// If the value is <see langword="null"/> the default behavior is determined by the <see cref="MessageDialogStyle"/>.
        /// </remarks>
        public MessageDialogResult? DialogResultOnCancel { get; set; } = null;

        /// <summary>
        /// Gets or sets the size of the dialog title font.
        /// </summary>
        /// <value>
        /// The size of the dialog title font.
        /// </value>
        public double DialogTitleFontSize { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the text used for the first auxiliary button.
        /// </summary>
        public string? FirstAuxiliaryButtonText { get; set; }

        /// <summary>
        /// Gets or sets the maximum height. (Default is unlimited height, <a href="http://msdn.microsoft.com/de-de/library/system.double.nan">Double.NaN</a>)
        /// </summary>
        public double MaximumBodyHeight { get; set; } = double.NaN;

        /// <summary>
        /// Gets or sets the text used for the Negative button. For example: "Cancel" or "No".
        /// </summary>
        public string NegativeButtonText { get; set; } = DefaultNegativeButtonText;

        /// <summary>
        /// Gets or sets the text used for the second auxiliary button.
        /// </summary>
        public string? SecondAuxiliaryButtonText { get; set; }

        /// <summary>
        /// Gets or sets the content used for the Icon ContentPresenter.
        /// </summary>
        public object? Icon { get; set; }

        /// <summary>
        /// Gets or sets the DataTemplate used for the Icon ContentPresenter.
        /// </summary>
        public DataTemplate? IconTemplate { get; set; }
    }
}