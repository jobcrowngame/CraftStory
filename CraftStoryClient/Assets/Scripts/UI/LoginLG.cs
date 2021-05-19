using UnityEngine;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        GS2.E.CreateAccount((userId, pw) => 
        {
            CreateNewAccountResponse(userId, pw);
        });
    }

    private void CreateNewAccountResponse(string id, string pw)
    {
        Debug.Log("新しいアカウント作成成功しました。");
        Debug.LogFormat("ID:\n{0}, \nPW:\n{1}", id, pw);

        DataMng.E.NewUser(id, pw);
        Login();
    }

    public void Login()
    {
        if (DataMng.E.UserData == null)
        {
            UICtl.E.OpenUI<TermsUI>(UIType.Terms);

            return;
        }

        string id = DataMng.E.UserData.UserID;
        string pw = DataMng.E.UserData.UserPW;

        GS2.E.Login((session) => 
        {
            ui.LoginResponse();
        }, id, pw);
    }
}
