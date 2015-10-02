using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MetroDemo.Annotations;

namespace MetroDemo.Models
{
    public class Artist : INotifyPropertyChanged
    {
        private int _artistId;
        private string _name;
        private List<Album> _albums;

        public int ArtistId
        {
            get { return _artistId; }
            set
            {
                if (value == _artistId) return;
                _artistId = value;
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