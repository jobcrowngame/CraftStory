
namespace JsonConfigData
{
    public class TransferGate : ConfigBase
    {
        public int EntityID { get; set; }
        public int NextMap { get; set; }
        public string NextMapSceneName { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PosZ { get; set; }
        public int CreatePosOffset { get; set; }
    }
}
