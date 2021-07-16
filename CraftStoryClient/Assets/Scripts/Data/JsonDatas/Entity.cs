namespace JsonConfigData
{
    public class Entity : ConfigBase
    {
        public int ItemID { get; set; }
        public string Resources { get; set; }
        public int Type { get; set; }
        public float DestroyTime { get; set; }
        public int ScaleX { get; set; }
        public int ScaleZ { get; set; }
        public int ScaleY { get; set; }
        public int BonusID { get; set; }
    }
}
