
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