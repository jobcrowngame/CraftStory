using System;

/// <summary>
/// 向きがない普通Entity
/// </summary>
public class EntityDefalt : EntityBase
{
    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();
    }
}
