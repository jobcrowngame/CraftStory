using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

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
                entity = CommonFunction.CreateGlobalObject<WorldMng>();

            return entity;
        }
    }
    private static WorldMng entity;

    public MapCtl MapCtl { get; set; }
    public GameTimeCtl GameTimeCtl { get; set; }
    public CharacterCtl CharacterCtl { get; set; }

    //public Transform CharacterParent;
    public Transform EffectParent;

    private int timer = 0;

    public void Init()
    {
        //CharacterParent = CreateParentObj("CharacterParent");
        EffectParent = CreateParentObj("EffectParent");

        GameTimeCtl = new GameTimeCtl();
        MapCtl = new MapCtl();
        CharacterCtl = new CharacterCtl();

        // 定期時間でローカルデータをセーブ
        timer = SettingMng.AutoSaveDataTime;
        TimeZoneMng.E.AddTimerEvent03(AutomaticSave);
    }

    private void FixedUpdate()
    {
        if (CharacterCtl != null) CharacterCtl.FixedUpdate();
        if (GameTimeCtl != null) GameTimeCtl.FixedUpdate();
    }

    public void Clear()
    {
        // メッシュをクリア
        MapCtl.ClearMesh();
        MapMng.E.ClearCrops();

        // キャラクタクリア
        CharacterCtl.ClearCharacter();
    }

    /// <summary>
    /// 世界の　GameObject　をインスタンス
    /// </summary>
    public void CreateWorld()
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            MapMng.E.Init();
        }
        else
        {
            MapCtl.CreateMap();
        }

        CharacterCtl.CreateCharacter();

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

    /// <summary>
    /// 定期時間でローカルデータをセーブ
    /// </summary>
    public void AutomaticSave()
    {
        timer--;
        if (timer <= 0)
        {
            if(DataMng.E != null) DataMng.E.Save();
            timer = SettingMng.AutoSaveDataTime;
        }
    }
}