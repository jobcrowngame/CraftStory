

namespace JsonConfigData
{
    public class Item : ConfigBase
    {
        public int Sort { get; set; }
        public string Name { get; set; }
        public string IconResourcesPath { get; set; }
        public string Explanatory { get; set; }
        public int Type { get; set; }
        public int BagType { get; set; }
        public int ReferenceID { get; set; }
        public int MaxCount { get; set; }
        public int CanDelete { get; set; }
        public int CanEquip { get; set; }
    }
}
