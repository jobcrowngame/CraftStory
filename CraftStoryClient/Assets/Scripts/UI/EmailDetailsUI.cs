using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmailDetailsUI : UIBase
{
    Text Title { get => FindChiled<Text>("TitleText"); }
    Text Message { get => FindChiled<Text>("Message"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }

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
        Title.text = cell.Data.title;
        Message.text = cell.Data.message;
        Time.text = cell.Data.created_at.ToString();

        if (!cell.Data.IsAlreadyRead)
        {
            NWMng.E.ReadEmail((rp) => 
            {
                cell.Read();
            }, cell.Data.id);
        }
    }
}
