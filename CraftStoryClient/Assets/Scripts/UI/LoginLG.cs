using LitJson;
using System.Collections.Generic;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    public void CreateNewAccount()
    {
        NWMng.E.CreateNewAccount((rp) =>
        {
            Logger.Log("新しいアカウント作成成功しました。");
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
                CommonFunction.Maintenance();
            else
                UICtl.E.OpenUI<TermsUI>(UIType.Terms);

            return;
        }

        string id = DataMng.E.UserData.Account;
        string pw = DataMng.E.UserData.UserPW;

        UICtl.E.LockUI();
        NWMng.E.Login((rp) =>
        {
            DataMng.E.token = (string)rp["token"];
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];
            DataMng.E.RuntimeData.GuideEnd = (int)rp["guide_end"];
            if(rp["nickname"] != null) DataMng.E.RuntimeData.NickName = (string)rp["nickname"];
            //if (rp["comment"] != null) DataMng.E.RuntimeData.Comment = (string)rp["comment"];

            // IAPMngを初期化
            IAPMng.E.Init();

            // マイショップデータ
            NWMng.E.GetMyShopInfo((rp) =>
            {
                DataMng.E.MyShop.Clear();
                if (!string.IsNullOrEmpty(rp.ToString()))
                {
                    List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp.ToJson());
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        DataMng.E.MyShop.MyShopItem[i] = shopItems[i];
                    }
                }
            });

            // サブスクリプションの状態
            NWMng.E.GetSubscriptionInfo();

            // 新しいメールヒント
            NWMng.E.GetNewEmailCount();

            NoticeLG.E.IsFirst = true;

            // ローカルデータがある場合サーバーにセーブ
            if (DataMng.E.GetHomeData() != null)
            {
                NWMng.E.SaveHomeData(null, DataMng.E.GetHomeData().ToStringData());
                ui.LoginResponse();
            }
            // あるいは、サーバーからデータをもらう
            else
            {
                NWMng.E.LoadHomeData((rp) =>
                {
                    if (!string.IsNullOrEmpty(rp.ToString()))
                    {
                        DataMng.E.SetMapData(new MapData((string)rp["homedata"]), MapType.Home);
                    }
                    ui.LoginResponse();
                });
            }

        }, id, pw);
    }
}
