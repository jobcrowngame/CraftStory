using System.Collections;
using UnityEngine;
using LitJson;

/// <summary>
/// メインクラス
/// </summary>
public class Main : MonoBehaviour
{
    public static Main E;

    public bool Initing = false;

    private void Awake()
    {
        E = this;
    }

    void Start()
    {
        DontDestroyOnLoad(this);

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        Initing = true;

        DataMng.E.Init();
        TaskMng.E.Init();
        UICtl.E.Init();
        TimeZoneMng.E.Init();
        WorldMng.E.Init();
        AudioMng.Init();
        LocalDataMng.E.Init();
        MobileAdsMng.E.Init();

        yield return ConfigMng.E.InitInitCoroutine();
        yield return ResourcesMng.E.InitInitCoroutine();
        yield return LoadData();
        yield return NWMng.E.InitInitCoroutine();

        UICtl.E.OpenUI<LoginUI>(UIType.Login);
        AudioMng.E.ShowBGM("bgm_01");

        // Test
        //DataMng.E.UserData.LocalDataLoaded = false;

        if (DataMng.E.UserData != null && !DataMng.E.UserData.LocalDataLoaded)
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
        else
        {
            if (DataMng.E.UserData == null)
            {
                DataMng.E.NewUser();
                UICtl.E.OpenUI<TermsUI>(UIType.Terms);

                if (DataMng.E.MapData == null)
                    DataMng.E.NewHomeData();
            }
            else
            {
                LoginLg.E.UI.LoginResponse();
                Initing = false;
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (DataMng.E != null && !Initing) 
            DataMng.E.Save();
    }

    private void OnApplicationPause(bool pauseStatus)
    {

        if (DataMng.E != null && !Initing) 
            DataMng.E.Save();
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
