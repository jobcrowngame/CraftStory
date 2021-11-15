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
            var loginRP = JsonMapper.ToObject<LoginRP>(rp.ToJson());

            DataMng.E.token = loginRP.token;
            DataMng.E.MyShop.myShopLv = loginRP.myShopLv;
            DataMng.E.RuntimeData.GuideEnd = loginRP.guide_end;
            DataMng.E.RuntimeData.GuideEnd2 = loginRP.guide_end2;
            DataMng.E.RuntimeData.GuideEnd3 = loginRP.guide_end3;
            DataMng.E.RuntimeData.GuideEnd4 = loginRP.guide_end4;
            DataMng.E.RuntimeData.NickName = loginRP.nickname;
            DataMng.E.RuntimeData.Comment = loginRP.comment;
            DataMng.E.RuntimeData.Email = loginRP.email;
            DataMng.E.RuntimeData.UseGoodNum = loginRP.goodNum;
            DataMng.E.RuntimeData.Lv = loginRP.lv;
            DataMng.E.RuntimeData.Exp = loginRP.exp;
            DataMng.E.RuntimeData.MyGoodNum = loginRP.myGoodNum;

            TaskMng.E.MainTaskId = loginRP.mainTaskID;
            TaskMng.E.MainTaskClearedCount = loginRP.mainTaskClearedCount;
            TaskMng.E.CheckClearedCount();

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

            NWMng.E.GetTotalSetBlockCount((rp) =>
            {
                if (string.IsNullOrEmpty(rp.ToString()))
                    return;

                DataMng.E.RuntimeData.TotalSetBlockCount = (int)rp;
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

    private struct LoginRP
    {
        // Token
        public string token { get; set; }

        /// <summary>
        /// マイショップレベル
        /// </summary>
        public int myShopLv { get; set; }

        /// <summary>
        /// ガイド終了１
        /// </summary>
        public int guide_end { get; set; }

        /// <summary>
        /// ガイド終了２
        /// </summary>
        public int guide_end2 { get; set; }

        /// <summary>
        /// ガイド終了１３
        /// </summary>
        public int guide_end3 { get; set; }

        /// <summary>
        /// ガイド終了４
        /// </summary>
        public int guide_end4 { get; set; }

        /// <summary>
        /// ニックネーム
        /// </summary>
        public string nickname { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// メール
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// いいねした数
        /// </summary>
        public int goodNum { get; set; }

        /// <summary>
        /// レベル
        /// </summary>
        public int lv { get; set; }

        /// <summary>
        /// 経験値
        /// </summary>
        public int exp { get; set; }

        /// <summary>
        /// 自分のいいね数
        /// </summary>
        public int myGoodNum { get; set; }

        /// <summary>
        /// 今のメインタスクID
        /// </summary>
        public int mainTaskID { get; set; }

        /// <summary>
        /// クリアしたメインタスク内容数
        /// </summary>
        public int mainTaskClearedCount { get; set; }
    }
}
