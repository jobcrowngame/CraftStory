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
        Debug.LogWarning("OnApplicationQuit");
        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        Debug.LogWarning("OnApplicationPause:" + pauseStatus);

        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }

    private IEnumerator LoadData()
    {
        Debug.Log("������ LoadData");

        yield return DataMng.E.Load();
    }
}
