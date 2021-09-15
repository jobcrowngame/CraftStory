/// <summary>
/// 掲示板エンティティ
/// </summary>
public class EntityMission : EntityBase
{
    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();
    }

    /// <summary>
    /// クリックイベント
    /// </summary>
    public override void OnClick()
    {
        base.OnClick();

        UICtl.E.OpenUI<MissionUI>(UIType.Mission);
    }
}