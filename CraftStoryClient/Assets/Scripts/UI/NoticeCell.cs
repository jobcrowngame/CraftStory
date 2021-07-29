using UnityEngine.UI;
using LitJson;
using UnityEngine;

public class NoticeCell : UIBase
{
    Button OnClick { get => gameObject.GetComponent<Button>(); }
    Image Category { get => FindChiled<Image>("Category"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Image New { get => FindChiled<Image>("New"); }
    Text Title { get => FindChiled<Text>("Title"); }

    NoticeLG.NoticeData cellData;

    public void Set(NoticeLG.NoticeData data)
    {
        cellData = data;

        OnClick.onClick.AddListener(() => 
        {
            NWMng.E.GetNotice((rp) => 
            {
                cellData.text = (string)rp["text"];

                var ui = UICtl.E.OpenUI<NoticeDetailUI>(UIType.NoticeDetail, UIOpenType.BeforeClose);
                ui.Init(cellData);
            }, cellData.id);
        });

        Category.sprite = ReadResources<Sprite>(NoticeLG.E.GetCategoryPath((NoticeLG.CategoryType)data.category));
        New.sprite = ReadResources<Sprite>("Textures/icon_noimg");
        New.gameObject.SetActive(data.newflag == 1 ? true : false);
        Time.text = data.activedate.ToString("yyyy/MM/dd");
        Title.text = data.title;
    }
}