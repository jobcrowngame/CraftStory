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

    public void StartGacha(int gachaId)
    {
        int costId = ConfigMng.E.Gacha[gachaId].Cost;
        int costCount = ConfigMng.E.Gacha[gachaId].CostCount;
        if (DataMng.E.GetCoinByID(costId) < costCount)
        {
            if (costId == 9000) CommonFunction.ShowHintBar(1010001);
            if (costId == 9001) CommonFunction.ShowHintBar(1010002);
            if (costId == 9002) CommonFunction.ShowHintBar(1017001);

            return;
        }

        NWMng.E.Gacha10((rp) =>
        {
            if (string.IsNullOrEmpty(rp.ToString()))
            {
                Logger.Error("Bad gacha result");
            }

            var result = LitJson.JsonMapper.ToObject<GachaResponse>(rp.ToJson());
            var ui = UICtl.E.OpenUI<GachaBonusUI>(UIType.GachaBonus);
            if (ui != null)
            {
                ui.Set(result, gachaId);
            }

            DataMng.E.ConsumableCoin(costId, costCount);
            UI.RefreshCoins();
        }, gachaId);
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