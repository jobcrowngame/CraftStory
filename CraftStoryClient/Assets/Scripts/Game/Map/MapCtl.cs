using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapCtl
{
    private Transform mapCellParent;

    private MapData mData;

    public MapCtl()
    {
        mapCellParent = new GameObject("Ground").transform;

        InitMap(DataMng.E.MapData);
    }

    private void InitMap(MapData mData)
    {
        this.mData = mData;

        for (int i = 0; i < mData.MapSize.x; i++)
        {
            for (int j = 0; j < mData.MapSize.y; j++)
            {
                for (int k = 0; k < mData.MapSize.z; k++)
                {
                    if (mData.Map[i, j, k] != null)
                        CreateMapCell(mData.Map[i, j, k].Pos, mData.Map[i, j, k].CellType, true);
                }
            }
        }
    }

    public void OnQuit()
    {

    }

    public MapCell CreateMapCell(Vector3 pos, MapCellType cellType, bool isFirst = false)
    {
        string sourcePath = GetMapCellResourcesPath(cellType);
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
        cell.Add(new MapCellData()
        {
            Pos = pos,
            CellType = cellType
        }, obj);

        if (!isFirst)
            mData.Add(pos, cell.data);

        return cell;
    }

    public void DeleteMapCell(MapCell mCell)
    {
        mData.Remove(mCell.data.Pos);
        GameObject.Destroy(mCell.gameObject);
    }

    public string GetMapCellResourcesPath(MapCellType type)
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
