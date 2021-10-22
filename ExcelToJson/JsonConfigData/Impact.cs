

namespace JsonConfigData
{
    public class Impact : ConfigBase
    {
        public int Type { get; set; }
        public int PercentDamage { get; set; }
        public int FixtDamage { get; set; }
        public float Delay { get; set; }
        public int Count { get; set; }
        public float Interval { get; set; }
        public float TargetFreezeTime { get; set; }
    }
}
