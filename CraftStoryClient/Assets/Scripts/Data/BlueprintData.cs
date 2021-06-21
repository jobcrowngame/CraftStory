using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlueprintData
{
    public List<MapBlockData> BlockList 
    { 
        get
        {
            if (blocks == null)
                blocks = new List<MapBlockData>();
            
            return blocks; 
        } 
        set => blocks = value;
    }
    private List<MapBlockData> blocks;

    private int sizeX { get; set; }
    private int sizeZ { get; set; }

    [NonSerialized]
    private bool isDuplicate;
    public bool IsDuplicate { get => isDuplicate; set => isDuplicate = value; }

    public BlueprintData(string json)
    {
        BlueprintJsonData data = ToData(json);
        BlockList = new List<MapBlockData>();
        IsDuplicate = false;

        for (int i = 0; i < data.blocks.Count; i++)
        {
            BlockList.Add(new MapBlockData()
            {
                ID = data.blocks[i].id,
                Pos = new Vector3Int(data.blocks[i].posX, data.blocks[i].posY, data.blocks[i].posZ)
            });
        }

        sizeX = data.sizeX;
        sizeZ = data.sizeZ;
    }
    public BlueprintData(List<MapBlockData> blocks, Vector2Int size)
    {
        BlockList = blocks;
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

    public string ToJosn()
    {
        BlueprintJsonData data = new BlueprintJsonData();
        data.sizeX = sizeX;
        data.sizeZ = sizeZ;
        data.blocks = new List<BlueprintBlockData>();

        for (int i = 0; i < BlockList.Count; i++)
        {
            data.blocks.Add(new BlueprintBlockData()
            {
                id = BlockList[i].ID,
                posX = BlockList[i].Pos.x,
                posY = BlockList[i].Pos.y,
                posZ = BlockList[i].Pos.z
            });
        }

        return JsonMapper.ToJson(data);
    }
    public BlueprintJsonData ToData(string json)
    {
        if (string.IsNullOrEmpty(json))
            return new BlueprintJsonData();
        
        return JsonMapper.ToObject<BlueprintJsonData>(json);
    }

    public struct BlueprintJsonData
    {
        public int sizeX;
        public int sizeZ;
        public List<BlueprintBlockData> blocks;
    }

    public struct BlueprintBlockData
    {
        public int id;
        public int posX;
        public int posY;
        public int posZ;
    }
}