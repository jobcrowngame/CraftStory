using LitJson;
using System.Collections.Generic;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
    /// <summary>
    /// 新しいアカウントを作成
    /// </summary>
    public void CreateNewAccount()
    {
        NWMng.E.CreateNewAccount((rp) =>
        {
            DataMng.E.UserData.Account = (string)rp["acc"];
            DataMng.E.UserData.UserPW = (string)rp["pw"];

            Logger.Log("新しいアカウント作成成功しました。\n{0}", DataMng.E.UserData.Account);

            Login(0);
        });
    }

    /// <summary>
    /// ログイン
    /// </summary>
    /// <param name="IsMaintenance">メンテナンス 1=on 2=off</param>
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

        UICtl.E.LockUI();
        NWMng.E.Login((rp) =>
        {
            DataMng.E.token = (string)rp["token"];
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];
            DataMng.E.RuntimeData.GuideEnd = (int)rp["guide_end"];
            if(rp["nickname"] != null) DataMng.E.RuntimeData.NickName = (string)rp["nickname"];
            if (rp["comment"] != null) DataMng.E.RuntimeData.Comment = (string)rp["comment"];

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

        }, DataMng.E.UserData.UserPW);
    }
}
