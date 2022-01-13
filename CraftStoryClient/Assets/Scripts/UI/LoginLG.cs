using LitJson;
using System;
using System.Collections.Generic;

public class LoginLg : UILogicBase<LoginLg, LoginUI>
{
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

            LocalDataMng.E.Data.UserDataT.lv = loginRP.lv;
            LocalDataMng.E.Data.UserDataT.myShopLv = loginRP.myShopLv;
            LocalDataMng.E.Data.UserDataT.nickname = loginRP.nickname;
            LocalDataMng.E.Data.UserDataT.comment = loginRP.comment;
            LocalDataMng.E.Data.UserDataT.email = loginRP.email;
            LocalDataMng.E.Data.UserDataT.exp = loginRP.exp;

            LocalDataMng.E.Data.limitedT.main_task = loginRP.mainTaskID;
            LocalDataMng.E.Data.limitedT.main_task_count = loginRP.mainTaskClearedCount;
            LocalDataMng.E.Data.limitedT.guide_end = loginRP.guide_end;
            LocalDataMng.E.Data.limitedT.guide_end2 = loginRP.guide_end2;
            LocalDataMng.E.Data.limitedT.guide_end3 = loginRP.guide_end3;
            LocalDataMng.E.Data.limitedT.guide_end4 = loginRP.guide_end4;
            LocalDataMng.E.Data.limitedT.guide_end5 = loginRP.guide_end5;

            DataMng.E.token = loginRP.token;
            TaskMng.E.CheckClearedCount();

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

            NWMng.E.GetMaxBraveLevel((rp) =>
            {
                LocalDataMng.E.Data.Statistics_userT.maxArrivedFloor = (int)rp["maxArrivedFloor"];
            });

            // サブスクリプションの状態
            NWMng.E.GetSubscriptionInfo();

            // 新しいメールヒント
            NWMng.E.GetNewEmailCount();

            NoticeLG.E.IsFirst = true;

            //NWMng.E.GetItems();

            //NWMng.E.GetCoins((rp) =>
            //{
            //    DataMng.GetCoins(rp);
            //});

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
            // 強制表示お知らせ当日チェックマップがない場合は追加
            if (DataMng.E.UserData.PickupNoticeCheckMap == null)
            {
                DataMng.E.UserData.PickupNoticeCheckMap = new Dictionary<int, DateTime>();
            }

            Main.LoginFailed = false;

            // 既存ユーザーの空腹度を100にする為
            if (DataMng.E.UserData.FreeFoodEated == 0)
            {
                DataMng.E.UserData.Hunger = 100;
                DataMng.E.UserData.FreeFoodEated = 1;
            }

            LocalDataMng.E.LoadServerData();
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
        /// ガイド終了５
        /// </summary>
        public int guide_end5 { get; set; }

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


        /// <summary>
        /// 本日の最初ログイン
        /// </summary>
        public int firstLoginDaily { get; set; }
    }
}
