// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from RatingItemImageInfo.h of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Describes the images a <see cref="RatingControl"/> draws for its items, one per state.
    /// </summary>
    public class RatingItemImageInfo : RatingItemInfo
    {
        /// <summary>Identifies the <see cref="Image"/> dependency property.</summary>
        public static readonly DependencyProperty ImageProperty
            = DependencyProperty.Register(nameof(Image),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the set items.</summary>
        public ImageSource? Image
        {
            get => (ImageSource?)this.GetValue(ImageProperty);
            set => this.SetValue(ImageProperty, value);
        }

        /// <summary>Identifies the <see cref="UnsetImage"/> dependency property.</summary>
        public static readonly DependencyProperty UnsetImageProperty
            = DependencyProperty.Register(nameof(UnsetImage),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the items that are not set, falls back to <see cref="Image"/>.</summary>
        public ImageSource? UnsetImage
        {
            get => (ImageSource?)this.GetValue(UnsetImageProperty);
            set => this.SetValue(UnsetImageProperty, value);
        }

        /// <summary>Identifies the <see cref="PlaceholderImage"/> dependency property.</summary>
        public static readonly DependencyProperty PlaceholderImageProperty
            = DependencyProperty.Register(nameof(PlaceholderImage),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the items of the placeholder value, falls back to <see cref="Image"/>.</summary>
        public ImageSource? PlaceholderImage
        {
            get => (ImageSource?)this.GetValue(PlaceholderImageProperty);
            set => this.SetValue(PlaceholderImageProperty, value);
        }

        /// <summary>Identifies the <see cref="PointerOverImage"/> dependency property.</summary>
        public static readonly DependencyProperty PointerOverImageProperty
            = DependencyProperty.Register(nameof(PointerOverImage),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the items under the pointer while a rating is set, falls back to <see cref="Image"/>.</summary>
        public ImageSource? PointerOverImage
        {
            get => (ImageSource?)this.GetValue(PointerOverImageProperty);
            set => this.SetValue(PointerOverImageProperty, value);
        }

        /// <summary>Identifies the <see cref="PointerOverPlaceholderImage"/> dependency property.</summary>
        public static readonly DependencyProperty PointerOverPlaceholderImageProperty
            = DependencyProperty.Register(nameof(PointerOverPlaceholderImage),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the items under the pointer while no rating is set, falls back to <see cref="PlaceholderImage"/>, then <see cref="Image"/>.</summary>
        public ImageSource? PointerOverPlaceholderImage
        {
            get => (ImageSource?)this.GetValue(PointerOverPlaceholderImageProperty);
            set => this.SetValue(PointerOverPlaceholderImageProperty, value);
        }

        /// <summary>Identifies the <see cref="DisabledImage"/> dependency property.</summary>
        public static readonly DependencyProperty DisabledImageProperty
            = DependencyProperty.Register(nameof(DisabledImage),
                                          typeof(ImageSource),
                                          typeof(RatingItemImageInfo),
                                          new PropertyMetadata(null));

        /// <summary>Gets or sets the image of the set items of a disabled control, falls back to <see cref="Image"/>.</summary>
        public ImageSource? DisabledImage
        {
            get => (ImageSource?)this.GetValue(DisabledImageProperty);
            set => this.SetValue(DisabledImageProperty, value);
        }

        /// <inheritdoc />
        protected override Freezable CreateInstanceCore()
        {
            return new RatingItemImageInfo();
        }
    }
}
