
namespace JsonConfigData
{
    public class GuideStep : ConfigBase
    {
        public string Des { get; set; }
        public string CellName { get; set; }
        public string Message { get; set; }
        public float MsgPosX { get; set; }
        public float MsgPosY { get; set; }
        public float MsgSizeX { get; set; }
        public float MsgSizeY { get; set; }
        public int HideHand { get; set; }
        public int NextMask { get; set; }
        public int AutoMove { get; set; }
        public string ClickType { get; set; }
        public string CreateBlockCount { get; set; }
        public string DisplayObject { get; set; }
        public string HideObject { get; set; }

    }
}
