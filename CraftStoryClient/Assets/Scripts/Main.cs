using System.Collections;
using UnityEngine;
using LitJson;

/// <summary>
/// メインクラス
/// </summary>
public class Main : MonoBehaviour
{
    public static Transform E;

    void Start()
    {
        DontDestroyOnLoad(this);
        E = transform;

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        DataMng.E.Init();
        TaskMng.E.Init();
        UICtl.E.Init();
        TimeZoneMng.E.Init();
        WorldMng.E.Init();
        AudioMng.Init();
        LocalDataMng.E.Init();
        //UserTest.E.Init();

        yield return ConfigMng.E.InitInitCoroutine();
        yield return ResourcesMng.E.InitInitCoroutine();
        yield return LoadData();
        yield return NWMng.E.InitInitCoroutine();
        yield return AWSS3Mng.E.InitInitCoroutine();

        UICtl.E.OpenUI<LoginUI>(UIType.Login);
        AudioMng.E.ShowBGM("bgm_01");

        // Test
        //DataMng.E.UserData.LocalDataLoaded = false;

        if (DataMng.E.UserData == null)
        {
            DataMng.E.NewUser("local", "local");
            LocalDataMng.E.LoadServerData();

            DataMng.E.AddItem(101, 100);
            DataMng.E.AddItem(105, 100);
            DataMng.E.AddItem(10001, 1);
            DataMng.E.AddItem(10002, 1);
            DataMng.E.AddItem(10003, 1);
        }
        else if (DataMng.E.UserData.LocalDataLoaded)
        {
            LocalDataMng.E.LoadServerData();
        }
        else
        {
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

            StartCoroutine(OnLoginFailed());
        }
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


    #region OnLogin fail

    public static bool LoginFailed = true;
    string text = @"ログインに問題が発生しています。
詳細は公式ツイッターをご確認ください。

※OKボタンをタップすると公式ツイッターへ遷移します。";
    private IEnumerator OnLoginFailed()
    {
        yield return new WaitForSeconds(15);

        if (LoginFailed)
        {
            Logger.Error("ログイン失敗しました。{0}", DataMng.E.UserData.Account);

            CommonFunction.ShowHintBox(text, () =>
            {
                Application.OpenURL("https://twitter.com/CraftStory37/");
            });
        }
    }

    #endregion OnLogin fail
}
