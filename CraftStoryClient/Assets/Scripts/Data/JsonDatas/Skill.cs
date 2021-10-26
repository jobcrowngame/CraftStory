

namespace JsonConfigData
{
    public class Skill : ConfigBase
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Impact { get; set; }
        public int Animation { get; set; }
        public float CD { get; set; }
        public float Distance { get; set; }
        public int RangeAngle { get; set; }
        public float Radius { get; set; }
        public int ReadyAnimation { get; set; }
        public string ReadyEffect { get; set; }
        public float ReadyEffectTime { get; set; }
        public string ReadyTargetEffect { get; set; }
        public float ReadyTargetEffectTime { get; set; }
        public string AttackerEffect { get; set; }
        public float AttackerEffectTime { get; set; }
        public string TargetEffect { get; set; }
        public float TargetEffectTime { get; set; }
        public int TargetEffectParentInModel { get; set; }
        public float ReadyTime { get; set; }
        public float ProcessTime { get; set; }
        public int AttackCount { get; set; }
        public float Interval { get; set; }
        public float DelayDamageTime { get; set; }
        public int NextSkill { get; set; }
        public int CanEquipment { get; set; }
        public string Des { get; set; }
    }
}
