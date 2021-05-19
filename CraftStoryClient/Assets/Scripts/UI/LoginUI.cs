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

    string LoginText01 = "���O�C��...";
    string LoginText02 = "�Q�[���X�^�[�g";

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
        Debug.Log("���O�C���������܂����B");
        Debug.LogFormat("�悤�����A{0}�l", DataMng.E.UserData.UserID);

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
