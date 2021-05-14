using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    private GameObject obj;

    public MapCellData data { get; set; }

    public void Add(MapCellData mcData, GameObject obj)
    {
        this.obj = obj;
        data = mcData;
    }
}