using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

public class EntityResources : EntityBase
{
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        AdventureCtl.E.AddBonus(EConfig.BonusID);
        WorldMng.E.MapCtl.DeleteEntity(this);
    }
}