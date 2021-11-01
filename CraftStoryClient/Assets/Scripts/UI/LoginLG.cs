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
            DataMng.E.NewUser((string)rp["acc"], (string)rp["pw"]);

            Logger.Log("新しいアカウント作成成功しました。\n{0}", (string)rp["acc"]);

            DataMng.E.RuntimeData.IsNewUser = true;

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
            DataMng.E.RuntimeData.GuideEnd2 = (int)rp["guide_end2"];
            if (rp["nickname"] != null) DataMng.E.RuntimeData.NickName = (string)rp["nickname"];
            if (rp["comment"] != null) DataMng.E.RuntimeData.Comment = (string)rp["comment"];
            if (rp["email"] != null) DataMng.E.RuntimeData.Email = (string)rp["email"];
            if (rp["goodNum"] != null) DataMng.E.RuntimeData.MyShopGoodNum = (int)rp["goodNum"];
            DataMng.E.RuntimeData.Lv = (int)rp["lv"];
            DataMng.E.RuntimeData.Exp = (int)rp["exp"];

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

            NWMng.E.GetItems();

            NWMng.E.GetCoins((rp) =>
            {
                DataMng.GetCoins(rp);
            });

            // 新規ユーザーの場合、S3に送信しない
            if (DataMng.E.RuntimeData.IsNewUser)
            {
                DataMng.E.SetMapData(WorldMng.E.MapCtl.CreateMapData(100), MapType.Home);
                ui.LoginResponse();
            }
            // ローカルデータがある場合、サーバーにセーブ
            else if (DataMng.E.GetHomeData() != null)
            {
                AWSS3Mng.E.SaveHomeData(DataMng.E.UserData.Account, DataMng.E.GetHomeData().ToStringData());
                ui.LoginResponse();
            }
            // ローカルデータがない場合、サーバーからデータをもらう
            else
            {
                LoadHomeData(5);
            }
        }, DataMng.E.UserData.UserPW);
    }

    /// <summary>
    /// S3から　ホームデータをロード
    /// </summary>
    /// <param name="retryCount"></param>
    private void LoadHomeData(int retryCount)
    {
        AWSS3Mng.E.LoadHomeData(DataMng.E.UserData.Account, (rp) =>
        {
            if (!string.IsNullOrEmpty(rp))
            {
                DataMng.E.SetMapData(new MapData(rp), MapType.Home);
            }
            else
            {
                DataMng.E.SetMapData(WorldMng.E.MapCtl.CreateMapData(100), MapType.Home);
            }

            ui.LoginResponse();
        }, ()=>
        {
            if (retryCount > 1)
            {
                LoadHomeData(--retryCount);
            }
            else
            {
                DataMng.E.SetMapData(WorldMng.E.MapCtl.CreateMapData(100), MapType.Home);
                ui.LoginResponse();
            }
        });
    }
}
