using UnityEngine.UI;
using LitJson;
using UnityEngine;

public class NoticeCell : UIBase
{
    Button OnClick { get => gameObject.GetComponent<Button>(); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Image Category { get => FindChiled<Image>("Category"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Image New { get => FindChiled<Image>("New"); }
    Text Title { get => FindChiled<Text>("Title"); }

    NoticeLG.NoticeData cellData;

    public void Set(NoticeLG.NoticeData data, bool clickable = true)
    {
        cellData = data;

        if (clickable)
        {
            OnClick.onClick.AddListener(() =>
            {
                NWMng.E.GetNotice((rp) =>
                {
                    cellData.text = (string)rp["text"];

                    var ui = UICtl.E.OpenUI<NoticeDetailUI>(UIType.NoticeDetail, UIOpenType.BeforeClose);
                    ui.Init(cellData);

                }, cellData.id);
            });
        }
        Icon.sprite = ReadResources<Sprite>("Textures/infomation_2d_005");
        if (!string.IsNullOrEmpty(data.titleIcon))
        {
            AWSS3Mng.E.DownLoadTexture2D(Icon, data.titleIcon);
        }
        Category.sprite = ReadResources<Sprite>(NoticeLG.E.GetCategoryPath((NoticeLG.CategoryType)data.category));
        New.gameObject.SetActive(data.newflag == 1 ? true : false);
        Time.text = data.activedate.ToString("yyyy/MM/dd");
        Title.text = data.title;
    }
}