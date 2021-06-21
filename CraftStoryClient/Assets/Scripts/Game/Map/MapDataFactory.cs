using JsonConfigData;
using System;
using UnityEngine;

public class MapDataFactory
{
    private MapData mData;
    private PlayerData playerData;
    private Map mapConfig;

    public MapData CreateMapData(int id)
    {
        var startTime = DateTime.Now;

        mapConfig = ConfigMng.E.Map[id];
        mData = new MapData(id, new Vector3Int(mapConfig.SizeX, mapConfig.SizeY, mapConfig.SizeZ));

        AddBaseBlocks();
        AddMountains();
        AddResources();
        AddTransferGateConfig();
        //AddBuildings();

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Debug.LogWarningFormat("mapData 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        return mData;
    }
    private void AddBaseBlocks()
    {
        int groundHeight = mapConfig.Block01Height + mapConfig.Block02Height + mapConfig.Block03Height;

        for (int x = 0; x < mData.MapSize.x; x++)
        {
            for (int z = 0; z < mData.MapSize.z; z++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    int blockId = 0;

                    if (y < mapConfig.Block01Height)
                        blockId = mapConfig.Block01;
                    else if (y < mapConfig.Block02Height + mapConfig.Block01Height)
                        blockId = mapConfig.Block02;
                    else
                        blockId = mapConfig.Block03;

                    mData.Map[x, y, z] = new MapBlockData(blockId, new Vector3Int(x, y, z));
                }
            }
        }
    }
    private void AddMountains()
    {
        if (mapConfig.Mountains == "N")
            return;

        var mountains = mapConfig.Mountains.Split(',');

        for (int i = 0; i < mountains.Length; i++)
        {
            Mountain mountainConfig = null;

            try
            {
                mountainConfig = ConfigMng.E.Mountain[int.Parse(mountains[i])];
                if (mountainConfig == null)
                    continue;
            }
            catch (Exception ex)
            {
                Debug.LogError("not find Mountain " + mountains[i]);
                Debug.LogError(ex.Message);
                continue;
            }

            int startPosX = mountainConfig.StartPosX;
            int startPosY = mountainConfig.StartPosY;
            int startPosZ = mountainConfig.StartPosZ;

            if (startPosX < 0) startPosX = UnityEngine.Random.Range(0, mapConfig.SizeX);
            if (startPosZ < 0) startPosZ = UnityEngine.Random.Range(0, mapConfig.SizeZ);

            AddMountains(mData.Map[startPosX, startPosY, startPosZ], mountainConfig.Height, mountainConfig.Wide);
        }
    }
    private void AddMountains(MapBlockData parent, int height, int offset = 0)
    {
        if (parent == null)
        {
            Debug.LogError("bad parent ");
            return;
        }

        for (int x = parent.Pos.x - (height - 1 + offset); x <= parent.Pos.x + (height - 1 + offset); x++)
        {
            for (int z = parent.Pos.z - (height - 1 + offset); z <= parent.Pos.z + (height - 1 + offset); z++)
            {
                int abX = Mathf.Abs(x - parent.Pos.x);
                int abZ = Mathf.Abs(z - parent.Pos.z);
                int aby = height - abX - abZ + offset;

                if (aby <= 0)
                    continue;

                int random = UnityEngine.Random.Range(-1, 1);

                for (int i = 1; i <= aby; i++)
                {
                    int offsetY = i;
                    if (offsetY > height)
                        offsetY = height;

                    Vector3Int newPos = new Vector3Int(x, offsetY + parent.Pos.y + random, z);
                    if (MapCtl.IsOutRange(mData, newPos))
                        continue;

                    mData.Map[x, newPos.y, z] = mData.Map[x, parent.Pos.y, z].Copy();
                    mData.Map[x, newPos.y, z].Pos = newPos;
                }
            }
        }
    }
    private void AddResources()
    {
        var data = mapConfig.Resources.Split(',');
        if (data[0] == "N")
            return;

        for (int i = 0; i < data.Length; i++)
        {
            Resource config = null;

            try
            {
                config = ConfigMng.E.Resource[int.Parse(data[i])];
                if (config == null)
                    continue;
            }
            catch (Exception ex)
            {
                Debug.LogError("not find resource " + data[i]);
                Debug.LogError(ex.Message);
                continue;
            }

            for (int j = 0; j < config.Count; j++)
            {
                var pos = MapCtl.GetGroundPos(mData, config.PosX, config.PosZ, config.OffsetY);

                pos = MapCtl.FixEntityPos(mData, pos, config.CreatePosOffset);
                pos = MapCtl.GetGroundPos(mData, (int)pos.x, (int)pos.z, config.OffsetY);

                mData.AddResources(new EntityData(config.ID, (ItemType)config.Type, pos));
            }
        }
    }
    private void AddTransferGateConfig()
    {
        var transferGateID = mapConfig.TransferGateID;
        if (transferGateID < 0)
            return;

        TransferGate config = null;

        try
        {
            config = ConfigMng.E.TransferGate[transferGateID];
            if (config == null)
                return;
        }
        catch (Exception ex)
        {
            Debug.LogError("not find transferGate " + transferGateID);
            Debug.LogError(ex.Message);
            return;
        }

        var pos = MapCtl.GetGroundPos(mData, config.PosX, config.PosZ);
        if (config.PosX > 0) pos.x = config.PosX;
        if (config.PosY > 0) pos.y = config.PosY;
        if (config.PosZ > 0) pos.z = config.PosZ;

        pos = MapCtl.FixEntityPos(mData, pos, config.CreatePosOffset);
        pos = MapCtl.GetGroundPos(mData, (int)pos.x, (int)pos.z);

        mData.TransferGate = new EntityData(config.ID, ItemType.TransferGate, pos);
    }
}