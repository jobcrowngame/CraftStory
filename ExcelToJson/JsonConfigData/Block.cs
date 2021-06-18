namespace JsonConfigData
{
    public class Block : Base
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string ResourcesName { get; set; }
        public int Type { get; set; }
        public float DestroyTime { get; set; }

        public Block() { }
    }
}
