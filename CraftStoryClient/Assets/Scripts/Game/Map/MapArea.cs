using JsonConfigData;

public class MapAreaData
{
    private int mMapId;
    private Map mConfig { get => ConfigMng.E.Map[mMapId]; }
    private MapData mMapData;

    // エリアによる偏差
    private int offsetX, offsetZ;

    public MapAreaData() { }
    public MapAreaData(int areaId)
    {
        var areaConfig = ConfigMng.E.MapArea[areaId];
        mMapId = areaConfig.MapId;
        mMapData = WorldMng.E.MapCtl.CreateMapData(mMapId);

        CalculationOffsetX(areaId, ref offsetX);
        CalculationOffsetZ(areaId, ref offsetZ);
    }

    /// <summary>
    /// 偏差を計算X
    /// </summary>
    private void CalculationOffsetX(int areaId, ref int offsetX)
    {
        var areaConfig = ConfigMng.E.MapArea[areaId];
        if (areaConfig.Left >0)
        {
            var newAreaConfig = ConfigMng.E.MapArea[areaConfig.Left];
            offsetX += ConfigMng.E.Map[newAreaConfig.MapId].SizeX;

            if (newAreaConfig.Left > 0)
            {
                CalculationOffsetX(areaConfig.Left, ref offsetX);
            }
        }
    }
    /// <summary>
    /// 偏差を計算Z
    /// </summary>
    private void CalculationOffsetZ(int areaId, ref int offsetZ)
    {
        var areaConfig = ConfigMng.E.MapArea[areaId];
        if (areaConfig.Back > 0)
        {
            var newAreaConfig = ConfigMng.E.MapArea[areaConfig.Back];
            offsetZ += ConfigMng.E.Map[newAreaConfig.MapId].SizeZ;

            if (newAreaConfig.Back > 0)
            {
                CalculationOffsetZ(areaConfig.Back, ref offsetZ);
            }
        }
    }
}