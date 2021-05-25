using JsonConfigData;
using System;
using UnityEngine;

[Serializable]
public class BlockData
{
    private int X { get; set; }
    private int Y { get; set; }
    private int Z { get; set; }
    private int blockID { get; set; }
    private bool isIn { get; set; }

    public Block BaseData 
    {
        get => ConfigMng.E.BlockConfig[blockID];
    }

    public BlockData(int blockID) 
    {
        this.blockID = blockID;
    }

    public Vector3Int Pos
    {
        get { return new Vector3Int(X, Y, Z); }
        set
        {
            X = value.x;
            Y = value.y;
            Z = value.z;
        }
    }

    public bool IsIn { get => isIn; set => isIn = value; }

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
