using System.Collections.Generic;
using UnityEngine;

public class NoticeUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    Transform parent { get => FindChiled("Content"); }

    public override void Init()
    {
        base.Init();

        NoticeLG.E.Init(this);

        title.SetTitle("お知らせ");
        title.SetOnClose(() => { Close(); });
    }

    public override void Open()
    {
        base.Open();
        NoticeLG.E.GetNoticeList();
    }

    public void SetCell(List<NoticeLG.NoticeData> list)
    {
        ClearCell(parent);

        if (list == null || list.Count == 0)
            return;

        // 最新時間によるソート
        list.Sort(delegate (NoticeLG.NoticeData x, NoticeLG.NoticeData y) {
            if (x.activedate == null && y.activedate == null) return 0;
            else if (x.activedate == null) return 1;
            else if (y.activedate == null) return -1;
            else return y.activedate.CompareTo(x.activedate);
        });

        foreach (var item in list)
        {
            var cell = AddCell<NoticeCell>("Prefabs/UI/NoticeCell", parent);
            if (cell != null)
            {
                cell.Set(item);
            }
        }
    }
}
