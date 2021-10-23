
public class ShopChargeLG : UILogicBase<ShopChargeLG, ShopChargeUI>
{
    public UiType Type 
    { 
        get => mType;
        set
        {
            mType = value;

            UI.OnUITypeChange(value);
        }
    }
    private UiType mType;

    public int GetSubscriptionTypeByShopId(int shopId)
    {
        switch (shopId)
        {
            case 80: return 1;
            case 81: return 2;
            default: return 3;
        }
    }


    public enum UiType
    {
        /// <summary>
        /// 課金
        /// </summary>
        Charge,

        /// <summary>
        /// サブスクリプション
        /// </summary>
        Subscription,

        /// <summary>
        /// ポイント交換
        /// </summary>
        ExchangePoint,
    }
}


public enum ShopType
{
    Charge = 0,
    Subscription,
    Gacha,
    Point,
    Exchange,
    Blueprint,
    Blueprint2,
    CraftResource,
    EventItem,
}