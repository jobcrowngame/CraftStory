using System.Collections;
using UnityEngine;
using LitJson;

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
        TaskMng.E.Init();

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
                var result = JsonMapper.ToObject<GetVersionRP>(rp.ToJson());
                if (Application.version == result.version)
                {
                    LoginLg.E.Login(result.IsMaintenance);
                }
                else
                {
                    CommonFunction.VersionUp(result.version);
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


    private struct GetVersionRP
    {
        public string version { get; set; }
        public int IsMaintenance { get; set; }
    }
}
