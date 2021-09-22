using System.Collections;
using UnityEngine;

/// <summary>
/// メインクラス
/// </summary>
public class Main : MonoBehaviour
{
    void Start()
    {
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
        yield return NWMng.E.InitInitCoroutine();
        yield return AWSS3Mng.E.InitInitCoroutine();

        UICtl.E.OpenUI<LoginUI>(UIType.Login);
        AudioMng.E.ShowBGM("bgm_01");

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
                    CommonFunction.VersionUp((string)rp["version"]);
                }
            });
        });
    }

    private void OnApplicationQuit()
    {
        if (DataMng.E != null) DataMng.E.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {

        if (DataMng.E != null) DataMng.E.Save();
    }

    private IEnumerator LoadData()
    {
        Logger.Log("初期化 LoadData");

        yield return DataMng.E.Load();
    }
}
