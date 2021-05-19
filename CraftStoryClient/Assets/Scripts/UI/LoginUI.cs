using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Text;

public class LoginUI : UIBase
{
    Text Login;
    Text Ver;
    Button BGBtn;
    Button Terms01Btn;
    Button Terms02Btn;

    string LoginText01 = "ログイン...";
    string LoginText02 = "ゲームスタート";

    string[] texts;

    private void Awake()
    {
        var textAsset = Resources.Load("TestText/Test") as TextAsset;
        texts = textAsset.text.Split('^');
        Debug.Log(textAsset);

        Init();
    }

    public override void Init()
    {
        base.Init();

        LoginLg.E.Init(this);

        Login = FindChiled<Text>("Login");
        Login.text = texts[0];

        Ver = FindChiled<Text>("Ver");
        Ver.text = "Ver:1.0.0";

        BGBtn = FindChiled<Button>("BG");
        BGBtn.onClick.AddListener(()=> { SceneManager.LoadSceneAsync("NowLoading"); });
        BGBtn.enabled = false;

        Terms01Btn = FindChiled<Button>("Terms01Btn");
        Terms01Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms01UI>(UIType.Terms01); });

        Terms02Btn = FindChiled<Button>("Terms02Btn");
        Terms02Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms02UI>(UIType.Terms02); });
    }

    public void LoginResponse()
    {
        Debug.Log("ログイン成功しました。");
        Debug.LogFormat("ようこそ、{0}様", DataMng.E.UserData.UserID);

        Login.text = texts[1];

        BGBtn.enabled = true;
    }
}
