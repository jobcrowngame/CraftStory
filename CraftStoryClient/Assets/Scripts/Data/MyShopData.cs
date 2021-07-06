using System;

public class MyShopData
{
    public int firstUseMyShop { get; set; }
    public int myShopLv { get; set; }
    public MyShopItem[] myShopItem { get; set; }

    public MyShopData()
    {
        myShopItem = new MyShopItem[3];
    }
}

public struct MyShopItem
{
    public int itemId { get; set; }
    public int site { get; set; }
    public DateTime created_at { get; set; }
    public string data { get; set; }
}