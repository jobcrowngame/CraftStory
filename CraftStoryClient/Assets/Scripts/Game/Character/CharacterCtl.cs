using UnityEngine;

public class CharacterCtl
{
    private PlayerEntity player;

    public void CreateCharacter()
    {
        player = PlayerCtl.E.AddEntity();
    }

    public void OnQuit()
    {
        if (player == null)
            return;

        Debug.Log("Save player pos " + player.transform.position);

        DataMng.E.PlayerData.Pos = player.transform.position;
        DataMng.E.PlayerData.EulerAngles = player.transform.rotation.eulerAngles;
    }
}