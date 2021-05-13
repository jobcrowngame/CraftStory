using Gs2.Unity.Gs2Showcase.Model;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    Transform itemGridRoot;
    Button CloseBtn;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        ShopLG.E.Init(this);

        InitUI();
    }

    public override void Open()
    {
        base.Open();

        ShopLG.E.GetShowcase("ShowcaseShop01");
    }
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
    }

    private void InitUI()
    {
        itemGridRoot = FindChiled("Content");

        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(() => { Close(); });
    }

    public void AddItems(EzShowcase showcase)
    {
        foreach (var item in showcase.DisplayItems)
        {
            AddItem(item);
        }
    }
    private void AddItem(EzDisplayItem item)
    {
        var cell =  AddCell<ShopItemCell>("Prefabs/UI/ShopItem", itemGridRoot);
        if (cell != null)
        {
            cell.Init(cell.gameObject);
            cell.Add(item);
        }
    }
}
