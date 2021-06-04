using JsonConfigData;
using System;

[Serializable]
public class ItemData
{
    public int ItemID { get => itemID; }
    private int itemID;

    public int Count { get => count; set => count = value; }
    private int count;

    public object Data { get => data; set => data = value; }
    private object data;

    public int CanAddCount { get => Config.MaxCount - count; }
    public Item Config { get => ConfigMng.E.Item[itemID]; }

    public ItemData() { }
    public ItemData(int id, object data) 
    {
        itemID = id;
        this.data = data;
    }

    public string ToString()
    {
        return ItemID + "x" + Count;
    }
}
