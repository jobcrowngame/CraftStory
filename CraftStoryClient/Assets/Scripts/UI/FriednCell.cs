using System;
using UnityEngine.UI;

public class FriednCell : UIBase
{
    Text NickName { get => FindChiled<Text>("NickName"); }
    Text LoginTime { get => FindChiled<Text>("LoginTime"); }

    FriendLG.FriednCell info;

    public void Set(FriendLG.FriednCell info)
    {
        this.info = info;
        NickName.text = info.nickname;

        var time = DateTime.Now - info.loginTime;
        LoginTime.text = time.Days > 0 ? time.Days + "日前" : time.Hours + "時間前";

        GetComponent<Button>().onClick.AddListener(() => 
        {
            var ui = UICtl.E.OpenUI<FriendDescriptionUI>(UIType.FriendDescription);
            if (ui != null)
            {
                ui.Set(info, FriendDescriptionLG.UIType.DeFollow);
            }
        });
    }
}