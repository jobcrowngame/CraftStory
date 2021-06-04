using JsonConfigData;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "MapData.dat";
    //const string CharacterDataName = "CharacterData.dat";
    const string UserDataName = "UserData.dat";
    const string ItemsDataName = "ItemsData.dat";

    public string NextSceneName { get; set; }
    public int NextSceneID { get; set; }
    public int CurrentSceneID { get; set; }
    public Map CurrentMapConfig { get => ConfigMng.E.Map[CurrentSceneID]; }

    public UserData UserData
    {
        get => uData;
        private set => uData = value;
    }
    private UserData uData;

    public MapData MapData 
    {
        get => mData;
        set => mData = value;
    }
    private MapData mData;

    public MapData HomeData
    {
        get => homeData;
        set => homeData = value;
    }
    private MapData homeData;

    public PlayerData PlayerData
    {
        get => playerData;
        set => playerData = value;
    }
    private PlayerData playerData;

    public List<ItemData> Items { get
        {
            if (items == null)
                items = new List<ItemData>();

            return items;
        }
    }
    private List<ItemData> items;

    public void NewUser(string id, string pw)
    {
        uData = new UserData()
        {
            UserID = id,
            UserPW = pw
        };

        AddItem(3000, null);
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

        //if (playerData != null)
        //    SaveLoadFile.E.Save(playerData, PublicPar.SaveRootPath + CharacterDataName);

        if (items != null)
            SaveLoadFile.E.Save(items, PublicPar.SaveRootPath + ItemsDataName);
    }

    public bool Load()
    {
        uData = (UserData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + UserDataName);

        homeData = (MapData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if(homeData != null) homeData.ParseStringData();

        //playerData = (PlayerData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + CharacterDataName);

        items = (List<ItemData>)SaveLoadFile.E.Load(PublicPar.SaveRootPath + ItemsDataName);

        return true;
    }

    #region Item

    public void AddItem(int itemID, object data, int count = 1)
    {
        var config = ConfigMng.E.Item[itemID];
        int addItemObjCount = count / config.MaxCount;

        for (int i = 0; i < addItemObjCount; i++)
        {
            var item = NewItem(itemID, data);
            item.Count = item.Config.MaxCount;
            Items.Add(item);
            count -= item.Config.MaxCount;
        }

        List<ItemData> itemList;
        if (!TryGetItems(itemID, out itemList))
        {
            var item = NewItem(itemID, data);
            item.Count = count;
            count = 0;
            Items.Add(item);
        }

        foreach (var item in itemList)
        {
            if (item.Count == item.Config.MaxCount)
                continue;

            if (item.CanAddCount >= count)
            {
                item.Count += count;
                count = 0;
            }
            else
            {
                item.Count += item.CanAddCount;
                count -= item.CanAddCount;
            }
        }

        if (count > 0)
        {
            var item = NewItem(itemID, data);
            item.Count = count;
            Items.Add(item);
        }
    }
    private ItemData NewItem(int itemID, object data)
    {
        return new ItemData(itemID, data);
    }
    private bool TryGetItems(int itemID, out List<ItemData> itemList)
    {
        itemList = new List<ItemData>();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].ItemID == itemID)
            {
                itemList.Add(items[i]);
            }
        }

        return itemList.Count > 0;
    }

    #endregion
}