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

    public void OnClickLeftBtn(string nickName)
    {
        if (SelectPage > 1)
        {
            SelectPage--;
            GetBlueprint2Items(nickName);
        }
    }
    public void OnClickRightBtn(string nickName)
    {
        SelectPage++;
        GetBlueprint2Items(nickName);
    }

    public void GetBlueprint2Items(string nickName)
    {
        NWMng.E.SearchMyShopItems((rp) =>
        {
            List<MyShopBlueprintData> items = null;
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                items = JsonMapper.ToObject<List<MyShopBlueprintData>>(rp.ToJson());
            }
            
            UI.RefreshBlueprint2(items);
        }, SelectPage, nickName);
    }
}

public enum ShopUiType
{
    Charge = 1,
    Exchange,
    Blueprint,
    Blueprint2,
}