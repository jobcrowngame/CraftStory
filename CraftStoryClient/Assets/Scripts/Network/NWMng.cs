using JsonConfigData;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// サーバーとの通信マネージャー
/// </summary>
public partial class NWMng : MonoBehaviour
{
    public static NWMng E
    {
        get
        {
            if (entity == null)
                entity = CommonFunction.CreateGlobalObject<NWMng>();

            return entity;
        }
    }
    private static NWMng entity;
    

    private string url; // サーバーURL
    public string URL { get => url; set => url = value; }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitInitCoroutine()
    {
        yield return null;
    }

    /// <summary>
    /// サーバーに通信（サーバーURLゲット）
    /// </summary>
    /// <param name="rp"></param>
    /// <returns></returns>
    private IEnumerator ConnectIE(Action<JsonData> rp)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(PublicPar.TestURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                Logger.Error(www.error);
            else
            {
                try
                {
                    JsonData jd = JsonMapper.ToObject(www.downloadHandler.text);
                    rp(jd);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }
        }
    }
    /// <summary>
    /// サーバーとの通信
    /// </summary>
    /// <param name="rp">Response</param>
    /// <param name="data">送信データ</param>
    /// <param name="cmd">識別コード</param>
    /// <returns></returns>
    public IEnumerator HttpRequest(Action<JsonData> rp, NWData data, CMD cmd)
    {
        // ガイドの場合、サーバーとの通信はやめます。
        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            yield return null;

        // ログ
        Logger.Log("[CMD:{0}-{1}---Send]\n{2}",cmd, (int)cmd, data.ToString());

        // 暗号化
        string cryptData = string.IsNullOrEmpty(data.ToString())
            ? ""
            : CryptMng.E.EncryptString(data.ToString());

        // URLがない場合、終了
        if (string.IsNullOrEmpty(URL))
            yield return null;

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("code", (int)cmd);
        wwwForm.AddField("data", cryptData, System.Text.Encoding.UTF8);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, wwwForm))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                Logger.Error(www.error);
            else
            {
                if (string.IsNullOrEmpty(www.downloadHandler.text))
                {
                    Logger.Error("Null Response.[CMD: {0}]", cmd);
                    CommonFunction.ShowHintBar(35);
                    yield return null;
                }
                else
                {
                    // 暗号化の解析
                    var resultJson = CryptMng.E.DecryptString(www.downloadHandler.text);

                    // Json to Object
                    JsonData jd = JsonMapper.ToObject(resultJson);

                    // ログ
                    Logger.Log("[CMD:{0}-{1}---Result]\n{2}", cmd, (int)cmd, jd.ToJson());

                    // エラーコード
                    int errorCode = (int)jd["error"];
                    if (errorCode > 0)
                    {
                        // エラーコード　998　の場合はメンテナンスメッセージボックスを出す
                        if (errorCode == 998)
                            CommonFunction.Maintenance();
                        // 他のエラーコードは設定ファイルのメッセージを出す
                        else
                            CommonFunction.ShowHintBar(errorCode);
                    }
                    else
                    {
                        // 通信成功後のCallBack
                        if (rp != null)
                            rp(jd["result"]);
                    }
                }
            }
        }
    }

    public void Connect(Action<JsonData> rp)
    {
        StartCoroutine(ConnectIE(rp));
    }

    /// <summary>
    /// バージョンゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetVersion(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.Version));
    }

    /// <summary>
    /// ログイン
    /// </summary>
    public void Login(Action<JsonData> rp,  string pw)
    {
        var data = new NWData();
        data.Add("pw", pw);

        StartCoroutine(HttpRequest(rp, data, CMD.Login));
    }

    /// <summary>
    /// アイテムリストをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetItemList(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.ItemList));
    }

    /// <summary>
    /// アイテムを装備
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    /// <param name="site"></param>
    public void EquitItem(Action<JsonData> rp, int guid, int site)
    {
        var item = DataMng.E.GetItemByGuid(guid);
        if (item != null)
            item.equipSite = site;

        if (rp != null) rp("");
    }

    /// <summary>
    /// クラフト
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="craft"></param>
    /// <param name="count"></param>
    public void Craft(Action<JsonData> rp, Craft craft, int count)
    {
        DataMng.E.RemoveItemByItemId(craft.Cost1, craft.Cost1Count);
        DataMng.E.RemoveItemByItemId(craft.Cost2, craft.Cost2Count);
        DataMng.E.RemoveItemByItemId(craft.Cost3, craft.Cost3Count);
        DataMng.E.RemoveItemByItemId(craft.Cost4, craft.Cost4Count);

        DataMng.E.AddItem(craft.ItemID, count);

        rp(null);
    }

    /// <summary>
    /// 新しいメール数をゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetNewEmailCountRequest(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetNewEmailCount));
    }

    /// <summary>
    /// ランダムボーナスをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="randomBonusId"></param>
    public void GetRandomBonus(Action<Dictionary<int, int>> rp, int randomBonusId)
    {
        Dictionary<int, int> dic = CommonFunction.GetRandomBonus(randomBonusId);
        rp(dic);
    }

    /// <summary>
    /// マイショップのデータをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetMyShopInfo(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetMyShopInfo));
    }

    /// <summary>
    /// サブスクリプション情報をゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetSubscriptionInfoRequest(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetSubscriptionInfo));
    }

    /// <summary>
    /// 設計図詳細データをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="myshopId"></param>
    public void GetBlueprintPreviewData(Action<JsonData> rp, int myshopId)
    {
        var data = new NWData();
        data.Add("myshopId", myshopId);

        StartCoroutine(HttpRequest(rp, data, CMD.GetBlueprintPreviewData));
    }

    /// <summary>
    /// フレンドホームデータをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="userGuid">ユーザーGUID</param>
    public void GetFriendHomeData(Action<JsonData> rp, int userGuid)
    {
        var data = new NWData();
        data.Add("userGuid", userGuid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetFriendHomeData));
    }

    /// <summary>
    /// アイテム関連データをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="itemGuid">GUID</param>
    public void GetItemRelationData(Action<JsonData> rp, int itemGuid)
    {
        var data = new NWData();
        data.Add("itemGuid", itemGuid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetItemRelationData));
    }

    public void GetEquipmentInfoList(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetEquipmentInfoList));
    }

    /// <summary>
    /// 装備鑑定
    /// </summary>
    public void AppraisalEquipment(Action<string> rp, int itemGuid, int equipmentId)
    {
        var config = ConfigMng.E.Equipment[equipmentId];
        string[] pondArr = config.PondId.Split(',');
        string skillStr = "";

        for (int i = 0; i < pondArr.Length; i++)
        {
            int pondId = int.Parse(pondArr[i]);

            List<int> bonusList = new List<int>();
            CommonFunction.GetRandomBonusPond(pondId, ref bonusList);

            if (bonusList.Count == 0)
                continue;

            if (string.IsNullOrEmpty(skillStr))
            {
                skillStr = bonusList[0].ToString();
            }
            else
            {
                skillStr = skillStr + "," + bonusList[0].ToString();
            }
        }

        rp(skillStr);
    }

    /// <summary>
    /// 冒険エリアに入る場合
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="exp"></param>
    public void ArriveFloor(Action<JsonData> rp, int arrivedFloor)
    {
        if (arrivedFloor > LocalDataMng.E.Data.Statistics_userT.maxArrivedFloor)
            LocalDataMng.E.Data.Statistics_userT.maxArrivedFloor = arrivedFloor;

        LocalDataMng.E.Data.Statistics_userT.lastFloorCount++;
    }

    /// <summary>
    /// トータル設置済ブロック数を取得
    /// </summary>
    /// <param name="rp"></param>
    public void GetTotalSetBlockCount(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetTotalSetBlockCount));
    }

    /// <summary>
    /// メインタスククリア数追加
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="count"></param>
    public void AddMainTaskClearCount(Action rp, int count)
    {
        LocalDataMng.E.Data.limitedT.main_task_count++;
        rp();
    }

    /// <summary>
    /// 最大冒険レベルゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetMaxBraveLevel(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetMaxBraveLevel));
    }


    public enum CMD
    {
        Version = 99,
        CreateNewAccount = 100,
        Login = 101,

        ItemList = 1001,
        UseItem,
        RemoveItemByItemId,
        AddItem,
        AddItemInData,
        AddItems,
        EquitItem,
        Craft,
        ClearAdventure,
        Buy = 1010,
        GetCoins,
        GetBonus,
        LevelUpMyShop,
        UploadBlueprintToMyShop,
        UpdateNickName,
        SearchMyShopItems,
        BuyMyShopItem,
        LoadBlueprint,
        GetEmail,
        ReadEmail = 1020,
        GetNewEmailCount,
        GetRandomBonus,
        GetNoticeList,
        GetNotice,
        GetMyShopInfo,
        ReceiveEmailItem,
        BuySubscription,
        GetSubscriptionInfo,
        GuideEnd,
        Gacha = 1030,
        DeleteItem,
        DeleteItems,
        Follow,
        DeFollow,
        ReadFollow,
        ReadFollower,
        UpdateComment,
        SearchFriend,
        GetBlueprintPreviewData,
        GetFriendHomeData = 1040,
        ExchangePoints,
        GachaAddBonusAgain,
        GetItemRelationData,
        GetMissionInfo,
        ClearMission,
        GetMissionBonus,
        MyShopGoodEvent,
        GetBlueprintPreviewDataByItemGuid,
        GetGacha,
        GetAllShopLimitedCounts = 1050,
        GetShopLimitedCount,
        GetEquipmentInfo,
        GetEquipmentInfoList,
        AppraisalEquipment,
        AddExp,
        ArriveFloor,
        Resurrection,
        GetTotalSetBlockCount,
        MainTaskEnd,
        AddMainTaskClearCount = 1060,
        GetMaxBraveLevel,
        GetLoginBonusInfo,
        GetLoginBonus,
        GetTotalUploadBlueprintCount,

        SaveHomeData = 6000,
        LoadHomeData = 6001,

        Charge = 9000,

        ShowClientLog = 9999,
    }

    struct NWItemData
    {
        public int itemId;
        public int count;
    }
}
