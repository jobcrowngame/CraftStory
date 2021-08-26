using System;
using System.Collections.Generic;

[Serializable]
public class UserData
{
    /// <summary>
    /// アカウント
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// パスワード
    /// </summary>
    public string UserPW { get; set; }
}