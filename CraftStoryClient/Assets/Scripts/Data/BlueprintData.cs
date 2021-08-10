using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlueprintData
{
    public int sizeX { get; set; }
    public int sizeZ { get; set; }

    public List<BlueprintEntityData> blocks 
    { 
        get
        {
            if (mblocks == null)
                mblocks = new List<BlueprintEntityData>();
            
            return mblocks; 
        } 
        set => mblocks = value;
    }
    private List<BlueprintEntityData> mblocks;

    [NonSerialized]
    private bool isDuplicate;
    public bool IsDuplicate { get => isDuplicate; set => isDuplicate = value; }

    public BlueprintData() { }
    public BlueprintData(string json)
    {
        try
        {
            var obj = JsonMapper.ToObject<BlueprintData>(json);
            sizeX = obj.sizeX;
            sizeZ = obj.sizeZ;
            mblocks = obj.blocks;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
        finally
        {
            sizeX = 0;
            sizeZ = 0;
        }
    }
    public BlueprintData(List<EntityBase> entitys, Vector2Int size, Vector3Int centerPos)
    {
        sizeX = size.x;
        sizeZ = size.y;
        foreach (var entity in entitys)
        {
            blocks.Add(new BlueprintEntityData()
            {
                id = entity.EntityID,
                posX = entity.Pos.x - centerPos.x,
                posY = entity.Pos.y - centerPos.y,
                posZ = entity.Pos.z - centerPos.z,
                direction = (int)entity.DirectionType
            });
        }
    }

    public string ToJosn()
    {
        return JsonMapper.ToJson(this);
    }

    /// <summary>
    /// 有効範囲チェック
    /// </summary>
    public bool CheckPos(Vector3Int buildPos)
    {
        foreach (var item in mblocks)
        {
            var newPos = CommonFunction.Vector3Sum(item.GetPos(), buildPos);
            if (MapCtl.IsOutRange(DataMng.E.MapData, newPos))
            {
                CommonFunction.ShowHintBar(8);
                return false;
            }
        }

        return true;
    }

    public class BlueprintEntityData
    {
        public int id { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }
        public int direction { get; set; }

        public Vector3Int GetPos()
        {
            return new Vector3Int(posX, posY, posZ);
        }
        public void SetPos(Vector3Int Pos)
        {
            posX = Pos.x;
            posY = Pos.y;
            posZ = Pos.z;
        }
    }
}

public struct BlueprintDataOld
{
    public int sizeX;
    public int sizeZ;
    public List<BlueprintDataCellOld> blocks;
}

public struct BlueprintDataCellOld
{
    public int id;
    public int posX;
    public int posY;
    public int posZ;
}