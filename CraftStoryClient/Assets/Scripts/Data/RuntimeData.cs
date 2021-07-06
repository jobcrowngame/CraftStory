

public class RuntimeData
{
    public int Coin1 { get; set; }
    public int Coin2 { get; set; }
    public int Coin3 { get; set; }

    public MyShopData MyShop { get; set; }

    public RuntimeData()
    {
        MyShop = new MyShopData();
    }
}