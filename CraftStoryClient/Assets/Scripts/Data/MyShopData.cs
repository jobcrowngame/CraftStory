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
    public void Clear()
    {
        myShopItem = new MyShopItem[3];
    }
}

public struct MyShopItem
{
    public int id { get; set; }
    public int itemId { get; set; }
    public int site { get; set; }
    public DateTime created_at { get; set; }
    public string data { get; set; }
    public string newName { get; set; }
}

public struct MyShopBlueprintData
{
    public int id { get; set; }
    public int itemId { get; set; }
    public string data { get; set; }
    public string nickName { get; set; }
    public string newName { get; set; }
    public int price { get; set; }
    public DateTime created_at { get; set; }
}