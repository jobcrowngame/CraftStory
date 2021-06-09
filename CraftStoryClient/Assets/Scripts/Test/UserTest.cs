using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTest : MonoBehaviour
{
    private static UserTest entity;
    public static UserTest E
    {
        get
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<UserTest>();

            return entity;
        }
    }

    public string id = "aa4d4e86-0303-4f23-bbeb-3344f12f9198";
    public string pw = "kfHtHQ4LWWv6IKdraivCXIySlBuTOHVG";

    public string id2 = "e89190f3-a18f-4f51-acdb-f34ec65f56ac";
    public string pw2 = "dvSte5fBIeojTfaVwdnvmQIVlVx4ESzY";

    private void GetGachaItem()
    {
        Debug.Log("get gacha item");

        GS2.E.Exchange((r) =>
        {
            var giftBoxUI = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox, UIOpenType.BeforeClose) as GiftBoxUI;
            if (giftBoxUI != null)
                giftBoxUI.AddItem(r);
        }, 1, "ExchangeGachaItem");
    }
    private void GetShowcase()
    {
        Debug.Log("GetShowcase");

        GS2.E.GetShowcase((r) => 
        { 

        }, "ShowcaseShop01");
    }
}
