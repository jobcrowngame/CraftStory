using System.Collections.Generic;
using UnityEngine;

public class EntityResources : EntityBase
{
    private float clickingTime;

    public void OnClicking(float time)
    {
        clickingTime += time;

        if (clickingTime > Data.Config.DestroyTime)
        {
            clickingTime = 0;
            var items = new Dictionary<int, int>();
            CommonFunction.GetItemsByBonus(Data.Config.BonusID, ref items);

            foreach (var key in items.Keys)
            {
                DataMng.E.AddItems(items, ()=> 
                {
                    WorldMng.E.MapCtl.DeleteResource(this);
                });
            }
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}