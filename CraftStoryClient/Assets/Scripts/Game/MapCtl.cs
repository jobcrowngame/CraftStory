using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCtl
{
    private MapCell[,] map;
    private Transform mapCellParent;

    public MapCtl()
    {
        mapCellParent = new GameObject("Ground").transform;

        map = new MapCell[30, 30];

        InitMap();
    }

    private void InitMap()
    {
        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                var Pos = new Vector3(i, 1, j);
                var CellType = (MapCellType)UnityEngine.Random.Range(0,4);
                map[i, j] = CreateCell(Pos, CellType);
            }
        }
    }

    public MapCell CreateCell(Vector3 pos, MapCellType cellType)
    {
        string sourcePath = GetMapCellPath(cellType);
        if (sourcePath == "")
            return null;

        var resources = CommonFunction.ReadResourcesPrefab(sourcePath);
        if (resources == null)
        {
            Debug.LogError("not find resources " + sourcePath);
            return null;
        }

        var obj = GameObject.Instantiate(resources, mapCellParent);
        if (obj == null)
            return null;

        obj.transform.position = pos;

        var cell = obj.AddComponent<MapCell>();
        cell.Pos = pos;
        cell.CellType = cellType;

        return cell;
    }

    public string GetMapCellPath(MapCellType type)
    {
        string root = "Prefabs/Game/";

        switch (type)
        {
            case MapCellType.Black: return root + "Cubes/Cube_Black";
            case MapCellType.Blue: return root + "Cubes/Cube_Blue";
            case MapCellType.Red: return root + "Cubes/Cube_Red";
            case MapCellType.Green: return root + "Cubes/Cube_Green";
            default: Debug.LogError("not find MapCellType " + type); return "";
        }
    }
}
