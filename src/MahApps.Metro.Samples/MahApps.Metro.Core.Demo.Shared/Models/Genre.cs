using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace MetroDemo.Models
{
    public class Genre : INotifyPropertyChanged
    {
        private int _genreId;
        private string _name;
        private string _description;
        private List<Album> _albums;

        public int GenreId
        {
            get { return _genreId; }
            set
            {
                if (value == _genreId) return;
                _genreId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (value == _description) return;
                _description = value;
                OnPropertyChanged();
            }
        }

        public List<Album> Albums
        {
            get { return _albums; }
            set
            {
                if (Equals(value, _albums)) return;
                _albums = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
