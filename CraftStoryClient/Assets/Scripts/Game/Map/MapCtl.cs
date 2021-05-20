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

    public void CreateMap()
    {
        mapCellParent = new GameObject("Ground").transform;

        mData = DataMng.E.MapData;

        for (int i = 0; i < mData.MapSize.x; i++)
        {
            for (int j = 0; j < mData.MapSize.y; j++)
            {
                for (int k = 0; k < mData.MapSize.z; k++)
                {
                    if (mData.Map[i, j, k] != null)
                        CreateBlock(mData.Map[i, j, k]);
                }
            }
        }
    }

    public void OnQuit()
    {

    }

    public MapBlock CreateBlock(Vector3 pos, BlockData block)
    {
        block.Pos = pos;
        return CreateBlock(block);
    }
    public MapBlock CreateBlock(BlockData block)
    {
        Debug.Log("Create block. \n" + block.BaseData.Name);

        string sourcesFullPath = PublicPar.BlockRootPath + block.BaseData.ResourcesName;

        var resources = CommonFunction.ReadResources(sourcesFullPath);
        if (resources == null)
        {
            Debug.LogError("not find resources " + sourcesFullPath);
            return null;
        }

        var obj = GameObject.Instantiate(resources, mapCellParent);
        if (obj == null)
            return null;

        obj.transform.position = block.Pos;

        var cell = obj.AddComponent<MapBlock>();
        cell.SetData(block);

        mData.Add(block);

        return cell;
    }

    public void DeleteMapCell(MapBlock mCell)
    {
        mData.Remove(mCell.data.Pos);
        GameObject.Destroy(mCell.gameObject);
    }
}
