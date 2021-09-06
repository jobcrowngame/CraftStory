/// <summary>
/// ブロック
/// </summary>
public class EntityBlock : EntityBase
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