

namespace JsonConfigData
{
    public class Skill : ConfigBase
    {
        public string Icon { get; set; }
        public int Type { get; set; }
        public string Impact { get; set; }
        public int Animation { get; set; }
        public float CD { get; set; }
        public float Distance { get; set; }
        public int RangeAngle { get; set; }
        public float ReadyTime { get; set; }
        public float ProcessTime { get; set; }
        public float TargetFreezeTime { get; set; }
    }
}
