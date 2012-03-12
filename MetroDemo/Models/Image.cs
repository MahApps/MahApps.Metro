namespace MetroDemo.Models
{
    using Newtonsoft.Json;

    public class Image
    {
        #region Public Properties

        public string size { get; set; }

        [JsonProperty(PropertyName = "#text")]
        public string text { get; set; }

        #endregion
    }
}