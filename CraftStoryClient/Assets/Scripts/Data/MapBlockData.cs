using JsonConfigData;
using System;
using UnityEngine;

[Serializable]
public class MapBlockData
{
    private int blockID { get; set; }
    private int x { get; set; }
    private int y { get; set; }
    private int z { get; set; }
    private bool isIn { get; set; }
    [NonSerialized]
    private MapBlock block;

    public MapBlockData() { }
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

    public int ID { get => blockID; }
    public int ItemID { get => Config.ItemID; }
    public Block Config { get => ConfigMng.E.Block[ID]; }
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
        return (MapBlockData)MemberwiseClone();
    }
    public void ClearBlock()
    {
        block = null;
    }

    public string ToStringData()
    {
        return string.Format("{0}^{1}", blockID, isIn ? "t" : "f");
    }

    public MapBlock ActiveBlock(Transform parent, bool active = true)
    {
        if (active)
        {
            if (block == null)
            {
                block = CommonFunction.Instantiate<MapBlock>(BaseData.ResourcesName, parent, Pos);
                block.SetData(this);

                if (Pos.y > 15)
                    Debug.Log("Creeate Block." + this);
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
   
    public MapBlock ActiveBlock(bool active = true)
    {
        
        return ActiveBlock(WorldMng.E.MapCtl.CellParent, active);
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
