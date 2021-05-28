using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapBlock : MonoBehaviour
{
    public BlockData data { get; set; }

    private float clickingTime;

    public void SetData(BlockData mcData)
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
            Delete();
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }

    //public void OnWillRenderObject()
    //{
    //    Debug.Log("OnWillRenderObject");
    //}
}