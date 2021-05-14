using UnityEngine;

public class WorldMng : MonoBehaviour
{
    public static WorldMng E;


    private MapCtl mapCtl;
    private CharacterCtl characterCtl;

    public MapCtl MapCtl { get { return mapCtl; } }

    public void Init()
    {
        E = this;

        mapCtl = new MapCtl();
        characterCtl = new CharacterCtl();

        characterCtl.AddPlayer();
    }

    public void OnQuit()
    {
        mapCtl.OnQuit();
        characterCtl.OnQuit();
    }
}