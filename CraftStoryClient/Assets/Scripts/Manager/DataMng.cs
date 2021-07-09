using JsonConfigData;
using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "/MapData.dat";
    const string UserDataName = "/UserData.dat";

    public string token { get; set; }

    public RuntimeData RuntimeData { get; set; }

    public UserData UserData
    {
        get => uData;
        private set => uData = value;
    }
    private UserData uData;

    public MapData MapData 
    {
        get {
            if (mData == null)
                mData = new MapData();

            return mData;
        }
        set => mData = value;
    }
    private MapData mData;

    public MapData HomeData
    {
        get => homeData;
        set => homeData = value;
    }
    private MapData homeData;

    public MyShopData MyShop
    {
        get
        {
            if (myShop == null)
                myShop = new MyShopData();
            return myShop;
        }
        set => myShop = value;
    }
    private MyShopData myShop;

    public List<ItemData> Items { 
        get
        {
            if (items == null)
                items = new List<ItemData>();

            return items;
        }
        set => items = value;
    }
    private List<ItemData> items;

    public override void Init()
    {
        base.Init();

        RuntimeData = new RuntimeData();
    }

    public void NewUser(string id, string pw)
    {
        uData = new UserData()
        {
            Account = id,
            UserPW = pw
        };
    }

    public void Save()
    {
        Logger.Log("Save Data");

        if (uData != null)
            SaveLoadFile.E.Save(uData, PublicPar.SaveRootPath + UserDataName);

        if (HomeData != null)
        {
            HomeData.MapDataToStringData();
            HomeData.EntityDataToStringData();
            SaveLoadFile.E.Save(HomeData, PublicPar.SaveRootPath + MapDataName);
        }
    }

    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + UserDataName);

        HomeData = (MapData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if (HomeData != null) HomeData.ParseStringData();

        return true;
    }

#region Item

    public ItemData GetItemByGuid(int guid)
    {
        if (items == null)
            return null;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == guid)
            {
                return items[i];
            }
        }
        return null;
    }
    public ItemData GetItemByEquipedSite(int site)
    {
        if (items == null)
            return null;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].equipSite == site)
            {
                return items[i];
            }
        }
        return null;
    }
    public int GetItemCountByItemID(int itemId)
    {
        if (Items == null)
            return 0;

        int count = 0;

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].itemId == itemId)
            {
                count += Items[i].count;
            }
        }

        return count;
    }

    public void AddItem(int itemID, int count = 1, Action action = null)
    {
        NWMng.E.AddItem((rp)=> 
        {
            NWMng.E.GetItemList((rp2) =>
            {
                GetItems(rp2);
                if (action != null) action();
            });
        }, itemID, count);
    }
    public void AddItems(Dictionary<int,int> items, Action action = null)
    {
        if (items.Count <= 0)
            return;

        NWMng.E.AddItems((rp) =>
        {
            NWMng.E.GetItemList((rp2) =>
            {
                GetItems(rp2);
                if (action != null) action();
            });
        }, items);
    }
    public void AddItemInData(int itemID, int count, string newName, string data, Action action = null)
    {

        NWMng.E.AddItemInData((rp) =>
        {
            NWMng.E.GetItemList((rp2) =>
            {
                GetItems(rp2);
                if (action != null) action();
            });
        }, itemID, count, newName, data);
    }
    /// <summary>
    /// 消耗アイテム
    /// </summary>
    public void ConsumableItemByGUID(int guid, int count = 1)
    {
        if (GetItemByGuid(guid).count < count)
        {
            Logger.Log("No item stip");
        }
        else
        {
            NWMng.E.RemoveItemByGuid((rp) =>
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    GetItems(rp2);
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                });
            }, guid, count);
        }
    }
    public bool ConsumableItem(int itemID, int count = 1)
    {
        bool itemCheck = CheckConsumableItem(itemID, count);
        if (itemCheck)
        {
            NWMng.E.RemoveItem((rp) =>
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    GetItems(rp2);
                });
            }, itemID, count);
        }
        else
        {
            CommonFunction.ShowHintBar(1);
        }

        return itemCheck;
    }
    /// <summary>
    /// 消耗するアイテムの数が足りかをチェック
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="count">消耗数</param>
    /// <returns>消耗できるかの結果</returns>
    public bool CheckConsumableItem(int itemID, int count = 1)
    {
        int itemCount = 0;
        List<ItemData> items = null;
        TryGetItems(itemID, out items);

        for (int i = 0; i < items.Count; i++)
        {
            itemCount += items[i].count;
        }

        return itemCount >= count;
    }
    

    private bool TryGetItems(int itemID, out List<ItemData> itemList)
    {
        itemList = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemId == itemID)
            {
                itemList.Add(items[i]);
            }
        }

        return itemList.Count > 0;
    }

    public static void GetItems(JsonData jsonData)
    {
        try
        {
            if (string.IsNullOrEmpty(jsonData.ToString()))
                return;

            E.Items = JsonMapper.ToObject<List<ItemData>>(jsonData.ToJson());
            if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }
    }
    public static void GetCoins(JsonData jsonData)
    {
        if (string.IsNullOrEmpty(jsonData.ToString()))
            return;

        E.UserData.Coin1 = (int)jsonData["coin1"];
        E.UserData.Coin2 = (int)jsonData["coin2"];
        E.UserData.Coin3 = (int)jsonData["coin3"];
    }

#endregion
}