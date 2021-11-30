using System;
using System.Collections.Generic;
using LitJson;

public class NoticeLG : UILogicBase<NoticeLG, NoticeUI>
{
    public bool IsFirst { get; set; }
    public List<NoticeLG.NoticeData> NoticeList { get; private set; }

    public void GetNoticeList()
    {
        NWMng.E.GetNoticeList((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                return;
            }

            NoticeList = JsonMapper.ToObject<List<NoticeData>>(rp.ToJson());
           
            if (IsFirst)
            {
                // 強制表示設定あり かつ 当日中に「今日は表示しない」のチェックをつけていない場合、
                // 初回表示で強制表示対象のお知らせ詳細を表示
                DateTime nowDate = DateTime.Now.Date;
                List<NoticeData> pickupList = new List<NoticeData>();
                Dictionary<int, DateTime> map = DataMng.E.UserData.PickupNoticeCheckMap;
                foreach (NoticeData nd in NoticeList)
                {
                    if (nd.pickup != null && IsPickupNoticeDisplay(map, nd.id, nowDate)) 
                    {
                        pickupList.Add(nd);
                    }
                }
                if (pickupList.Count > 0)
                {
                    // 強制表示順序(昇順)と最新時間(降順)によるソート
                    pickupList.Sort(delegate (NoticeLG.NoticeData x, NoticeLG.NoticeData y) {
                        if (((int)(x.pickup)).CompareTo((int)y.pickup) != 0) return ((int)(x.pickup)).CompareTo((int)y.pickup);
                        if (x.activedate == null && y.activedate == null) return 0;
                        else if (x.activedate == null) return 1;
                        else if (y.activedate == null) return -1;
                        else return y.activedate.CompareTo(x.activedate);
                    });

                    var ui = UICtl.E.OpenUI<NoticeDetailUI>(UIType.NoticeDetail);
                    ui.SetPickup(pickupList);
                }

                IsFirst = false;
            }
            else
            {
                UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
            }
        });
    }

    public string GetCategoryPath(CategoryType type)
    {
        switch (type)
        {
            case CategoryType.notice: return "Textures/infomation_2d_015";
            case CategoryType.important: return "Textures/infomation_2d_007";
            case CategoryType.gameEvent: return "Textures/infomation_2d_008";
            default: return "";
        }
    }

    private bool IsPickupNoticeDisplay(Dictionary<int, DateTime> map, int id, DateTime nowDate)
    {
        return !map.ContainsKey(id) || map[id] < nowDate;
    }

    public struct NoticeData
    {
        public int id { get; set; }
        public int category { get; set; }
        public int newflag { get; set; }
        public DateTime activedate { get; set; }
        public int? priority { get; set; }
        public int? pickup { get; set; }
        public string title { get; set; }
        public string titleIcon { get; set; }
        public string detailIcon { get; set; }
        public string url { get; set; }
        public string text { get; set; }
    }

    public enum CategoryType
    {
        /// <summary>
        /// お知らせ
        /// </summary>
        notice = 1,

        /// <summary>
        /// 重要
        /// </summary>
        important,

        /// <summary>
        /// イベント
        /// </summary>
        gameEvent,
    }
}