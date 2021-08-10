﻿using UnityEngine;

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

    public void CreateGameObjects()
    {
        MapCtl.CreateMap();
        CharacterCtl.CreateCharacter();

        // ガイドの場合、一時的のアイテムを追加
        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            DataMng.E.SetGuideItems();

        // ホームマップの場合、設計図プレイビューコンソールObjectを追加
        if (DataMng.E.RuntimeData.MapType == MapType.Home)
            PlayerCtl.E.BlueprintPreviewCtl = BlueprintPreviewCtl.Instantiate();

        AdventureCtl.E.Init();
        GoogleMobileAdsMng.E.Init();
    }
}