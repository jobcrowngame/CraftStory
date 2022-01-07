using JsonConfigData;
using System;
using System.Collections.Generic;
using UnityEngine;


public class MapInstance : MonoBehaviour
{

    public CombineMeshCtl CombineMeshCtl { get => combineObj; }
    CombineMeshCtl combineObj;

    int areaId;

    public int AreaID { get => areaId; }

    public MapData Data { get => data; }
    MapData data;

    public bool Active { get; set; }
    public bool Actived { get; set; }

    public Dictionary<Vector3Int, MapCell> ObjDic { get => objDic; }
    Dictionary<Vector3Int, MapCell> objDic;

    public MapArea MapAreaConfig { get => ConfigMng.E.MapArea[areaId]; }

    public EntityBase TransferGate { get; set; }

    public int OffsetX { get => MapAreaConfig.OffsetX * SettingMng.AreaMapSize; }
    public int OffsetZ { get => MapAreaConfig.OffsetZ * SettingMng.AreaMapSize; }

    public void Init(int areaId)
    {
        this.areaId = areaId;

        objDic = new Dictionary<Vector3Int, MapCell>();

        combineObj = gameObject.AddComponent<CombineMeshCtl>();
        transform.localPosition = new Vector3(MapAreaConfig.OffsetX * SettingMng.AreaMapSize, 0, MapAreaConfig.OffsetZ * SettingMng.AreaMapSize);

        // ローカルデータロード
        var mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaId + ".dat");
        if (!string.IsNullOrEmpty(mapData))
        {
            data = new MapData(mapData, MapAreaConfig.MapId);
        }
        else
        {
            data = MapDataFactory.E.CreateMapData(MapAreaConfig.MapId);
        }
    }

    public MapCell GetCell(Vector3Int localPosition)
    {
        if (ObjDic.ContainsKey(localPosition))
        {
            return ObjDic[localPosition];
        }
        return null;
    }

    private void OnDestroy()
    {
        StopCoroutine(InstantiateEntitysIE());
    }

    public void Execution(bool isAsync = true)
    {
        if (Active)
        {
            Instance(isAsync);
        }
        else
        {
            DestroyInstance();
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="isAsync"></param>
    private void Instance(bool isAsync = true)
    {
        if (Actived)
            return;

        Actived = true;

        if (isAsync)
        {
            StartCoroutine(InstantiateEntitysIE());
        }
        else
        {
            InstantiateEntitys();
        }
    }
    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="mData"></param>
    private void InstantiateEntitys()
    {
        Logger.Log("Instance Entitys Area:{0}", MapAreaConfig.ID);

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[localPosition] = new MapCell(this, Data.Map[x, y, z], localPosition);
                }
            }
        }

        foreach (var cell in ObjDic.Values)
        {
            cell.InstanceObj();
        }

        CombineMesh();
    }
    private System.Collections.IEnumerator InstantiateEntitysIE()
    {
        Logger.Log("Instance Entitys Area:{0}", MapAreaConfig.ID);

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[localPosition] = new MapCell(this, Data.Map[x, y, z], localPosition);
                }
            }
        }

        foreach (var cell in ObjDic.Values)
        {
            var result = cell.InstanceObj();
            if (result == 1) yield return null;

            if (ObjDic == null || ObjDic.Count == 0 || !Actived)
                break;
        }

        CombineMesh();
        yield return null;
    }

    private void DestroyInstance()
    {
        Actived = false;
        StopCoroutine(InstantiateEntitysIE());

        CommonFunction.ClearCell(transform);
        objDic.Clear();
        combineObj.Clear();
    }

    /// <summary>
    /// メッシュを結合
    /// </summary>
    public void CombineMesh()
    {
        combineObj.Combine();
    }

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="pos"></param>
    public void OnDestroyEntity(MapCell mapCell)
    {
        if (mapCell == null || mapCell.Config.CanDestroy == 0)
            return;

        //Logger.Warning("Destroy {0},{1},{2}", mapCell.EntityId, mapCell.WorldPosition, mapCell.Entity.name);

        if (mapCell.Entity != null)
        {
            CombineMeshCtl.RemoveMesh(mapCell.EntityId, mapCell.LocalPosition);
        }

        Data.SetData(new MapData.MapCellData() { entityID = 0 }, mapCell.LocalPosition);
        mapCell.OnDestroy();
    }
}