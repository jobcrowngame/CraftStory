using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData
{
    /// <summary>
    /// ローカル化してるかのフラグ
    /// </summary>
    public bool LocalDataLoaded = false;

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
    /// 劣化版のフラグ
    /// </summary>
    public bool IsDeterioration { get; set; }

    /// <summary>
    /// 今のエリア
    /// </summary>
    public int AreaIndexX { get; set; }
    public int AreaIndexZ { get; set; }
    public int PlayerDefaltSpawnPosX { get; set; }
    public int PlayerDefaltSpawnPosZ { get; set; }
    public int PlayerSpawnPosX { get; set; }
    public int PlayerSpawnPosZ { get; set; }

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
            if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            {
                if (mCyberMapCropsTimers == null) mCyberMapCropsTimers = new Dictionary<string, DateTime>();
                return mCyberMapCropsTimers;
            }
            else
            {
                if (mCropsTimers == null) mCropsTimers = new Dictionary<string, DateTime>();
                return mCropsTimers;
            }
        }
        set => mCropsTimers = value;
    }
    Dictionary<string, DateTime> mCropsTimers;
    Dictionary<string, DateTime> mCyberMapCropsTimers;

    public int NewItemGuid = 1;
}