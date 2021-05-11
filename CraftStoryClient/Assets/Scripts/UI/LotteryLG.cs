using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LotteryLG : UILogicBase<LotteryLG, LotteryUI>
{
    public void LotteryOne()
    {
        GS2.E.Exchange((response) => 
        {
            var giftBoxUI = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox, UIOpenType.BeforeClose) as GiftBoxUI;
            if (giftBoxUI != null)
                giftBoxUI.Add(response);
        }, 1, "ExchangeRate01");
    }

    public void LotteryTen()
    {
        GS2.E.Exchange((response) =>
        {
            var giftBoxUI = UICtl.E.OpenUI<GiftBoxUI>(UIType.GiftBox, UIOpenType.BeforeClose) as GiftBoxUI;
            if (giftBoxUI != null)
                giftBoxUI.Add(response);
        }, 10, "ExchangeRate01");
    }
}