using System.ComponentModel;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// Represents a grouping of tiles
    /// </summary>
    public class PanoramaGroup
    {
        public string Header { get; private set; }
        public ICollectionView Tiles { get; private set; }

        public PanoramaGroup(string header, ICollectionView tiles)
        {
            Header = header;
            Tiles = tiles;
        }
    }
}
