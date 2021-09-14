using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendSearchUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text UserId { get => FindChiled<Text>("UserId"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    InputField Input { get => FindChiled<InputField>("InputField"); }
    public override void Init()
    {
        base.Init();
        FriendSearchLG.E.Init(this);

        Title.SetTitle("フレンド検索");
        Title.SetOnClose(Close);

        UserId.text = "あなたのアカウント: " + DataMng.E.UserData.Account;

        CancelBtn.onClick.AddListener(Close);
        OkBtn.onClick.AddListener(() => 
        {
            FriendSearchLG.E.FriendSearch(Input.text);
            Close();
        });
    }
}
