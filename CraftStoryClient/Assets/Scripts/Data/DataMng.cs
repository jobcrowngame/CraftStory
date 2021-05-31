using System;
using UnityEngine;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "MapData.dat";
    const string CharacterDataName = "CharacterData.dat";
    const string UserDataName = "UserData.dat";

    public string NextSceneName { get; set; }
    public int NextSceneID { get; set; }
    public int CurrentSceneID { get; set; }

    private UserData uData;
    public UserData UserData
    {
        get => uData;
        private set => uData = value;
    }

    private MapData mData;
    public MapData MapData 
    {
        get => mData;
        set => mData = value;
    }

    private MapData homeData;
    public MapData HomeData
    {
        get => homeData;
        set => homeData = value;
    }

    private CharacterData cData;
    public CharacterData CharacterData
    {
        get 
        {
            if (cData == null)
                cData = TestDataFoctry.CreateTestCharacter();

            return cData; 
        }
        private set => cData = value;
    }

    public void NewUser(string id, string pw)
    {
        uData = new UserData()
        {
            UserID = id,
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

        if (cData != null)
            SaveLoadFile.E.Save(cData, PublicPar.SaveRootPath + CharacterDataName);
    }

    public bool Load()
    {
        var uData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + UserDataName);
        if (uData == null)
            return false;

        this.uData = (UserData)uData;

        var homeData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if (homeData == null)
            return false;

        this.homeData = (MapData)homeData;
        this.homeData.ParseStringData();

        var cData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + CharacterDataName);
        if (cData == null)
            return false;

        this.cData = (CharacterData)cData;

        return true;
    }
}