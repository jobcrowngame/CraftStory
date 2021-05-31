using System.Collections.Generic;
using UnityEngine;

public class CharacterCtl
{
    private PlayerEntity player;

    public void CreateCharacter()
    {
        AddPlayer();
    }

    public void OnQuit()
    {
        if (player == null)
            return;

        Debug.Log("Save player pos " + player.transform.position);

        DataMng.E.PlayerData.Pos = player.transform.position;
        DataMng.E.PlayerData.EulerAngles = player.transform.rotation.eulerAngles;
    }

    public void AddPlayer()
    {
        var resource = Resources.Load("Prefabs/Game/CharacterPlayer") as GameObject;
        if (resource == null)
            return;

        var pos = GetPlayerPos();
        pos = MapCtl.FixEntityPos(DataMng.E.MapData, pos, DataMng.E.CurrentMapConfig.CreatePosOffset);

        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
            return;

        player = obj.GetComponent<PlayerEntity>();
        if (player == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return;
        }

        player.Init();
    }
    private Vector3 GetPlayerPos()
    {
        var pos = WorldMng.E.MapCtl.GetRandomGroundPos();

        var posX = DataMng.E.CurrentMapConfig.PlayerPosX;
        var posZ = DataMng.E.CurrentMapConfig.PlayerPosZ;

        if (posX > 0) pos.x = posX;
        if (posZ > 0) pos.z = posZ;

        pos.y += 5;

        return pos;
    }
}