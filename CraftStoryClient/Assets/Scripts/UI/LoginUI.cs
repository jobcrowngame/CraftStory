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

        InitUI();

        AutoLogin();
    }

    private void InitUI()
    {
        idInput = FindChiled<InputField>("ID");
        pwInput = FindChiled<InputField>("PW");

        newAccBtn = FindChiled<Button>("NewAccountBtn");
        newAccBtn.onClick.AddListener(()=> { LoginLg.E.CreateNewAccount(); });

        loginBtn = FindChiled<Button>("LoginBtn");
        loginBtn.onClick.AddListener(() => { LoginLg.E.Login(idInput.text, pwInput.text); });
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

    public void AutoLogin()
    {
        LoginLg.E.Login(UserTest.E.id, UserTest.E.pw);
    }
    public void AutoLogin2()
    {
        LoginLg.E.Login(UserTest.E.id2, UserTest.E.pw2);
    }
    public void WritIDPW(string id, string pw)
    {
        idInput.text = id;
        pwInput.text = pw;
    }

    #endregion
}
