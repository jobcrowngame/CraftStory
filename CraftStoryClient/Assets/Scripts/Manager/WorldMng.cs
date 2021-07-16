using UnityEngine;

public class WorldMng : MonoBehaviour
{
    public static WorldMng E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<WorldMng>();

            return entity;
        }
    }
    private static WorldMng entity;

    public CharacterCtl CharacterCtl
    {
        get
        {
            if (characterCtl == null)
                characterCtl = new CharacterCtl();

            return characterCtl;
        }
    }
    private CharacterCtl characterCtl;

    public MapCtl MapCtl
    {
        get
        {
            if (mapCtl == null)
                mapCtl = new MapCtl();

            return mapCtl;
        }
    }
    private MapCtl mapCtl;

    public void Init()
    {
    }

    public void CreateGameObjects(bool isHome = true)
    {
        if (isHome)
        {
            MapCtl.CreateMap(100);
            PlayerCtl.E.BlueprintPreviewCtl = BlueprintPreviewCtl.Instantiate();
        }
        else
        {
            MapCtl.CreateMap(NowLoadingLG.E.NextMapID);
        }

        CharacterCtl.CreateCharacter();
        AdventureCtl.E.Init();
    }

    public void OnQuit()
    {
        if (CharacterCtl != null) CharacterCtl.OnQuit();
    }
}