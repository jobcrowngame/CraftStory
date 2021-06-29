using UnityEngine;

public class MapBlock : MonoBehaviour
{
    public MapBlockData data { get; set; }

    private float clickingTime;

    public void SetData(MapBlockData mcData)
    {
        data = mcData;
    }

    public void OnClicking(float time)
    {
        if (clickingTime == 0)
            EffectMng.E.AddDestroyEffect(transform.position);

        clickingTime += time;

        if (clickingTime > data.BaseData.DestroyTime)
        {
            clickingTime = 0;
            DataMng.E.AddItem(ConfigMng.E.Block[data.ID].ItemID, 1);

            Logger.Log("Delete cube. \n" + data.ToString()); ;
            WorldMng.E.MapCtl.DeleteBlock(this);

            EffectMng.E.RemoveDestroyEffect();
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}