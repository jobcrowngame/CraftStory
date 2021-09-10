using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class ShopLG : UILogicBase<ShopLG, ShopUI>
{
    public ShopType ShopUIType
    {
        get => shopUIType;
        set
        {
            shopUIType = value;

            UI.SelectBtnIndex = shopUIType;
            UI.ChangeUIType(shopUIType);
            UI.SetTitle2(GetTitle2Text());
            UI.RefreshItems(shopUIType);
        }
    }
    private ShopType shopUIType;

    private string GetTitle2Text()
    {
        switch (ShopUIType)
        {
            case ShopType.Charge: return "クラフトシード";
            case ShopType.Gacha: return "ガチャ";
            case ShopType.Exchange: return "交換";
            case ShopType.Blueprint: return "設計図";
            case ShopType.Blueprint2: return "設計図";
            case ShopType.Point: return "ポイント";
            case ShopType.Subscription: return "クラパス";

            default: Logger.Error("not find shop ui type " + ShopUIType); break;
        }

        return "";
    }

    public int SelectPage
    {
        get => selectPage;
        set
        {
            selectPage = value;
            UI.SetPageText(value.ToString());
        }
    }
    int selectPage = 1;

    /// <summary>
    /// 選択したガチャインデックス
    /// </summary>
    public int SelectedGachaIndex 
    {
        get => mSelectedGachaIndex;
        set
        {
            if (value < 0 || value >= GachaIds.Length)
                return;

            mSelectedGachaIndex = value;

            UI.ShowGachaRightBtn(mSelectedGachaIndex != GachaIds.Length - 1);
            UI.ShowGachaLeftBtn(mSelectedGachaIndex > 0);

            mSelectedGachaId = GachaIds[mSelectedGachaIndex];
        }
    }
    private int mSelectedGachaIndex = 0;

    /// <summary>
    /// 選択したガチャID
    /// </summary>
    public int SelectGachaId { get => mSelectedGachaId; }
    private int mSelectedGachaId;
    private int[] GachaIds = new int[1]{ 1};

    public void OnClickLeftBtn(string nickName, int sortType)
    {
        if (SelectPage > 1)
        {
            SelectPage--;
            GetBlueprint2Items(nickName, sortType);
        }
    }
    public void OnClickRightBtn(string nickName, int sortType)
    {
        SelectPage++;
        GetBlueprint2Items(nickName, sortType);
    }

    public void GetBlueprint2Items(string nickName, int sortType)
    {
        NWMng.E.SearchMyShopItems((rp) =>
        {
            List<MyShopItem> items = null;
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                items = JsonMapper.ToObject<List<MyShopItem>>(rp.ToJson());
            }
            
            UI.RefreshBlueprint2(items);
        }, SelectPage, nickName, sortType);
    }
    public void GetSubscriptions()
    {
        UI.RefreshSubscription();
    }

    public int GetSubscriptionTypeByShopId(int shopId)
    {
        switch (shopId)
        {
            case 80: return 1;
            case 81: return 2;
            default: return 3;
        }
    }

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

public enum ShopType
{
    Charge = 0,
    Subscription,
    Gacha,
    Point,
    Exchange,
    Blueprint,
    Blueprint2,
}