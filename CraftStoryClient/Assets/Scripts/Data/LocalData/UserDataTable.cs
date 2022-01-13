using System;

[Serializable]
public class UserDataTable
{
    public UserDataTable()
    {
        lv = 1;
    }

    public string nickname { get; set; }
    public string comment { get; set; }
    public string email { get; set; }
    public int lv { get; set; }
    public int exp { get; set; }
    public int myShopLv { get; set; }
}