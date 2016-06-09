namespace MahApps.Metro.Controls.Dialogs
{
    using System;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        public MetroDialogSettings()
        {
            AffirmativeButtonText = "OK";
            NegativeButtonText = "Cancel";

            ColorScheme = MetroDialogColorScheme.Theme;
            AnimateShow = AnimateHide = true;

            MaximumBodyHeight = Double.NaN;

            DefaultText = "";

            CancellationToken = CancellationToken.None;
        }

        /// <summary>
        /// Gets/sets the text used for the Affirmative button. For example: "OK" or "Yes".
        /// </summary>
        public string AffirmativeButtonText { get; set; }
        /// <summary>
        /// Gets/sets the text used for the Negative button. For example: "Cancel" or "No".
        /// </summary>
        public string NegativeButtonText { get; set; }
        public string FirstAuxiliaryButtonText { get; set; }
        public string SecondAuxiliaryButtonText { get; set; }

        /// <summary>
        /// Gets/sets whether the metro dialog should use the default black/white appearance (theme) or try to use the current accent.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; }

        /// <summary>
        /// Enable/disable dialog showing animation.
        /// "True" - play showing animation.
        /// "False" - skip showing animation.
        /// </summary>
        public bool AnimateShow { get; set; }

        /// <summary>
        /// Enable/disable dialog hiding animation
        /// "True" - play hiding animation.
        /// "False" - skip hiding animation.
        /// </summary>
        public bool AnimateHide { get; set; }

        /// <summary>
        /// Gets/sets the default text( just the inputdialog needed)
        /// </summary>
        public string DefaultText { get; set; }

        /// <summary>
        /// Gets/sets the maximum height. (Default is unlimited height, <a href="http://msdn.microsoft.com/de-de/library/system.double.nan">Double.NaN</a>)
        /// </summary>
        public double MaximumBodyHeight { get; set; }

        /// <summary>
        /// Gets/sets the token to cancel the dialog.
        /// </summary>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// Gets/sets a custom resource dictionary which can contains custom styles, brushes or something else.
        /// </summary>
        public ResourceDictionary CustomResourceDictionary { get; set; }

        /// <summary>
        /// If set, stops standard resource dictionaries being applied to the dialog.
        /// </summary>
        public bool SuppressDefaultResources { get; set; }
    }
}