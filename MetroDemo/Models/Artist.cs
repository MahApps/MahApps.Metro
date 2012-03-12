namespace MetroDemo.Models
{
    using System.Linq;

    public class Artist
    {
        #region Public Properties

        public string LargeImage
        {
            get
            {
                var x = this.image.Where(i => i.size == "extralarge").FirstOrDefault();
                if (x != null)
                {
                    return x.text;
                }
                return string.Empty;
            }
        }

        public Image[] image { get; set; }

        public string mbid { get; set; }

        public string name { get; set; }

        public string percentagechange { get; set; }

        public string streamable { get; set; }

        public string url { get; set; }

        #endregion
    }
}