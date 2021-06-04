using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlueprintData
{
    private List<MapBlockData> blocks { get; set; }
    private int sizeX { get; set; }
    private int sizeZ { get; set; }

    public BlueprintData()
    {
        blocks = new List<MapBlockData>();
    }
    public BlueprintData(object data)
    {
        var bData = (BlueprintData)data;
        blocks = new List<MapBlockData>();

        foreach (MapBlockData item in bData.BlockList)
        {
            blocks.Add(item.Copy());
        }

        sizeX = bData.sizeX;
        sizeZ = bData.sizeZ;
    }
    public BlueprintData(List<MapBlockData> blocks, Vector2Int size)
    {
        this.blocks = blocks;
        Size = size;
    }

    public Vector2Int Size
    {
        get => new Vector2Int(sizeX, sizeZ);
        set
        {
            sizeX = value.x;
            sizeZ = value.y;
        }
    }

    public List<MapBlockData> BlockList { get => blocks; set => blocks = value; }
}