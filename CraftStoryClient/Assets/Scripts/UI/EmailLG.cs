using LitJson;
using System;
using System.Collections.Generic;

public class EmailLG : UILogicBase<EmailLG, EmailUI>
{
    public int SelectPage
    {
        get => selectPage;
        set
        {
            selectPage = value;
            UI.SetPageText(value.ToString());
        }
    }
    int selectPage = 1;

    public void OnClickLeftBtn()
    {
        if (SelectPage > 1)
        {
            SelectPage--;
            Refresh();
        }
    }
    public void OnClickRightBtn()
    {
        SelectPage++;
        Refresh();
    }

    public void Refresh()
    {
        NWMng.E.GetEmail((rp) =>
        {
            List<EmailRP> list = new List<EmailRP>();

            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                list = JsonMapper.ToObject<List<EmailRP>>(rp.ToJson());
            }

            UI.Refresh(list);
        }, SelectPage);
    }
}

public class EmailRP
{
    public int id { get; set; }
    public string title { get; set; }
    public string message { get; set; }
    public DateTime created_at { get; set; }
    public int is_already_read { get; set; }

    public bool IsAlreadyRead { get => is_already_read == 1; }
}
