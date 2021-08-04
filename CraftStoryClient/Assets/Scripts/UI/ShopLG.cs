using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class ShopLG : UILogicBase<ShopLG, ShopUI>
{
    public ShopUiType ShopUIType
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
    private ShopUiType shopUIType;

    private string GetTitle2Text()
    {
        switch (ShopUIType)
        {
            case ShopUiType.Charge: return "クラフトシード";
            case ShopUiType.Exchange: return "交換";
            case ShopUiType.Blueprint: return "設計図";
            case ShopUiType.Blueprint2: return "設計図";
            case ShopUiType.Point: return "ポイント";
            case ShopUiType.Subscription: return "クラパス";

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
            List<MyShopBlueprintData> items = null;
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                items = JsonMapper.ToObject<List<MyShopBlueprintData>>(rp.ToJson());
            }
            
            UI.RefreshBlueprint2(items);
        }, SelectPage, nickName, sortType);
    }
    public void GetSubscriptions()
    {
        UI.RefreshSubscription();
    }
}

public enum ShopUiType
{
    Charge = 1,
    Exchange,
    Blueprint,
    Point,
    Blueprint2,
    Subscription,
}