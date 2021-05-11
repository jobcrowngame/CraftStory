using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTest : MonoBehaviour
{
    private static UserTest entity;
    public static UserTest E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<UserTest>();

            return entity;
        }
    }

    string id = "aa4d4e86-0303-4f23-bbeb-3344f12f9198";
    string pw = "kfHtHQ4LWWv6IKdraivCXIySlBuTOHVG";

    public void Init()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            WritLoginIDPW();
        }
    }

    public void WritLoginIDPW()
    {
        LoginUI loginUI = UICtl.E.GetUI(UIType.Login) as LoginUI;
        loginUI.WritIDPW(id, pw);
    }
}
