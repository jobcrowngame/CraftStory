using System.Collections.Generic;

public class DeleteItemLG : UILogicBase<DeleteItemLG, DeleteItemUI>
{
    private List<ItemData.DeleteItemData> list = new List<ItemData.DeleteItemData>();

    public void Add(ItemData item)
    {
        list.Add(new ItemData.DeleteItemData() { guid = item.id });
    }
    public void Remove(ItemData item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (item.id == list[i].guid)
            {
                list.RemoveAt(i);
                break;
            }
        }
    }
    public void Delete()
    {
        if (list.Count > 0)
        {
            NWMng.E.DeleteItems((rp) => 
            {
                foreach (var item in list)
                {
                    DataMng.E.RemoveItemByGuid(item.guid, 1);
                }

                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
                if (BagLG.E.UI != null) BagLG.E.UI.RefreshItems();
                if (BagLG.E.UI != null) BagLG.E.UI.RefreshSelectItemBtns();
                UI.Close();
            }, list);
        }
        else {
            CommonFunction.ShowHintBar(21);
        }
    }
    public void Clear()
    {
        list.Clear();
    }
}