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
            UI.ToggleBtns.SetValue(0);
            UI.SetCell(0);
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

    public struct NoticeData
    {
        public int id { get; set; }
        public int category { get; set; }
        public int newflag { get; set; }
        public DateTime activedate { get; set; }
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