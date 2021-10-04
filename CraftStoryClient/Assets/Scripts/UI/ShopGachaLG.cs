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
            mGachaType = value;

            UI.RefreshGachaUI();
        }
    }
    private int mGachaType = 0;

    public int SelectGachaId
    {
        get => GachaIds[mGachaType, mIndex];
    }

    public int[,] GachaIds = new int[3,3] {
        { 1, 2, 3 },
        { 1, 2, 3 }, 
        { 1, 2, 3 }
    };

    public struct GachaResponse
    {
        public List<GacheBonusData> bonusList { get; set; }
        public int index { get; set; }
    }
    public struct GacheBonusData
    {
        public int bonusId { get; set; }
        public int rare { get; set; }
    }
}
