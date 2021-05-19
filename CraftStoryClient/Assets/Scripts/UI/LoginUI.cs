using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    Text Login;
    Text Ver;
    Button BGBtn;
    Button Terms01Btn;
    Button Terms02Btn;

    public override void Init()
    {
        base.Init();

        LoginLg.E.Init(this);

        Login = FindChiled<Text>("Login");
        Login.text = "ログイン...";

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

        Login.text = "ゲームスタート";
        BGBtn.enabled = true;
    }
}
