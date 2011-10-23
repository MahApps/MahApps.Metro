using Newtonsoft.Json;

namespace MetroDemo.Models
{
    public class Image
    {
        [JsonProperty(PropertyName = "#text")]
        public string text { get; set; }
        public string size { get; set; }
    }
}