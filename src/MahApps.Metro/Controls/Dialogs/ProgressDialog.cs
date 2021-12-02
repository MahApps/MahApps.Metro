// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    [TemplatePart(Name = nameof(PART_ProgressBar), Type = typeof(MetroProgressBar))]
    [TemplatePart(Name = nameof(PART_NegativeButton), Type = typeof(Button))]
    public class ProgressDialog : BaseMetroDialog
    {
        #region Controls

        internal Button? PART_NegativeButton;
        private MetroProgressBar? PART_ProgressBar;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_NegativeButton = this.GetTemplateChild(nameof(this.PART_NegativeButton)) as Button ?? throw new MissingRequiredTemplatePartException(this, nameof(this.PART_NegativeButton));
            this.PART_ProgressBar = this.GetTemplateChild(nameof(this.PART_ProgressBar)) as MetroProgressBar ?? throw new MissingRequiredTemplatePartException(this, nameof(this.PART_ProgressBar));
        }

        #endregion Controls

        #region DependecyProperties

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message),
                                          typeof(string),
                                          typeof(ProgressDialog),
                                          new PropertyMetadata(default(string)));

        public string? Message
        {
            get => (string?)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="IsCancelable"/> dependency property.</summary>
        public static readonly DependencyProperty IsCancelableProperty
            = DependencyProperty.Register(nameof(IsCancelable),
                                          typeof(bool),
                                          typeof(ProgressDialog),
                                          new PropertyMetadata(BooleanBoxes.FalseBox));

        public bool IsCancelable
        {
            get => (bool)this.GetValue(IsCancelableProperty);
            set => this.SetValue(IsCancelableProperty, BooleanBoxes.Box(value));
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty
            = DependencyProperty.Register(nameof(NegativeButtonText),
                                          typeof(string),
                                          typeof(ProgressDialog),
                                          new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)this.GetValue(NegativeButtonTextProperty);
            set => this.SetValue(NegativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="ProgressBarForeground"/> dependency property.</summary>
        public static readonly DependencyProperty ProgressBarForegroundProperty
            = DependencyProperty.Register(nameof(ProgressBarForeground),
                                          typeof(Brush),
                                          typeof(ProgressDialog),
                                          new FrameworkPropertyMetadata(default(Brush), FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush? ProgressBarForeground
        {
            get => (Brush?)this.GetValue(ProgressBarForegroundProperty);
            set => this.SetValue(ProgressBarForegroundProperty, value);
        }

        #endregion DependecyProperties

        #region Constructor

        internal ProgressDialog()
            : this(null)
        {
        }

        internal ProgressDialog(MetroWindow? parentWindow)
            : this(parentWindow, null)
        {
        }

        internal ProgressDialog(MetroWindow? parentWindow, MetroDialogSettings? settings)
            : base(parentWindow, settings)
        {
            this.SetCurrentValue(NegativeButtonTextProperty, this.DialogSettings.NegativeButtonText);
        }

        static ProgressDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProgressDialog), new FrameworkPropertyMetadata(typeof(ProgressDialog)));
        }

        #endregion Constructor

        internal CancellationToken CancellationToken => this.DialogSettings.CancellationToken;

        internal double Minimum
        {
            get => this.PART_ProgressBar!.Minimum;
            set => this.PART_ProgressBar!.Minimum = value;
        }

        internal double Maximum
        {
            get => this.PART_ProgressBar!.Maximum;
            set => this.PART_ProgressBar!.Maximum = value;
        }

        internal double ProgressValue
        {
            get => this.PART_ProgressBar!.Value;
            set
            {
                this.PART_ProgressBar!.IsIndeterminate = false;
                this.PART_ProgressBar!.Value = value;
                this.PART_ProgressBar.ApplyTemplate();
            }
        }

        internal void SetIndeterminate()
        {
            this.PART_ProgressBar!.IsIndeterminate = true;
        }
    }
}