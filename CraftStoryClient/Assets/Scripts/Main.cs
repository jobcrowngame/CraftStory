using System;
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
        var uiRoot = GameObject.Find("Canvas");

        yield return UICtl.E.InitCoroutine(gameObject, uiRoot);
        var loginUI = UICtl.E.OpenUI<LoginUI>(UIType.Login) as LoginUI;

        yield return LoadData();

        yield return GS2.E.InitCoroutine();

        LoginLg.E.Login();
    }

    private void OnApplicationQuit()
    {
        if (WorldMng.E != null) WorldMng.E.OnQuit();
        if (DataMng.E != null) DataMng.E.Save();
    }

    private IEnumerator LoadData()
    {
        Debug.Log("èâä˙âª LoadData");

        yield return DataMng.E.Load();
    }
}
