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
                case MapType.Brave: return mBraveData;
                default: return null;
            }
        }
    }
    private MapData mHomeData;
    private MapData mGuideData;
    private MapData mBraveData;
    private MapData mFriendHomeData;
    private MapData mMarketData;
    private MapData mEventData;

    public List<ItemData> Items { 
        get => RuntimeData.MapType == MapType.Guide ? guideItems : LocalDataMng.E.Data.ItemT.list;
    }
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
        if (Main.E == null)
            return;

        Logger.Log("Save Data");
        if (uData != null)
        {
            Task task = SaveLoadFile.E.Save(uData, PublicPar.SaveRootPath + PublicPar.UserDataName);
        }

        if (mHomeData != null)
        {
            Task task = SaveLoadFile.E.Save(mHomeData.ToStringData(), PublicPar.SaveRootPath + PublicPar.MapDataName);
        }

        if (WorldMng.E.MapMng != null)
        {
            WorldMng.E.MapMng.SaveData();
        }

        LocalDataMng.E.Save();
    }

    /// <summary>
    /// ローカルデータのロード
    /// </summary>
    /// <returns></returns>
    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.UserDataName);
        if (uData != null && uData.PlayerPositionX == 0 && uData.PlayerPositionZ == 0)
        {
            uData.PlayerPositionX = 5;
            uData.PlayerPositionZ = 5;
        }

        //uData = new UserData();
        //uData.Account = "OC1RUMAjDeFv";
        //uData.UserPW = "WeSEmdWkzLaB";

        try
        {
            var mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.MapDataName);
            if (!string.IsNullOrEmpty(mapData)) 
                mHomeData = new MapData(mapData);
        }
        catch (Exception ex)
        {
            Logger.Error("Load local map fail!! \n" + ex.Message);
        }

        if (mHomeData == null) mHomeData = WorldMng.E.MapCtl.CreateMapData(100); ;

        LocalDataMng.E.LoadLocalData();

        return true;
    }

    #region User
    /// <summary>
    /// 新しいユーザーを追加
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    public void NewUser()
    {
        uData = new UserData();
        uData.LocalDataLoaded = true;
        uData.Account = "local";
        uData.UserPW = "local";
        uData.PickupNoticeCheckMap = new Dictionary<int, DateTime>();
        uData.Hunger = 100;
        uData.AreaIndexX = 5;
        uData.AreaIndexX = 5;

        uData.PlayerPositionX = 5;
        uData.PlayerPositionZ = 5;

        AddItem(101, 100);
        AddItem(105, 100);
        AddItem(10001, 1);
        AddItem(10002, 1);
        AddItem(10003, 1);
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

    public int GetNewItemGuid()
    {
        int guid = UserData.NewItemGuid++;

        if (GetItemByGuid(guid) != null)
        {
            guid = GetNewItemGuid();
        }

        return guid;
    }
    /// <summary>
    /// アイテム数追加
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="count"></param>
    public void AddItem(int itemId, int count)
    {
        if (itemId <= 0)
            return;

        var item = GetItemByItemId(itemId);
        if (item == null)
        {
            item = new ItemData(itemId, count);
            Items.Add(item);
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

    public void AddBonus(int bonusId)
    {
        var config = ConfigMng.E.Bonus[bonusId];

        AddItem(config.Bonus1, config.BonusCount1);
        AddItem(config.Bonus2, config.BonusCount2);
        AddItem(config.Bonus3, config.BonusCount3);
        AddItem(config.Bonus4, config.BonusCount4);
        AddItem(config.Bonus5, config.BonusCount5);
        AddItem(config.Bonus6, config.BonusCount6);
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
            });
            if (action != null) action();
        }
        else
        {
            HomeLG.E.AddItem(itemID, count);
            //NWMng.E.AddItemInData((rp) =>
            //{
            //    NWMng.E.GetItems(() =>
            //    {
            //        if (action != null) action();
            //    });
            //}, itemID, count, newName, data, textureName);
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
                RemoveItemByGuid(guid, count);
                if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();

                var item = GetItemByGuid(guid);
                if (item == null || item.count <= 0)
                    PlayerCtl.E.SelectItem = null;
            }
        }
    }
    public bool ConsumableItemByItemId(int itemID, int count = 1)
    {
        bool itemCheck = CheckConsumableItemByItemId(itemID, count);
        if (itemCheck)
        {
            RemoveItemByItemId(itemID, count);
            if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
            if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
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
   

#endregion
}