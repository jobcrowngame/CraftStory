

public class EntityFlowoer : EntityBase
{
    /// <summary>
    /// 長い時間クリック終了場合のロジック
    /// </summary>
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        // 壊したブロックを手に入る
        NWMng.E.AddItem((rp) =>
        {
            NWMng.E.GetItems(() =>
            {
                // ブロックエンティティを削除
                WorldMng.E.MapCtl.DeleteEntity(this);

                // ブロック壊した場合のエフェクト追加
                var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.BlockDestroyEnd);
                effect.Init();
            });
        }, EConfig.ItemID, 1);
    }
}