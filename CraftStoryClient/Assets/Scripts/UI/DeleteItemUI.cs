using UnityEngine;
using UnityEngine.UI;

public class DeleteItemUI : UIBase
{
    Transform itemGridRoot { get => FindChiled("Content"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }

    public override void Init()
    {
        base.Init();
        DeleteItemLG.E.Init(this);

        OKBtn.onClick.AddListener(DeleteItemLG.E.Delete);
        CancelBtn.onClick.AddListener(Close);
    }

    public override void Open()
    {
        base.Open();
        RefreshItems();
    }
    public override void Close()
    {
        base.Close();
        ClearCell(itemGridRoot);
        DeleteItemLG.E.Clear();
    }

    public void RefreshItems()
    {
        ClearCell(itemGridRoot);

        if (DataMng.E.Items == null)
            return;

        foreach (var item in DataMng.E.Items)
        {
            if (item.Config().Type == (int)ItemType.Blueprint)
            {
                AddItem(item);
            }
        }
    }
    private void AddItem(ItemData item)
    {
        var cell = AddCell<DeleteItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Init(item);
        }
    }
}
