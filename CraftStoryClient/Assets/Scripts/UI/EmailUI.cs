using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailUI : UIBase
{
    TitleUI title { get => FindChiled<TitleUI>("Title"); }
    Transform cellParent { get => FindChiled("Content"); }
    Text Page { get => FindChiled<Text>("Page"); }
    Button LeftBtn { get => FindChiled<Button>("LeftBtn"); }
    Button RightBtn { get => FindChiled<Button>("RightBtn"); }

    public override void Init()
    {
        base.Init();
        EmailLG.E.Init(this);

        title.SetTitle("メッセージ");
        title.SetOnClose(() => { Close(); });

        LeftBtn.onClick.AddListener(EmailLG.E.OnClickLeftBtn);
        RightBtn.onClick.AddListener(EmailLG.E.OnClickRightBtn);
        EmailLG.E.SelectPage = 1;
    }

    public override void Open()
    {
        base.Open();

        //EmailLG.E.SelectPage = 1;
        EmailLG.E.Refresh();
    }

    public void Refresh(List<EmailRP> datas)
    {
        ClearCell(cellParent);
        foreach (var item in datas)
        {
            var cell = AddCell<EmailCell>("Prefabs/UI/EmailCell", cellParent);
            cell.Set(item);
        }
    }

    public void SetPageText(string msg)
    {
        Page.text = msg;
    }
}
