using UnityEngine;
using UnityEngine.UI;

public class ShopItemCell : UIBase
{
    Text Name;
    Button BuyBtn;

    private void InitUI()
    {
        Name = FindChiled<Text>("Name");

        BuyBtn = FindChiled<Button>("BuyBtn");
        BuyBtn.onClick.AddListener(() => { Debug.Log("BuyBtn"); });
    }

}
