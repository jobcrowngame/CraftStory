using UnityEngine;

public class ShopLG : UILogicBase<ShopLG, ShopUI>
{
    public ShopUiType ShopUIType
    {
        get => shopUIType;
        set
        {
            if (shopUIType == value)
                return;

            shopUIType = value;

            UI.IsChargeWind(shopUIType == ShopUiType.Charge);
            UI.SetTitle2(GetTitle2Text());
            UI.RefreshItems(shopUIType);
        }
    }
    private ShopUiType shopUIType;

    private string GetTitle2Text()
    {
        switch (ShopUIType)
        {
            case ShopUiType.Charge: return "クラフトシード";
            case ShopUiType.Exchange: return "交換";

            default: Debug.LogError("not find shop ui type " + ShopUIType); break;
        }

        return "";
    }
}

public enum ShopUiType
{
    Charge = 1,
    Exchange,
}