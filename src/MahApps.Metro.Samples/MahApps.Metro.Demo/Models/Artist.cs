// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    public class Artist : ViewModelBase
    {
        private int _artistId;
        private string _name;
        private List<Album> _albums;

        public int ArtistId
        {
            get => this._artistId;
            set => this.Set(ref this._artistId, value);
        }

        public string Name
        {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        public List<Album> Albums
        {
            get => this._albums;
            set => this.Set(ref this._albums, value);
        }
    }
}