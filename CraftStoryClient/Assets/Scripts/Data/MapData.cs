using System;
using System.Text;
using System.IO;
using UnityEngine;

[Serializable]
public class MapData
{
    [NonSerialized]
    private BlockData[,,] map;
    private int sizeX { get; set; }
    private int sizeY { get; set; }
    private int sizeZ { get; set; }
    private string strMap { get; set; }

    public MapData(Vector3Int size)
    {
        sizeX = size.x;
        sizeY = size.y;
        sizeZ = size.z;

        map = new BlockData[sizeX, sizeY, sizeZ];
    }
    public MapData(BlockData[,,] map, Vector3Int size)
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
    public BlockData[,,] Map 
    { 
        get { return map; }
    }
    public BlockData GetMap(Vector3Int pos)
    {
        return map[pos.x, pos.y, pos.z];
    }

    public void Add(BlockData data)
    {
        if (data.Pos.x > sizeX || data.Pos.y > sizeY || data.Pos.z > sizeZ)
        {
            Debug.LogError("add mapdata file." + data.Pos);
            return;
        }

        map[data.Pos.x, data.Pos.y, data.Pos.z] = data.Copy(); ;
    }
    public void Remove(Vector3 pos)
    {
        if (pos.x > sizeX || pos.y > sizeY || pos.z > sizeZ)
        {
            Debug.LogError("Remove mapdata file." + pos);
            return;
        }

        map[(int)pos.x, (int)pos.y, (int)pos.z] = null;
    }

    public void ToStringData()
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
        sb.Append("e");

        strMap = sb.ToString();
        Debug.Log(strMap);
    }
    public void ParseStringData()
    {
        map = new BlockData[sizeX, sizeY, sizeZ];

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
                        : new BlockData(data, new Vector3Int(x,y,z));
                }
            }
        }
    }
}