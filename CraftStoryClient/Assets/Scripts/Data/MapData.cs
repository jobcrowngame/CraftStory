using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

[Serializable]
public class MapData
{
    private int mapID;

    private int sizeX { get; set; }
    private int sizeY { get; set; }
    private int sizeZ { get; set; }
    private string strMap { get; set; }
    private string strEntity { get; set; }

    public bool IsHome { get => mapID == 100; }
    public Map Config { get => ConfigMng.E.Map[mapID]; }
    public string NextSceneName { get; set; }
    public int NextMapID { get; set; }
    public Vector3Int MapSize { get => new Vector3Int(sizeX, sizeY, sizeZ); }

    public MapBlockData[,,] Map { get => map; }
    [NonSerialized]
    private MapBlockData[,,] map;

    public EntityData TransferGate
    {
        get => transferGate;
        set 
        {
            transferGate = value;

            if (transferGate.ID != 0)
            {
                NextMapID = ConfigMng.E.TransferGate[TransferGate.ID].NextMap;
                NextSceneName = ConfigMng.E.TransferGate[TransferGate.ID].NextMapSceneName;
            }
            else
            {
                NextMapID = 100;
                NextSceneName = "Home";
            }
        }
    }
    [NonSerialized]
    private EntityData transferGate;

    public MapData() { }
    public MapData(int mapID, Vector3Int size)
    {
        this.mapID = mapID;

        sizeX = size.x;
        sizeY = size.y;
        sizeZ = size.z;

        map = new MapBlockData[sizeX, sizeY, sizeZ];
    }

    public List<EntityData> Resources 
    {
        get
        {
            if (entityList == null)
                entityList = new List<EntityData>();
            
            return entityList;
        }
    }
    [NonSerialized]
    private List<EntityData> entityList;

    public MapBlockData GetMap(Vector3Int pos)
    {
        return map[pos.x, pos.y, pos.z];
    }

    public void AddBlock(MapBlockData data)
    {
        try
        {
            if (data.Pos.x > sizeX || data.Pos.y > sizeY || data.Pos.z > sizeZ 
                || data.Pos.x < 0 || data.Pos.y < 0 || data.Pos.z < 0)
            {
                Logger.Error("add mapdata file." + data.Pos);
                return;
            }

            map[data.Pos.x, data.Pos.y, data.Pos.z] = data.Copy(); ;
        }
        catch (Exception ex)
        {
            Logger.Error("Add blockData file." + data.Pos);
            Logger.Error(ex.Message);
            Logger.Error(ex.StackTrace);
        }
        
    }
    public void RemoveBlock(Vector3 pos)
    {
        try
        {
            if (pos.x > sizeX || pos.y > sizeY || pos.z > sizeZ)
            {
                Logger.Error("Remove mapdata file." + pos);
                return;
            }

            map[(int)pos.x, (int)pos.y, (int)pos.z] = null;
        }
        catch (Exception ex)
        {
            Logger.Error("Remove blockData file." + pos);
            Logger.Error(ex.Message);
            Logger.Error(ex.StackTrace);
        }
    }

    public void AddEntity(EntityData entity)
    {
        Resources.Add(entity);
    }
    public void RemoveEntity(EntityData entity)
    {
        Resources.Remove(entity);
    }

    public void MapDataToStringData()
    {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (map[x, y, z] == null)

                        sb.Append("n,");
                    else
                        sb.Append(map[x, y, z].ToStringData() + ",");
                }
            }
        }

        strMap = sb.ToString();
        Logger.Log("Map");
        Logger.Log(strMap);
    }
    public void EntityDataToStringData()
    {
        StringBuilder sb = new StringBuilder();

        if(TransferGate != null) sb.Append(TransferGate.ToStringData());

        for (int i = 0; i < Resources.Count; i++)
        {
            sb.Append("," + Resources[i].ToStringData());
        }

        strEntity = sb.ToString();
    }

    public void ParseStringData()
    {
        map = new MapBlockData[sizeX, sizeY, sizeZ];

        string[] blocks = strMap.Split(',');
        string data;
        int index = 0;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (x == sizeX - 1 && y == sizeY - 1 && z == sizeZ - 1)
                        break;

                    data = blocks[index++];
                    map[x,y,z] = data == "n" 
                        ? null 
                        : new MapBlockData(data, new Vector3Int(x,y,z));
                }
            }
        }

        string[] entitys = strEntity.Split(',');
        TransferGate = new EntityData(entitys[0]);
        for (int i = 1; i < entitys.Length; i++)
        {
            Resources.Add(new EntityData(entitys[i]));
        }
    }

    public void AddResources(EntityData resourceData)
    {
        Resources.Add(resourceData);
    }
}