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
            LocalDataMng.E.Data.UserDataT.exp = loginRP.exp;

            LocalDataMng.E.Data.LimitedT.main_task = loginRP.mainTaskID;
            LocalDataMng.E.Data.LimitedT.main_task_count = loginRP.mainTaskClearedCount;
            LocalDataMng.E.Data.LimitedT.guide_end3 = loginRP.guide_end3;

            DataMng.E.token = loginRP.token;
            //TaskMng.E.CheckClearedCount();

            NWMng.E.GetMaxBraveLevel((rp) =>
            {
                LocalDataMng.E.Data.StatisticsUserT.maxArrivedFloor = (int)rp["maxArrivedFloor"];
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

            LocalDataMng.E.LoadData();
        }, DataMng.E.UserData.UserPW);
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
