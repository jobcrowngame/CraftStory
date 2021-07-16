using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

public class MapData
{
    private int id;

    public Vector3Int MapSize { get => new Vector3Int(ConfigMng.E.Map[id].SizeX, ConfigMng.E.Map[id].SizeY, ConfigMng.E.Map[id].SizeZ); }

    public Map Config { get => ConfigMng.E.Map[id]; }
    public bool IsHome { get => id == 100; }

    public int[,,] Map { get => map; }
    private int[,,] map;

    Dictionary<Vector3Int, EntityBase> entityDic = new Dictionary<Vector3Int, EntityBase>();

    public MapData(int mapID)
    {
        id = mapID;
        map = new int[MapSize.x, MapSize.y, MapSize.z];
    }

    public bool IsNull(Vector3Int site)
    {
        return map[site.x, site.y, site.z] == 0;
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

            map[pos.x, pos.y, pos.z] = 0;
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
            entityDic[pos] = null;
        }
        else
        {
            Logger.Warning("Remove entity Failure");
        }
    }
    public EntityBase Add(int entityId, Vector3Int pos)
    {
        try
        {
            if (entityId < 1)
                return null;

            if (entityDic.ContainsKey(pos))
            {
                return entityDic[pos];
            }

            var config = ConfigMng.E.Entity[entityId];
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

                case EntityType.Craft:
                case EntityType.Kamado:
                case EntityType.Dor:
                    entity = CommonFunction.Instantiate<EntityBuilding>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    break;

                case EntityType.TransferGate:
                    entity = CommonFunction.Instantiate<EntityTransferGate>(config.Resources, WorldMng.E.MapCtl.CellParent, pos);
                    break;
                default:
                    break;
            }

            entity.EntityID = entityId;
            entity.Pos = pos;
            entityDic[pos] = entity;
            Map[pos.x, pos.y, pos.z] = entityId;

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
                    sb.Append(map[x, y, z] + ",");
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
        string data;
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
                    data = entitys[index++];
                    map[x, y, z] = int.Parse(data);
                }
            }
        }
    }
}