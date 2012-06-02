using System.ComponentModel;
using System.Windows.Input;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The minimum specification that a tile needs to support  
    /// </summary>
    public interface IPanoramaTile
    {
        ICommand TileClickedCommand { get; }
    }

    public class PanoramaTile : INotifyPropertyChanged, IPanoramaTile
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Text { get; set; }
        public ICommand TileClickedCommand
        {
            get { return null; }
        }
    }
}
