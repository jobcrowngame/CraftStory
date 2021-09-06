﻿using System;


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
            case Direction.foward: CommonFunction.FindChiledByName(transform, "Foword").gameObject.SetActive(true); break;
            case Direction.back: CommonFunction.FindChiledByName(transform, "Back").gameObject.SetActive(true); break;
            case Direction.right: CommonFunction.FindChiledByName(transform, "Right").gameObject.SetActive(true); break;
            case Direction.left: CommonFunction.FindChiledByName(transform, "Left").gameObject.SetActive(true); break;
        }
    }
}