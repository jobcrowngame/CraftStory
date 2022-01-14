using System;

[Serializable]
public class UserDataTable
{
    public UserDataTable()
    {
        lv = 1;
    }

    public int lv { get; set; }
    public int exp { get; set; }
}