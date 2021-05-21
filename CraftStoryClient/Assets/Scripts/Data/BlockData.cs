using JsonConfigData;
using System;
using UnityEngine;

[Serializable]
public class BlockData
{
    private float X { get; set; }
    private float Y { get; set; }
    private float Z { get; set; }
    private int blockID { get; set; }

    public Block BaseData 
    {
        get => ConfigMng.E.BlockConfig[blockID];
    }

    public BlockData(int blockID) 
    {
        this.blockID = blockID;
    }

    public Vector3 Pos
    {
        get { return new Vector3(X, Y, Z); }
        set
        {
            X = value.x;
            Y = value.y;
            Z = value.z;
        }
    }

    public override string ToString()
    {
        return string.Format("POS:{0}, BlockType:{1}, DeleteTime:{2}", Pos, BaseData.Name, BaseData.DestroyTime);
    }

    public BlockData Copy()
    {
        return new BlockData(blockID)
        {
            X = X,
            Y = Y,
            Z = Z
        };
    }
}

//public enum MapCellType
//{
//    Black,
//    Blue,
//    Red,
//    Green,
//}

public enum BlockType
{
    Soil,
    Stone,
}
