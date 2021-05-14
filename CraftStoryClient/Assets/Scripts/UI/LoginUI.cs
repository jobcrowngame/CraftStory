using UnityEngine;
using UnityEngine.UI;

public class LoginUI : UIBase
{
    const string idInputNm = "ID";
    const string pwInputNm = "PW";
    const string newAccBtnNm = "NewAccountBtn";
    const string loginBtnNm = "LoginBtn";

    const string msgNm = "Text";
    const string BGBtnNm = "BG";

    InputField idInput;
    InputField pwInput;
    Button newAccBtn;
    Button loginBtn;

    Text msg;
    Button BGBtn;

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
        idInput = FindChiled<InputField>(idInputNm);
        pwInput = FindChiled<InputField>(pwInputNm);

        newAccBtn = FindChiled<Button>(newAccBtnNm);
        newAccBtn.onClick.AddListener(()=> { LoginLg.E.CreateNewAccount(); });

        loginBtn = FindChiled<Button>(loginBtnNm);
        loginBtn.onClick.AddListener(() => { LoginLg.E.Login(idInput.text, pwInput.text); });

        msg = FindChiled<Text>(msgNm);
        msg.text = "ログイン...";

        BGBtn = FindChiled<Button>(BGBtnNm);
        BGBtn.onClick.AddListener(OnClickBGBtn);
    }

    private void OnClickBGBtn()
    {
        Debug.Log("On Click OnClickBGBtn");
        UICtl.E.OpenUI<HomeUI>(UIType.Home, UIOpenType.BeforeClose);
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

        WorldMng.E.Init();
        msg.text = "画面をクリックして開始します";
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
