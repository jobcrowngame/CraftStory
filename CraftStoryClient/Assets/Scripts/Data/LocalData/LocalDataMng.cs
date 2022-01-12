using LitJson;
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
            Task task = SaveLoadFile.E.Save(localData, PublicPar.SaveRootPath + PublicPar.LocalDataName);
        }
    }

    #region データをサーバーからダウンロード

    public override void Init()
    {
        base.Init();

        if (DataMng.E.UserData.LocalDataLoaded)
        {
            Logger.Warning("ローカル");
            localData = (LocalData)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.LocalDataName);
            LoginLg.E.UI.LoginResponse();
            return;
        }

        localData = new LocalData();

        SetItemList();

    }

    #endregion

    #region Get

    public List<ItemData> GetItemList()
    {
        return localData.ItemT.GetItemList();
    }

    public List<EquipListLG.EquipListRP> GetEquipmentInfoList()
    {
        List<EquipListLG.EquipListRP> list = new List<EquipListLG.EquipListRP>();
        foreach (var item in localData.EquipmentT.list)
        {
            list.Add(new EquipListLG.EquipListRP()
            {
                id = item.id,
                itemId = item.item_guid,
                islocked = 1,
                skills = item.skills
            });
        }
        return list;
    }

    #endregion

    #region Set

    public void SetItemList()
    {
        localData.ItemT = new ItemTable();

        NWMng.E.GetItemList((rp) =>
        {
            if (!string.IsNullOrEmpty(rp.ToString())) 
            {
                var items = JsonMapper.ToObject<List<ItemData>>(rp.ToJson());
                foreach (var item in items)
                {
                    localData.ItemT.items.Add(new ItemTable.Row()
                    {
                        id = item.id,
                        itemId = item.itemId,
                        newName = item.newName,
                        count = item.count,
                        equipSite = item.equipSite,
                        relationData = item.relationData,
                        textureName = item.textureName,
                        islocked = item.IsLocked
                    });
                }

                SetEquipmentList();
            }
        });
    }

    public void SetEquipmentList()
    {
        localData.EquipmentT = new EquipmentTable();

        NWMng.E.GetEquipmentInfoList((rp) =>
        {
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                var result = JsonMapper.ToObject<List<EquipListLG.EquipListRP>>(rp.ToJson());
                foreach (var item in result)
                {
                    localData.EquipmentT.list.Add(new EquipmentTable.Row()
                    {
                        id = item.id,
                        item_guid = item.itemId,
                        skills = item.skills
                    });
                }
            }

            SetLimited();
            SetStatistics_user();
            SetUserData();

            DataMng.E.UserData.LocalDataLoaded = true;
        });
    }

    public void SetLimited()
    {
        localData.limitedT = new limitedTable();

        localData.limitedT.guide_end = DataMng.E.RuntimeData.GuideEnd;
        localData.limitedT.guide_end2 = DataMng.E.RuntimeData.GuideEnd2;
        localData.limitedT.guide_end3 = DataMng.E.RuntimeData.GuideEnd3;
        localData.limitedT.guide_end4 = DataMng.E.RuntimeData.GuideEnd4;
        localData.limitedT.guide_end5 = DataMng.E.RuntimeData.GuideEnd5;
        localData.limitedT.goodNum_daily = DataMng.E.RuntimeData.UseGoodNum;
        localData.limitedT.goodNum_total = DataMng.E.RuntimeData.MyGoodNum;
        localData.limitedT.main_task = TaskMng.E.MainTaskId;
        localData.limitedT.main_task_count = TaskMng.E.MainTaskClearedCount;
        localData.limitedT.logined = DataMng.E.RuntimeData.FirstLoginDaily;
    }

    public void SetStatistics_user()
    {
        localData.Statistics_userT = new Statistics_userTable();
    }

    public void SetUserData()
    {
        localData.UserDataT = new UserDataTable();

        localData.UserDataT.myShopLv = DataMng.E.MyShop.myShopLv;
        localData.UserDataT.nickname = DataMng.E.RuntimeData.NickName;
        localData.UserDataT.comment = DataMng.E.RuntimeData.Comment;
        localData.UserDataT.email = DataMng.E.RuntimeData.Email;
        localData.UserDataT.exp = (int)DataMng.E.RuntimeData.Exp;
    }

    #endregion
}
