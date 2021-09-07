using System;


/// <summary>
/// トーチ
/// </summary>
public class EntityTorch : EntityBase
{
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        OnDestroyEntity();
    }

    /// <summary>
    /// 向きを設定
    /// </summary>
    /// <param name="tType"></param>
    public override void SetDirection(Direction tType)
    {
        switch (tType)
        {
            case Direction.up: CommonFunction.FindChiledByName(transform, "Center").gameObject.SetActive(true); break;
            default: CommonFunction.FindChiledByName(transform, "Foword").gameObject.SetActive(true); break;
        }
    }
}