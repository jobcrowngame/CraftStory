using System;
using System.Collections.Generic;

[Serializable]
public class ItemTable
{
    public List<ItemData> list = new List<ItemData>();

    [Serializable]
    public class Row
    {
        public int id { get; set; }
        public int itemId { get; set; }
        public string newName { get; set; }
        public int count { get; set; }
        public int equipSite { get; set; }
        public string relationData { get; set; }
        public string textureName { get; set; }
        public bool islocked { get; set; }
        public string skills { get; set; }
    }
}