// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    /// <summary>
    /// One set of header settings for every control on the page. The example view binds them through a
    /// style per control type, so a value changed here reaches all of them at once and they can be
    /// compared side by side.
    /// </summary>
    public class HeaderSettings : ViewModelBase
    {
        public HeaderSettings()
        {
            this.BackgroundChoices = new[]
                                     {
                                         new BrushChoice("Each control's own"),
                                         new BrushChoice("Accent", "MahApps.Brushes.Accent"),
                                         new BrushChoice("Accent 3", "MahApps.Brushes.Accent3"),
                                         new BrushChoice("Highlight", "MahApps.Brushes.Highlight"),
                                         new BrushChoice("Gray 2", "MahApps.Brushes.Gray2"),
                                         new BrushChoice("Gray 8", "MahApps.Brushes.Gray8"),
                                         new BrushChoice("Theme background", "MahApps.Brushes.ThemeBackground"),
                                         new BrushChoice("Transparent", brush: Brushes.Transparent)
                                     };

            this.ForegroundChoices = new[]
                                     {
                                         new BrushChoice("Automatic"),
                                         new BrushChoice("Theme foreground", "MahApps.Brushes.ThemeForeground"),
                                         new BrushChoice("Ideal foreground", "MahApps.Brushes.IdealForeground"),
                                         new BrushChoice("Accent", "MahApps.Brushes.Accent"),
                                         new BrushChoice("Gray 2", "MahApps.Brushes.Gray2")
                                     };

            this.AvailableFontWeights = new[]
                                        {
                                            FontWeights.Thin,
                                            FontWeights.ExtraLight,
                                            FontWeights.Light,
                                            FontWeights.Normal,
                                            FontWeights.Medium,
                                            FontWeights.SemiBold,
                                            FontWeights.Bold,
                                            FontWeights.ExtraBold,
                                            FontWeights.Black
                                        };

            this.AvailableFontStretches = new[]
                                          {
                                              FontStretches.UltraCondensed,
                                              FontStretches.ExtraCondensed,
                                              FontStretches.Condensed,
                                              FontStretches.SemiCondensed,
                                              FontStretches.Normal,
                                              FontStretches.SemiExpanded,
                                              FontStretches.Expanded,
                                              FontStretches.ExtraExpanded,
                                              FontStretches.UltraExpanded
                                          };

            this.headerBackgroundChoice = this.BackgroundChoices[0];
            this.headerForegroundChoice = this.ForegroundChoices[0];
            this.headerBackgroundMouseOverChoice = this.BackgroundChoices[0];
            this.headerBackgroundPressedChoice = this.BackgroundChoices[0];
            this.headerForegroundMouseOverChoice = this.ForegroundChoices[0];
            this.headerForegroundPressedChoice = this.ForegroundChoices[0];
            this.toggleButtonForegroundChoice = this.ForegroundChoices[0];
            this.toggleButtonForegroundMouseOverChoice = this.ForegroundChoices[0];
            this.toggleButtonForegroundPressedChoice = this.ForegroundChoices[0];
        }

        /// <summary>
        /// What the header can be painted with. The first entry hands nothing over, and every control
        /// then keeps the background its own style gives it, which is not the same one for all of them.
        /// </summary>
        public IReadOnlyList<BrushChoice> BackgroundChoices { get; }

        /// <summary>
        /// The same for the text. The first entry leaves it to the header to take the colour that reads
        /// on whatever is behind it, black or white, worked out from the background.
        /// </summary>
        public IReadOnlyList<BrushChoice> ForegroundChoices { get; }

        /// <summary>Every font on the machine, in the order somebody would look for one.</summary>
        public IEnumerable<FontFamily> AvailableFontFamilies { get; } = Fonts.SystemFontFamilies.OrderBy(family => family.Source).ToList();

        /// <summary>The weights that have a name of their own. FontWeight is a struct, so there is no enum to list.</summary>
        public IEnumerable<FontWeight> AvailableFontWeights { get; }

        /// <summary>The same for the stretches. Most fonts draw only a few of them.</summary>
        public IEnumerable<FontStretch> AvailableFontStretches { get; }

        private BrushChoice? headerBackgroundChoice;

        /// <summary>
        /// The entry rather than the brush it holds, so a brush the theme has swapped reaches the
        /// controls through the same binding instead of being left behind as the colour of before.
        /// </summary>
        public BrushChoice? HeaderBackgroundChoice
        {
            get => this.headerBackgroundChoice;
            set => this.Set(ref this.headerBackgroundChoice, value);
        }

        private BrushChoice? headerForegroundChoice;

        public BrushChoice? HeaderForegroundChoice
        {
            get => this.headerForegroundChoice;
            set => this.Set(ref this.headerForegroundChoice, value);
        }

        private BrushChoice? headerBackgroundMouseOverChoice;

        /// <summary>The brush while the mouse is over the header. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? HeaderBackgroundMouseOverChoice
        {
            get => this.headerBackgroundMouseOverChoice;
            set => this.Set(ref this.headerBackgroundMouseOverChoice, value);
        }

        private BrushChoice? headerBackgroundPressedChoice;

        /// <summary>The brush while the header is held down. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? HeaderBackgroundPressedChoice
        {
            get => this.headerBackgroundPressedChoice;
            set => this.Set(ref this.headerBackgroundPressedChoice, value);
        }

        private BrushChoice? headerForegroundMouseOverChoice;

        /// <summary>The brush for the text while the mouse is over the header. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? HeaderForegroundMouseOverChoice
        {
            get => this.headerForegroundMouseOverChoice;
            set => this.Set(ref this.headerForegroundMouseOverChoice, value);
        }

        private BrushChoice? headerForegroundPressedChoice;

        /// <summary>The brush for the text while the header is held down. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? HeaderForegroundPressedChoice
        {
            get => this.headerForegroundPressedChoice;
            set => this.Set(ref this.headerForegroundPressedChoice, value);
        }

        private BrushChoice? toggleButtonForegroundChoice;

        /// <summary>The brush for the glyph of an expander. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? ToggleButtonForegroundChoice
        {
            get => this.toggleButtonForegroundChoice;
            set => this.Set(ref this.toggleButtonForegroundChoice, value);
        }

        private BrushChoice? toggleButtonForegroundMouseOverChoice;

        /// <summary>The brush for the glyph while the mouse is over the header. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? ToggleButtonForegroundMouseOverChoice
        {
            get => this.toggleButtonForegroundMouseOverChoice;
            set => this.Set(ref this.toggleButtonForegroundMouseOverChoice, value);
        }

        private BrushChoice? toggleButtonForegroundPressedChoice;

        /// <summary>The brush for the glyph while the header is held down. The first entry hands nothing over, and the header stays as it is.</summary>
        public BrushChoice? ToggleButtonForegroundPressedChoice
        {
            get => this.toggleButtonForegroundPressedChoice;
            set => this.Set(ref this.toggleButtonForegroundPressedChoice, value);
        }

        private FontFamily? headerFontFamily = new FontFamily("Segoe UI");

        public FontFamily? HeaderFontFamily
        {
            get => this.headerFontFamily;
            set => this.Set(ref this.headerFontFamily, value);
        }

        private double headerFontSize = 12;

        public double HeaderFontSize
        {
            get => this.headerFontSize;
            set => this.Set(ref this.headerFontSize, value);
        }

        private FontWeight headerFontWeight = FontWeights.Normal;

        public FontWeight HeaderFontWeight
        {
            get => this.headerFontWeight;
            set => this.Set(ref this.headerFontWeight, value);
        }

        private FontStretch headerFontStretch = FontStretches.Normal;

        public FontStretch HeaderFontStretch
        {
            get => this.headerFontStretch;
            set => this.Set(ref this.headerFontStretch, value);
        }

        private Thickness headerMargin = new(4);

        public Thickness HeaderMargin
        {
            get => this.headerMargin;
            set => this.Set(ref this.headerMargin, value);
        }

        private double padding = 4;

        /// <summary>
        /// The one number the four sides of <see cref="HeaderMargin"/> are built from, so the room around
        /// a header can be tried out with a single control.
        /// </summary>
        public double Padding
        {
            get => this.padding;
            set
            {
                if (this.Set(ref this.padding, value))
                {
                    this.HeaderMargin = new Thickness(value);
                }
            }
        }

        private HorizontalAlignment headerHorizontalContentAlignment = HorizontalAlignment.Stretch;

        public HorizontalAlignment HeaderHorizontalContentAlignment
        {
            get => this.headerHorizontalContentAlignment;
            set => this.Set(ref this.headerHorizontalContentAlignment, value);
        }

        private VerticalAlignment headerVerticalContentAlignment = VerticalAlignment.Center;

        public VerticalAlignment HeaderVerticalContentAlignment
        {
            get => this.headerVerticalContentAlignment;
            set => this.Set(ref this.headerVerticalContentAlignment, value);
        }

        private CharacterCasing contentCharacterCasing = CharacterCasing.Normal;

        /// <summary>
        /// ControlsHelper.ContentCharacterCasing rather than one of the header properties, but it is the
        /// header text it works on, so it belongs on this page.
        /// </summary>
        public CharacterCasing ContentCharacterCasing
        {
            get => this.contentCharacterCasing;
            set => this.Set(ref this.contentCharacterCasing, value);
        }

        private CornerRadius cornerRadius;

        public CornerRadius CornerRadius
        {
            get => this.cornerRadius;
            set => this.Set(ref this.cornerRadius, value);
        }

        private double roundness;

        /// <summary>The one number <see cref="CornerRadius"/> is built from, for the same reason as <see cref="Padding"/>.</summary>
        public double Roundness
        {
            get => this.roundness;
            set
            {
                if (this.Set(ref this.roundness, value))
                {
                    this.CornerRadius = new CornerRadius(value);
                }
            }
        }

        private bool showToggleButton = true;

        /// <summary>ExpanderHelper.ShowToggleButton, which only the expander has.</summary>
        public bool ShowToggleButton
        {
            get => this.showToggleButton;
            set => this.Set(ref this.showToggleButton, value);
        }

        /// <summary>
        /// Hands every entry the brush its key stands for. Called once when the page comes up and again
        /// whenever the theme changes, because that is when the brush behind a key is another one.
        /// </summary>
        public void ReadTheBrushesFromTheTheme(Func<string, object?> lookUp)
        {
            foreach (var choice in this.BackgroundChoices.Concat(this.ForegroundChoices))
            {
                choice.ReadFromTheTheme(lookUp);
            }
        }
    }
}
