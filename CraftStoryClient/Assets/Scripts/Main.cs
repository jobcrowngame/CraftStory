using System;
using System.Text;
using System.Collections;
using UnityEngine;
using System.Collections.Specialized;

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


        NWMng.E.Connect((rp) =>
        {
            NWMng.E.URL = (string)rp["url"];
            Logger.Warning("[URL]-" + NWMng.E.URL);

            NWMng.E.GetVersion((rp) =>
            {
                if (Application.version == (string)rp["version"])
                {
                    LoginLg.E.Login((int)rp["IsMaintenance"]);
                }
                else
                {
                    string msg = string.Format(@"アプリバージョンが古いです。
最新のバージョンに更新してください。

今のバージョン: v.{0}
最新のバージョン: v.{1}",
    Application.version,
    (string)rp["version"]);

                    CommonFunction.ShowHintBox(msg, () => { CommonFunction.QuitGame(); });
                }
            });
        });
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
        Logger.Log("初期化 LoadData");

        yield return DataMng.E.Load();
    }
}
