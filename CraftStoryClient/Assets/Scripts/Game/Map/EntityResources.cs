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
            DataMng.E.AddItem(Data.ID, 1);

            var items = new Dictionary<int, int>();
            WorldMng.E.MapCtl.DeleteResource(this);
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}