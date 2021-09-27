using System;

/// <summary>
/// 向きがある普通Entity
/// </summary>
public class EntityHaveDirection : EntityBase
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