﻿using JsonConfigData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
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

    //public Dictionary<Vector3Int, MapCell> ObjDic { get => mObjDic; }
    //Dictionary<Vector3Int, MapCell> mObjDic;
    public MapCell[,,] ObjDic { get => mObjDic; }
    MapCell[,,] mObjDic;


    public List<MapCell> TorchDic { get => mTorchDic; }
    List<MapCell> mTorchDic;
    public MapArea MapAreaConfig { get => ConfigMng.E.MapArea[areaId]; }

    public EntityBase TransferGate { get; set; }

    public int OffsetX { get => MapAreaConfig.OffsetX * SettingMng.AreaMapSize; }
    public int OffsetZ { get => MapAreaConfig.OffsetZ * SettingMng.AreaMapSize; }

    public void Init(int areaId)
    {
        this.areaId = areaId;

        mObjDic = new MapCell[SettingMng.AreaMapSize,100, SettingMng.AreaMapSize];
        mTorchDic = new List<MapCell>();

        combineObj = gameObject.AddComponent<CombineMeshCtl>();
        transform.localPosition = new Vector3(MapAreaConfig.OffsetX * SettingMng.AreaMapSize, 0, MapAreaConfig.OffsetZ * SettingMng.AreaMapSize);
    }

    public MapCell GetCell(Vector3Int localPosition)
    {
        return mObjDic[localPosition.x, localPosition.y, localPosition.z];
    }
    public MapData.MapCellData GetCellData(Vector3Int localPosition)
    {
        if (Data == null)
            return new MapData.MapCellData() { entityID = -1 };

        return Data.Map[localPosition.x, localPosition.y, localPosition.z];
    }

    private void OnDestroy()
    {
        SaveData(true);
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

    string mapData;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isAsync"></param>
    private void Instance(bool isAsync = true)
    {
        if (Actived)
            return;

        if (isAsync)
        {
            //引数にstringが渡せるSubjectを作成
            var sub = new Subject<int>();

            //onNextで通常時の処理、onErrorでエラー時の処理、onCompletedで終了時の処理を登録
            sub.Subscribe(
                onNext: text =>
                {
                    mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaId + ".dat");
                },
                onError: error => Debug.Log("エラー！ : " + error),
                onCompleted: () =>
                {
                    if (!string.IsNullOrEmpty(mapData))
                    {
                        data = new MapData(mapData, MapAreaConfig.MapId);
                    }
                    else
                    {
                        data = MapDataFactory.E.CreateMapData(MapAreaConfig.MapId);
                    }

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
            );

            sub.OnNext(1);
            sub.OnCompleted();
        }
        else
        {
            string mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaId + ".dat");
            if (!string.IsNullOrEmpty(mapData))
            {
                data = new MapData(mapData, MapAreaConfig.MapId);
            }
            else
            {
                data = MapDataFactory.E.CreateMapData(MapAreaConfig.MapId);
            }

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
                    if (!Actived)
                        break;

                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x,y,z] = new MapCell(this, Data.Map[x, y, z], localPosition);
                    ObjDic[x, y, z].InstanceObj();
                }
            }
        }

        if (Actived)
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
                    if (!Actived)
                        break;

                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x,y,z] = new MapCell(this, Data.Map[x, y, z], localPosition);
                }
            }
                yield return null;
        }

        int count = 0;
        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    if (!Actived)
                        break;

                    if (ObjDic[x, y, z] == null)
                    {
                        Logger.Error("area {3} [{0},{1},{2}] is null", x, y, z, areaId);
                        continue;
                    }

                    var result = ObjDic[x,y,z].InstanceObj();

                    if (result == 1)
                        count++;

                    if (count == 10)
                    {
                        count = 0;
                        yield return null;
                    }
                }
            }
        }

        if (Actived)
            CombineMesh();
    }

    private void DestroyInstance()
    {
        Actived = false;
        StopCoroutine(InstantiateEntitysIE());

        CommonFunction.ClearCell(transform);
        mObjDic = new MapCell[SettingMng.AreaMapSize, 100, SettingMng.AreaMapSize];
        mTorchDic.Clear();
        combineObj.Clear();

        SaveData(true);
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

    /// <summary>
    /// データセーブ
    /// </summary>
    public void SaveData(bool deleteData = false)
    {
        if (!Actived || Data == null)
            return;

        Task task = SaveLoadFile.E.Save(Data.ToStringData(), PublicPar.SaveRootPath + PublicPar.AreaMapName + MapAreaConfig.ID + ".dat", () =>
        {
            if (deleteData) 
                data = null;
        });
    }
}