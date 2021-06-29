using System.Collections.Generic;
using UnityEngine;

public class EntityResources : EntityBase
{
    private float clickingTime;

    public void OnClicking(float time)
    {
        if (clickingTime == 0)
            EffectMng.E.AddDestroyEffect(transform.position);

        clickingTime += time;

        if (clickingTime > Data.Config.DestroyTime)
        {
            clickingTime = 0;

            if (Data.Type == ItemType.Workbench ||
                Data.Type == ItemType.Kamado)
            {
                NWMng.E.GetBonus((rp) =>
                {
                    NWMng.E.GetItemList((rp2) =>
                    {
                        DataMng.GetItems(rp2[0]);
                    });
                }, Data.Config.BonusID);
            }

            AdventureCtl.E.AddBonus(Data.Config.BonusID);

            var items = new Dictionary<int, int>();
            WorldMng.E.MapCtl.DeleteResource(this);

            EffectMng.E.RemoveDestroyEffect();
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}