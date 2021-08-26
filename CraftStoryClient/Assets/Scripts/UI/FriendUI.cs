using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フレンド
/// </summary>
public class FriendUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    ToggleBtns toggles { get => FindChiled<ToggleBtns>("ToggleBtns"); }
    Transform cellParent { get => FindChiled("Content"); }
    Button SortBtn { get => FindChiled<Button>("SortBtn"); }
    Button SearchBtn { get => FindChiled<Button>("SearchBtn"); }
    Text Count { get => FindChiled<Text>("Count"); }

    public override void Init()
    {
        base.Init();
        FriendLG.E.Init(this);

        Title.SetTitle("フレンド");
        Title.SetOnClose(Close);
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);

        toggles.Init();
        toggles.SetBtnText(0, "フォロー");
        toggles.SetBtnText(1, "フォロワー");
        toggles.OnValueChangeAddListener((index) => 
        {
            FriendLG.E.Type = (FriendLG.UIType)index;
        });

        SortBtn.onClick.AddListener(() => 
        {
            FriendLG.E.Sort = FriendLG.E.Sort == FriendLG.SortType.Asc ? FriendLG.SortType.Des : FriendLG.SortType.Asc;
        });
        SearchBtn.onClick.AddListener(() => 
        {
            UICtl.E.OpenUI<FriendSearchUI>(UIType.FriendSearch);
        });

        toggles.SetValue(0);
    }

    /// <summary>
    /// サブを更新
    /// </summary>
    /// <param name="list">サブデータリスト</param>
    public void RefreshCell(List<FriendLG.FriednCell> list)
    {
        // ソート
        if (FriendLG.E.Sort == FriendLG.SortType.Asc)
        {
            list.Sort(delegate (FriendLG.FriednCell x, FriendLG.FriednCell y) {
                if (x.loginTime == null && y.loginTime == null) return 0;
                else if (x.loginTime == null) return 1;
                else if (y.loginTime == null) return -1;
                else return y.loginTime.CompareTo(x.loginTime);
            });
        }
        else
        {
            list.Sort(delegate (FriendLG.FriednCell x, FriendLG.FriednCell y) {
                if (x.loginTime == null && y.loginTime == null) return 0;
                else if (x.loginTime == null) return -1;
                else if (y.loginTime == null) return 1;
                else return x.loginTime.CompareTo(y.loginTime);
            });
        }

        // サブをクリア
        ClearCell(cellParent);

        // サブをインスタンス
        foreach (var item in list)
        {
            var cell = AddCell<FriednCell>("Prefabs/UI/FriendCell", cellParent);
            if (cell != null)
            {
                cell.Set(item);
            }
        }

        // フレンド数更新
        Count.text = list.Count + "/" + SettingMng.E.MaxFriendCount;
    }
}
