using Gs2.Unity.Gs2Showcase.Model;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemCell : UIBase
{
    Text Name;
    Button BuyBtn;

    private EzDisplayItem item;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");

        BuyBtn = FindChiled<Button>("BuyBtn");
        BuyBtn.onClick.AddListener(() => { Debug.Log("BuyBtn"); });
    }

    public void Add(EzDisplayItem item)
    {
        this.item = item;

        InitUI();

        Name.text = item.SalesItem.Name;
    }
}
