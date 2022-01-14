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
    public bool IsState3 { get => ElapsedTime > cropsConfig.StateTime2; }

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

            if (State1 != null) State1.gameObject.SetActive(value == 1);
            if (State2 != null) State2.gameObject.SetActive(value == 2);
            if (State3 != null) State3.gameObject.SetActive(value == 3);
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
        startTime = GetTimer(LocalPos);
        CheckState();
    }

    public void OnDestroy()
    {
        WorldMng.E.MapMng.RemoveCrops(WorldPos);
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

        // ローカルのアイテム数変更
        DataMng.E.AddItem(itemId, count);
        HomeLG.E.AddItem(itemId, count);

        // エンティティインスタンスを削除
        WorldMng.E.MapMng.DeleteEntity(this);

        // 削除Effectを追加
        var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.BlockDestroyEnd);
        effect.Init();

        OnRemoveCropsEntity();
    }

    public void OnRemoveCropsEntity()
    {
        WorldMng.E.MapMng.RemoveCrops(LocalPos);
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