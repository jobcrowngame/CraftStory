using UnityEngine;

public class EntityResources : EntityBase
{
    public MapBlockData data { get; set; }

    private float clickingTime;


    public void OnClicking(float time)
    {
        clickingTime += time;

        if (clickingTime > data.BaseData.DestroyTime)
        {
            DataMng.E.AddItem(ConfigMng.E.Block[data.ID].ItemID, 1);

            Debug.Log("Delete cube. \n" + data.ToString()); ;
            //WorldMng.E.MapCtl.DeleteBlock(this);
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}