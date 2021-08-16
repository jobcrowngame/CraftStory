using LitJson;
using System;
using System.Collections.Generic;

[Serializable]
public class DataMng : Single<DataMng>
{
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
        get
        {
            switch (RuntimeData.MapType)
            {
                case MapType.Home: return mHomeData;
                case MapType.Guide: return mGuideData;
                default: return mBraveData;
            }
        }
    }
    private MapData mHomeData;
    private MapData mGuideData;
    private MapData mBraveData;

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
        get => RuntimeData.MapType == MapType.Guide ? guideItems : items;
    }
    private List<ItemData> items = new List<ItemData>();
    private List<ItemData> guideItems = new List<ItemData>();
    public List<ItemData> GuideItems { get => guideItems; }

    public override void Init()
    {
        base.Init();

        RuntimeData = new RuntimeData();
    }

    public void Save()
    {
        Logger.Log("Save Data");

        if (uData != null)
            SaveLoadFile.E.Save(uData, PublicPar.SaveRootPath + PublicPar.UserDataName);

        if (mHomeData != null)
            SaveLoadFile.E.Save(mHomeData.ToStringData(), PublicPar.SaveRootPath + PublicPar.MapDataName);
    }

    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.UserDataName);

        var mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.MapDataName);
        if (!string.IsNullOrEmpty(mapData)) mHomeData = new MapData(mapData);

        if (mHomeData == null) mHomeData = WorldMng.E.MapCtl.CreateMapData(100); ;

        return true;
    }

    #region User
    public void NewUser(string id, string pw)
    {
        uData = new UserData()
        {
            Account = id,
            UserPW = pw
        };
    }
    #endregion
    #region Map

    public void SetMapData(int mapId)
    {
        MapData mData = null;
        MapType mType = MapType.Home;

        if (mapId == 100)
        {
            mType = MapType.Home;
            mData = mHomeData;
        }
        else if (mapId == 101)
        {
            mType = MapType.Guide;
            mData = WorldMng.E.MapCtl.CreateMapData(mapId);
        }
        else
        {
            mType = MapType.Brave;
            mData = WorldMng.E.MapCtl.CreateMapData(mapId);
        }

        RuntimeData.MapType = mType;
        SetMapData(mData, mType);
    }
    public void SetMapData(MapData mData, MapType mType)
    {
        switch (mType)
        {
            case MapType.Home: mHomeData = mData; break;
            case MapType.Guide: mGuideData = mData; break;
            case MapType.Brave: mBraveData = mData; break;
        }
    }
    public void GetMapData(int mapId)
    {

    }
    public MapData GetHomeData()
    {
        return mHomeData;
    }

    #endregion
    #region Item

    public void SetItems(List<ItemData> list)
    {
        items = list;
    }

    public ItemData GetItemByGuid(int guid)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].id == guid)
            {
                return Items[i];
            }
        }
        return null;
    }
    public ItemData GetItemByEquipedSite(int site)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].equipSite == site)
            {
                return Items[i];
            }
        }
        return null;
    }
    public int GetItemCountByItemID(int itemId)
    {
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

    public void AddItems(Dictionary<int,int> items, Action action = null)
    {
        if (items.Count <= 0)
            return;

        NWMng.E.AddItems((rp) =>
        {
            NWMng.E.GetItems(() =>
            {
                if (action != null) action();
            });
        }, items);
    }
    public void AddItemInData(int itemID, int count, string newName, string data, Action action = null)
    {
        if (RuntimeData.MapType == MapType.Guide)
        {
            GuideLG.E.AddGuideItem(new ItemData()
            {
                itemId = itemID,
                count = count,
                newName = newName,
                relationData = data
            });
            if (action != null) action();
        }
        else
        {
            NWMng.E.AddItemInData((rp) =>
            {
                NWMng.E.GetItems(() =>
                {
                    if (action != null) action();
                });
            }, itemID, count, newName, data);
        }
    }
    public void RemoveItemByGuid(int guid, int count)
    {
        for (int i = 0; i < E.Items.Count; i++)
        {
            if (E.Items[i].id == guid)
            {
                if (E.Items[i].count >= count)
                {
                    E.Items[i].count -= count;

                    if (E.Items[i].count == 0)
                    {
                        E.Items.Remove(E.Items[i]);
                    }
                }
                else
                {
                    Logger.Error("Remve item fild " + guid);
                }
            }
        }
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
            if (RuntimeData.MapType == MapType.Guide)
            {
                RemoveItemByGuid(guid, count);
                if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();

                GuideLG.E.CreateBlock();
            }
            else
            {
                NWMng.E.RemoveItemByGuid((rp) =>
                {
                    RemoveItemByGuid(guid, count);
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                    if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
                }, guid, count);
            }
        }
    }
    public bool ConsumableItem(int itemID, int count = 1)
    {
        bool itemCheck = CheckConsumableItem(itemID, count);
        if (itemCheck)
        {
            NWMng.E.RemoveItem((rp) =>
            {
                NWMng.E.GetItems(null);
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

    public int GetCoinByID(int id)
    {
        switch (id)
        {
            case 9000: return RuntimeData.Coin1;
            case 9001: return RuntimeData.Coin2;
            default: return RuntimeData.Coin3;
        }
    }
    public void ConsumableCoin(int id, int count)
    {
        switch (id)
        {
            case 9000: RuntimeData.Coin1 -= count; break;
            case 9001: RuntimeData.Coin2 -= count; break;
            default: RuntimeData.Coin3 -= count; break;
        }
    }
    

    private bool TryGetItems(int itemID, out List<ItemData> itemList)
    {
        itemList = new List<ItemData>();

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].itemId == itemID)
            {
                itemList.Add(Items[i]);
            }
        }

        return itemList.Count > 0;
    }

    public static void GetItems(JsonData jsonData)
    {
        try
        {
            if (string.IsNullOrEmpty(jsonData.ToString()))
                E.Items.Clear();
            else
            {
                var items = JsonMapper.ToObject<List<ItemData>>(jsonData.ToJson());
                E.SetItems(items);
            }

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

        E.RuntimeData.Coin1 = (int)jsonData["coin1"];
        E.RuntimeData.Coin2 = (int)jsonData["coin2"];
        E.RuntimeData.Coin3 = (int)jsonData["coin3"];
    }

#endregion
}