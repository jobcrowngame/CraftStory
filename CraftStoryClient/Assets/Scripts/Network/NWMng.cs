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

    private IEnumerator HttpRequest(NWData data, CMD cmd, Action<string[]> rp)
    {
        WWWForm wwwForm = new WWWForm();
        wwwForm.AddField("code", (int)cmd);
        wwwForm.AddField("data", data.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(PublicPar.URL, wwwForm))
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
                    rp(datas);
                }
            }
        }
    }

    public void Login(string id, string pw, Action<string[]> rp)
    {
        var data = new NWData();
        data.Add(id);
        data.Add(pw);

        StartCoroutine(HttpRequest(data, CMD.Login, rp));
    }
    public void CreateNewAccount(Action<string[]> rp)
    {
        NWData data = new NWData();
        StartCoroutine(HttpRequest(data, CMD.CreateNewAccount, rp));
    }
    

    public enum CMD
    {
        CreateNewAccount = 100,
        Login = 101,
    }
}
