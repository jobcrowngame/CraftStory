using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoticeDetailUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    NoticeCell noticeCell { get => FindChiled<NoticeCell>("NoticeCell"); }
    Image URLImage { get => FindChiled<Image>("URLImage"); }
    Button URLImageBtn { get => FindChiled<Button>("URLImage"); }
    Text Dec { get => FindChiled<Text>("Dec"); }
    Button NoticeListBtn { get => FindChiled<Button>("NoticeListBtn"); }
    ScrollRect ScrollRect { get => FindChiled<ScrollRect>("Scroll View"); }

    private void Awake()
    {
        NoticeListBtn.onClick.AddListener(() => { UICtl.E.OpenUI<NoticeUI>(UIType.Notice); Close(); });
    }

    public override void Open()
    {
        base.Open();
        ScrollRect.verticalNormalizedPosition = 1;
    }

    public override void Init(object data)
    {
        base.Init(data);

        NoticeDetailLG.E.Init(this);
        var uiinfo = (NoticeLG.NoticeData)data;

        title.SetTitle("お知らせ");
        title.SetOnClose(() => { Close(); });

        noticeCell.Set((NoticeLG.NoticeData)data, false);

        URLImageBtn.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(uiinfo.detailIcon))
        {
            URLImage.sprite = ReadResources<Sprite>("Textures/url_image_notice_detail_dummy");
            AWSS3Mng.E.DownLoadTexture2D(URLImage, uiinfo.detailIcon, () =>
            {
                if(!string.IsNullOrEmpty(uiinfo.url))
                {
                    URLImageBtn.onClick.AddListener(() => 
                    { 
                        Application.OpenURL(uiinfo.url);
                    });
                }
            }, 
            () =>
            {
                URLImage.sprite = null;
            });
        }
        else
        {
            URLImage.sprite = null;
        }

        Dec.text = uiinfo.text;
    }
}