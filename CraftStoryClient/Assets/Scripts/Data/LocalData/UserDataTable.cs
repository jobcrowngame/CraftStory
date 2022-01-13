using System;

[Serializable]
public class UserDataTable
{
    public UserDataTable()
    {
        lv = 1;
    }

    public string acc { get; set; }
    public string pw { get; set; }
    public string nickname { get; set; }
    public string comment { get; set; }
    public string email { get; set; }
    public int lv { get; set; }
    public int exp { get; set; }
    public int coin1 { get; set; }
    public int coin2 { get; set; }
    public int coin3 { get; set; }
    public int totalPoint { get; set; }
    public int myShopLv { get; set; }
    public int subscriptionLv01 { get; set; }
    public int subscriptionLv02 { get; set; }
    public int subscriptionLv03 { get; set; }
}