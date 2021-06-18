using Newtonsoft.Json;

namespace JsonConfigData
{
    public class Base
    {
        public int ID { get; set; }

        [JsonConstructor]
        public Base() { }
    }
}
