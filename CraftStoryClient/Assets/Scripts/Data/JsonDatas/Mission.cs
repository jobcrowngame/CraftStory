using System;

namespace JsonConfigData
{
    public class Mission : ConfigBase
    {
        public int Type { get; set; }
        public string Des { get; set; }
        public int Bonus { get; set; }
        public int EndNumber { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Chat1 { get; set; }
        public string Chat2 { get; set; }
        public int RojicType { get; set; }
    }
}
