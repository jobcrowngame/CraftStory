using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        NWMng.E.CreateNewAccount((rp) =>
        {
            Logger.Log("�V�����A�J�E���g�쐬�������܂����B");
            Logger.Log("ID:\n{0}, \nPW:\n{1}", rp["acc"], rp["pw"]);
            DataMng.E.NewUser((string)rp["acc"], (string)rp["pw"]);
            Login(0);

            PlayDescriptionLG.E.IsFirst = true;
        });
    }

    public void Login(int IsMaintenance)
    {
        if (DataMng.E.UserData == null)
        {
            if (IsMaintenance == 1)
            {
                CommonFunction.ShowHintBox(PublicPar.Maintenance, () => { Logger.Warning("Quit"); Application.Quit(); });
            }
            else
            {
                UICtl.E.OpenUI<TermsUI>(UIType.Terms);
            }

            return;
        }

        string id = DataMng.E.UserData.Account;
        string pw = DataMng.E.UserData.UserPW;

        NWMng.E.Login((rp) =>
        {
            DataMng.E.token = (string)rp["token"];
            DataMng.E.MyShop.firstUseMyShop = (int)rp["firstUseMyShop"];
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];

            // MyShop
            NWMng.E.GetMyShopInfo((rp) => 
            {
                DataMng.E.MyShop.Clear();
                if (!string.IsNullOrEmpty(rp.ToString()))
                {
                    List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp.ToJson());
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        DataMng.E.MyShop.myShopItem[i] = shopItems[i];
                    }
                }
            });

            // �V�������[���q���g
            NWMng.E.GetNewEmailCount((rp) =>
            {
                DataMng.E.RuntimeData.NewEmailCount = (int)rp["count"];
            });

            // ���[�J���f�[�^������ꍇ�T�[�o�[�ɃZ�[�u
            if (DataMng.E.HomeData != null)
            {
                NWMng.E.SaveHomeData(null, DataMng.E.HomeData.ToStringData());
            }
            // ���邢�́A�T�[�o�[����f�[�^�����炤
            else
            {
                NWMng.E.LoadHomeData((rp) =>
                {
                    if (!string.IsNullOrEmpty(rp.ToString()))
                    {
                        DataMng.E.HomeData = new MapData((string)rp["homedata"]);
                    }
                });
            }

            ui.LoginResponse();
        }, id, pw);
    }
}
