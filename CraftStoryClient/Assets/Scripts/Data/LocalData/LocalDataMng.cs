﻿using LitJson;
using System.Collections.Generic;
using System.Threading.Tasks;

public class LocalDataMng : Single<LocalDataMng>
{
    LocalData localData;
    public LocalData Data { get => localData; }

    public void Save()
    {
        if (localData != null)
        {
            Task task = FileIO.E.Save(localData, PublicPar.SaveRootPath + PublicPar.LocalDataName);
        }
    }

    #region データをサーバーからダウンロード

    public override void Init()
    {
        base.Init();

        localData = new LocalData();
    }

    public void LoadServerData()
    {
        SetItemList();
    }
    public void LoadLocalData()
    {
        var mLocalData = (LocalData)FileIO.E.Load(PublicPar.SaveRootPath + PublicPar.LocalDataName);
        if (mLocalData != null)
            localData = mLocalData;
    }

    #endregion

    #region Set

    public void SetItemList()
    {
        NWMng.E.GetItemList((rp) =>
        {
            if (!string.IsNullOrEmpty(rp.ToString())) 
            {
                localData.ItemT.list = JsonMapper.ToObject<List<ItemData>>(rp.ToJson());

                SetEquipmentList();
            }
        });
    }

    public void SetEquipmentList()
    {
        NWMng.E.GetEquipmentInfoList((rp) =>
        {
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                var result = JsonMapper.ToObject<List<EquipListLG.EquipListRP>>(rp.ToJson());
                foreach (var equipment in result)
                {
                    var item = DataMng.E.GetItemByGuid(equipment.id);
                    item.islocked = equipment.islocked;
                    item.skills = equipment.skills;
                }
            }

            DataMng.E.UserData.LocalDataLoaded = true;
            LoginLg.E.UI.LoginResponse();
        });
    }

    #endregion
}
