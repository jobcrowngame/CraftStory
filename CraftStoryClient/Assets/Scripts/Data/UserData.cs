using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public string Account { get; set; }
    public string UserPW { get; set; }

    public int Coin1 { get; set; }
    public int Coin2 { get; set; }
    public int Coin3 { get; set; }
}