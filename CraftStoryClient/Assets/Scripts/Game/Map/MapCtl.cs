using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCtl
{
    private MapDataFactory mapF;

    private Transform mapCellParent;
    public Transform CellParent { get => mapCellParent; }

    public MapCtl()
    {
        mapF = new MapDataFactory();
    }

    public void CreateMap()
    {
        CreateMap(DataMng.E.MapData);
    }
    public void CreateMap(int mapID)
    {
        var mData = mapF.CreateMap(mapID);
        DataMng.E.MapData = mData;
        CreateMap(mData);
    }
    public void CreateMap(MapData mData)
    {
        mapCellParent = new GameObject("Ground").transform;
        var startTime = DateTime.Now;

        for (int i = 0; i < mData.MapSize.x; i++)
        {
            for (int j = 0; j < mData.MapSize.y; j++)
            {
                for (int k = 0; k < mData.MapSize.z; k++)
                {
                    if (mData.Map[i, j, k] != null)
                    {
                        if (mData.Map[i, j, k].IsIn)
                            continue;

                        mData.Add(mData.Map[i, j, k]);
                        mData.Map[i, j, k].ActiveBlock();
                    }
                }
            }
        }

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("map 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }

    public void OnQuit()
    {

    }

    public MapBlock CreateBlock(Vector3Int pos, int blockId)
    {
        if (IsOutRange(pos))
            return null;

        BlockData bData = new BlockData(blockId, pos);
        return CreateBlock(bData);
    }
    public MapBlock CreateBlock(BlockData data)
    {
        var block = data.ActiveBlock();
        DataMng.E.MapData.Add(data);
        CheckNextToBlocks(data);
        return block;
    }
   
    public void DeleteMapCell(MapBlock block)
    {
        DataMng.E.MapData.Remove(block.data.Pos);
        GameObject.Destroy(block.gameObject);
        CheckNextToBlocks(block.data);
    }
   
    private void CheckNextToBlocks(BlockData data)
    {
        var list = GetNextToBlocks(data);

        for (int i = 0; i < list.Count; i++)
        {
            var isSurface = CheckBlockIsSurface(list[i]);
            list[i].ActiveBlock(isSurface);
        }
    }
    public bool CheckBlockIsSurface(BlockData data)
    {
        var isSurface = IsSurface(new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (!isSurface) isSurface = IsSurface(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        return isSurface;
    }
    private bool IsSurface(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return false;

        return GetNextToBlock(pos) == null;
    }

    private List<BlockData> GetNextToBlocks(BlockData data)
    {
        BlockData bData;
        List<BlockData> blockList = new List<BlockData>();

        bData = GetNextToBlock(new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (bData != null) blockList.Add(bData);
        bData = GetNextToBlock(new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (bData != null) blockList.Add(bData);
        bData = GetNextToBlock(new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (bData != null) blockList.Add(bData);
        bData = GetNextToBlock(new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (bData != null) blockList.Add(bData);
        bData = GetNextToBlock(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (bData != null) blockList.Add(bData);
        bData = GetNextToBlock(new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        if (bData != null) blockList.Add(bData);

        return blockList;
    }
    private BlockData GetNextToBlock(Vector3Int pos)
    {
        if (IsOutRange(pos))
            return null;

        return DataMng.E.MapData.GetMap(pos);
    }
    private bool IsOutRange(Vector3Int pos)
    {
        return pos.x < 0 || pos.x > DataMng.E.MapData.MapSize.x - 1
            || pos.y < 0 || pos.y > DataMng.E.MapData.MapSize.y - 1
            || pos.z < 0 || pos.z > DataMng.E.MapData.MapSize.z - 1;
    }

    public bool OutOfMapRangeX(float posX)
    {
        return posX > DataMng.E.MapData.MapSize.x - 1 || posX < 0;
    }
    public bool OutOfMapRangeZ(float posZ)
    {
        return posZ > DataMng.E.MapData.MapSize.z - 1 || posZ < 0;
    }
}
