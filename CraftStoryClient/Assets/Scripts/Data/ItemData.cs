﻿using JsonConfigData;
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

    public Item Config()
    {
        return ConfigMng.E.Item[itemId];
    }

    public override string ToString()
    {
        return itemId + "x" + count;
    }
}
