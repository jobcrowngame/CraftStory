
namespace JsonConfigData
{
    public class Map : ConfigBase
    {
        public string Name { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int SizeZ { get; set; }
        public int Entity01 { get; set; }
        public int Entity01Height { get; set; }
        public int Entity02 { get; set; }
        public int Entity02Height { get; set; }
        public int Entity03 { get; set; }
        public int Entity03Height { get; set; }
        public string Mountains { get; set; }
        public string Resources { get; set; }
        public int TransferGateID { get; set; }
        public string Buildings { get; set; }
        public int PlayerPosX { get; set; }
        public int PlayerPosZ { get; set; }
        public int CreatePosOffset { get; set; }

    }
}
