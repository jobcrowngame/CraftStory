using System;
using UnityEngine;

[Serializable]
public class MapData
{
    private MapCellData[,,] map { get; set; }
    private int sizeX { get; set; }
    private int sizeY { get; set; }
    private int sizeZ { get; set; }

    public MapData(MapCellData[,,] map, Vector3 size)
    {
        this.map = map;
        sizeX = (int)size.x;
        sizeY = (int)size.y;
        sizeZ = (int)size.z;
    }

    public Vector3 MapSize
    {
        get { return new Vector3(sizeX, sizeY, sizeZ); }
    }
    public MapCellData[,,] Map 
    { 
        get { return map; }
    }

    public void Add(Vector3 pos, MapCellData mcData)
    {
        if (pos.x > sizeX || pos.y > sizeY || pos.z > sizeZ)
        {
            Debug.LogError("add mapdata file." + pos);
            return;
        }

        map[(int)pos.x, (int)pos.y, (int)pos.z] = mcData;
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
}