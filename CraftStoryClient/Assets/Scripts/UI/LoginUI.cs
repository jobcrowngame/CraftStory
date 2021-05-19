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

    public override void Init()
    {
        base.Init();

        LoginLg.E.Init(this);

        Login = FindChiled<Text>("Login");
        Login.text = LoginText01;

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

        Login.text = LoginText02;

        string msg = "";
        for (int i = 0; i < LoginText02.ToCharArray().Length; i++)
        {
            msg += LoginText02.ToCharArray()[i];
        }
        Debug.Log(msg);

        BGBtn.enabled = true;
    }
}
