using System;

namespace JsonConfigData
{
    public class Block : ConfigBase
    {
        public string Name { get; set; }
        public string ResourcesName { get; set; }
        public int Type { get; set; }
        public float DestroyTime { get; set; }
    }
}
