using Newtonsoft.Json;

namespace JsonConfigData
{
    [JsonObject]
    public class TestData
    {
        [JsonProperty("clm01")]
        public string Key1 { get; set; }

        [JsonProperty("clm02")]
        public int Key2 { get; set; }
    }
}
