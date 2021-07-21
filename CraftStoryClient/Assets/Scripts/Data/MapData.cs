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

    public Vector3Int MapSize { get => new Vector3Int(ConfigMng.E.Map[id].SizeX, ConfigMng.E.Map[id].SizeY, ConfigMng.E.Map[id].SizeZ); }

    public Map Config { get => ConfigMng.E.Map[id]; }
    public bool IsHome { get => id == 100; }

    public MapCellData[,,] Map { get => map; }
    private MapCellData[,,] map;

    public string strMap { get; set; }
    public string strEntity { get; set; }

    Dictionary<Vector3Int, EntityBase> entityDic = new Dictionary<Vector3Int, EntityBase>();

    public MapData(int mapID)
    {
        id = mapID;
        map = new MapCellData[MapSize.x, MapSize.y, MapSize.z];
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

    public void Remove(Vector3Int pos)
    {
        try
        {
            if (pos.x > MapSize.x || pos.y > MapSize.y || pos.z > MapSize.z)
            {
                Logger.Error("Remove mapdata file." + pos);
                return;
            }

            map[pos.x, pos.y, pos.z].entityID = 0;
            if (entityDic.ContainsKey(pos))
            {
                GameObject.Destroy(entityDic[pos].gameObject);
                entityDic.Remove(pos);
            }
            else
            {
                Logger.Warning("Remove entity Failure");
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
    public void DeleteObj(Vector3Int pos)
    {
        if (entityDic.ContainsKey(pos))
        {
            GameObject.Destroy(entityDic[pos].gameObject);
            entityDic.Remove(pos);
        }
        else
        {
            Logger.Warning("Remove entity Failure");
        }
    }
    public EntityBase Add(MapCellData entityCell, Vector3Int pos)
    {
        try
        {
            if (entityCell.entityID < 1)
                return null;

            if (entityDic.ContainsKey(pos))
            {
                return entityDic[pos];
            }

            var config = ConfigMng.E.Entity[entityCell.entityID];
            EntityBase entity = null;
            switch ((EntityType)config.Type)
            {
                case EntityType.Obstacle:
                    break;

                case EntityType.Block:
                case EntityType.Block2:
                    entity = CommonFunction.Instantiate<EntityBlock>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    break;

                case EntityType.Resources:
                    entity = CommonFunction.Instantiate<EntityResources>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    break;

                case EntityType.Workbench:
                case EntityType.Kamado:
                case EntityType.Door:
                    entity = CommonFunction.Instantiate<EntityBuilding>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    var angle = CommonFunction.GetCreateEntityAngleByDirection((DirectionType)entityCell.direction);
                    entity.transform.localRotation = Quaternion.Euler(0, angle, 0);
                    entity.DirectionType = (DirectionType)entityCell.direction;
                    break;

                case EntityType.TransferGate:
                    entity = CommonFunction.Instantiate<EntityTransferGate>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    break;

                case EntityType.Torch:
                    if ((DirectionType)entityCell.direction != DirectionType.down)
                    {
                        entity = CommonFunction.Instantiate<EntityTorch>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                        entity.SetTouchType((DirectionType)entityCell.direction);
                    }
                    break;
                default:
                    break;
            }

            if (entity != null)
            {
                entity.EntityID = entityCell.entityID;
                entity.Pos = pos;
                entityDic[pos] = entity;
                Map[pos.x, pos.y, pos.z] = entityCell;
            }

            //for (int x = 0; x < config.ScaleX; x++)
            //{
            //    for (int z = 0; z < config.ScaleZ; z++)
            //    {
            //        for (int y = 0; y < config.ScaleY; y++)
            //        {
            //            if (x == 0 && y == 0 && z == 0)
            //                continue;
                        
            //            map[pos.x + x, pos.y + y, pos.z + z] = -1;
            //        }
            //    }
            //}

            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }
    public EntityBase Add(Vector3Int pos)
    {
        return Add(map[pos.x, pos.y, pos.z], pos);
    }
    public void ClearMapObj()
    {
        entityDic.Clear();
    }

    public string ToStringData()
    {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < MapSize.x; x++)
        {
            for (int y = 0; y < MapSize.y; y++)
            {
                for (int z = 0; z < MapSize.z; z++)
                {
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
        sb.Append(string.Format("{0}-{1}-{2}", MapSize.x, MapSize.y, MapSize.z));


        var strMap = sb.ToString();
        Logger.Log("Map");
        Logger.Log(strMap);
        return strMap;
    }
    public void ParseStringData(string strMap)
    {
        string[] entitys = strMap.Split(',');
        string[] data;
        int index = 0;

        string[] sizeStr = entitys[entitys.Length - 1].Split('-');
        Vector3Int mapSize = new Vector3Int(int.Parse(sizeStr[0]), int.Parse(sizeStr[1]), int.Parse(sizeStr[2]));

        //map = new EntityBase[mapSize.x, mapSize.y, mapSize.z];

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
    }
    public void ParseStringDataOld(string mapData, string resourcesData)
    {
        string[] maps = mapData.Split(',');
        string data;
        int index = 0;

        for (int x = 0; x < 100; x++)
        {
            for (int y = 0; y < 100; y++)
            {
                for (int z = 0; z < 100; z++)
                {
                    data = maps[index++];
                    int entityId = data == "n" ? 0 : int.Parse(data);
                    map[x, y, z] = new MapCellData() { entityID = entityId };
                }
            }
        }

        string[] resources;
        if (!string.IsNullOrEmpty(resourcesData))
        {
            resources = resourcesData.Split(',');
        }
    }

    public struct MapCellData
    {
        public int entityID;
        public int direction;
    }
}