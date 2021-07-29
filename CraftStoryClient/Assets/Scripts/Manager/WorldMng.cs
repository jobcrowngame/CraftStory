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

    public CharacterCtl CharacterCtl { get; set; }
    public MapCtl MapCtl { get; set; }
    public GameTimeCtl GameTimeCtl { get; set; }

    public void Init()
    {
        GameTimeCtl = new GameTimeCtl();
        MapCtl = new MapCtl();
        CharacterCtl = new CharacterCtl();
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
        GoogleMobileAdsMng.E.Init();
    }
}