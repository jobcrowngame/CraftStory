using UnityEngine;

public class EntityResources : EntityBase
{
    private float clickingTime;

    public void OnClicking(float time)
    {
        clickingTime += time;

        if (clickingTime > Data.Config.DestroyTime)
        {
            AdventureCtl.E.BreackResources(Data.ID);
            WorldMng.E.MapCtl.DeleteResource(this);
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}