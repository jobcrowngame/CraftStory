using UnityEngine;
using UnityEngine.UI;

public class ChargeCell : UIBase
{
    Text Des;
    Text Price;
    Button BuyBtn;

    private void Awake()
    {
        Des = FindChiled<Text>("Des");
        Des.text = "";

        Price = FindChiled<Text>("Price");
        Price.text = "";

        BuyBtn = FindChiled<Button>("BuyBtn");
        BuyBtn.onClick.AddListener(ChargeLG.E.Buy);
    }
}
