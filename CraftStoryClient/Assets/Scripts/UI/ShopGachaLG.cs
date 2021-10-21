using System.Collections.Generic;

public class ShopGachaLG : UILogicBase<ShopGachaLG, ShopGachaUI>
{
    public int Index
    {
        get => mIndex;
        set
        {
            mIndex = value;

            UI.RefreshGachaUI();
        }
    }
    private int mIndex = 0;

    public int GachaType
    {
        get => mGachaType;
        set
        {
            mIndex = 0;
            UI.ToggleBtns1.SetValue(0);

            mGachaType = value;

            UI.RefreshGachaUI();
        }
    }
    private int mGachaType = 0;

    public struct GachaResponse
    {
        public List<GacheBonusData> bonusList { get; set; }
        public int index { get; set; }
    }
    public struct GetGachaResponse
    {
        public int gacha { get; set; }
    }
    public struct GacheBonusData
    {
        public int bonusId { get; set; }
        public int rare { get; set; }
    }
}
