using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ShopBlueprintLG : UILogicBase<ShopBlueprintLG, ShopBlueprintUI>
{
    public UIType Type
    {
        get => mType;
        set
        {
            mType = value;

            UI.RefreshItemWindow(value);
        }
    }
    private UIType mType;

    public int SelectMyShopPage
    {
        get => mSelectMyShopPage;
        set
        {
            mSelectMyShopPage = value;
            UI.SetMyShopPageText(value);
        }
    }
    int mSelectMyShopPage = 1;

    public void RefreshMyShopBlueprint()
    {
        RefreshMyShopBlueprint("", 0);
    }
    public void RefreshMyShopBlueprint(string nickName, int sortType)
    {
        NWMng.E.SearchMyShopItems((rp) =>
        {
            List<MyShopItem> items = null;
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                items = JsonMapper.ToObject<List<MyShopItem>>(rp.ToJson());
            }

            UI.RefreshBlueprint1(items);
        }, SelectMyShopPage, nickName, sortType);
    }

    public void OnClickLeftBtn(string nickName, int sortType)
    {
        if (SelectMyShopPage > 1)
        {
            SelectMyShopPage--;
            RefreshMyShopBlueprint(nickName, sortType);
        }
    }
    public void OnClickRightBtn(string nickName, int sortType)
    {
        SelectMyShopPage++;
        RefreshMyShopBlueprint(nickName, sortType);
    }

    /// <summary>
    /// いいね数を更新
    /// </summary>
    public void RefreshGoodNum(string targetAcc)
    {
        UI.RefreshGoodNum(targetAcc);
    }

    /// <summary>
    /// マイショップレベルアップ
    /// </summary>
    /// <param name="cost"></param>
    public void UpdateMyShopLevel(int cost)
    {
        NWMng.E.LevelUpMyShop((rp) =>
        {
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];
            DataMng.E.RuntimeData.Coin1 = (int)rp["coin1"];

            UI.RefreshMyShopWindow();
        });
    }

    public enum UIType
    {
        /// <summary>
        /// ユーザー設計図
        /// </summary>
        Blueprint1,

        /// <summary>
        /// 公式市場
        /// </summary>
        Blueprint2,

        /// <summary>
        /// マイショップ
        /// </summary>
        MyShop,
    }
}