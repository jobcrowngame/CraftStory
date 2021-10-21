using UnityEngine;

/// <summary>
/// 世界マネージャー
/// </summary>
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

    public MapCtl MapCtl { get; set; }
    public GameTimeCtl GameTimeCtl { get; set; }

    public void Init()
    {
        GameTimeCtl = new GameTimeCtl();
        MapCtl = new MapCtl();
    }

    /// <summary>
    /// 世界の　GameObject　をインスタンス
    /// </summary>
    public void CreateGameObjects()
    {
        MapCtl.CreateMap();
        CharacterCtl.E.CreateCharacter();

        // ガイドの場合、一時的のアイテムを追加
        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            GuideLG.E.SetGuideItems();

        // 設計図プレイビューコンソールObjectを追加
        PlayerCtl.E.BlueprintPreviewCtl = BlueprintPreviewCtl.Instantiate();

        AdventureCtl.E.Init();
        GoogleMobileAdsMng.E.Init();
    }
}