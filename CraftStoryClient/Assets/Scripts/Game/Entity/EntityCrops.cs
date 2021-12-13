using JsonConfigData;
using System;
using UnityEngine;
/// <summary>
/// ブロック
/// </summary>
public class EntityCrops : EntityBase
{
    GameObject State1 { get => CommonFunction.FindChiledByName(transform, "State1"); }
    GameObject State2 { get => CommonFunction.FindChiledByName(transform, "State2"); }
    GameObject State3 { get => CommonFunction.FindChiledByName(transform, "State3"); }

    // 成熟
    bool IsState3 { get => ElapsedTime > cropsConfig.StateTime2; }

    Crops cropsConfig { get => ConfigMng.E.GetCropsByEntityID(EConfig.ID); }
    DateTime startTime;

    /// <summary>
    /// 状態変更
    /// </summary>
    int State
    {
        set
        {
            if (mState == value)
                return;

            mState = value;

            State1.gameObject.SetActive(value == 1);
            State2.gameObject.SetActive(value == 2);
            State3.gameObject.SetActive(value == 3);
        }
    }
    int mState = 0;

    double ElapsedTime 
    { 
        get
        {
            return (DateTime.Now - startTime).TotalSeconds;
        } 
    }

    public override void Init()
    {
        startTime = GetTimer(Pos);
        CheckState();
    }

    // 毎秒実行
    public void Update1S()
    {
        CheckState();
    }
    private void CheckState()
    {
        if (EConfig.ID == 0 || cropsConfig == null)
            return;

        if (ElapsedTime <= cropsConfig.StateTime1)
        {
            State = 1;
        }
        else if (ElapsedTime <= cropsConfig.StateTime2)
        {
            State = 2;
        }
        else
        {
            State = 3;
        }
    }

    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        int itemId, count;
        var cropsConfig = ConfigMng.E.GetCropsByEntityID(EConfig.ID);

        if (IsState3)
        {
            itemId = cropsConfig.DestroyAddItem;
            count = cropsConfig.Count;
        }
        else
        {
            itemId = cropsConfig.ItemID;
            count = 1;
        }

        // アイテム追加
        NWMng.E.AddItem(null, itemId, count);

        // ローカルのアイテム数変更
        DataMng.E.AddItem(itemId, count);

        // エンティティインスタンスを削除
        WorldMng.E.MapCtl.DeleteEntity(this);

        // 削除Effectを追加
        var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.BlockDestroyEnd);
        effect.Init();

        TimeZoneMng.E.RemoveTimerEvent03(Update1S);
        WorldMng.E.MapCtl.RemoveCrops(Pos);
    }

    /// <summary>
    /// 農業開始時間をゲット
    /// </summary>
    /// <param name="pos">key</param>
    /// <returns></returns>
    private DateTime GetTimer(Vector3 pos)
    {
        DateTime timer;
        string key = CommonFunction.Vector3ToString(pos);
        if (!DataMng.E.UserData.CropsTimers.TryGetValue(key, out timer))
        {
            timer = DateTime.Now;
            DataMng.E.UserData.CropsTimers[key] = DateTime.Now;
        }

        return timer;
    }
}