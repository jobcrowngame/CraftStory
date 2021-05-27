﻿using UnityEngine;

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

    public void StartGame()
    {
        //mapCtl.CreateMap();
        mapCtl.CreateMap(1000);
        characterCtl.CreateCharacter();
    }

    public void OnQuit()
    {
        if (mapCtl != null) mapCtl.OnQuit();
        if (characterCtl != null) characterCtl.OnQuit();
    }
}