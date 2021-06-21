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

    private List<EntityResources> entityList;
    private int entityID;

    public Transform CellParent { get => mapCellParent; }

    public MapCtl()
    {
        mapFactory = new MapDataFactory();
        entityList = new List<EntityResources>();

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

        CreateBlocks(mData);
        CreateResources(mData);
        CreateTransferGate(mData);

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("map 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }
    private void CreateBlocks(MapData mData)
    {
        HideBlocks(mData);

        for (int y = 0; y < mData.MapSize.y; y++)
        {
            for (int z = 0; z < mData.MapSize.z; z++)
            {
                for (int x = 0; x < mData.MapSize.x; x++)
                {
                    if (mData.Map[x, y, z] != null)
                    {
                        if (mData.Map[x, y, z].NoInstantiate)
                            continue;

                        mData.AddBlock(mData.Map[x, y, z]);
                        mData.Map[x, y, z].ActiveBlock();
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
                entityList.Add(resourceEntity);
            }
        }
    }
    private void CreateTransferGate(MapData mData)
    {
        if (mData.TransferGate == null || mData.TransferGate.ID == 0)
            return;

        var entity = CommonFunction.Instantiate<EntityTransferGate>(mData.TransferGate.ResourcePath, resourceParent, mData.TransferGate.Pos);
        if (entity != null)
        {
            entity.Init(mData.TransferGate);
        }
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

                if (DataMng.E.MapData.GetMap(Vector3Int.CeilToInt(block.transform.position)) != null)
                {
                    render.material.color = new Color(1, 0, 0, 0.5f);
                    blueprint.IsDuplicate = true;
                }
                else
                {
                    render.material.color = new Color(1, 1, 1, 0.5f);
                }
            }
        }
    }
    public void CreateBlocks(BlueprintData blueprint, Vector3Int startPos)
    {
        if (blueprint == null)
        {
            Debug.LogError("blueprint is null");
            return;
        }

        foreach (var item in blueprint.BlockList)
        {
            item.Pos = Vector3Int.CeilToInt(item.Block.transform.position);
            item.ClearBlock();
            CreateBlock(item);
        }
    }
    public MapBlock CreateBlock(Vector3Int pos, int blockId)
    {
        if (IsOutRange(DataMng.E.MapData, pos))
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
        DataMng.E.MapData.AddBlock(data);
        CheckNextToBlocks(data);
        return block;
    }

    public EntityResources CreateResources(Vector3Int pos, int resourcesId)
    {
        if (!ConfigMng.E.Resource.ContainsKey(resourcesId))
            return null;

        if (IsOutRange(DataMng.E.MapData, pos))
            return null;

        var config = ConfigMng.E.Resource[resourcesId];
        var entityData = new EntityData(config.ID, (ItemType)config.Type, pos);
        
        var resourceEntity = CommonFunction.Instantiate<EntityResources>(config.ResourcePath, resourceParent, pos);
        if (resourceEntity != null)
        {
            resourceEntity.Init(entityData);
            DataMng.E.MapData.AddResources(resourceEntity.Data);
        }

        return resourceEntity;
    }

    public void DeleteBlock(MapBlock block)
    {
        DataMng.E.MapData.RemoveBlock(block.data.Pos);
        GameObject.Destroy(block.gameObject);
        CheckNextToBlocks(block.data);
    }
    public void DeleteBuilderPencil()
    {
        if(builderPencilParent != null) GameObject.Destroy(builderPencilParent.gameObject);
    }
    public void DeleteResource(EntityResources resource)
    {
        DataMng.E.MapData.RemoveEntity(resource.Data);
        entityList.Remove(resource);
        GameObject.Destroy(resource.gameObject);
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
        var isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (!isSurface) isSurface = IsSurface(DataMng.E.MapData, new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        return isSurface;
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
        if (IsOutRange(DataMng.E.MapData, pos))
            return null;

        return DataMng.E.MapData.GetMap(pos);
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

    public static void HideBlocks(MapData mapData)
    {
        for (int x = 0; x < mapData.MapSize.x; x++)
        {
            for (int z = 0; z < mapData.MapSize.z; z++)
            {
                for (int y = 0; y < mapData.MapSize.y; y++)
                {
                    if (mapData.Map[x, y, z] == null)
                        continue;

                    mapData.Map[x, y, z].NoInstantiate = !CheckBlockIsSurface(mapData, mapData.Map[x, y, z]);
                }
            }
        }
    }
    private static bool CheckBlockIsSurface(MapData mapData, MapBlockData data)
    {
        var isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x - 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x + 1, data.Pos.y, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x, data.Pos.y - 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x, data.Pos.y + 1, data.Pos.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z - 1));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(data.Pos.x, data.Pos.y, data.Pos.z + 1));
        return isSurface;
    }
    private static bool IsSurface(MapData mapData, Vector3Int pos)
    {
        if (IsOutRange(mapData, pos))
            return false;

        return mapData.Map[pos.x, pos.y, pos.z] == null;
    }
    /// <summary>
    /// マップの最大サイズ外
    /// </summary>
    public static bool IsOutRange(MapData mapData, Vector3Int pos)
    {
        return pos.x < 0 || pos.x > mapData.MapSize.x - 1
            || pos.y < 0 || pos.y > mapData.MapSize.y - 1
            || pos.z < 0 || pos.z > mapData.MapSize.z - 1;
    }

    #endregion
}
