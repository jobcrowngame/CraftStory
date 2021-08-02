using JsonConfigData;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class NWMng : MonoBehaviour
{
    public static NWMng E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<NWMng>();

            return entity;
        }
    }
    private static NWMng entity;
    


    private string url;
    public string URL { get => url; set => url = value; }

    public IEnumerator InitCoroutine()
    {
        yield return null;
    }
    private IEnumerator ConnectIE(Action<JsonData> rp)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(PublicPar.DevelopURL))
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
    public IEnumerator HttpRequest(Action<JsonData> rp, NWData data, CMD cmd)
    {
        Logger.Log("Send:[CMD:{0}],[data]{1}", (int)cmd, data.ToString());
        string cryptData = string.IsNullOrEmpty(data.ToString())
            ? ""
            : CryptMng.E.EncryptString(data.ToString());

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
                try
                {
                    if (string.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        if (rp != null) 
                            rp("");
                    }
                    else
                    {
                        var resultJson = CryptMng.E.DecryptString(www.downloadHandler.text);
                        JsonData jd = JsonMapper.ToObject(resultJson);
                        Logger.Log("Result:[CMD:{0}],[data]{1}", (int)cmd, jd.ToJson());

                        int errorCode = (int)jd["error"];

                        if (errorCode > 0)
                        {
                            if (errorCode == 998)
                                CommonFunction.ShowHintBox(PublicPar.Maintenance, () => { Logger.Warning("Quit"); Application.Quit(); });
                            else
                                CommonFunction.ShowHintBar(errorCode);
                        }
                        else
                        {
                            if (rp != null)
                                rp(jd["result"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("[CMD:{0}]{1}", (int)cmd, www.downloadHandler.text);
                    Logger.Error(ex);
                }
            }
        }
    }

    public void Connect(Action<JsonData> rp)
    {
        StartCoroutine(ConnectIE(rp));
    }
    public void GetVersion(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.Version));
    }
    public void CreateNewAccount(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.CreateNewAccount));
    }
    public void Login(Action<JsonData> rp, string id, string pw)
    {
        var data = new NWData();
        data.Add("acc", id);
        data.Add("pw", pw);

        StartCoroutine(HttpRequest(rp, data, CMD.Login));
    }
    public void GetItemList(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.ItemList));
    }
    public void AddItem(Action<JsonData> rp, int itemId, int count)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("itemId", itemId);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItem));
    }
    public void AddItemInData(Action<JsonData> rp, int itemId, int count, string newName, string rdata)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("itemId",itemId);
        data.Add("count", count);
        data.Add("newName",newName);
        data.Add("rdata",rdata);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItemInData));
    }
    public void AddItems(Action<JsonData> rp, Dictionary<int, int> items)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        List<NWItemData> list = new List<NWItemData>();
        foreach (var item in items)
        {
            list.Add(new NWItemData() { itemId = item.Key, count = item.Value });
        }
        data.Add("items", JsonMapper.ToJson(list));

        StartCoroutine(HttpRequest(rp, data, CMD.AddItems));
    }
    public void RemoveItemByGuid(Action<JsonData> rp, int guid, int count)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", guid);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByGuid));
    }
    public void RemoveItem(Action<JsonData> rp, int itemid, int count)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("itemId", itemid);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByItemId));
    }
    public void EquitItem(Action<JsonData> rp, int guid, int site)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", guid);
        data.Add("site", site);

        StartCoroutine(HttpRequest(rp, data, CMD.EquitItem));
    }
    public void Craft(Action<JsonData> rp, Craft craft, int count)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("creaftId",craft.ID);
        data.Add("count", count);

        StartCoroutine(HttpRequest(rp, data, CMD.Craft));
    }
    public void Buy(Action<JsonData> rp, int shopId)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("shopId", shopId);

        StartCoroutine(HttpRequest(rp, data, CMD.Buy));
    }
    public void GetCoins(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.GetCoins));
    }
    public void Charge(Action<JsonData> rp, string productId, string transactionID)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("productId", productId);
        data.Add("transactionId", transactionID);

        StartCoroutine(HttpRequest(rp, data, CMD.Charge));
    }
    public void GetBonus(Action<JsonData> rp, int bonusId)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("bonusId", bonusId);
        StartCoroutine(HttpRequest(rp, data, CMD.GetBonus));
    }
    public void ClearAdventure(Action<JsonData> rp, List<int> resources)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

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
    public void LevelUpMyShop(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.LevelUpMyShop));
    }
    public void UploadBlueprintToMyShop(Action<JsonData> rp, int itemGuid, int site, int price)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("nickName", DataMng.E.UserData.NickName);
        data.Add("itemGuid", itemGuid);
        data.Add("site", site);
        data.Add("price", price);

        StartCoroutine(HttpRequest(rp, data, CMD.UploadBlueprintToMyShop));
    }
    public void UpdateNickName(Action<JsonData> rp, string nickName)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("nickName", nickName);

        StartCoroutine(HttpRequest(rp, data, CMD.UpdateNickName));
    }
    public void SearchMyShopItems(Action<JsonData> rp, int page, string nickName, int sortType)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("page", page);
        data.Add("nickName", nickName);
        data.Add("sortType", sortType);

        StartCoroutine(HttpRequest(rp, data, CMD.SearchMyShopItems));
    }
    public void BuyMyShopItem(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.BuyMyShopItem));
    }
    public void LoadBlueprint(Action<JsonData> rp, int site, int isfree)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("site", site);
        data.Add("isfree", isfree);

        StartCoroutine(HttpRequest(rp, data, CMD.LoadBlueprint));
    }
    public void GetEmail(Action<JsonData> rp, int page)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("page", page);

        StartCoroutine(HttpRequest(rp, data, CMD.GetEmail));
    }
    public void ReadEmail(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.ReadEmail));
    }
    public void GetNewEmailCount(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.GetNewEmailCount));
    }
    public void GetRandomBonus(Action<JsonData> rp, int randomBonusId)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("randomBonusId", randomBonusId);

        StartCoroutine(HttpRequest(rp, data, CMD.GetRandomBonus));
    }
    public void GetNoticeList(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.GetNoticeList));
    }
    public void GetNotice(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", guid);

        StartCoroutine(HttpRequest(rp, data, CMD.GetNotice));
    }
    public void GetMyShopInfo(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.GetMyShopInfo));
    }
    public void ReceiveEmailItem(Action<JsonData> rp, int emailGuid)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("guid", emailGuid);

        StartCoroutine(HttpRequest(rp, data, CMD.ReceiveEmailItem));
    }

    public void SaveHomeData(Action<JsonData> rp, string homedata)
    {
        if (DataMng.E.UserData == null)
            return;

        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);
        data.Add("homedata", homedata);

        StartCoroutine(HttpRequest(rp, data, CMD.SaveHomeData));
    }
    public void LoadHomeData(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add("token", DataMng.E.token);
        data.Add("acc", DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.LoadHomeData));
    }


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
        RemoveItemByGuid,
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
