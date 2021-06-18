namespace JsonConfigData
{
    public class Resource : Base
    {
        public string ResourcePath { get; set; }
        public int Type { get; set; }
        public int PosX { get; set; }
        public int PosZ { get; set; }
        public int Angle { get; set; }
        public float OffsetY { get; set; }
        public int CreatePosOffset { get; set; }
        public float Scale { get; set; }
        public int Count { get; set; }
        public int CanBreak { get; set; }
        public float DestroyTime { get; set; }
        public int BonusID { get; set; }

        public Resource() { }
    }
}