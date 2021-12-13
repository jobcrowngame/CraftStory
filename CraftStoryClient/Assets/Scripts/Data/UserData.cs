﻿using System;
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

    /// <summary>
    /// 最初の掲示板作り
    /// </summary>
    public int FirstCraftMission { get; set; }

    /// <summary>
    /// 始めに妖精を出す
    /// </summary>
    public bool FirstShowFairy = true;

    /// <summary>
    /// 強制表示お知らせ当日チェックマップ
    /// </summary>
    public Dictionary<int, DateTime> PickupNoticeCheckMap { get; set; }

    /// <summary>
    /// 農作物時間
    /// </summary>
    public Dictionary<UnityEngine.Vector3, DateTime> CropsTimers { get; set; }
}