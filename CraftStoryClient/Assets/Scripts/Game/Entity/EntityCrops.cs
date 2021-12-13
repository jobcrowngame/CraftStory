using JsonConfigData;
using System;
/// <summary>
/// ブロック
/// </summary>
public class EntityCrops : EntityBase
{
    // 成熟
    bool IsState3 { get => ElapsedTime > cropsConfig.StateTime2; }

    Crops cropsConfig;
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

    public void Init()
    {
        cropsConfig = ConfigMng.E.GetCropsByEntityID(EConfig.ID);
        startTime = GetTimer(Pos);

        TimeZoneMng.E.AddTimerEvent03(Update1S);
    }

    // 0.02秒
    private void Update1S()
    {
        if (cropsConfig == null || ElapsedTime > cropsConfig.StateTime2)
            return;

        if (ElapsedTime < cropsConfig.StateTime1)
        {
            State = 1;
        }
        else if (ElapsedTime < cropsConfig.StateTime2)
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
        DataMng.E.UserData.CropsTimers.Remove(Pos);
    }

    /// <summary>
    /// 農業開始時間をゲット
    /// </summary>
    /// <param name="pos">key</param>
    /// <returns></returns>
    private DateTime GetTimer(UnityEngine.Vector3 pos)
    {
        DateTime timer;
        if (!DataMng.E.UserData.CropsTimers.TryGetValue(pos, out timer))
        {
            DataMng.E.UserData.CropsTimers[pos] = DateTime.Now;
        }

        return timer;
    }
}