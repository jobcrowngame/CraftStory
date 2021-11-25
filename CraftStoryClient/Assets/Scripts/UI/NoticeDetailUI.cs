using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeDetailUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    NoticeCell noticeCell { get => FindChiled<NoticeCell>("NoticeCell"); }
    Image URLImage { get => FindChiled<Image>("URLImage"); }
    Image URLImageMsg { get => FindChiled<Image>("URLImageMsg"); }
    Button URLImageBtn { get => FindChiled<Button>("URLImage"); }
    Text Dec { get => FindChiled<Text>("Dec"); }
    Button NoticeListBtn { get => FindChiled<Button>("NoticeListBtn"); }
    ScrollRect ScrollRect { get => FindChiled<ScrollRect>("Scroll View"); }
    Toggle HideToday { get => FindChiled<Toggle>("HideToday"); }

    int PickupIdIdx;

    private void Awake()
    {
        NoticeListBtn.onClick.AddListener(() => { UICtl.E.OpenUI<NoticeUI>(UIType.Notice); Close(); });
    }

    public override void Open()
    {
        base.Open();
        ScrollRect.verticalNormalizedPosition = 1;

        PickupIdIdx = -1;
        title.SetOnClose(() => { Close(); });
        NoticeListBtn.gameObject.SetActive(true);
        HideToday.gameObject.SetActive(false);
    }

    public void SetPickup(List<NoticeLG.NoticeData> dataList)
    {
        PickupIdIdx = 0;
        NoticeListBtn.gameObject.SetActive(false);
        HideToday.gameObject.SetActive(true);
        HideToday.isOn = false;
        GetDataOnPickup(dataList[PickupIdIdx]);
        title.SetOnClose(() => 
        {
            if(HideToday.isOn)
            {
                DataMng.E.UserData.PickupNoticeCheckMap[dataList[PickupIdIdx].id] = DateTime.Now.Date;
            }

            if(PickupIdIdx >= dataList.Count - 1)
            {
                UICtl.E.OpenUI<NoticeUI>(UIType.Notice);
                Close();
            }
            else
            {
                GetDataOnPickup(dataList[++PickupIdIdx]);
                ScrollRect.verticalNormalizedPosition = 1;
                HideToday.isOn = false;
            }
        });
    }

    public override void Init(object data)
    {
        base.Init(data);

        NoticeDetailLG.E.Init(this);
        var uiinfo = (NoticeLG.NoticeData)data;

        title.SetTitle("お知らせ");

        noticeCell.Set((NoticeLG.NoticeData)data, false);

        URLImageBtn.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(uiinfo.url))
        {
            URLImageMsg.gameObject.SetActive(true);
        }
        else
        {
            URLImageMsg.gameObject.SetActive(false);
        }
        if (!string.IsNullOrEmpty(uiinfo.detailIcon))
        {
            if (!string.IsNullOrEmpty(uiinfo.url))
            {
                URLImageBtn.onClick.AddListener(() =>
                {
                    Application.OpenURL(uiinfo.url);
                });
            }
            URLImage.sprite = ReadResources<Sprite>("Textures/infomation_2d_013");
            AWSS3Mng.E.DownLoadTexture2D(URLImage, uiinfo.detailIcon, () =>
            {
                URLImageMsg.gameObject.SetActive(false);
            });
        }
        else
        {
            URLImage.sprite = null;
        }

        Dec.text = uiinfo.text;
    }

    private void GetDataOnPickup(NoticeLG.NoticeData cellData)
    {
        NWMng.E.GetNotice((rp) =>
        {
            cellData.text = (string)rp["text"];
            Init(cellData);

        }, cellData.id);
    }
}