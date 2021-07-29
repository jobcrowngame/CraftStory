using UnityEngine;
using UnityEngine.UI;

public class NoticeDetailUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    Image Category { get => FindChiled<Image>("Category"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Text NoticeTitle { get => FindChiled<Text>("NoticeTitle"); }
    Text Dec { get => FindChiled<Text>("Dec"); }
    Button NoticeListBtn { get => FindChiled<Button>("NoticeListBtn"); }

    private void Awake()
    {
        NoticeListBtn.onClick.AddListener(() => { UICtl.E.OpenUI<NoticeUI>(UIType.Notice); Close(); });
    }

    public override void Init(object data)
    {
        base.Init(data);

        NoticeDetailLG.E.Init(this);
        var uiinfo = (NoticeLG.NoticeData)data;

        title.SetTitle("お知らせ");
        title.SetOnClose(() => { Close(); });
        title.EnActiveCoin(1);
        title.EnActiveCoin(2);
        title.EnActiveCoin(3);

        Category.sprite = ReadResources<Sprite>(NoticeLG.E.GetCategoryPath((NoticeLG.CategoryType)uiinfo.category));
        Time.text = uiinfo.activedate.ToString("yyyy/MM/dd");
        NoticeTitle.text = uiinfo.title;
        Dec.text = uiinfo.text;
    }
}
