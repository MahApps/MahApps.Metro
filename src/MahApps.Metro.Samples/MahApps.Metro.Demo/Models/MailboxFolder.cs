// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MahApps.Metro.IconPacks;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    /// <summary>
    /// One folder of the mailbox the badge page shows on a tab: what to call it, what to draw beside
    /// the name, and how many are waiting in it, which is what the badge reads.
    /// </summary>
    public class MailboxFolder : ViewModelBase
    {
        private int count;

        public string Caption { get; set; } = string.Empty;

        public PackIconMaterialKind Glyph { get; set; }

        public int Count
        {
            get => this.count;
            set => this.Set(ref this.count, value);
        }
    }
}
