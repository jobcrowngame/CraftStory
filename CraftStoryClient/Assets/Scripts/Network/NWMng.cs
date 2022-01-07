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
        using (UnityWebRequest www = UnityWebRequest.Get(PublicPar.LocalURL))
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
    /// 新しいアカウント作成
    /// </summary>
    /// <param name="rp"></param>
    public void CreateNewAccount(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.CreateNewAccount));
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
    private void GetItemList(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.ItemList));
    }

    /// <summary>
    /// アイテムを手に入る
    /// </summary>
    public void AddItem(Action<JsonData> rp, int itemId, int count)
    {
        var data = new NWData();
        data.Add("itemId", itemId);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItem));
    }

    /// <summary>
    /// 設計図を手に入る
    /// </summary>
    public void AddItemInData(Action<JsonData> rp, int itemId, int count, string newName, string rdata, string textureName)
    {
        var data = new NWData();
        data.Add("itemId",itemId);
        data.Add("count", count);
        data.Add("newName",newName);
        data.Add("rdata",rdata);
        data.Add("textureName", textureName);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItemInData));
    }

    /// <summary>
    /// 服すのアイテムを手に入る
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="items"></param>
    public void AddItems(Action<JsonData> rp, Dictionary<int, int> items)
    {
        var data = new NWData();

        List<NWItemData> list = new List<NWItemData>();
        foreach (var item in items)
        {
            list.Add(new NWItemData() { itemId = item.Key, count = item.Value });
        }
        data.Add("items", JsonMapper.ToJson(list));

        StartCoroutine(HttpRequest(rp, data, CMD.AddItems));
    }

    /// <summary>
    /// アイテムを消耗
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    /// <param name="count"></param>
    public void UseItem(Action<JsonData> rp, int guid, int count)
    {
        var data = new NWData();
        data.Add("guid", guid);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.UseItem));
    }

    /// <summary>
    /// アイテムを削除
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="itemid"></param>
    /// <param name="count"></param>
    public void RemoveItem(Action<JsonData> rp, int itemid, int count)
    {
        var data = new NWData();
        data.Add("itemId", itemid);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByItemId));
    }

    /// <summary>
    /// アイテムを装備
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    /// <param name="site"></param>
    public void EquitItem(Action<JsonData> rp, int guid, int site)
    {
        var data = new NWData();
        data.Add("guid", guid);
        data.Add("site", site);

        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            var item = DataMng.E.GetItemByGuid(guid);
            if (item != null)
                item.equipSite = site;
            
            if (rp != null) rp("");
        }
        else
        {
            StartCoroutine(HttpRequest(rp, data, CMD.EquitItem));
        }
    }

    /// <summary>
    /// クラフト
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="craft"></param>
    /// <param name="count"></param>
    public void Craft(Action<JsonData> rp, Craft craft, int count)
    {
        var data = new NWData();
        data.Add("craftId", craft.ID);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.Craft));
    }

    /// <summary>
    /// ショップで買う
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="shopId"></param>
    public void Buy(Action<JsonData> rp, int shopId)
    {
        var data = new NWData();
        data.Add("shopId", shopId);

        StartCoroutine(HttpRequest(rp, data, CMD.Buy));
    }

    /// <summary>
    /// 持っているコインをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetCoins(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetCoins));
    }

    /// <summary>
    /// 課金
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="productId"></param>
    /// <param name="transactionID"></param>
    public void Charge(Action<JsonData> rp, string productId, string transactionID)
    {
        var data = new NWData();
        data.Add("productId", productId);
        data.Add("transactionId", transactionID);

        StartCoroutine(HttpRequest(rp, data, CMD.Charge));
    }

    /// <summary>
    /// ボーナスをもらう
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="bonusId"></param>
    public void GetBonus(Action<JsonData> rp, int bonusId)
    {
        var data = new NWData();
        data.Add("bonusId", bonusId);
        StartCoroutine(HttpRequest(rp, data, CMD.GetBonus));
    }

    /// <summary>
    /// 冒険マップのボーナス計算
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="resources"></param>
    public void ClearAdventure(Action<JsonData> rp, List<int> resources)
    {
        var data = new NWData();

        string msg = "";
        if (resources != null && resources.Count > 0)
        {
            msg = resources[0].ToString();
            for (int i = 1; i < resources.Count; i++)
            {
                msg += "," + resources[i];
            }
        }
        else
        {
            msg = "-1";
        }
        data.Add("bonusList", msg);

        StartCoroutine(HttpRequest(rp, data, CMD.ClearAdventure));
    }

    /// <summary>
    /// マイショップのレベルアップ
    /// </summary>
    /// <param name="rp"></param>
    public void LevelUpMyShop(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.LevelUpMyShop));
    }

    /// <summary>
    /// マイショップの設計図アップロード
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="itemGuid"></param>
    /// <param name="site"></param>
    /// <param name="price"></param>
    /// <param name="fileName">S3にアップロードするテクスチャ名</param>
    public void UploadBlueprintToMyShop(Action<JsonData> rp, int itemGuid, int site, int price, string texture)
    {
        var data = new NWData();
        data.Add("nickName", DataMng.E.RuntimeData.NickName);
        data.Add("itemGuid", itemGuid);
        data.Add("site", site);
        data.Add("price", price);
        data.Add("texture", texture);

        StartCoroutine(HttpRequest(rp, data, CMD.UploadBlueprintToMyShop));
    }

    /// <summary>
    /// ニックネーム改修
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="nickName"></param>
    public void UpdateNickName(Action<JsonData> rp, string nickName)
    {
        var data = new NWData();
        data.Add("nickName", nickName);

        StartCoroutine(HttpRequest(rp, data, CMD.UpdateNickName));
    }

    /// <summary>
    /// ショップの設計図を検索
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="page"></param>
    /// <param name="nickName"></param>
    /// <param name="sortType"></param>
    public void SearchMyShopItems(Action<JsonData> rp, int page, string nickName, int sortType)
    {
        var data = new NWData();
        data.Add("page", page);
        data.Add("nickName", nickName);
        data.Add("sortType", sortType);

        StartCoroutine(HttpRequest(rp, data, CMD.SearchMyShopItems));
    }

    /// <summary>
    /// 設計図を買う
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void BuyMyShopItem(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.BuyMyShopItem));
    }

    /// <summary>
    /// マイショップの設計図をロード
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="site"></param>
    /// <param name="isfree"></param>
    public void LoadBlueprint(Action<JsonData> rp, int site, int isfree)
    {
        var data = new NWData();
        data.Add("site", site);
        data.Add("isfree", isfree);

        StartCoroutine(HttpRequest(rp, data, CMD.LoadBlueprint));
    }

    /// <summary>
    /// メールをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="page"></param>
    public void GetEmail(Action<JsonData> rp, int page)
    {
        var data = new NWData();
        data.Add("page", page);

        StartCoroutine(HttpRequest(rp, data, CMD.GetEmail));
    }

    /// <summary>
    /// メールを読む
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void ReadEmail(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.ReadEmail));
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
    public void GetRandomBonus(Action<JsonData> rp, int randomBonusId)
    {
        var data = new NWData();
        data.Add("randomBonusId", randomBonusId);

        StartCoroutine(HttpRequest(rp, data, CMD.GetRandomBonus));
    }

    /// <summary>
    /// お知らせリストをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetNoticeList(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetNoticeList));
    }

    /// <summary>
    /// お知らせ詳細をゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void GetNotice(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetNotice));
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
    /// メール内のアイテムをもらう
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="emailGuid"></param>
    public void ReceiveEmailItem(Action<JsonData> rp, int emailGuid)
    {
        var data = new NWData();
        data.Add("guid", emailGuid);

        StartCoroutine(HttpRequest(rp, data, CMD.ReceiveEmailItem));
    }

    /// <summary>
    /// サブスクリプションを買う
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="type"></param>
    public void BuySubscription(Action<JsonData> rp, int type)
    {
        var data = new NWData();
        data.Add("type", type);

        StartCoroutine(HttpRequest(rp, data, CMD.BuySubscription));
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
    /// チュートリアル完了
    /// </summary>
    /// <param name="rp"></param>
    public void GuideEnd(Action<JsonData> rp, int guidId)
    {
        var data = new NWData();
        data.Add("guidId", guidId);

        StartCoroutine(HttpRequest(rp, data, CMD.GuideEnd));
    }

    /// <summary>
    /// ガチャ
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="gachaId"></param>
    /// <param name="gachaGroup"></param>
    public void Gacha(Action<JsonData> rp, int gachaId, int gachaGroup)
    {
        var data = new NWData();
        data.Add("gachaId", gachaId);
        data.Add("gachaGroup", gachaGroup);

        StartCoroutine(HttpRequest(rp, data, CMD.Gacha));
    }

    /// <summary>
    /// アイテムを削除
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void DeleteItem(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.DeleteItem));
    }

    /// <summary>
    /// 複数のアイテムを削除
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="items"></param>
    public void DeleteItems(Action<JsonData> rp, List<ItemData.DeleteItemData> items)
    {
        var data = new NWData();
        data.Add("items", JsonMapper.ToJson(items));

        StartCoroutine(HttpRequest(rp, data, CMD.DeleteItems));
    }

    /// <summary>
    /// フォローする
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void Follow(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.Follow));
    }

    /// <summary>
    /// フォローを解除
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="guid"></param>
    public void DeFollow(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.DeFollow));
    }

    /// <summary>
    /// フォローリストをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void ReadFollow(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.ReadFollow));
    }

    /// <summary>
    /// フォロワーリストををゲット
    /// </summary>
    /// <param name="rp"></param>
    public void ReadFollower(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.ReadFollower));
    }

    /// <summary>
    /// コメントを改修
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="msg"></param>
    public void UpdateComment(Action<JsonData> rp, string msg)
    {
        var data = new NWData();
        data.Add("comment", msg);

        StartCoroutine(HttpRequest(rp, data, CMD.UpdateComment));
    }

    /// <summary>
    /// フレンドを検索
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="userAcc"></param>
    public void SearchFriend(Action<JsonData> rp, string userAcc)
    {
        var data = new NWData();
        data.Add("userAcc", userAcc);

        StartCoroutine(HttpRequest(rp, data, CMD.SearchFriend));
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
    /// ポイント交換
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="point">交換するポイント数</param>
    /// <param name="email">メールアドレス</param>
    public void ExchangePoints(Action<JsonData> rp, int point, int money, string email)
    {
        var data = new NWData();
        data.Add("point", point);
        data.Add("money", money);
        data.Add("email", email);

        StartCoroutine(HttpRequest(rp, data, CMD.ExchangePoints));
    }

    /// <summary>
    /// ガチャ追加ボーナスもう一回もらう
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="gachaId"></param>
    public void GachaAddBonusAgain(Action<JsonData> rp, int gachaId)
    {
        var data = new NWData();
        data.Add("gachaId", gachaId);

        StartCoroutine(HttpRequest(rp, data, CMD.GachaAddBonusAgain));
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

    /// <summary>
    /// ミッション情報をゲット
    /// </summary>
    public void GetMissionInfo(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetMissionInfo));
    }

    /// <summary>
    /// ミッションをクリア
    /// </summary>
    /// /// <param name="missionId">ミッションID</param>
    /// <param name="missionType">ミッションタイプ</param>
    public void ClearMission(Action<JsonData> rp, int missionId, int missionType, int count = 1)
    {
        var data = new NWData();
        data.Add("missionId", missionId);
        data.Add("missionType", missionType);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.ClearMission));
    }

    /// <summary>
    /// ミッションボーナスをもらう
    /// </summary>
    /// <param name="missionId">ミッションID</param>
    /// <param name="missionType">ミッションタイプ</param>
    public void GetMissionBonus(Action<JsonData> rp, int missionId, int missionType)
    {
        var data = new NWData();
        data.Add("missionId", missionId);
        data.Add("missionType", missionType);

        StartCoroutine(HttpRequest(rp, data, CMD.GetMissionBonus));
    }

    /// <summary>
    /// マイショップいいね
    /// </summary>
    public void MyShopGoodEvent(Action<JsonData> rp, string targetAcc)
    {
        var data = new NWData();
        data.Add("targetAcc", targetAcc);

        StartCoroutine(HttpRequest(rp, data, CMD.MyShopGoodEvent));
    }

    /// <summary>
    /// アイテムGUIDによって設計図詳細データをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="myshopId"></param>
    public void GetBlueprintPreviewDataByItemGuid(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetBlueprintPreviewDataByItemGuid));
    }

    /// <summary>
    /// ガチャ実施回数取得
    /// </summary>
    public void GetGacha(Action<JsonData> rp, int gachaGroup)
    {
        var data = new NWData();
        data.Add("gachaGroup", gachaGroup);

        StartCoroutine(HttpRequest(rp, data, CMD.GetGacha));
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetAllShopLimitedCounts(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetAllShopLimitedCounts));
    }

    /// <summary>
    /// 
    /// </summary>
    public void GetShopLimitedCount(Action<JsonData> rp, int shopId)
    {
        var data = new NWData();
        data.Add("shopId", shopId);

        StartCoroutine(HttpRequest(rp, data, CMD.GetShopLimitedCount));
    }

    /// <summary>
    /// 装備データをゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="itemGuid">アイテムGUID</param>
    public void GetEquipmentInfo(Action<JsonData> rp, int itemGuid)
    {
        var data = new NWData();
        data.Add("itemGuid", itemGuid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetEquipmentInfo));
    }

    /// <summary>
    /// 装備一覧ゲット
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="itemGuid"></param>
    public void GetEquipmentInfoList(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetEquipmentInfoList));
    }

    /// <summary>
    /// 装備鑑定
    /// </summary>
    public void AppraisalEquipment(Action<JsonData> rp, int itemGuid, int equipmentId)
    {
        var data = new NWData();
        data.Add("itemGuid", itemGuid);
        data.Add("equipmentId", equipmentId);

        StartCoroutine(HttpRequest(rp, data, CMD.AppraisalEquipment));
    }

    /// <summary>
    /// 経験値追加
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="exp"></param>
    public void AddExp(Action<JsonData> rp, int exp)
    {
        var data = new NWData();
        data.Add("exp", exp);

        StartCoroutine(HttpRequest(rp, data, CMD.AddExp));
    }

    /// <summary>
    /// 冒険エリアに入る場合
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="exp"></param>
    public void ArriveFloor(Action<JsonData> rp, int arrivedFloor)
    {
        var data = new NWData();
        data.Add("arrivedFloor", arrivedFloor);

        StartCoroutine(HttpRequest(rp, data, CMD.ArriveFloor));
    }

    /// <summary>
    /// 復活
    /// </summary>
    /// <param name="rp"></param>
    public void Resurrection(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.Resurrection));
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
    /// タスク完了
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="taskId"></param>
    public void MainTaskEnd(Action<JsonData> rp, int taskId)
    {
        var data = new NWData();
        data.Add("taskId", taskId);

        StartCoroutine(HttpRequest(rp, data, CMD.MainTaskEnd));
    }

    /// <summary>
    /// メインタスククリア数追加
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="count"></param>
    public void AddMainTaskClearCount(Action<JsonData> rp, int count)
    {
        var data = new NWData();
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.AddMainTaskClearCount));
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

    /// <summary>
    /// ログインボーナス情報をゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetLoginBonusInfo(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetLoginBonusInfo));
    }

    /// <summary>
    /// ログインボーナスをゲット
    /// </summary>
    /// <param name="rp"></param>
    public void GetLoginBonus(Action<JsonData> rp, int id, int step)
    {
        var data = new NWData();
        data.Add("id", id);
        data.Add("step", step);

        StartCoroutine(HttpRequest(rp, data, CMD.GetLoginBonus));
    }

    /// <summary>
    /// 設計図アップロード回数を取得
    /// </summary>
    /// <param name="rp"></param>
    public void GetTotalUploadBlueprintCount(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.GetTotalUploadBlueprintCount));
    }

    /// <summary>
    /// ホームデータをサーバーにセーブ
    /// </summary>
    /// <param name="rp"></param>
    /// <param name="homedata"></param>
    public void SaveHomeData(Action<JsonData> rp, string homedata)
    {
        if (DataMng.E.UserData == null)
            return;

        var data = new NWData();
        data.Add("homedata", homedata);

        StartCoroutine(HttpRequest(rp, data, CMD.SaveHomeData));
    }

    /// <summary>
    /// ホームデータをサーバーからロード
    /// </summary>
    /// <param name="rp"></param>
    public void LoadHomeData(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.LoadHomeData));
    }

    /// <summary>
    /// クライアントメッセージをサーバーに送る
    /// </summary>
    /// <param name="msg"></param>
    public void ShowClientLog(string msg)
    {
        var data = new NWData();
        data.Add("msg", msg);

        StartCoroutine(HttpRequest(null, data, CMD.ShowClientLog));
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
