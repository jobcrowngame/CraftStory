using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    Transform parent { get => FindChiled("Content"); }
    public MyToggleGroupCtl ToggleBtns { get => FindChiled<MyToggleGroupCtl>("ToggleBtns"); }
    ScrollRect ScrollRect { get => FindChiled<ScrollRect>("Scroll View"); }

    public override void Init()
    {
        base.Init();

        NoticeLG.E.Init(this);

        title.SetTitle("お知らせ");
        title.SetOnClose(() => { Close(); });

        ToggleBtns.Init();
        ToggleBtns.OnValueChangeAddListener((index) => 
        { 
            SetCell(index);
        });
    }

    public override void Open()
    {
        base.Open();
        NoticeLG.E.GetNoticeList();
    }

    public override void Close()
    {
        base.Close();

        // 今日の始めのログインの場合、ログインボーナス画面を出す
        if (DataMng.E.RuntimeData.FirstLoginDaily == 1)
        {
            LoginBonusLG.E.GetInfo();
        }
    }

    public void MoveToTop()
    {
        ScrollRect.verticalNormalizedPosition = 1;
    }

    public void SetCell(int index)
    {
        ClearCell(parent);

        var list = NoticeLG.E.NoticeList;

        if (list == null || list.Count == 0)
            return;

        foreach (var item in list)
        {
            if (index == (int)NoticeFilterTab.Important && item.category != (int)Category.Important) continue;
            if (index == (int)NoticeFilterTab.Event && item.category != (int)Category.Event) continue;
            if (index == (int)NoticeFilterTab.Other && item.category != (int)Category.Notice) continue;

            var cell = AddCell<NoticeCell>("Prefabs/UI/NoticeCell", parent);
            if (cell != null)
            {
                cell.Set(item);
            }
        }

    }

    enum Category
    {
        Notice = 1,
        Important,
        Event
    }

    enum NoticeFilterTab
    {
        All,
        Important,
        Event,
        Other
    }


}
