using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

[Serializable]
public class MapData
{
    private int id;
    public int SizeX { get; set; }
    public int SizeY { get; set; }
    public int SizeZ { get; set; }

    public Map Config { get => ConfigMng.E.Map[id]; }
    public bool IsHome { get => id == 100; }

    public MapCellData[,,] Map { get => map; }
    [NonSerialized]
    private MapCellData[,,] map;

    [NonSerialized]
    Dictionary<Vector3Int, EntityBase> entityDic = new Dictionary<Vector3Int, EntityBase>();

    public MapData(int mapID)
    {
        id = mapID;
        map = new MapCellData[Config.SizeX, Config.SizeY, Config.SizeZ];
        SizeX = Config.SizeX;
        SizeY = Config.SizeY;
        SizeZ = Config.SizeZ;
    }
    public MapData(string stringData)
    {
        id = 100;
         
        string[] entitys = stringData.Split(',');
        string[] data;
        int index = 0;

        string[] sizeStr = entitys[entitys.Length - 1].Split('-');
        Vector3Int mapSize = new Vector3Int(int.Parse(sizeStr[0]), int.Parse(sizeStr[1]), int.Parse(sizeStr[2]));
        SizeX = mapSize.x;
        SizeY = mapSize.y;
        SizeZ = mapSize.z;

        map = new MapCellData[mapSize.x, mapSize.y, mapSize.z];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    data = entitys[index++].Split('-');
                    int entityId = int.Parse(data[0]);

                    if ((EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Workbench
                        || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Kamado
                        || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Door
                        || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Torch)
                    {
                        map[x, y, z] = new MapCellData() { entityID = entityId, direction = int.Parse(data[1]) };
                    }
                    else
                    {
                        map[x, y, z] = new MapCellData() { entityID = entityId };
                    }
                }
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                Map[x, 0, z] = new MapCellData() { entityID = 1999 };
            }
        }
    }

    public bool IsNull(Vector3Int site)
    {
        return map[site.x, site.y, site.z].entityID == 0;
    }
    public EntityBase GetEntity(Vector3Int site)
    {
        EntityBase entity = null;
        entityDic.TryGetValue(site, out entity);
        return entity;
    }
    public Vector3Int GetMapSize()
    {
        return new Vector3Int(SizeX, SizeY, SizeZ);
    }

    public void IsSurface(Vector3Int pos, bool b = true)
    {
        if (b)
        {
            if (entityDic.ContainsKey(pos))
                return;

            var entity = InstantiateEntity(Map[pos.x, pos.y, pos.z], WorldMng.E.MapCtl.CellParent, pos);
            if (entity != null)
            {
                entityDic[pos] = entity;
            }
        }
        else
        {
            if (entityDic.ContainsKey(pos))
            {
                GameObject.Destroy(entityDic[pos].gameObject);
                entityDic.Remove(pos);
            }
        }
    }
    
    public static EntityBase InstantiateEntity(MapCellData entityCell, Transform parent, Vector3Int pos)
    {
        try
        {
            if (entityCell.entityID < 1)
                return null;

            var config = ConfigMng.E.Entity[entityCell.entityID];
            EntityBase entity = null;
            switch ((EntityType)config.Type)
            {
                case EntityType.Obstacle:
                    break;

                case EntityType.Block:
                case EntityType.Block2:
                case EntityType.Block99:
                    entity = CommonFunction.Instantiate<EntityBlock>(config.Resources, parent, pos);
                    break;

                case EntityType.Resources:
                    entity = CommonFunction.Instantiate<EntityResources>(config.Resources, parent, pos);
                    break;

                case EntityType.Workbench:
                case EntityType.Kamado:
                case EntityType.Door:
                    entity = CommonFunction.Instantiate<EntityBuilding>(config.Resources, parent, pos);
                    var angle = CommonFunction.GetCreateEntityAngleByDirection((DirectionType)entityCell.direction);
                    entity.transform.localRotation = Quaternion.Euler(0, angle, 0);
                    break;

                case EntityType.TransferGate:
                    entity = CommonFunction.Instantiate<EntityTransferGate>(config.Resources, parent, pos);
                    break;

                case EntityType.Torch:
                    if ((DirectionType)entityCell.direction != DirectionType.down)
                    {
                        entity = CommonFunction.Instantiate<EntityTorch>(config.Resources, parent, pos);
                        entity.SetTouchType((DirectionType)entityCell.direction);
                    }
                    break;

                case EntityType.TreasureBox:
                    entity = CommonFunction.Instantiate<EntityTreasureBox>(config.Resources, parent, pos);
                    break;
            }

            if (entity != null)
            {
                entity.EntityID = entityCell.entityID;
                entity.Pos = pos;
                entity.DirectionType = (DirectionType)entityCell.direction;
            }

            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }
    public EntityBase Add(MapCellData entityCell, Vector3Int pos)
    {
        if (entityCell.entityID < 1)
            return null;

        if (entityDic.ContainsKey(pos))
        {
            return entityDic[pos];
        }

        var config = ConfigMng.E.Entity[entityCell.entityID];
        if ((EntityType)config.Type == EntityType.Obstacle)
            return null;

        var entity = InstantiateEntity(entityCell, WorldMng.E.MapCtl.CellParent, pos);
        if (entity != null)
        {
            entityDic[pos] = entity;
            Map[pos.x, pos.y, pos.z] = entityCell;
        }

        var obstacleList = MapCtl.GetEntityPosListByDirection(entityCell.entityID, pos, (DirectionType)entityCell.direction);
        foreach (var item in obstacleList)
        {
            map[item.x, item.y, item.z] = new MapCellData() { entityID = 10000 };
        }

        return entity;
    }
    public void Remove(Vector3Int pos)
    {
        try
        {
            if (MapCtl.IsOutRange(this, pos))
            {
                Logger.Error("Remove mapdata file." + pos);
                return;
            }

            var entity = map[pos.x, pos.y, pos.z];
            var config = ConfigMng.E.Entity[entity.entityID];
            if ((EntityType)config.Type == EntityType.Obstacle)
                return;

            map[pos.x, pos.y, pos.z].entityID = 0;

            // 阻害を削除
            var obstacleList = MapCtl.GetEntityPosListByDirection(entity.entityID, pos, (DirectionType)entity.direction);
            foreach (var item in obstacleList)
            {
                map[item.x, item.y, item.z].entityID = 0;
            }

            if (entityDic.ContainsKey(pos))
            {
                GameObject.Destroy(entityDic[pos].gameObject);
                entityDic.Remove(pos);
            }
            else
            {
                Logger.Error("Remove entity Failure");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public void ClearMapObj()
    {
        entityDic.Clear();
    }

    public string ToStringData()
    {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    if (map[x, y, z].entityID == 10000)
                    {
                        sb.Append(0 + ",");
                        continue;
                    }

                    int entityId = map[x, y, z].entityID;

                    if ((EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Workbench
                       || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Kamado
                       || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Door
                       || (EntityType)ConfigMng.E.Entity[entityId].Type == EntityType.Torch)
                    {
                        sb.Append(entityId + "-" + map[x, y, z].direction + ",");
                    }
                    else
                    {
                        sb.Append(entityId + ",");
                    }
                }
            }
        }
        sb.Append(string.Format("{0}-{1}-{2}", SizeX, SizeY, SizeZ));


        var strMap = sb.ToString();
        Logger.Log("Map");
        Logger.Log(strMap);
        return strMap;
    }
    //public void ParseStringDataOld(string mapData, string resourcesData)
    //{
    //    string[] maps = mapData.Split(',');
    //    string data;
    //    int index = 0;

    //    for (int x = 0; x < 100; x++)
    //    {
    //        for (int y = 0; y < 100; y++)
    //        {
    //            for (int z = 0; z < 100; z++)
    //            {
    //                data = maps[index++];
    //                int entityId = data == "n" ? 0 : int.Parse(data);
    //                map[x, y, z] = new MapCellData() { entityID = entityId };
    //            }
    //        }
    //    }

    //    string[] resources;
    //    if (!string.IsNullOrEmpty(resourcesData))
    //    {
    //        resources = resourcesData.Split(',');
    //    }

    //    for (int x = 0; x < 100; x++)
    //    {
    //        for (int z = 0; z < 100; z++)
    //        {
    //            Map[x, 0, z] = new MapCellData() { entityID = 1999 };
    //        }
    //    }
    //}

    [Serializable]
    public struct MapCellData
    {
        public int entityID;
        public int direction;
    }
}

public enum MapType
{
    Home = 1,
    Guide = 2,
    Brave = 3,
    Test = 99,
}