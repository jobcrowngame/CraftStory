using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    public int Version1 { get; set; }
    public int Version2 { get; set; }
    public int Version3 { get; set; }

    public string Account { get; set; }
    public string UserPW { get; set; }
    public string NickName { get; set; }

    public int Coin1 { get; set; }
    public int Coin2 { get; set; }
    public int Coin3 { get; set; }
}