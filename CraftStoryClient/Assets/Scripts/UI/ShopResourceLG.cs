
using LitJson;
using System;
using System.Collections.Generic;

public class ShopResourceLG : UILogicBase<ShopResourceLG, ShopResourceUI>
{
    List<ShopLimitedCount> ShopLimitedCounts;

    public struct ShopLimitedCount
    {
        public int shopId;
        public int limitedCount;
    }

    public void SetAllLimitedCounts(JsonData rp)
    {
        string jsonStr = rp.ToJson();
        ShopLimitedCounts = jsonStr != "\"\"" ?
            LitJson.JsonMapper.ToObject<List<ShopResourceLG.ShopLimitedCount>>(jsonStr) : new List<ShopLimitedCount>();
    }

    public int GetLimitedCount(int shopId)
    {
        int index = ShopLimitedCounts.FindIndex(val => val.shopId == shopId);
        return index != -1 ? ShopLimitedCounts[index].limitedCount : 0;
    }

    public void SetLimitedCount(int shopId, int limitedCount)
    {
        int index = ShopLimitedCounts.FindIndex(val => val.shopId == shopId);
        if (index != -1) {
            ShopLimitedCount val = ShopLimitedCounts[index];
            val.limitedCount = limitedCount;
            ShopLimitedCounts[index] = val;
        }
        else
        {
            ShopLimitedCounts.Add(new ShopLimitedCount{ shopId = shopId, limitedCount = limitedCount });
        }
    }
}