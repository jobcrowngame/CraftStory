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

    string areaKey;

    public string AreaKey { get => areaKey; }

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
    public MapArea MapAreaConfig { get => config; }
    private MapArea config;

    public EntityBase TransferGate { get; set; }

    public int OffsetX { get => MapAreaConfig.OffsetX * SettingMng.AreaMapSize; }
    public int OffsetZ { get => MapAreaConfig.OffsetZ * SettingMng.AreaMapSize; }

    public void Init(MapArea config)
    {
        this.config = config;
        areaKey = "x" + config.OffsetX + "z" + config.OffsetZ; ;

        mObjDic = new MapCell[SettingMng.AreaMapSize, SettingMng.AreaMapV3Y, SettingMng.AreaMapSize];
        mTorchDic = new List<MapCell>();

        combineObj = gameObject.AddComponent<CombineMeshCtl>();
        transform.localPosition = new Vector3(MapAreaConfig.OffsetX * SettingMng.AreaMapSize, 0, MapAreaConfig.OffsetZ * SettingMng.AreaMapSize);
    }

    public void InstanceData()
    {
        var startTime = DateTime.Now;

        string mapData = (string)FileIO.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaKey + ".dat");

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("{1} をロードするに {0} かかりました。", elapsedSpan.TotalMilliseconds, PublicPar.AreaMapName + areaKey);

        if (!string.IsNullOrEmpty(mapData))
        {
            startTime = DateTime.Now;

            data = new MapData(mapData, MapAreaConfig.MapId);

            elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            Logger.Log("{1} をInstanceDataするに {0} かかりました。", elapsedSpan.TotalMilliseconds, PublicPar.AreaMapName + areaKey);
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
        if (Actived || Activeing)
            yield break;

        Activeing = true;

        // データを作成
        {
            var startTime = DateTime.Now;


            string mapData = (string)FileIO.E.Load(PublicPar.SaveRootPath + PublicPar.AreaMapName + areaKey + ".dat");
            if (!string.IsNullOrEmpty(mapData))
            {
                MapData mData = new MapData(MapAreaConfig.MapId);

                string[] entitys = mapData.Split(',');
                string[] dataStr;
                int index = 0;

                string[] sizeStr = entitys[entitys.Length - 1].Split('-');
                for (int x = 0; x < mData.SizeX; x++)
                {
                    for (int y = 0; y < mData.SizeY; y++)
                    {
                        for (int z = 0; z < mData.SizeZ; z++)
                        {
                            dataStr = entitys[index++].Split('-');
                            int entityId = int.Parse(dataStr[0]);

                            if (y == 0)
                            {
                                mData.Map[x, y, z] = new MapData.MapCellData() { entityID = 1999 };
                            }
                            else if (ConfigMng.E.Entity[entityId].HaveDirection == 1)
                            {
                                mData.Map[x, y, z] = new MapData.MapCellData() { entityID = entityId, direction = int.Parse(dataStr[1]) };
                            }
                            else
                            {
                                mData.Map[x, y, z] = new MapData.MapCellData() { entityID = entityId };
                            }
                        }
                    }

                    yield return 1;
                }

                data = mData;
            }
            else
            {
                data = MapDataFactory.E.CreateMapData(MapAreaConfig.MapId);
            }

            TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            Logger.Log("{1} をロードするに {0} かかりました。", elapsedSpan.TotalMilliseconds, PublicPar.AreaMapName + areaKey);
        }

        // MapCellを生成
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
                yield return 1;
            }

            var elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            Logger.Log("InstanceEntitys するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
        }

        // InstanceObj
        {
            var startTime = DateTime.Now;

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

            var elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
            Logger.Log("InstanceObjs するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
        }

        yield return 1;
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

        Task task = FileIO.E.Save(Data.ToStringData(), PublicPar.SaveRootPath + PublicPar.AreaMapName + config.ID + ".dat");
    }
}