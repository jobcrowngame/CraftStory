using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リソース（冒険エリアの資源）
/// </summary>
public class EntityResources : EntityBase
{
    public override void OnClick()
    {
        base.OnClick();

        if (DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            GuideLG.E.OnClickEntityResource(this);
        }
        else
        {
            var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.ResourcesDestroy);
            effect.Init();

            AdventureCtl.E.AddBonus(EConfig.BonusID);
            WorldMng.E.MapCtl.DeleteEntity(this);
        }
    }
}