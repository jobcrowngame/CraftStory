using JsonConfigData;
using System;

/// <summary>
/// アイテムデータ
/// </summary>
[Serializable]
public class ItemData
{
    public int id { get; set; } // GUID
    public int itemId { get; set; } // アイテムＩＤ
    public int count { get; set; } // 数
    public string newName { get; set; } // 設計図などの新しい名
    public int equipSite { get; set; } // 装備している箇所
    public string relationData { get; set; } // 連れてるデータ（設計図など）
    public int islocked { get; set; } // 販売出来ないようにロック

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
