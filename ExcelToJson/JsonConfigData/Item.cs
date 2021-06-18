
using Newtonsoft.Json;

namespace JsonConfigData
{
    public class Item : Base
    {
        public string Name { get; set; }
        public string IconResourcesPath { get; set; }
        public int Type { get; set; }
        public int ReferenceID { get; set; }
        public int MaxCount { get; set; }

        [JsonConstructor]
        public Item() { }
    }
}
