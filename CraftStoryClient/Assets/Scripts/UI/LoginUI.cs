﻿using UnityEngine;
using UnityEngine.UI;

using System.Text;

public class LoginUI : UIBase
{
    Image Start;
    Text Ver;
    Button BGBtn;
    Button Terms01Btn;
    Button Terms02Btn;

    private void Awake()
    {
        LoginLg.E.Init(this);

        Start = FindChiled<Image>("Start");

        Ver = FindChiled<Text>("Ver");
        Ver.text = "Ver:1.0.0";

        BGBtn = FindChiled<Button>("BG");
        DataMng.E.MapData.TransferGate = new EntityData(100, ItemType.TransferGate);
        BGBtn.onClick.AddListener(CommonFunction.GoToNextScene);
        BGBtn.enabled = false;

        Terms01Btn = FindChiled<Button>("Terms01Btn");
        Terms01Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms01UI>(UIType.Terms01); });

        Terms02Btn = FindChiled<Button>("Terms02Btn");
        Terms02Btn.onClick.AddListener(() => { UICtl.E.OpenUI<Terms02UI>(UIType.Terms02); });
    }

    public void LoginResponse()
    {
        Debug.Log("ログイン成功しました。");
        Debug.LogFormat("ようこそ、{0}様", DataMng.E.UserData.Account);

        Start.sprite = ReadResources<Sprite>("Textures/icon_gamestart");

        BGBtn.enabled = true;
    }
}
