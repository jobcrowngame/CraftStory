using System;
using UnityEngine;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "MapData.dat";
    const string CharacterDataName = "CharacterData.dat";
    const string UserDataName = "UserData.dat";

    private UserData uData;
    public UserData UserData
    {
        get => uData;
        private set => uData = value;
    }

    private MapData mData;
    public MapData MapData 
    {
        get
        {
            if (mData == null)
                mData = TestDataFoctry.CreateTestMap();

            return mData;
        }
        private set => mData = value;
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

        if (mData != null)
            SaveLoadFile.E.Save(mData, PublicPar.SaveRootPath + MapDataName);

        if (cData != null)
            SaveLoadFile.E.Save(cData, PublicPar.SaveRootPath + CharacterDataName);
    }

    public bool Load()
    {
        var uData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + UserDataName);
        if (uData == null)
            return false;

        this.uData = (UserData)uData;

        var mData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if (mData == null)
            return false;

        this.mData = (MapData)mData;

        var cData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + CharacterDataName);
        if (cData == null)
            return false;

        this.cData = (CharacterData)cData;

        return true;
    }
}