using LitJson;
using System;
using System.Collections.Generic;

public class EmailLG : UILogicBase<EmailLG, EmailUI>
{
    public int MaxCount { get; private set; }

    private int OnePageCount = 7;

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
            var result = JsonMapper.ToObject<EmailRP>(rp.ToJson());
            MaxCount = result.maxCount;

            if (MaxCount <= OnePageCount)
            {
                UI.ActiveLeftBtn(false);
                UI.ActiveRightBtn(false);
            }
            else if (SelectPage == 1)
            {
                UI.ActiveLeftBtn(false);
                UI.ActiveRightBtn();
            }
            else if (SelectPage * OnePageCount >= MaxCount)
            {
                UI.ActiveLeftBtn();
                UI.ActiveRightBtn(false);
            }
            else
            {
                UI.ActiveLeftBtn();
                UI.ActiveRightBtn();
            }

            UI.Refresh(result.data);
        }, SelectPage);
    }

    public struct EmailRP
    {
        public int maxCount { get; set; }
        public List<EmailCellRP> data { get; set; }
    }

    public class EmailCellRP
    {
        public int id { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public DateTime created_at { get; set; }
        public int is_already_read { get; set; }
        public string related_data { get; set; }
        public int is_already_received { get; set; }

        public bool IsAlreadyRead { get => is_already_read == 1; }
        public bool Is_already_received { get => is_already_received == 1; }
        public bool IsInObject { get => !string.IsNullOrEmpty(related_data); }
    }
}