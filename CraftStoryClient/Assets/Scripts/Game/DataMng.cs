using System;

[Serializable]
public class DataMng : Single<DataMng>
{
    const string MapDataName = "MapData.dat";
    const string CharacterDataName = "CharacterData.dat";

    private MapData mData;
    public MapData MapData 
    {
        get
        {
            if (mData == null)
                mData = TestDataFoctry.CreateTestMap();

            return mData;
        }
        set { mData = value; }
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
        set { cData = value; }
    }

    public void Save()
    {
        if (mData != null)
            SaveLoadFile.E.Save(mData, PublicPar.SaveRootPath + MapDataName);

        if (cData != null)
            SaveLoadFile.E.Save(cData, PublicPar.SaveRootPath + CharacterDataName);
    }

    public void Load()
    {
        var mData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + MapDataName);
        if (mData == null)
            return;

        this.mData = (MapData)mData;

        var cData = SaveLoadFile.E.Load(PublicPar.SaveRootPath + CharacterDataName);
        if (cData == null)
            return;

        this.cData = (CharacterData)cData;
    }
}