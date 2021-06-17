using JsonConfigData;
using System;
using System.Collections;
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

    private IEnumerator HttpRequest(Action<string[]> rp, NWData data, CMD cmd)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("code", (int)cmd);
        wwwForm.AddField("data", data.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(PublicPar.LocalURL, wwwForm))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                Debug.LogError(www.error);
            else
            {
                string[] datas = www.downloadHandler.text.Split('^');
                if (datas[0] == "error")
                {
                    Debug.LogError(datas[1]);
                }
                else
                {
                    if(rp != null) rp(datas);
                }
            }
        }
    }

    public void Login(Action<string[]> rp, string id, string pw)
    {
        var data = new NWData();
        data.Add(id);
        data.Add(pw);

        StartCoroutine(HttpRequest(rp, data, CMD.Login));
    }
    public void CreateNewAccount(Action<string[]> rp)
    {
        var data = new NWData();

        StartCoroutine(HttpRequest(rp, data, CMD.CreateNewAccount));
    }


    public void GetItemList(Action<string[]> rp)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);

        StartCoroutine(HttpRequest(rp, data, CMD.ItemList));
    }
    public void AddItem(Action<string[]> rp, int itemId, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);
        data.Add(itemId);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.AddItem));
    }
    public void RemoveItemByGuid(Action<string[]> rp, int guid, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByGuid));
    }
    public void RemoveItem(Action<string[]> rp, int itemid, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);
        data.Add(itemid);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.RemoveItemByGuid));
    }
    public void EquitItem(Action<string[]> rp, int guid, int site)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);
        data.Add(guid);
        data.Add(site);

        StartCoroutine(HttpRequest(rp, data, CMD.EquitItem));
    }
    public void Craft(Action<string[]> rp, Craft craft, int count)
    {
        var data = new NWData();
        data.Add(DataMng.E.session);
        data.Add(DataMng.E.UserData.Account);
        data.Add(craft.ID);
        data.Add(count);

        StartCoroutine(HttpRequest(rp, data, CMD.Craft));
    }


    public enum CMD
    {
        CreateNewAccount = 100,
        Login = 101,

        ItemList = 1001,
        RemoveItemByGuid = 1002,
        RemoveItemByItemId = 1003,
        AddItem = 1004,
        EquitItem = 1005,
        Craft = 1006,
    }
}
