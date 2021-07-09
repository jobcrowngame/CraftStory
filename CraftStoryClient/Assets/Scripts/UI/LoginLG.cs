using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        NWMng.E.CreateNewAccount((rp) =>
        {
            Logger.Log("新しいアカウント作成成功しました。");
            Logger.Log("ID:\n{0}, \nPW:\n{1}", rp["acc"], rp["pw"]);
            DataMng.E.NewUser((string)rp["acc"], (string)rp["pw"]);
            Login();

            PlayDescriptionLG.E.IsFirst = true;
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
            DataMng.E.token = (string)rp["token"];
            DataMng.E.MyShop.firstUseMyShop = (int)rp["firstUseMyShop"];
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];

            DataMng.E.MyShop.Clear();
            if (!string.IsNullOrEmpty(rp["myShopItems"].ToString()))
            {
                List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp["myShopItems"].ToJson());
                for (int i = 0; i < shopItems.Count; i++)
                {
                    DataMng.E.MyShop.myShopItem[i] = shopItems[i];
                }
            }

            ui.LoginResponse();

            NWMng.E.GetNewEmailCount((rp) =>
            {
                DataMng.E.RuntimeData.NewEmailCount = (int)rp["count"];
            });
        }, id, pw);
    }
}
