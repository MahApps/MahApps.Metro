// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    /// <summary>
    /// One entry of a combo box that offers brushes: what to call it and the brush itself.
    /// </summary>
    public class BrushChoice : ViewModelBase
    {
        public BrushChoice(object content, string? resourceKey = null, Brush? brush = null)
        {
            this.Content = content;
            this.ResourceKey = resourceKey;
            this.brush = brush;
        }

        public object Content { get; }

        /// <summary>
        /// Where the brush comes from, if it comes from the theme. Held as a key rather than as the brush
        /// it stands for, because a theme swaps the brush behind the key and the entry has to follow.
        /// </summary>
        public string? ResourceKey { get; }

        private Brush? brush;

        /// <summary>Left empty, the entry stands for handing nothing over at all.</summary>
        public Brush? Brush
        {
            get => this.brush;
            private set => this.Set(ref this.brush, value);
        }

        /// <summary>Reads the brush for this entry again, which is what a change of theme calls for.</summary>
        public void ReadFromTheTheme(Func<string, object?> lookUp)
        {
            if (this.ResourceKey is not null)
            {
                this.Brush = lookUp(this.ResourceKey) as Brush;
            }
        }
    }
}
