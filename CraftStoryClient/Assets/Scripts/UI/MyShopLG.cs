
public class MyShopLG : UILogicBase<MyShopLG, MyShopUI>
{
    public void UpdateMyShopLevel(int cost)
    {
        NWMng.E.LevelUpMyShop((rp) => 
        {
            DataMng.E.MyShop.myShopLv = (int)rp["myShopLv"];
            DataMng.E.UserData.Coin1 = (int)rp["coin1"];

            UI.RefreshUI();
        });
    }
}