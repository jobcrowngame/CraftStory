using System;

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

    /// <summary>
    /// 最初の掲示板作り
    /// </summary>
    public int FirstCraftMission { get; set; }
}