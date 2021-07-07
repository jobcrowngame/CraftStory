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

    public IEnumerator InitCoroutine()
    {
        yield return null;
    }
    private IEnumerator HttpRequest(Action<JsonData> rp, NWData data, CMD cmd)
    {
        Logger.Log("Send:[CMD:{0}],[data]{1}", (int)cmd, data.ToString());
        string cryptData = string.IsNullOrEmpty(data.ToString())
            ? ""
            : CryptMng.E.EncryptString(data.ToString());

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("code", (int)cmd);
        wwwForm.AddField("data", cryptData, System.Text.Encoding.UTF8);

        using (UnityWebRequest www = UnityWebRequest.Post(PublicPar.URL, wwwForm))
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
                        rp("");
                    }
                    else
                    {
                        var resultJson = CryptMng.E.DecryptString(www.downloadHandler.text);
                        JsonData jd = JsonMapper.ToObject(resultJson);
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
                    Logger.Error(ex.Message);
                }
            }
        }
    }

    public void CreateNewAccount(Action<JsonData> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.CreateNewAccount));
    }
    public void Login(Action<JsonData> rp, string id, string pw)
    {
        var data = new NWData();
        data.Add("N");
        data.Add(id);
        data.Add(pw);

        StartCoroutine(HttpRequest(rp, data, CMD.Login));
    }
    public void GetItemList(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.ItemList));
    }
    public void AddItem(Action<JsonData> rp, int itemId, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(itemId);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItem));
    }
    public void AddItemInData(Action<JsonData> rp, int itemId, int count, string newName, string rdata)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(itemId);
        data.Add(count);
        data.Add(newName);
        data.Add(rdata);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItemInData));
    }
    public void AddItems(Action<JsonData> rp, Dictionary<int, int> items)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);

        List<NWItemData> list = new List<NWItemData>();
        foreach (var item in items)
        {
            list.Add(new NWItemData() { itemId = item.Key, count = item.Value });
        }
        data.Add(JsonMapper.ToJson(list));

        StartCoroutine(HttpRequest(rp, data, CMD.AddItems));
    }
    public void RemoveItemByGuid(Action<JsonData> rp, int guid, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByGuid));
    }
    public void RemoveItem(Action<JsonData> rp, int itemid, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(itemid);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByItemId));
    }
    public void EquitItem(Action<JsonData> rp, int guid, int site)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);
        data.Add(site);

        StartCoroutine(HttpRequest(rp, data, CMD.EquitItem));
    }
    public void Craft(Action<JsonData> rp, Craft craft, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(craft.ID);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.Craft));
    }
    public void Buy(Action<JsonData> rp, int shopId)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(shopId);

        StartCoroutine(HttpRequest(rp, data, CMD.Buy));
    }
    public void GetCoins(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.GetCoins));
    }
    public void Charge(Action<JsonData> rp, string productId, string transactionID)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(productId);
        data.Add(transactionID);

        StartCoroutine(HttpRequest(rp, data, CMD.Charge));
    }
    public void GetBonus(Action<JsonData> rp, int bonusId)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(bonusId);
        StartCoroutine(HttpRequest(rp, data, CMD.GetBonus));
    }
    public void ClearAdventure(Action<JsonData> rp, List<int> resources)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);

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
        data.Add(msg);

        StartCoroutine(HttpRequest(rp, data, CMD.ClearAdventure));
    }
    public void LevelUpMyShop(Action<JsonData> rp)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.LevelUpMyShop));
    }
    public void UploadBlueprintToMyShop(Action<JsonData> rp, int itemGuid, int site, int price)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(DataMng.E.UserData.NickName);
        data.Add(itemGuid);
        data.Add(site);
        data.Add(price);

        StartCoroutine(HttpRequest(rp, data, CMD.UploadBlueprintToMyShop));
    }
    public void UpdateNickName(Action<JsonData> rp, string nickName)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(nickName);

        StartCoroutine(HttpRequest(rp, data, CMD.UpdateNickName));
    }
    public void SearchMyShopItems(Action<JsonData> rp, int page, string nickName)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(page);
        data.Add(nickName);

        StartCoroutine(HttpRequest(rp, data, CMD.SearchMyShopItems));
    }
    public void BuyMyShopItem(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);

        StartCoroutine(HttpRequest(rp, data, CMD.BuyMyShopItem));
    }
    public void LoadBlueprint(Action<JsonData> rp, int guid)
    {
        var data = new NWData();
        data.Add(DataMng.E.token);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);

        StartCoroutine(HttpRequest(rp, data, CMD.LoadBlueprint));
    }


    public enum CMD
    {
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

        Charge = 9000,
    }

    struct NWItemData
    {
        public int itemId;
        public int count;
    }
}
