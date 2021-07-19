

namespace JsonConfigData
{
    public class Item : ConfigBase
    {
        public string Name { get; set; }
        public string IconResourcesPath { get; set; }
        public int Type { get; set; }
        public int BagType { get; set; }
        public int ReferenceID { get; set; }
        public int MaxCount { get; set; }
    }
}
