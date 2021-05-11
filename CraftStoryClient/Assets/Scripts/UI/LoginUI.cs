using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    private InputField idInput;
    private InputField pwInput;
    private Button newAccBtn;
    private Button loginBtn;

    // Start is called before the first frame update
    public override void Init(GameObject obj)
    {
        base.Init(obj);

        LoginLg.E.Init(this);

        if (!InitUI())
        {
            MLog.Error("LoginUI Init fail!");
        }
    }

    private bool InitUI()
    {
        idInput = FindChiled<InputField>("ID");
        pwInput = FindChiled<InputField>("PW");

        newAccBtn = FindChiled<Button>("NewAccountBtn");
        newAccBtn.onClick.AddListener(()=> { LoginLg.E.CreateNewAccount(); });

        loginBtn = FindChiled<Button>("LoginBtn");
        loginBtn.onClick.AddListener(() => { LoginLg.E.Login(idInput.text, pwInput.text); });

        return true;
    }

    public void CreateNewAccountResponse(string id, string pw)
    {
        Debug.Log("新しいアカウント作成成功しました。");
        Debug.LogFormat("ID:\n{0}, \nPW:\n{1}", id, pw);

        idInput.text = id;
        pwInput.text = pw;
    }
    public void LoginResponse()
    {
        Debug.Log("ログイン成功しました。");

        UICtl.E.OpenUI<HomeUI>(UIType.Home, UIOpenType.BeforeClose);
    }

    #region Test

    public void WritIDPW(string id, string pw)
    {
        idInput.text = id;
        pwInput.text = pw;
    }

    #endregion
}
