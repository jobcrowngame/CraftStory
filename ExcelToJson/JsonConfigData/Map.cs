namespace JsonConfigData
{
    public class Map : Base
    {
        public string Name { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }
        public int Block01 { get; set; }
        public int Block01Height { get; set; }
        public int Block02 { get; set; }
        public int Block02Height { get; set; }
        public int Block03 { get; set; }
        public int Block03Height { get; set; }
        public string Mountains { get; set; }
        public string Trees { get; set; }
        public string Rocks { get; set; }
        public int TransferGateID { get; set; }
        public int PlayerPosX { get; set; }
        public int PlayerPosZ { get; set; }
        public int CreatePosOffset { get; set; }
    }
}
