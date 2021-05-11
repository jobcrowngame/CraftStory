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

    string id = "aa4d4e86-0303-4f23-bbeb-3344f12f9198";
    string pw = "kfHtHQ4LWWv6IKdraivCXIySlBuTOHVG";

    public void Init()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            WritLoginIDPW();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GetGachaItem();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GetShowcase();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            BuyItem();
        }
    }

    public void WritLoginIDPW()
    {
        LoginUI loginUI = UICtl.E.GetUI(UIType.Login) as LoginUI;
        loginUI.WritIDPW(id, pw);
    }

    private void GetGachaItem()
    {
        MLog.Log("get gacha item");

        GS2.E.Exchange((response) =>
        {
            var giftBoxUI = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox, UIOpenType.BeforeClose) as GiftBoxUI;
            if (giftBoxUI != null)
                giftBoxUI.Add(response);
        }, 1, "ExchangeGachaItem");
    }
    private void GetShowcase()
    {
        MLog.Log("GetShowcase");

        GS2.E.GetShowcase((r) => 
        { 

        }, "ShowcaseShop01");
    }
    private void BuyItem()
    {
        MLog.Log("BuyItem");

        GS2.E.Buy((r) =>
        {

        }, "ShowcaseShop01", "a059b507-c6a8-4703-80dd-13bc84b4e844");
    }
}
