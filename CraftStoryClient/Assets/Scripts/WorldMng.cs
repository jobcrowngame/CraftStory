using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WorldMng : MonoBehaviour
{
    public static WorldMng E;

    private GameObject playerRoot;

    private MapCtl mapCtl;

    public MapCtl MapCtl { get { return mapCtl; } }

    public void Init()
    {
        E = this;

        playerRoot = new GameObject("PlayerRoot");

        mapCtl = new MapCtl();

        AddPlayer();
    }

    private void AddPlayer()
    {
        var cInfo = new CharacterInfo();
        cInfo.Pos = new Vector3(3, 3, 3);


        var characterP = CommonFunction.CreateObj(playerRoot.transform, "Prefabs/Game/CharacterPlayer");
        if (characterP == null)
            return;

        var characterEntityP = characterP.GetComponent<CharacterEntityPlayer>();
        if (characterEntityP == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return;
        }

        characterEntityP.Init(cInfo);
    }
    private CharacterEntity AddCharacter(CharacterInfo characterInfo)
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

        characterEntity.Init(characterInfo);
        return characterEntity;
    }
}