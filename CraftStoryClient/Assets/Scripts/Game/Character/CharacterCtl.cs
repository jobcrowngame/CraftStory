using System.Collections.Generic;
using UnityEngine;

public class CharacterCtl
{
    private PlayerEntity player;
    //private List<CharacterEntity> characterList;
    //private GameObject playerRoot;

    public void CreateCharacter()
    {
        //characterList = new List<CharacterEntity>();
        //playerRoot = new GameObject("PlayerRoot");

        AddPlayer();
    }

    public void OnQuit()
    {
        if (player == null)
            return;

        Debug.Log("Save player pos " + player.transform.position);

        player.CharacterData.Pos = player.transform.position;
        player.CharacterData.EulerAngles = player.transform.rotation.eulerAngles;
    }

    public void AddPlayer()
    {
        var resource = Resources.Load("Prefabs/Game/CharacterPlayer") as GameObject;
        if (resource == null)
            return;

        var pos = DataMng.E.CharacterData.Pos;
        pos.y += 3f;
        var obj = GameObject.Instantiate(resource, pos, Quaternion.Euler(DataMng.E.CharacterData.EulerAngles));
        if (obj == null)
            return;

        player = obj.GetComponent<PlayerEntity>();
        if (player == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return;
        }

        player.CharacterData = DataMng.E.CharacterData;
        player.Init();
    }
}