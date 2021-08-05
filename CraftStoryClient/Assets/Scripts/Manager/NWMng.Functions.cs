

using System;

public partial class NWMng
{
    public void GetSubscriptionInfo(Action callBack = null)
    {
        GetSubscriptionInfoRequest((rp) =>
        {
            DataMng.E.RuntimeData.SubscriptionLv = (int)rp["subscriptionLv"];
            if(!string.IsNullOrEmpty((string)rp["updateTime"])) DataMng.E.RuntimeData.SubscriptionUpdateTime = DateTime.Parse((string)rp["updateTime"]);

            if (callBack != null) callBack();
        });
    }

    public void GetNewEmailCount(Action callBack = null)
    {
        NWMng.E.GetNewEmailCountRequest((rp) =>
        {
            DataMng.E.RuntimeData.NewEmailCount = (int)rp["count"];

            if (callBack != null) callBack();
        });
    }
}
