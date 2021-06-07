using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

[Serializable]
public class MapData
{
    public int MapID { get => mapID; }
    private int mapID;

    public Map Config { get => ConfigMng.E.Map[mapID]; }

    private int sizeX { get; set; }
    private int sizeY { get; set; }
    private int sizeZ { get; set; }
    private string strMap { get; set; }
    private string strEntity { get; set; }

    [NonSerialized]
    private MapBlockData[,,] map;
    [NonSerialized]
    private List<EntityData> entityList;
    [NonSerialized]
    private EntityData transferGate;

    public MapData(int mapID, Vector3Int size)
    {
        this.mapID = mapID;

        sizeX = size.x;
        sizeY = size.y;
        sizeZ = size.z;

        map = new MapBlockData[sizeX, sizeY, sizeZ];
        entityList = new List<EntityData>();
    }
    public MapData(int mapID, MapBlockData[,,] map, Vector3Int size)
    {
        this.map = map;
        sizeX = size.x;
        sizeY = size.y;
        sizeZ = size.z;
    }

    public Vector3Int MapSize
    {
        get { return new Vector3Int(sizeX, sizeY, sizeZ); }
    }
    public MapBlockData[,,] Map 
    { 
        get { return map; }
    }
    public MapBlockData GetMap(Vector3Int pos)
    {
        return map[pos.x, pos.y, pos.z];
    }
    public List<EntityData> Resources { get => entityList; }
    public EntityData TransferGate
    {
        get => transferGate;
        set => transferGate = value;
    }

    public void Add(MapBlockData data)
    {
        try
        {
            if (data.Pos.x > sizeX || data.Pos.y > sizeY || data.Pos.z > sizeZ)
            {
                Debug.LogError("add mapdata file." + data.Pos);
                return;
            }

            map[data.Pos.x, data.Pos.y, data.Pos.z] = data.Copy(); ;
        }
        catch (Exception ex)
        {
            Debug.LogError("Add blockData file." + data.Pos);
            Debug.LogError(ex.Message);
            Debug.LogError(ex.StackTrace);
        }
        
    }
    public void Remove(Vector3 pos)
    {
        try
        {
            if (pos.x > sizeX || pos.y > sizeY || pos.z > sizeZ)
            {
                Debug.LogError("Remove mapdata file." + pos);
                return;
            }

            map[(int)pos.x, (int)pos.y, (int)pos.z] = null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Remove blockData file." + pos);
            Debug.LogError(ex.Message);
            Debug.LogError(ex.StackTrace);
        }
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
        Debug.Log("Map");
        Debug.Log(strMap);
    }
    public void EntityDataToStringData()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(transferGate.ToStringData());

        for (int i = 0; i < Resources.Count; i++)
        {
            sb.Append("," + Resources[i].ToStringData());
        }

        strEntity = sb.ToString();
        Debug.Log("Entity");
        Debug.Log(strEntity);
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

        entityList = new List<EntityData>();
        string[] entitys = strEntity.Split(',');
        transferGate = new EntityData(entitys[0]);
        for (int i = 1; i < entitys.Length; i++)
        {
            entityList.Add(new EntityData(entitys[i]));
        }
    }

    public void AddResources(EntityData resourceData)
    {
        entityList.Add(resourceData);
    }
}