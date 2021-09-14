using UnityEngine;
using UnityEngine.UI;

public class EmailDetailsUI : UIBase
{
    RectTransform BG { get => FindChiled<RectTransform>("BG"); }
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Message { get => FindChiled<Text>("Message"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Transform ItemScrollView { get => FindChiled("Item Scroll View"); }
    Transform Parent { get => FindChiled("Content"); }

    public override void Init()
    {
        base.Init();
        EmailDetailsLG.E.Init(this);

        OKBtn.onClick.AddListener(()=>
        {
            UICtl.E.OpenUI<EmailUI>(UIType.Email);
            Close();
        });
    }

    public void Set(EmailCell cell)
    {
        BG.sizeDelta = string.IsNullOrEmpty(cell.Data.related_data) ? new Vector2(500, 310) : new Vector2(500, 400);

        Title.SetTitle("メッセージ");
        Title.SetOnClose(Close);

        Message.text = cell.Data.message;
        Time.text = cell.Data.created_at.ToString("yyyy/MM/dd");

        if (!cell.Data.IsAlreadyRead)
        {
            NWMng.E.ReadEmail((rp) => 
            {
                cell.Read();
                DataMng.E.RuntimeData.NewEmailCount--;
                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshRedPoint();
            }, cell.Data.id);
        }

        SetCell(cell);
        if (cell.Data.IsInObject && !cell.Data.IsAlreadyRead)
        {
            NWMng.E.ReceiveEmailItem((rp) =>
            {
                NWMng.E.GetItems(() =>
                {
                    CommonFunction.ShowHintBar(20);
                });
            }, cell.Data.id);
        }
    }
    private void SetCell(EmailCell cell)
    {
        ClearCell(Parent);

        if (!cell.Data.IsInObject)
        {
            ItemScrollView.gameObject.SetActive(false);
            return;
        }

        ItemScrollView.gameObject.SetActive(true);
        string[] data = cell.Data.related_data.Split('^');
        string[] itemIds = data[0].Split(',');
        string[] itemCount = data[1].Split(',');

        for (int i = 0; i < itemIds.Length; i++)
        {
            var cellItem = AddCell<EmailDetailsItem>("Prefabs/UI/EmailDetailsItem", Parent);
            if (cellItem != null)
            {
                cellItem.Set(itemIds[i], itemCount[i]);
            }
        }
    }
}
