using JsonConfigData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCtl
{
    private MapDataFactory mapFactory;
    private Transform mapCellParent;
    private Transform resourceParent;
    private Transform builderPencilParent;

    private List<EntityResources> resourcesEntitys;
    private int entityID;

    public Transform CellParent { get => mapCellParent; }

    public MapCtl()
    {
        mapFactory = new MapDataFactory();
        resourcesEntitys = new List<EntityResources>();

        entityID = 0;
    }

    public void CreateMap(int mapID)
    {
        if (mapID == 100 && DataMng.E.HomeData == null)
            DataMng.E.HomeData = mapFactory.CreateMapData(mapID);

        DataMng.E.MapData = mapID == 100
            ? DataMng.E.HomeData
            : mapFactory.CreateMapData(mapID);

        CreateMap(DataMng.E.MapData);
    }
    private void CreateMap(MapData mData)
    {
        mapCellParent = new GameObject("Ground").transform;
        resourceParent = new GameObject("Resources").transform;

        var startTime = DateTime.Now;

        CreateBlock(mData);
        CreateResources(mData);
        CreateTransferGate(mData);

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("map 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }
    private void CreateBlock(MapData mData)
    {

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
    }
    private void CreateResources(MapData mData)
    {
        if (mData.Resources == null)
            return;

        for (int i = 0; i < mData.Resources.Count; i++)
        {
            var resourceEntity = CommonFunction.Instantiate<EntityResources>(mData.Resources[i].ResourcePath, resourceParent, mData.Resources[i].Pos);
            if (resourceEntity != null)
            {
                resourceEntity.Init(mData.Resources[i]);
                resourceEntity.EntityID = entityID++;
            }
        }
    }
    private void CreateTransferGate(MapData mData)
    {
        if (mData.TransferGate == null)
            return;

        var entity = CommonFunction.Instantiate<EntityTransferGate>(mData.TransferGate.ResourcePath, resourceParent, mData.TransferGate.Pos);
        if (entity != null)
        {
            entity.Init(mData.TransferGate);
            entity.EntityID = entityID++;
        }

        DataMng.E.NextSceneID = entity.Config.NextMap;
        DataMng.E.NextSceneName = entity.Config.NextMapSceneName;
    }

    public void OnQuit()
    {

    }

    public void CreateTransparentBlocks(BlueprintData blueprint, Vector3Int startPos)
    {
        var shader = Shader.Find("SemiTransparent");
        builderPencilParent = new GameObject("BuilderPancil").transform;
        builderPencilParent.position = startPos;

        foreach (var item in blueprint.BlockList)
        {
            var block = item.ActiveBlock(builderPencilParent);
            block.transform.localPosition = item.Pos;

            if (shader != null)
            {
                block.GetComponent<BoxCollider>().enabled = false;

                var render = block.GetComponent<Renderer>();
                render.material.shader = shader;
                render.material.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
    public void CreateBlocks(BlueprintData blueprint, Vector3Int startPos)
    {
        foreach (var item in blueprint.BlockList)
        {
            item.Pos = Vector3Int.CeilToInt(item.Block.transform.position);
            item.ClearBlock();
            CreateBlock(item);
        }
    }
    public MapBlock CreateBlock(Vector3Int pos, int blockId)
    {
        if (IsOutRange(pos))
            return null;

        MapBlockData bData = new MapBlockData(blockId, pos);
        return CreateBlock(bData);
    }
    public MapBlock CreateBlock(MapBlockData data)
    {
        return CreateBlock(data, WorldMng.E.MapCtl.CellParent);
    }
    public MapBlock CreateBlock(MapBlockData data, Transform parent)
    {
        var block = data.ActiveBlock(parent);
        DataMng.E.MapData.Add(data);
        CheckNextToBlocks(data);
        return block;
    }
   
    public void DeleteBlock(MapBlock block)
    {
        DataMng.E.MapData.Remove(block.data.Pos);
        GameObject.Destroy(block.gameObject);
        CheckNextToBlocks(block.data);
    }
    public void DeleteBuilderPencil()
    {
        if(builderPencilParent != null) GameObject.Destroy(builderPencilParent.gameObject);
    }

    public void RotateBuilderPencilParent(float angle)
    {
        if (builderPencilParent != null)
            builderPencilParent.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    private void CheckNextToBlocks(MapBlockData data)
    {
        var list = GetNextToBlocks(data);

        for (int i = 0; i < list.Count; i++)
        {
            var isSurface = CheckBlockIsSurface(list[i]);
            list[i].ActiveBlock(isSurface);
        }
    }
    public bool CheckBlockIsSurface(MapBlockData data)
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

    private List<MapBlockData> GetNextToBlocks(MapBlockData data)
    {
        MapBlockData bData;
        List<MapBlockData> blockList = new List<MapBlockData>();

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
    private MapBlockData GetNextToBlock(Vector3Int pos)
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

    #region Static Function

    public static Vector3 FixEntityPos(MapData mData, Vector3 pos, int offset)
    {
        if (pos.x < offset)
            pos.x = offset;

        if (pos.x > mData.MapSize.x - offset)
            pos.x = mData.MapSize.x - offset;

        if (pos.z < offset)
            pos.z = offset;

        if (pos.z > mData.MapSize.z - offset)
            pos.z = mData.MapSize.z - offset;

        return pos;
    }

    public static Vector3 GetGroundPos(MapData mapData, int posX, int posZ, float offsetY = 0)
    {
        if (posX < 0) posX = UnityEngine.Random.Range(0, mapData.Config.SizeX);
        if (posZ < 0) posZ = UnityEngine.Random.Range(0, mapData.Config.SizeZ);

        MapBlockData block = null;
        for (int i = mapData.Config.SizeY - 1; i >= 0; i--)
        {
            if (mapData.Map[posX, i, posZ] == null)
                continue;

            block = mapData.Map[posX, i, posZ];
            break;
        }

        float posY = block.Pos.y + 1 + offsetY - 0.5f;

        return new Vector3(posX, posY, posZ);
    }

    #endregion
}
