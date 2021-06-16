using JsonConfigData;
using System;

[Serializable]
public class ItemData
{
    public int id { get; set; }

    public int itemId { get; set; }

    public int count { get; set; }

    public int equipSite { get; set; }

    public object Data { get => data; set => data = value; }
    private object data;

    public int CanAddCount { get => Config().MaxCount - count; }

    public ItemData() { }
    public ItemData(int id, object data)
    {
        itemId = id;
        this.data = data;
    }

    public Item Config()
    {
        return ConfigMng.E.Item[itemId];
    }

    public override string ToString()
    {
        return itemId + "x" + count;
    }
}
