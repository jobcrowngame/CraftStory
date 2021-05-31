using JsonConfigData;
using System;
using UnityEngine;

public class MapBlockData
{
    private int blockID;
    private int x, y, z;
    private bool isIn;
    private MapBlock block;

    public MapBlockData(int blockID, Vector3Int pos)
    {
        this.blockID = blockID;

        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
    public MapBlockData(string strData, Vector3Int pos)
    {
        string[] data = strData.Split('^');

        blockID = int.Parse(data[0]);
        IsIn = data[1] == "t";

        x = pos.x;
        y = pos.y;
        z = pos.z;
    }

    public Vector3Int Pos
    {
        get { return new Vector3Int(x, y, z); }
        set
        {
            x = value.x;
            y = value.y;
            z = value.z;
        }
    }
    public Block BaseData { get => ConfigMng.E.Block[blockID]; }
    public bool IsIn { get => isIn; set => isIn = value; }
    public MapBlock Block { get => block; }

    public override string ToString()
    {
        return string.Format("POS:{0}, BlockType:{1}, DeleteTime:{2}", Pos, BaseData.Name, BaseData.DestroyTime);
    }

    public MapBlockData Copy()
    {
        return new MapBlockData(blockID, Pos);
    }

    public string ToStringData()
    {
        return string.Format("{0}^{1}", blockID, isIn ? "t" : "f");
    }

    public MapBlock ActiveBlock(bool active = true)
    {
        if (active)
        {
            if (block == null)
            {
                string sourcesFullPath = PublicPar.BlockRootPath + BaseData.ResourcesName;
                block = CommonFunction.Instantiate<MapBlock>(sourcesFullPath, WorldMng.E.MapCtl.CellParent, Pos);
                block.SetData(this);
            }
            else
                block.gameObject.SetActive(active);
        }
        else
        {
            if (block != null)
                block.gameObject.SetActive(false);
        }

        IsIn = !active;
        return block;
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
