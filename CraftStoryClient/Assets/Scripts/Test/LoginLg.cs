using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Util;
using UnityEngine;

public class LoginLg : Single<LoginLg>
{
    private LoginUI ui;

    public void Init(LoginUI ui)
    {
        this.ui = ui;

        Init();
    }

    private void Init()
    {

    }

    public void CreateNewAccount()
    {
        GS2.E.CreateAccount((userId, pw) => 
        {
            ui.CreateNewAccountBtnResponse(userId, pw);
        });
    }

    public void Login(string userId, string pw)
    {
        GS2.E.Login((session) => 
        {
            ui.LoginBtnResponse(session.AccessToken.token);
        }, userId, pw);
    }
}
