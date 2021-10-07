using System;
using System.Collections.Generic;
using LitJson;

public class NoticeLG : UILogicBase<NoticeLG, NoticeUI>
{
    public bool IsFirst { get; set; }

    public void GetNoticeList()
    {
        NWMng.E.GetNoticeList((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                UI.SetCell(null);
                return;
            }

            var list = JsonMapper.ToObject<List<NoticeData>>(rp.ToJson());
            ui.SetCell(list);
        });
    }

    public string GetCategoryPath(CategoryType type)
    {
        switch (type)
        {
            case CategoryType.notice: return "Textures/icon_noimg";
            case CategoryType.important: return "Textures/icon_noimg";
            case CategoryType.maintenance: return "Textures/icon_noimg";
            case CategoryType.notice2: return "Textures/icon_noimg";
            case CategoryType.gameEvent: return "Textures/icon_noimg";
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
        /// メンテナンス
        /// </summary>
        maintenance,

        /// <summary>
        /// 予告
        /// </summary>
        notice2,

        /// <summary>
        /// イベント
        /// </summary>
        gameEvent,
    }
}