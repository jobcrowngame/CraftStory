using JsonConfigData;
using System;
using System.Collections.Generic;
using System.Threading;
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
    public bool Activeing{ get; set; }
    public bool Actived { get; set; }

    private bool Destroyed { get; set; }

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

        mObjDic = new MapCell[SettingMng.AreaMapSize, SettingMng.AreaMapV3Y, SettingMng.AreaMapSize];
        mTorchDic = new List<MapCell>();

        combineObj = gameObject.AddComponent<CombineMeshCtl>();
        transform.localPosition = new Vector3(MapAreaConfig.OffsetX * SettingMng.AreaMapSize, 0, MapAreaConfig.OffsetZ * SettingMng.AreaMapSize);
    }

    public void InstanceData()
    {
        string mapData = (string)SaveLoadFile.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaId + ".dat");
        if (!string.IsNullOrEmpty(mapData))
        {
            var startTime = DateTime.Now;

            data = new MapData(mapData, MapAreaConfig.MapId);

            TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            Logger.Log("mapData {1} をロードするに {0} かかりました。", elapsedSpan.TotalMilliseconds, PublicPar.AreaMapName + areaId);
        }
        else
        {
            data = MapDataFactory.E.CreateMapData(MapAreaConfig.MapId);
        }
    }
    public void InstanceEntitys()
    {
        var startTime = DateTime.Now;

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x, y, z] = new MapCell(this, Data.Map[x, y, z], localPosition);
                }
            }
        }

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("InstanceEntitys するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }
    public void InstanceObjs()
    {
        var startTime = DateTime.Now;

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    ObjDic[x, y, z].InstanceObj();
                }
            }
        }

        CombineMesh();
        Actived = true;

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("InstanceObjs するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }
    private IEnumerator<int> InstanceEntitysAndObjsIE()
    {
        InstanceData();
        yield return 1;

        var startTime = DateTime.Now;

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x, y, z] = new MapCell(this, Data.Map[x, y, z], localPosition);
                }
            }
            yield return 1;
        }

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("InstanceEntitys するに {0} かかりました。", elapsedSpan.TotalMilliseconds);


        startTime = DateTime.Now;

        Activeing = true;
        int count = 0;
        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    var result = ObjDic[x, y, z].InstanceObj();

                    if (result == 1)
                        count++;

                    if (count == 15)
                    {
                        count = 0;
                        yield return 1;
                    }
                }
            }

            yield return 1;
        }

        CombineMesh();
        Actived = true;
        Activeing = false;

        elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("InstanceObjs するに {0} かかりました。", elapsedSpan.TotalMilliseconds);

        yield return 0;
    }

    public MapCell GetCell(Vector3Int localPosition)
    {
        return mObjDic[localPosition.x, localPosition.y, localPosition.z];
    }
    public MapData.MapCellData GetCellData(Vector3Int localPosition)
    {
        if (Data == null)
            return new MapData.MapCellData() { entityID = 0 };

        if (MapMng.IsOutRange(data.GetMapSize(), localPosition))
            return new MapData.MapCellData() { entityID = 0 };

        return Data.Map[localPosition.x, localPosition.y, localPosition.z];
    }

    public void ActiveInstance()
    {
        Active = true;

        if (!Actived)
        {
            InstanceData();
            InstanceEntitys();
            InstanceObjs();
        }
    }
    public void AsyncActiveInstance()
    {
        Active = true;

        if (!Actived)
        {
            StartCoroutine(InstanceEntitysAndObjsIE());
        }
    }
    

    public void EnActiveInstance()
    {
        Active = false;

        Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(_ =>
        {
            if (Active || Destroyed)
                return;

            OnDestroyInstance();
        });
    }

    private void OnDestroyInstance()
    {
        SaveData();
        Actived = false;

        CommonFunction.ClearCell(transform);
        mObjDic = new MapCell[SettingMng.AreaMapSize, 100, SettingMng.AreaMapSize];
        mTorchDic.Clear();
        combineObj.Clear();

        if (data != null)
        {
            data.ClearMapObj();
            data = null;
        }

        Destroyed = true;
        GameObject.Destroy(gameObject);
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
    public void SaveData()
    {
        if (!Actived || Data == null)
            return;

        Task task = SaveLoadFile.E.Save(Data.ToStringData(), PublicPar.SaveRootPath + PublicPar.AreaMapName + MapAreaConfig.ID + ".dat");
    }
}