using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyShopLG : UILogicBase<MyShopLG, MyShopUI>
{
    public void UpdateMyShopLevel()
    {
        NWMng.E.LevelUpMyShop((rp) => 
        {
            DataMng.E.RuntimeData.MyShop.myShopLv = (int)rp["myShopLv"];
            DataMng.E.RuntimeData.Coin1 = (int)rp["coin1"];
            UI.RefreshUI();
        });
    }
}