

using LitJson;
using System;
using System.Collections.Generic;

public partial class NWMng
{
    /// <summary>
    /// アイテムリストをゲット
    /// </summary>
    /// <param name="callBack"></param>
    public void GetItems(Action callBack = null)
    {
        if (DataMng.E.RuntimeData.MapType != MapType.Guide)
        {
            GetItemList((rp) =>
            {
                DataMng.GetItems(rp);

                if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
                if (callBack != null) callBack();
            });
        }
        else
        {
            if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
            if (callBack != null) callBack();
        }
    }

    /// <summary>
    /// サブスクリプション情報をゲット
    /// </summary>
    /// <param name="callBack"></param>
    public void GetSubscriptionInfo(Action callBack = null)
    {
        GetSubscriptionInfoRequest((rp) =>
        {
            DataMng.E.RuntimeData.SubscriptionLv01 = (int)rp["subscriptionLv01"];
            DataMng.E.RuntimeData.SubscriptionLv02 = (int)rp["subscriptionLv02"];
            DataMng.E.RuntimeData.SubscriptionLv03 = (int)rp["subscriptionLv03"];
            if (!string.IsNullOrEmpty((string)rp["updateTime01"])) 
                DataMng.E.RuntimeData.SubscriptionUpdateTime01 = DateTime.Parse((string)rp["updateTime01"]);
            if (!string.IsNullOrEmpty((string)rp["updateTime02"]))
                DataMng.E.RuntimeData.SubscriptionUpdateTime02 = DateTime.Parse((string)rp["updateTime02"]);
            if (!string.IsNullOrEmpty((string)rp["updateTime03"]))
                DataMng.E.RuntimeData.SubscriptionUpdateTime03 = DateTime.Parse((string)rp["updateTime03"]);

            if (callBack != null) callBack();
        });
    }

    /// <summary>
    /// 新しいメール数をゲット
    /// </summary>
    /// <param name="callBack"></param>
    public void GetNewEmailCount(Action callBack = null)
    {
        NWMng.E.GetNewEmailCountRequest((rp) =>
        {
            DataMng.E.RuntimeData.NewEmailCount = (int)rp["count"];

            if (callBack != null) callBack();
        });
    }

    public void GetMyshopInfo(Action callBack = null)
    {
        NWMng.E.GetMyShopInfo((rp) =>
        {
            DataMng.E.MyShop.Clear();
            if (!string.IsNullOrEmpty(rp.ToString()))
            {
                List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp.ToJson());
                for (int i = 0; i < shopItems.Count; i++)
                {
                    DataMng.E.MyShop.MyShopItem[i] = shopItems[i];
                }

                if (callBack != null) callBack();
            }
        });
    }
}
