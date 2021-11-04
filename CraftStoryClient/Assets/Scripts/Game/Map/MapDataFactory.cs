using JsonConfigData;
using System;
using UnityEngine;

/// <summary>
/// マップデータ生成する工場
/// </summary>
public class MapDataFactory
{
    private MapData mData;
    private Map mapConfig;

    /// <summary>
    /// マップデータを生成
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MapData CreateMapData(int id)
    {
        var startTime = DateTime.Now;

        mapConfig = ConfigMng.E.Map[id];
        mData = new MapData(id);

        AddBaseBlocks();
        AddMountains();
        AddResources();
        AddTransferGateConfig();
        AddBuildings();

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("mapData 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        return mData;
    }
    /// <summary>
    /// 基本ブロック生成
    /// </summary>
    private void AddBaseBlocks()
    {
        int groundHeight = mapConfig.Entity01Height;
        if (groundHeight < 1)
        {
            Logger.Error("bad map config " + mapConfig.ID);
            return;
        }

        if (mapConfig.Entity02Height > 0) groundHeight += mapConfig.Entity02Height;
        if (mapConfig.Entity03Height > 0) groundHeight += mapConfig.Entity03Height;

        for (int x = 0; x < mData.GetMapSize().x; x++)
        {
            for (int z = 0; z < mData.GetMapSize().z; z++)
            {
                for (int y = 0; y < groundHeight; y++)
                {
                    int blockId = 0;

                    if (y == 0)
                        blockId = 1999;
                    else if (y < mapConfig.Entity01Height)
                        blockId = mapConfig.Entity01;
                    else if (y < mapConfig.Entity02Height + mapConfig.Entity01Height)
                        blockId = mapConfig.Entity02;
                    else
                        blockId = mapConfig.Entity03;

                    mData.Map[x, y, z] = new MapData.MapCellData() { entityID = blockId };
                }
            }
        }
    }

    /// <summary>
    /// 山を生成
    /// </summary>
    private void AddMountains()
    {
        if (mapConfig.Mountains == "N")
            return;

        var mountains = mapConfig.Mountains.Split(',');

        for (int i = 0; i < mountains.Length; i++)
        {
            Mountain config = null;

            try
            {
                config = ConfigMng.E.Mountain[int.Parse(mountains[i])];
                if (config == null)
                    continue;
            }
            catch (Exception ex)
            {
                Logger.Error("not find Mountain " + mountains[i]);
                Logger.Error(ex.Message);
                continue;
            }

            int startPosX = config.StartPosX;
            int startPosY = config.StartPosY;
            int startPosZ = config.StartPosZ;

            if (startPosX < 0) startPosX = UnityEngine.Random.Range(0, mapConfig.SizeX);
            if (startPosZ < 0) startPosZ = UnityEngine.Random.Range(0, mapConfig.SizeZ);

            int startEntityID = mData.Map[startPosX, startPosY, startPosZ].entityID;
            if (startEntityID != 0)
            {
                for (int x = startPosX - (config.Height - 1 + config.Wide); x <= startPosX + (config.Height - 1 + config.Wide); x++)
                {
                    for (int z = startPosZ - (config.Height - 1 + config.Wide); z <= startPosZ + (config.Height - 1 + config.Wide); z++)
                    {
                        int abX = Mathf.Abs(x - startPosX);
                        int abZ = Mathf.Abs(z - startPosZ);
                        int aby = config.Height - abX - abZ + config.Wide;

                        if (aby <= 0)
                            continue;

                        int random = UnityEngine.Random.Range(-1, 1);

                        for (int j = 1; j <= aby; j++)
                        {
                            int offsetY = j;
                            if (offsetY > config.Height)
                                offsetY = config.Height;

                            Vector3Int newPos = new Vector3Int(x, offsetY + startPosY + random, z);
                            if (MapCtl.IsOutRange(mData, newPos))
                                continue;

                            mData.Map[newPos.x, newPos.y, newPos.z] = new MapData.MapCellData() { entityID = startEntityID };
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 資源を生成
    /// </summary>
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
                Logger.Error("not find resource " + data[i]);
                Logger.Error(ex.Message);
                continue;
            }

            for (int j = 0; j < config.Count; j++)
            {
                var range = UnityEngine.Random.Range(0, 100);
                if (range > config.Percent)
                    continue;

                Vector3 newPos = MapCtl.GetGroundPos(mData, config.PosX, config.PosZ, config.OffsetY, config.CreatePosOffset);
                var entityConfig = ConfigMng.E.Entity[config.EntityID];
                for (int x = 0; x < entityConfig.ScaleX; x++)
                {
                    for (int y = 0; y < entityConfig.ScaleY; y++)
                    {
                        for (int z = 0; z < entityConfig.ScaleZ; z++)
                        {
                            mData.Map[(int)newPos.x + x, (int)newPos.y + y, (int)newPos.z + z] = new MapData.MapCellData { entityID = 10000 };
                        }
                    }
                }
                mData.Map[(int)newPos.x, (int)newPos.y, (int)newPos.z] = new MapData.MapCellData() { entityID = config.EntityID };
            }
        }
    }
    /// <summary>
    /// 転送門を生成
    /// </summary>
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
            Logger.Error("not find transferGate " + transferGateID);
            Logger.Error(ex.Message);
            return;
        }

        Vector3 newPos = MapCtl.GetGroundPos(mData, config.PosX, config.PosZ);

        mData.Map[(int)newPos.x, (int)newPos.y, (int)newPos.z] = new MapData.MapCellData() { entityID = config.EntityID };
    }

    /// <summary>
    /// 建物を生成
    /// </summary>
    private void AddBuildings()
    {
        var data = mapConfig.Buildings.Split(',');
        if (data[0] == "N")
            return;

        for (int i = 0; i < data.Length; i++)
        {
            Building config = null;

            try
            {
                config = ConfigMng.E.Building[int.Parse(data[i])];
                if (config == null)
                    continue;
            }
            catch (Exception ex)
            {
                Logger.Error("not find Building " + data[i]);
                Logger.Error(ex.Message);
                continue;
            }

            var blueprintData = new BlueprintData(ConfigMng.E.Blueprint[config.Relation].Data);
            var pos = new Vector3Int(config.PosX, config.PosY, config.PosZ);

            foreach (var item in blueprintData.blocks)
            {
                item.SetPos(CommonFunction.Vector3Sum(item.GetPos(), pos));
                mData.Map[item.GetPos().x, item.GetPos().y, item.GetPos().z] = new MapData.MapCellData() { entityID = item.id, direction=item.direction }; 
            }
        }
    }

    
}