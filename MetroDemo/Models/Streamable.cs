using Newtonsoft.Json;

namespace MetroDemo.Models
{
    public class Streamable
    {
        [JsonProperty("#text")]
        public string text { get; set; }
        public string fulltrack { get; set; }
    }
}