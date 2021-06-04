using UnityEngine;

public class MapBlock : MonoBehaviour
{
    public MapBlockData data { get; set; }

    private float clickingTime;

    public void SetData(MapBlockData mcData)
    {
        data = mcData;
    }

    public void Delete()
    {
        Debug.Log("Delete cube. \n" + data.ToString()); ;
        WorldMng.E.MapCtl.DeleteBlock(this);
    }

    public void OnClicking(float time)
    {
        clickingTime += time;

        if (clickingTime > data.BaseData.DestroyTime)
        {
            PlayerCtl.E.AddItem(ConfigMng.E.Block[data.ID].ItemID, 1);
            Delete();
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}