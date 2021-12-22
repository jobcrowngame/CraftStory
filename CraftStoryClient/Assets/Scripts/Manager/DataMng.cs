using JsonConfigData;
using LitJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// データマネージャー
/// </summary>
[Serializable]
public class DataMng : Single<DataMng>
{
    /// <summary>
    /// サーバーとの通信用 token
    /// </summary>
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
                case MapType.FriendHome: return mFriendHomeData;
                case MapType.Market: return mMarketData;
                case MapType.Event: return mEventData;
                default: return mBraveData;
            }
        }
    }
    private MapData mHomeData;
    private MapData mGuideData;
    private MapData mBraveData;
    private MapData mFriendHomeData;
    private MapData mMarketData;
    private MapData mEventData;

    /// <summary>
    /// マイショップデータ
    /// </summary>
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

    /// <summary>
    /// ローカルデータのセーブ
    /// </summary>
    public void Save()
    {
        Logger.Log("Save Data");
        if (uData != null)
        {
            Task task = SaveLoadFile.E.Save(uData, PublicPar.SaveRootPath + PublicPar.UserDataName);
        }

        if (mHomeData != null)
        {
            Task task = SaveLoadFile.E.Save(mHomeData.ToStringData(), PublicPar.SaveRootPath + PublicPar.MapDataName);
        }
    }

    /// <summary>
    /// ローカルデータのロード
    /// </summary>
    /// <returns></returns>
    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.UserDataName);

        //uData = new UserData();
        //uData.Account = "OC1RUMAjDeFv";
        //uData.UserPW = "WeSEmdWkzLaB";

        try
        {
            var mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.MapDataName);
            if (!string.IsNullOrEmpty(mapData)) mHomeData = new MapData(mapData);
        }
        catch (Exception ex)
        {
            Logger.Error("Load local map fail!! \n" + ex.Message);
        }

        //if (mHomeData == null) mHomeData = WorldMng.E.MapCtl.CreateMapData(100); ;

        return true;
    }

    #region User
    /// <summary>
    /// 新しいユーザーを追加
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    public void NewUser(string id, string pw)
    {
        uData = new UserData();
        uData.Account = id;
        uData.UserPW = pw;
        uData.PickupNoticeCheckMap = new Dictionary<int, DateTime>();
        uData.Hunger = 100;
    }
    #endregion
    #region Map

    /// <summary>
    /// マップデータを設置
    /// </summary>
    /// <param name="mapId">マップID</param>
    public void SetMapData(int mapId)
    {
        Map map = ConfigMng.E.Map[mapId];

        MapData mData = null;
        MapType mType = (MapType)map.MapType;

        if (mType == MapType.Home)
        {
            mData = mHomeData;
        }
        else if (mType == MapType.FriendHome)
        {
            mData = mFriendHomeData;
        }
        else
        {
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
            case MapType.FriendHome: mFriendHomeData = mData; break;
            case MapType.Market: mMarketData = mData; break;
            case MapType.Event: mEventData = mData; break;
            case MapType.Test: mBraveData = mData; break;
        }
    }

    /// <summary>
    /// ホームデータをゲット
    /// </summary>
    /// <returns></returns>
    public MapData GetHomeData()
    {
        return mHomeData;
    }

    #endregion
    #region Item

    /// <summary>
    /// アイテムリストをセット
    /// </summary>
    /// <param name="list"></param>
    public void SetItems(List<ItemData> list)
    {
        items = list;
    }

    /// <summary>
    /// アイテムGUIDによってアイテムをゲット
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
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

    /// <summary>
    /// アイテムIDによってアイテムをゲット
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
    public ItemData GetItemByItemId(int itemId)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].itemId == itemId)
            {
                return Items[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 装備した箇所によってアイテムをゲット
    /// </summary>
    /// <param name="site"></param>
    /// <returns></returns>
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

    /// <summary>
    /// アイテム数追加
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="count"></param>
    public void AddItem(int itemId, int count)
    {
        var item = GetItemByItemId(itemId);
        if (item == null)
        {
            item = new ItemData(itemId, count);
        }
        else
        {
            item.count += count;
        }

        if (item.count > 9999)
        {
            NWMng.E.GetItems(null);
        }

        if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
    }

    /// <summary>
    /// アイテムIDによって数をゲット
    /// </summary>
    /// <param name="itemId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 関連データがあるアイテムを追加（設計図）
    /// </summary>
    /// <param name="itemID">アイテムID</param>
    /// <param name="count">数</param>
    /// <param name="newName">新しいアイテム名</param>
    /// <param name="data">関連データ</param>
    /// <param name="action">成功した後のイベント</param>
    public void AddBlueprint(int itemID, int count, string newName, string data, string textureName, Action action = null)
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
            }, itemID, count, newName, data, textureName);
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
    // ローカルのアイテムを消費(チュートリアルでのみ使用すること)
    public void RemoveItemByItemId(int itemId, int count)
    {
        for (int i = 0; i < E.Items.Count; i++)
        {
            if (E.Items[i].itemId == itemId)
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
                    Logger.Error("Remve item fild(itemId) " + itemId);
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

                var item = GetItemByGuid(guid);
                if (item == null || item.count <= 0)
                    PlayerCtl.E.SelectItem = null;

                GuideLG.E.NextOnCreateBlock();
            }
            else
            {
                NWMng.E.UseItem((rp) =>
                {
                    RemoveItemByGuid(guid, count);
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                    if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();

                    var item = GetItemByGuid(guid);
                    if (item == null || item.count <= 0)
                        PlayerCtl.E.SelectItem = null;
                }, guid, count);
            }
        }
    }
    public bool ConsumableItemByItemId(int itemID, int count = 1)
    {
        bool itemCheck = CheckConsumableItemByItemId(itemID, count);
        if (itemCheck)
        {
            if (DataMng.E.RuntimeData.MapType != MapType.Guide)
            {
                NWMng.E.RemoveItem((rp) =>
                {
                    NWMng.E.GetItems(null);
                }, itemID, count);
            }
            else
            {
                RemoveItemByItemId(itemID, count);
                if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
            }
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
    public bool CheckConsumableItemByItemId(int itemID, int count = 1)
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