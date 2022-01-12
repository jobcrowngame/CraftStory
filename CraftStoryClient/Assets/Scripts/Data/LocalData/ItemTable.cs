using System;
using System.Collections.Generic;

[Serializable]
public class ItemTable
{
    public List<Row> items = new List<Row>();

    public List<ItemData> GetItemList()
    {
        List<ItemData> list = new List<ItemData>();
        foreach (var item in items)
        {
            list.Add(new ItemData(item));
        }
        return list;
    }

    [Serializable]
    public struct Row
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public string newName { get; set; }
        public int count { get; set; }
        public int equipSite { get; set; }
        public string relationData { get; set; }
        public string textureName { get; set; }
        public bool islocked { get; set; }
    }
}