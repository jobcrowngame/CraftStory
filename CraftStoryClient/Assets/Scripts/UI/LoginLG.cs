using UnityEngine;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        NWMng.E.CreateNewAccount((rp) => 
        {
            Debug.Log("�V�����A�J�E���g�쐬�������܂����B");
            Debug.LogFormat("ID:\n{0}, \nPW:\n{1}", rp[0], rp[1]);

            DataMng.E.NewUser(rp[0], rp[1]);
            Login();
        });
    }

    public void Login()
    {
        if (DataMng.E.UserData == null)
        {
            UICtl.E.OpenUI<TermsUI>(UIType.Terms);

            return;
        }

        string id = DataMng.E.UserData.Account;
        string pw = DataMng.E.UserData.UserPW;

        NWMng.E.Login((rp) => 
        {
            DataMng.E.session = rp[0];
            Debug.Log("token: " + DataMng.E.session);
            ui.LoginResponse();
        }, id, pw);
    }
}
