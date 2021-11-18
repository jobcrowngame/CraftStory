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

    public Transform CharacterParent;
    public Transform EffectParent;

    public void Init()
    {
        CharacterParent = CreateParentObj("CharacterParent");
        EffectParent = CreateParentObj("EffectParent");

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
        if (DataMng.E.RuntimeData.MapType == MapType.Home ||
            DataMng.E.RuntimeData.MapType == MapType.Market ||
            DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            PlayerCtl.E.BlueprintPreviewCtl = BlueprintPreviewCtl.Instantiate();
        }

        AdventureCtl.E.Init();
        GoogleMobileAdsMng.E.Init();
    }

    private Transform CreateParentObj(string name)
    {
        var obj = new GameObject();
        obj.transform.position = Vector3.zero;
        obj.name = name;

        return obj.transform;
    }
}