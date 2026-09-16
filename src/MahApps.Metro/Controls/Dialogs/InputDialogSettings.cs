// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// What an <see cref="InputDialog"/> is given beyond what every dialog is given.
    /// </summary>
    public class InputDialogSettings : MetroDialogSettings
    {
        public InputDialogSettings()
        {
        }

        public InputDialogSettings(MetroDialogSettings? source)
            : base(source)
        {
            if (source is InputDialogSettings settings)
            {
                this.ValidateInput = settings.ValidateInput;
            }
        }

        /// <summary>
        /// Gets or sets what decides whether the dialog can be left with the text that was typed.
        /// </summary>
        /// <remarks>
        /// Called with the text in the field every time the affirmative button is pressed. Return
        /// what is wrong with it to keep the dialog where it is and put that on the field, or
        /// <see langword="null"/> to let it close. Left unset, anything is accepted, which is what
        /// the dialog has always done.
        /// </remarks>
        /// <example>
        /// <code>
        /// var name = await this.ShowInputAsync("Name", "What should it be called?",
        ///                                      new InputDialogSettings
        ///                                      {
        ///                                          ValidateInput = input => string.IsNullOrWhiteSpace(input) ? "a name is needed" : null
        ///                                      });
        /// </code>
        /// </example>
        public Func<string?, string?>? ValidateInput { get; set; }
    }
}
