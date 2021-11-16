using JsonConfigData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : UIBase
{
    Text Chat { get => FindChiled<Text>("Chat"); }
    Image CharacterIcon { get => FindChiled<Image>("CharacterIcon"); }
    Image NameIcon { get => FindChiled<Image>("NameIcon"); }
    Button OnClick { get => FindChiled<Button>("OnClickBG"); }

    int chatId = 1;
    Chat config { get => ConfigMng.E.Chat[chatId]; }
    Action callback;

    public override void Init(object obj)
    {
        base.Init(obj);

        OnClick.onClick.AddListener(() =>
        {
            if (callback != null) callback();
            Close();
        });
    }

    public override void Open(object obj)
    {
        base.Open(obj);

        chatId = (int)obj;

        Chat.text = "";

        if (config.CharacterIcon == "N")
        {
            CharacterIcon.gameObject.SetActive(false);
        }
        else
        {
            CharacterIcon.gameObject.SetActive(true);
            CharacterIcon.sprite = ReadResources<Sprite>(config.CharacterIcon);
        }

        NameIcon.sprite = ReadResources<Sprite>(config.NameIcon);
        OnClick.gameObject.SetActive(false);

        StartCoroutine(StartChat());
    }

    public override void Close()
    {
        base.Close();

        callback = null;

        StopCoroutine(StartChat());
    }

    public void AddListenerOnClose(Action action)
    {
        callback = action;
    }

    private IEnumerator StartChat()
    {
        var chat = config.Text;

        yield return new WaitForSeconds(0.5f);

        foreach (var item in chat)
        {
            Chat.text += item;
            yield return new WaitForSeconds(0.05f);
        }

        OnClick.gameObject.SetActive(true);
    }
}
