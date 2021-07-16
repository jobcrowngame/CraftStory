using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

[Serializable]
public class MapData2
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


    public MapData2() { }
    public MapData2(int mapID, Vector3Int size)
    {
    }

    //public void MapDataToStringData()
    //{
    //    StringBuilder sb = new StringBuilder();

    //    for (int x = 0; x < sizeX; x++)
    //    {
    //        for (int y = 0; y < sizeY; y++)
    //        {
    //            for (int z = 0; z < sizeZ; z++)
    //            {
    //                if (map[x, y, z] == null)

    //                    sb.Append("n,");
    //                else
    //                    sb.Append(map[x, y, z].ToStringData() + ",");
    //            }
    //        }
    //    }

    //    strMap = sb.ToString();
    //    Logger.Log("Map");
    //    Logger.Log(strMap);
    //}
    //public void EntityDataToStringData()
    //{
    //    StringBuilder sb = new StringBuilder();

    //    if(TransferGate != null) sb.Append(TransferGate.ToStringData());

    //    for (int i = 0; i < Resources.Count; i++)
    //    {
    //        sb.Append("," + Resources[i].ToStringData());
    //    }

    //    strEntity = sb.ToString();
    //}

    //public void ParseStringData()
    //{
    //    map = new MapBlockData[sizeX, sizeY, sizeZ];

    //    string[] blocks = strMap.Split(',');
    //    string data;
    //    int index = 0;

    //    for (int x = 0; x < sizeX; x++)
    //    {
    //        for (int y = 0; y < sizeY; y++)
    //        {
    //            for (int z = 0; z < sizeZ; z++)
    //            {
    //                if (x == sizeX - 1 && y == sizeY - 1 && z == sizeZ - 1)
    //                    break;

    //                data = blocks[index++];
    //                map[x,y,z] = data == "n" 
    //                    ? null 
    //                    : new MapBlockData(data, new Vector3Int(x,y,z));
    //            }
    //        }
    //    }

    //    string[] entitys = strEntity.Split(',');
    //    TransferGate = new EntityData(entitys[0]);
    //    for (int i = 1; i < entitys.Length; i++)
    //    {
    //        Resources.Add(new EntityData(entitys[i]));
    //    }
    //}
}