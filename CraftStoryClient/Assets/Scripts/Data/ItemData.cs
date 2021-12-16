using JsonConfigData;
using System;
using UnityEngine;

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
    public string textureName { get; set; } // 設計図のテクスチャ
    public int islocked { get; set; } // 販売出来ないようにロック

    public ItemData() { }
    public ItemData(int itemId, int count)
    {
        this.itemId = itemId;
        this.count = count;
    }

    public Item Config { get => ConfigMng.E.Item[itemId];}

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

public enum ItemSite
{
    None = 0,

    // ホーム用アイテム装備
    HomeItem1 = 1,
    HomeItem2 = 2,
    HomeItem3 = 3,
    HomeItem4 = 4,
    HomeItem5 = 5,
    HomeItem6 = 6,

    /// <summary>
    /// 武器
    /// </summary>
    Weapon = 101,

    /// <summary>
    /// 防具
    /// </summary>
    Armor = 102,
}

public enum ItemType
{
    None = 0,
    Block = 1,

    /// <summary>
    /// 素材
    /// </summary>
    Resource = 2,

    /// <summary>
    /// 農作物
    /// </summary>
    Crops = 3,

    /// <summary>
    /// 向きありブロック
    /// </summary>
    Block2 = 4,

    /// <summary>
    /// 空の設計図
    /// </summary>
    NullBlueprint = 51,

    /// <summary>
    /// 設計図
    /// </summary>
    Blueprint = 52,

    /// <summary>
    /// 掲示板
    /// </summary>
    Mission = 53,

    /// <summary>
    /// クラフトシード
    /// </summary>
    CraftSeed = 90,

    /// <summary>
    /// クリスタル
    /// </summary>
    Crystal = 91,

    /// <summary>
    /// ポイント
    /// </summary>
    Point = 92,

    /// <summary>
    /// ガチャチケット
    /// </summary>
    GachaTicket = 93,

    /// <summary>
    /// ロイヤルコイン
    /// </summary>
    RoyalCoin = 94,

    /// <summary>
    /// 3倍チケット
    /// </summary>
    ThreeXTicket = 95,

    Workbench = 1000,
    Kamado = 1001,

    /// <summary>
    /// 装備作業台
    /// </summary>
    EquipmentWorkbench = 1002,

    /// <summary>
    /// 料理台
    /// </summary>
    CookingTable = 1003,

    Door = 2000,
    Bed = 2001,
    Torch = 2100,

    /// <summary>
    /// ランタン
    /// </summary>
    Lanthanum = 2101,

    /// <summary>
    /// 普通のObject
    /// </summary>
    NomoObject = 3000,

    /// <summary>
    /// 向きがある普通のObject
    /// </summary>
    HaveDirectionNomoObject = 3001,

    /// <summary>
    /// 爆弾
    /// </summary>
    Blast = 4000,

    /// <summary>
    /// クワ
    /// </summary>
    Hoe = 4100,

    /// <summary>
    /// 武器
    /// </summary>
    Weapon = 5001,

    /// <summary>
    /// 防具
    /// </summary>
    Armor = 5002,

    TransferGate = 9999,
}