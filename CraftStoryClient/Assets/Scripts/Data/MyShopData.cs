using System;

/// <summary>
/// マイショップデータ
/// </summary>
public class MyShopData
{
    /// <summary>
    /// ショップレベル （設計図桁）
    /// </summary>
    public int myShopLv { get; set; }
    public MyShopItem[] MyShopItem 
    {
        get
        {
            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                return mGuideMyShopItem;
            }
            else
            {
                return mMyShopItem;
            }
        }
        set => mMyShopItem = value;
    }
    private MyShopItem[] mMyShopItem;
    private MyShopItem[] mGuideMyShopItem;

    public MyShopData()
    {
        MyShopItem = new MyShopItem[3];
        mGuideMyShopItem = new MyShopItem[3];
    }
    public void Clear()
    {
        MyShopItem = new MyShopItem[3];
        mGuideMyShopItem = new MyShopItem[3];
    }
}

/// <summary>
/// マイショップアイテムデータ
/// </summary>
public struct MyShopItem
{
    public string targetAcc { get; set; }
    public int myshopid { get; set; }
    public int itemId { get; set; }
    public string nickName { get; set; }
    public string newName { get; set; }
    public string icon { get; set; }
    public int goodNum { get; set; }
    public int price { get; set; }
    public int site { get; set; }
    public DateTime created_at { get; set; }
}