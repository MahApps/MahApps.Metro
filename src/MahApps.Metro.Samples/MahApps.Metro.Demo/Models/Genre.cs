using System.Collections.Generic;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    public class Genre : ViewModelBase
    {
        private int _genreId;
        private string _name;
        private string _description;
        private List<Album> _albums;

        public int GenreId
        {
            get => this._genreId;
            set => this.Set(ref this._genreId, value);
        }

        public string Name
        {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        public string Description
        {
            get => this._description;
            set => this.Set(ref this._description, value);
        }

        public List<Album> Albums
        {
            get => this._albums;
            set => this.Set(ref this._albums, value);
        }
    }
}