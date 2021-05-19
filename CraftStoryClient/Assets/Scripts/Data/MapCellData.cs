using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class MapCellData
{
    private float X { get; set; }
    private float Y { get; set; }
    private float Z { get; set; }
    public MapCellType CellType { get; set; }

    public float DeleteTime { get; set; }

    public Vector3 Pos
    {
        get { return new Vector3(X, Y, Z); }
        set
        {
            X = value.x;
            Y = value.y;
            Z = value.z;
        }
    }

    public override string ToString()
    {
        return string.Format("POS:{0}, CType:{1}, DeleteTime:{2}", Pos, CellType, DeleteTime);
    }
}

public enum MapCellType
{
    Black,
    Blue,
    Red,
    Green,
}
