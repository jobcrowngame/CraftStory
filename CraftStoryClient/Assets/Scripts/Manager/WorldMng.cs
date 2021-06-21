using UnityEngine;

public class WorldMng : MonoBehaviour
{
    private static WorldMng entity;
    public static WorldMng E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<WorldMng>();

            return entity;
        }
    }

    private MapCtl mapCtl;
    private CharacterCtl characterCtl;

    public MapCtl MapCtl { get { return mapCtl; } }

    public void Init()
    {
        mapCtl = new MapCtl();
        characterCtl = new CharacterCtl();
    }

    public void CreateGameObjects()
    {
        mapCtl.CreateMap(DataMng.E.MapData.NextMapID);
        characterCtl.CreateCharacter();

        AdventureCtl.E.Init();
    }

    public void OnQuit()
    {
        if (mapCtl != null) mapCtl.OnQuit();
        if (characterCtl != null) characterCtl.OnQuit();
    }
}