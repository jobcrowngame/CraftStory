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

    public override void OnClick()
    {
        base.OnClick();

        if (DataMng.E.RuntimeData.MapType == MapType.Home || 
            DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            // クワを装備している場合
            if (PlayerCtl.E.SelectItem != null && PlayerCtl.E.SelectItem.Config.Type == 4100)
            {
                // 土・草土の場合は畑に変化
                if (EConfig.ID == 1005 || EConfig.ID == 1006)
                {
                    if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
                    {
                        WorldMng.E.MapMng.DeleteEntity(this);
                        WorldMng.E.MapMng.CreateEntity(3000, WorldPos, 0);
                    }
                    else
                    {
                        WorldMng.E.MapCtl.DeleteEntity(this);
                        WorldMng.E.MapCtl.CreateEntity(3000, this.LocalPos, 0);
                        WorldMng.E.MapCtl.CombineMesh();
                    }
                }
                else
                {
                    CommonFunction.ShowHintBar(37);
                }
            }
        }
    }
}