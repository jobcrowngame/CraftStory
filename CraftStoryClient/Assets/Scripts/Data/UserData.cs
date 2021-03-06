using System;
using System.Collections.Generic;
using UnityEngine;

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
    /// 飢え
    /// </summary>
    public int Hunger { get; set; }

    /// <summary>
    /// 無料で空腹度を回復したかのフラグ（既存のユーザーの為）
    /// </summary>
    public int FreeFoodEated { get; set; }

    /// <summary>
    /// 今のエリア
    /// </summary>
    public int AreaIndexX { get; set; }
    public int AreaIndexZ { get; set; }
    public int PlayerPositionX { get; set; }
    public int PlayerPositionZ { get; set; }

    /// <summary>
    /// 強制表示お知らせ当日チェックマップ
    /// </summary>
    public Dictionary<int, DateTime> PickupNoticeCheckMap { get; set; }

    /// <summary>
    /// 農作物時間
    /// </summary>
    public Dictionary<string, DateTime> CropsTimers 
    {
        get
        {
            if (mCropsTimers == null) mCropsTimers = new Dictionary<string, DateTime>();
            return mCropsTimers;
        }
        set => mCropsTimers = value;
    }
    Dictionary<string, DateTime> mCropsTimers;
}