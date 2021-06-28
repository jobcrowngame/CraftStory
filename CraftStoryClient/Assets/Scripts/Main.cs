using System;
using System.Text;
using System.Collections;
using UnityEngine;

public class Main : MonoBehaviour
{
    public static Main E;

    void Start()
    {
        E = this;

        DontDestroyOnLoad(this);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        DataMng.E.Init();

        yield return ConfigMng.E.InitInitCoroutine();
        yield return ResourcesMng.E.InitInitCoroutine();
        yield return UICtl.E.InitCoroutine(gameObject);
        yield return LoadData();
        yield return NWMng.E.InitCoroutine();

        UICtl.E.OpenUI<LoginUI>(UIType.Login);
        LoginLg.E.Login();
    }

    private void OnApplicationQuit()
    {
        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {

        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }

    private IEnumerator LoadData()
    {
        Logger.Log("èâä˙âª LoadData");

        yield return DataMng.E.Load();
    }
}
