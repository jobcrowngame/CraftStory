using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    public MapCellData data { get; set; }

    private float clickingTime;

    public void SetData(MapCellData mcData)
    {
        data = mcData;
    }

    public void Delete()
    {
        Debug.Log("Delete cube. ");
        WorldMng.E.MapCtl.DeleteMapCell(this);
    }

    public void OnClicking(float time)
    {
        clickingTime += time;

        if (clickingTime > data.DeleteTime)
        {
            Debug.Log(clickingTime);
            Delete();
        }
    }
    public void CancelClicking()
    {
        clickingTime = 0;
    }
}