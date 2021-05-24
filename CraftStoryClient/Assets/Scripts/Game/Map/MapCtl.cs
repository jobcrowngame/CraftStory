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
        if (block.Pos.y > 3)
        {
            Debug.LogFormat("Create block [{0}]. \n DT:{1}, Pos:{2}",
           block.BaseData.Name, block.BaseData.DestroyTime, block.Pos);
        }

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

    public bool OutOfMapRangeX(float posX)
    {
        return posX > mData.MapSize.x - 1 || posX < 0;
    }
    public bool OutOfMapRangeZ(float posZ)
    {
        return posZ > mData.MapSize.z - 1 || posZ < 0;
    }
}
