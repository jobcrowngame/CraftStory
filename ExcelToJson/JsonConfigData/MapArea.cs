
namespace JsonConfigData
{
    public class MapArea : ConfigBase
    {
        public int MapId { get; set; }
        public string RelatedArea { get; set; }
        public int OffsetX { get; set; }
        public int OffsetZ { get; set; }
    }
}