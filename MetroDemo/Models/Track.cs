using System.Collections.Generic;
using System.Linq;

namespace MetroDemo.Models
{
    public class Track
    {
        public string name { get; set; }
        public string duration { get; set; }
        public string percentagechange { get; set; }
        public string mbid { get; set; }
        public string url { get; set; }
        public Streamable streamable { get; set; }
        public Artist artist { get; set; }
        public List<Image> image { get; set; }
        public string downloadurl { get; set; }

        public string LargeImage
        {
            get
            {
                if (image == null)
                    return string.Empty;
                var x = image.FirstOrDefault(i => i.size == "extralarge");
                if (x != null)
                    return x.text;
                return string.Empty;
            }
        }

    }
}