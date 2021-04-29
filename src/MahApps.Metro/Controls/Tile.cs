// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls
{
    public class Tile : Button
    {
        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(nameof(Title),
                                          typeof(string),
                                          typeof(Tile),
                                          new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets the title of the <see cref="Tile"/>.
        /// </summary>
        public string? Title
        {
            get => (string?)this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>Identifies the <see cref="HorizontalTitleAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty HorizontalTitleAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalTitleAlignment),
                                        typeof(HorizontalAlignment),
                                        typeof(Tile),
                                        new FrameworkPropertyMetadata(HorizontalAlignment.Left));

        /// <summary> 
        /// Gets or sets the horizontal alignment of the <see cref="Title"/>.
        /// </summary> 
        [Bindable(true), Category("Layout")]
        public HorizontalAlignment HorizontalTitleAlignment
        {
            get => (HorizontalAlignment)this.GetValue(HorizontalTitleAlignmentProperty);
            set => this.SetValue(HorizontalTitleAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="VerticalTitleAlignment"/> dependency property.</summary> 
        public static readonly DependencyProperty VerticalTitleAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalTitleAlignment),
                                        typeof(VerticalAlignment),
                                        typeof(Tile),
                                        new FrameworkPropertyMetadata(VerticalAlignment.Bottom));

        /// <summary>
        /// Gets or sets the vertical alignment of the <see cref="Title"/>.
        /// </summary>
        [Bindable(true), Category("Layout")]
        public VerticalAlignment VerticalTitleAlignment
        {
            get => (VerticalAlignment)this.GetValue(VerticalTitleAlignmentProperty);
            set => this.SetValue(VerticalTitleAlignmentProperty, value);
        }

        /// <summary>Identifies the <see cref="Count"/> dependency property.</summary>
        public static readonly DependencyProperty CountProperty
            = DependencyProperty.Register(nameof(Count),
                                          typeof(string),
                                          typeof(Tile),
                                          new PropertyMetadata(default(string)));

        /// <summary>
        /// Gets or sets a Count text.
        /// </summary>
        public string? Count
        {
            get => (string?)this.GetValue(CountProperty);
            set => this.SetValue(CountProperty, value);
        }

        /// <summary>Identifies the <see cref="TitleFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty TitleFontSizeProperty
            = DependencyProperty.Register(nameof(TitleFontSize),
                                          typeof(double),
                                          typeof(Tile),
                                          new PropertyMetadata(16d));

        /// <summary>
        /// Gets or sets the font size of the <see cref="Title"/>.
        /// </summary>
        public double TitleFontSize
        {
            get => (double)this.GetValue(TitleFontSizeProperty);
            set => this.SetValue(TitleFontSizeProperty, value);
        }

        /// <summary>Identifies the <see cref="CountFontSize"/> dependency property.</summary>
        public static readonly DependencyProperty CountFontSizeProperty
            = DependencyProperty.Register(nameof(CountFontSize),
                                          typeof(double),
                                          typeof(Tile),
                                          new PropertyMetadata(28d));

        /// Gets or sets the font size of the <see cref="Count"/>.
        public double CountFontSize
        {
            get => (double)this.GetValue(CountFontSizeProperty);
            set => this.SetValue(CountFontSizeProperty, value);
        }

        static Tile()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Tile), new FrameworkPropertyMetadata(typeof(Tile)));
        }
    }
}