using JsonConfigData;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "/MapData.dat";
    const string CharacterDataName = "/CharacterData.dat";
    const string UserDataName = "/UserData.dat";
    const string ItemsDataName = "/ItemsData.dat";

    public string session { get; set; }

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

    public RuntimeData RuntimeData
    {
        get => runtimeData;
        private set => runtimeData = value;
    }
    private RuntimeData runtimeData;

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

        runtimeData = new RuntimeData();
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
        Debug.Log("Save Data");

        if (uData != null)
            SaveLoadFile.E.Save(uData, PublicPar.SaveRootPath + UserDataName);

        if (homeData != null)
        {
            homeData.MapDataToStringData();
            homeData.EntityDataToStringData();
            SaveLoadFile.E.Save(homeData, PublicPar.SaveRootPath + MapDataName);
        }
    }

    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + UserDataName);

        homeData = (MapData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if(homeData != null) homeData.ParseStringData();

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
        if (items == null)
            return 0;

        int count = 0;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemId == itemId)
            {
                count += items[i].count;
            }
        }

        return count;
    }

<<<<<<< HEAD
    public void AddItem(int itemID, object data, int count = 1, Action action = null)
=======
    public void AddItem(int itemID, int count = 1, Action action = null)
>>>>>>> parent of fa89fe2 (update)
    {
        NWMng.E.AddItem((rp)=> 
        {
            ConfigMng.JsonToItemList(rp[0]);

            var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
            if (homeUI != null) homeUI.RefreshItemBtns();

            if (action != null)
                action();
        }, itemID, count);
    }
    public void AddItems(Dictionary<int,int> items, Action action = null)
    {
        if (items.Count <= 0)
            return;

        NWMng.E.AddItems((rp) =>
        {
            ConfigMng.JsonToItemList(rp[0]);

            var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
            if (homeUI != null) homeUI.RefreshItemBtns();

            if (action != null)
                action();
        }, items);
    }
<<<<<<< HEAD
=======
    public void AddItemInData(int itemID, int count, string data, Action action = null)
    {

        NWMng.E.AddItemInData((rp) =>
        {
            ConfigMng.JsonToItemList(rp[0]);

            var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
            if (homeUI != null) homeUI.RefreshItemBtns();

            if (action != null)
                action();
        }, itemID, count, data);
    }
>>>>>>> parent of fa89fe2 (update)
    /// <summary>
    /// 消耗アイテム
    /// </summary>
    public void ConsumableSelectItem(int guid, int count = 1)
    {
        if (GetItemByGuid(guid).count < count)
        {
            Debug.LogWarning("No item stip");
        }
        else
        {
            NWMng.E.RemoveItemByGuid((rp) =>
            {
                NWMng.E.GetItemList((rp2) =>
                {
                    ConfigMng.JsonToItemList(rp2[0]);
                    if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshItemByGuid(guid);
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
                    ConfigMng.JsonToItemList(rp2[0]);
                    if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
                    if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                });
            }, itemID, count);
        }
        else
        {
            Debug.LogWarning("No item stip");
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

    #endregion
}