﻿using System.Collections.Generic;
using UnityEngine;

public class CharacterCtl
{
    private PlayerEntity player;
    private List<CharacterEntity> characterList;
    private GameObject playerRoot;

    public void CreateCharacter()
    {
        characterList = new List<CharacterEntity>();
        playerRoot = new GameObject("PlayerRoot");

        AddPlayer();
    }

    public void OnQuit()
    {
        if (player == null)
            return;

        player.CharacterData.Pos = player.transform.position;
    }

    public void AddPlayer()
    {
        var characterP = CommonFunction.CreateObj(playerRoot.transform, "Prefabs/Game/CharacterPlayer");
        if (characterP == null)
            return;

        player = characterP.GetComponent<PlayerEntity>();
        if (player == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return;
        }

        player.Init(DataMng.E.CharacterData);
    }
    public CharacterEntity AddCharacter(CharacterData cData)
    {
        var character = CommonFunction.CreateObj(playerRoot.transform, "Prefabs/Game/Character");
        if (character == null)
            return null;

        var characterEntity = character.GetComponent<CharacterEntity>();
        if (characterEntity == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return null;
        }

        characterEntity.Init(cData);
        return characterEntity;
    }
}