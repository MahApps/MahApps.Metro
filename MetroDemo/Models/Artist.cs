using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace MetroDemo.Models
{
    public class Artist : IPanoramaTile
    {
        public string name { get; set; }
        public string percentagechange { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public string streamable { get; set; }
        public Image[] image { get; set; }

        public string LargeImage
        {
            get
            {
                var x = image.Where(i => i.size == "extralarge").FirstOrDefault();
                if (x != null) return x.text;
                return string.Empty;
            }
        }

        public ICommand TileClickedCommand
        {
            get { return null; }
        }
    }
}