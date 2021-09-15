using JsonConfigData;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionChatUI : UIBase
{
    Transform Image1 { get => FindChiled("Image (1)"); }
    Text Chat1 { get => FindChiled<Text>("Chat1"); }
    Transform Image2 { get => FindChiled("Image (2)"); }
    Text Chat2 { get => FindChiled<Text>("Chat2"); }
    Button OnClick { get => FindChiled<Button>("OnClickBG"); }

    Mission config;

    public override void Init(object missionId)
    {
        base.Init(missionId);
        MissionChatLG.E.Init(this);

        OnClick.onClick.AddListener(Close);
    }

    public override void Open(object missionId)
    {
        base.Open(missionId);
        config = ConfigMng.E.Mission[(int)missionId];

        Image1.gameObject.SetActive(false);
        Image2.gameObject.SetActive(false);
        OnClick.gameObject.SetActive(false);
        Chat1.text = "";
        Chat2.text = "";

        StartCoroutine(StartChat());
    }

    public override void Close()
    {
        base.Close();

        StopCoroutine(StartChat());

        UICtl.E.DeleteUI(UIType.MissionChat);
    }

    private IEnumerator StartChat()
    {
        var chat1 = config.Chat1.ToCharArray();
        var chat2 = config.Chat2.ToCharArray();
        yield return null;

        Image1.gameObject.SetActive(true);
        foreach (var item in chat1)
        {
            Chat1.text += item;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1);

        Image2.gameObject.SetActive(true);
        foreach (var item in chat2)
        {
            Chat2.text += item;
            yield return new WaitForSeconds(0.05f);
        }

        OnClick.gameObject.SetActive(true);
    }
}
