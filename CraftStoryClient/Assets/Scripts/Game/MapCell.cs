using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    private Vector3 pos;
    private MapCellType cellType;
    private GameObject obj;

    public Vector3 Pos { get; set; }
    public MapCellType CellType { get; set; }
}

public enum MapCellType
{
    Black,
    Blue,
    Red,
    Green,
}
