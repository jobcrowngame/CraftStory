using JsonConfigData;
using System;

[Serializable]
public class ItemData
{
    public int id { get; set; }
    public int itemId { get; set; }
    public int count { get; set; }
    public string newName { get; set; }
    public int equipSite { get; set; }
    public string relationData { get; set; }
    public int islocked { get; set; }

    public ItemData() { }
    public ItemData(int itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }

    public Item Config()
    {
        return ConfigMng.E.Item[itemId];
    }

    public override string ToString()
    {
        return itemId + "x" + count;
    }

    public bool IsLocked { get => islocked == 1; }

    public struct DeleteItemData
    {
        public int guid { get; set; }
    }
}
