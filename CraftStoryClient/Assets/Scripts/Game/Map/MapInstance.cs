using JsonConfigData;
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
    public bool Activeing{ get; set; }
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
            return new MapData.MapCellData() { entityID = 0 };

        return Data.Map[localPosition.x, localPosition.y, localPosition.z];
    }

    public void ActiveInstance()
    {
        Active = true;

        if (data == null)
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
        }
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
        if (Actived || Activeing)
            return;

        Activeing = true;

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
                    if (!Active)
                    {
                        Activeing = false;
                        return;
                    }

                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x, y, z] = new MapCell(this, Data.Map[x, y, z], localPosition);
                    ObjDic[x, y, z].InstanceObj();
                }
            }
        }

        CombineMesh();

        Actived = true;
        Activeing = false;
    }

    private System.Collections.IEnumerator InstantiateEntitysIE()
    {
        Logger.Log("Instance Entitys Area:{0}", MapAreaConfig.ID);

        if (data == null)
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
        }

        for (int y = 0; y < Data.GetMapSize().y; y++)
        {
            for (int z = 0; z < Data.GetMapSize().z; z++)
            {
                for (int x = 0; x < Data.GetMapSize().x; x++)
                {
                    if (!Active)
                    {
                        Activeing = false;
                        yield break;
                    }

                    var localPosition = new Vector3Int(x, y, z);
                    ObjDic[x, y, z] = new MapCell(this, Data.Map[x, y, z], localPosition);
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
                    if (!Active)
                    {
                        Activeing = false;
                        yield break;
                    }

                    if (ObjDic[x, y, z] == null)
                    {
                        Logger.Error("area {3} [{0},{1},{2}] is null", x, y, z, areaId);
                        Activeing = false;
                        yield break;
                    }

                    var result = ObjDic[x, y, z].InstanceObj();

                    if (result == 1)
                        count++;

                    if (count == 20)
                    {
                        count = 0;
                        yield return null;
                    }
                }
            }
        }

        CombineMesh();

        Actived = true;
        Activeing = false;
    }

    private void DestroyInstance()
    {
        if (Actived)
        {
            Observable.Timer(TimeSpan.FromSeconds(10)).Subscribe(_ =>
            {
                if (Active)
                    return;

                OnDestroyInstance();
                SaveData();
                Actived = false;
            });
        }
        else
        {
            OnDestroyInstance();
        }
    }
    private void OnDestroyInstance()
    {
        CommonFunction.ClearCell(transform);
        mObjDic = new MapCell[SettingMng.AreaMapSize, 100, SettingMng.AreaMapSize];
        mTorchDic.Clear();
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