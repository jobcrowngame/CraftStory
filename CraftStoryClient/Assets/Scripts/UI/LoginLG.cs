

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        GS2.E.CreateAccount((userId, pw) => 
        {
            ui.CreateNewAccountResponse(userId, pw);
        });
    }

    public void Login(string userId, string pw)
    {
        GS2.E.Login((session) => 
        {
            ui.LoginResponse();
        }, userId, pw);
    }
}
