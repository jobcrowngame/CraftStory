using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using JsonConfigData;

/// <summary>
/// マップデータ
/// </summary>
[Serializable]
public class MapData
{
    private int id; // マップID
    public int SizeX { get; set; } // サイズ
    public int SizeY { get; set; } // サイズ
    public int SizeZ { get; set; } // サイズ

    public Map Config { get => ConfigMng.E.Map[id]; } // マップ設定ファイル

    // マップエンティティ
    public MapCellData[,,] Map { get => map; }
    [NonSerialized]
    private MapCellData[,,] map;

    // インスタンスされたエンティティ
    [NonSerialized]
    Dictionary<Vector3Int, EntityBase> entityDic = new Dictionary<Vector3Int, EntityBase>();

    public EntityBase TransferGate { get; set; }

    public MapData(int mapID)
    {
        id = mapID;
        map = new MapCellData[Config.SizeX, Config.SizeY, Config.SizeZ];
        SizeX = Config.SizeX;
        SizeY = Config.SizeY;
        SizeZ = Config.SizeZ;
    }
    public MapData(string stringData)
    {
        id = 100;
         
        string[] entitys = stringData.Split(',');
        string[] data;
        int index = 0;

        string[] sizeStr = entitys[entitys.Length - 1].Split('-');
        Vector3Int mapSize = new Vector3Int(int.Parse(sizeStr[0]), int.Parse(sizeStr[1]), int.Parse(sizeStr[2]));
        SizeX = mapSize.x;
        SizeY = mapSize.y;
        SizeZ = mapSize.z;

        map = new MapCellData[mapSize.x, mapSize.y, mapSize.z];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    data = entitys[index++].Split('-');
                    int entityId = int.Parse(data[0]);

                    if (ConfigMng.E.Entity[entityId].HaveDirection == 1)
                    {
                        map[x, y, z] = new MapCellData() { entityID = entityId, direction = int.Parse(data[1]) };
                    }
                    else
                    {
                        map[x, y, z] = new MapCellData() { entityID = entityId };
                    }
                }
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                Map[x, 0, z] = new MapCellData() { entityID = 1999 };
            }
        }
    }

    // 空のマップ判定
    public bool IsNull(Vector3Int site)
    {
        return map[site.x, site.y, site.z].entityID == 0;
    }
    // マップサイズゲット
    public Vector3Int GetMapSize()
    {
        return new Vector3Int(SizeX, SizeY, SizeZ);
    }

    /// <summary>
    /// 表面である場合の処理
    /// </summary>
    /// <param name="pos">ブロックの座標</param>
    /// <param name="b">表面の場合　true</param>
    public void OnSurface(Vector3Int pos, bool b = true)
    {
        if (b)
        {
            if (entityDic.ContainsKey(pos))
                return;

            // 表面の場合、エンティティをインスタンス
            var entity = InstantiateEntity(Map[pos.x, pos.y, pos.z], WorldMng.E.MapCtl.CellParent, pos);
            if (entity != null)
            {
                entityDic[pos] = entity;
                //Logger.Warning("AddEntity:{0}", pos);
            }
        }
        else
        {
            // 裏の場合、エンティティのインスタンスを削除
            if (entityDic.ContainsKey(pos))
            {
                RemoveEntity(entityDic[pos], false);
                //Logger.Warning("RemoveEntity:{0}", pos);
            }
        }
    }

    static Dictionary<int, GameObject> blocks = new Dictionary<int, GameObject>();
    
    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="entityCell">エンティティデータ</param>
    /// <param name="parent">親</param>
    /// <param name="pos">座標</param>
    /// <returns></returns>
    public static EntityBase InstantiateEntity(MapCellData entityCell, Transform parent, Vector3Int pos, bool isCombineMesh = true)
    {
        try
        {
            // 実際のエンティティIDがない場合、Pass
            if (entityCell.entityID < 1)
                return null;

            // 設定ファイル
            var config = ConfigMng.E.Entity[entityCell.entityID];
            EntityBase entity = null;
            GameObject obj = null;
            switch ((EntityType)config.Type)
            {
                case EntityType.Obstacle:
                    break;

                case EntityType.Block:
                case EntityType.Block2:
                case EntityType.Block99:
                case EntityType.Firm:
                    if (isCombineMesh)
                    {
                        if (!blocks.TryGetValue(entityCell.entityID, out obj))
                        {
                            obj = CommonFunction.Instantiate(config.Resources, null, pos);
                            obj.gameObject.SetActive(false);
                            blocks[entityCell.entityID] = obj;
                        }

                        var mesh = obj.GetComponent<MeshFilter>();
                        var render = obj.GetComponent<MeshRenderer>();

                        var combineMO = parent.GetComponent<CombineMeshObj>();
                        if (combineMO != null)
                        {
                            combineMO.AddObj(entityCell.entityID, mesh.mesh, render.material, pos);
                        }

                        entity = CommonFunction.Instantiate<EntityBlock>(ConfigMng.E.Entity[0].Resources, parent, pos);
                    }
                    else
                    {
                        entity = CommonFunction.Instantiate<EntityBlock>(config.Resources, parent, pos);
                    }
                    break;

                case EntityType.Grass:
                    entity = CommonFunction.Instantiate<EntityGrass>(config.Resources, parent, pos);
                    break;

                case EntityType.Crops:
                    entity = CommonFunction.Instantiate<EntityCrops>(config.Resources, parent, pos);
                    WorldMng.E.MapCtl.AddCrops(pos, (EntityCrops)entity);
                    break;

                case EntityType.Resources:
                    entity = CommonFunction.Instantiate<EntityResources>(config.Resources, parent, pos);
                    break;

                case EntityType.TransferGate:
                    entity = CommonFunction.Instantiate<EntityTransferGate>(config.Resources, parent, pos);
                    break;

                case EntityType.Torch:
                    if ((Direction)entityCell.direction != Direction.down)
                    {
                        entity = CommonFunction.Instantiate<EntityTorch>(config.Resources, parent, pos);
                        entity.SetDirection((Direction)entityCell.direction);
                    }
                    break;

                case EntityType.TreasureBox:
                    entity = CommonFunction.Instantiate<EntityTreasureBox>(config.Resources, parent, pos);
                    break;

                case EntityType.Flower:
                case EntityType.BigFlower:
                    entity = CommonFunction.Instantiate<EntityFlowoer>(config.Resources, parent, pos);
                    break;

                case EntityType.Workbench:
                case EntityType.Kamado:
                case EntityType.Door:
                case EntityType.Mission:
                case EntityType.ChargeShop:
                case EntityType.GachaShop:
                case EntityType.ResourceShop:
                case EntityType.BlueprintShop:
                case EntityType.GiftShop:
                    entity = CommonFunction.Instantiate<EntityFunctionalObject>(config.Resources, parent, pos);
                    break;

                case EntityType.DefaltEntity:
                case EntityType.DefaltSurfaceEntity:
                    entity = CommonFunction.Instantiate<EntityDefalt>(config.Resources, parent, pos);
                    break;

                case EntityType.HaveDirectionEntity:
                case EntityType.HaveDirectionSurfaceEntity:
                    entity = CommonFunction.Instantiate<EntityHaveDirection>(config.Resources, parent, pos);
                    break;

                case EntityType.Bed:
                    entity = CommonFunction.Instantiate<EntityBed>(config.Resources, parent, pos);
                    break;

                case EntityType.Blast:
                    var entityBlast = CommonFunction.Instantiate<EntityBlast>(config.Resources, parent, pos);
                    entityBlast.Set(config.ID);
                    entity = entityBlast;
                    break;


                default: Logger.Error("not find entityType "+ (EntityType)config.Type); break;
            }

            if (entity != null)
            {
                entity.EntityID = entityCell.entityID;
                entity.Pos = pos;
                entity.Direction = (Direction)entityCell.direction;
                entity.Init();

                if (entity.EConfig.HaveDirection == 1)
                {
                    var angle = CommonFunction.GetCreateEntityAngleByDirection((Direction)entityCell.direction);
                    entity.transform.localRotation = Quaternion.Euler(0, angle, 0);
                }
            }

            return entity;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return null;
        }
    }

    /// <summary>
    /// エンティティをゲット
    /// </summary>
    /// <param name="pos"></param>
    public EntityBase GetEntity(Vector3Int pos)
    {
        EntityBase entity = null;
        entityDic.TryGetValue(pos, out entity);
        return entity;
    }
    /// <summary>
    /// エンティティ追加
    /// </summary>
    /// <param name="entityCell">エンティティデータ</param>
    /// <param name="pos">座標</param>
    /// <returns></returns>
    public EntityBase Add(MapCellData entityCell, Vector3Int pos)
    {
        if (entityCell.entityID < 1)
            return null;

        if (entityDic.ContainsKey(pos))
        {
            return entityDic[pos];
        }

        var config = ConfigMng.E.Entity[entityCell.entityID];
        if ((EntityType)config.Type == EntityType.Obstacle)
            return null;

        var entity = InstantiateEntity(entityCell, WorldMng.E.MapCtl.CellParent, pos);
        if (entity != null)
        {
            entityDic[pos] = entity;
            Map[pos.x, pos.y, pos.z] = entityCell;
        }

        var obstacleList = MapCtl.GetEntityPosListByDirection(entityCell.entityID, pos, (Direction)entityCell.direction);
        foreach (var item in obstacleList)
        {
            map[item.x, item.y, item.z] = new MapCellData() { entityID = 10000 };

#if UNITY_EDITOR
            //CommonFunction.Instantiate<EntityBlock>("Prefabs/Game/Block/WaterBlock", WorldMng.E.MapCtl.CellParent, item);
#endif
        }

        // 転送門の場合、保存
        if ((EntityType)config.Type == EntityType.TransferGate)
        {
            TransferGate = entity;
            TransferGate.gameObject.SetActive(false);
        }

        return entity;
    }
    /// <summary>
    /// エンティティ削除
    /// </summary>
    /// <param name="entity">エンティティ</param>
    /// <param name="removeMapdat">マップデータを削除するか</param>
    public void RemoveEntity(EntityBase entity, bool removeMapdat = true)
    {
        try
        {
            // マップ範囲以外のエンティティチェック
            if (MapCtl.IsOutRange(this, entity.Pos))
            {
                Logger.Error("Remove mapdata file." + entity.Pos);
                return;
            }

            if ((EntityType)entity.EConfig.Type == EntityType.Obstacle)
                return;

            // マップデータを削除
            if (removeMapdat)
                map[entity.Pos.x, entity.Pos.y, entity.Pos.z].entityID = 0;

            // 阻害を削除
            var obstacleList = MapCtl.GetEntityPosListByDirection(entity.EntityID, entity.Pos, entity.direction);
            foreach (var item in obstacleList)
            {
                map[item.x, item.y, item.z].entityID = 0;
            }

            if (entityDic.ContainsKey(entity.Pos))
            {
                GameObject.Destroy(entity.gameObject);
                entityDic.Remove(entity.Pos);
            }
            else
            {
                Logger.Warning("Remove entity Failure");
            }

            if ((EntityType)entity.EConfig.Type == EntityType.Block ||
                (EntityType)entity.EConfig.Type == EntityType.Block2 ||
                (EntityType)entity.EConfig.Type == EntityType.Block99)
            {
                WorldMng.E.MapCtl.RemoveMesh(entity);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
    /// <summary>
    /// 全インスタンス削除
    /// </summary>
    public void ClearMapObj()
    {
        entityDic.Clear();
        blocks.Clear();

    }

    /// <summary>
    /// String型に変換
    /// </summary>
    /// <returns></returns>
    public string ToStringData()
    {
        StringBuilder sb = new StringBuilder();

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    int entityId = map[x, y, z].entityID;

                    if (ConfigMng.E.Entity[entityId].HaveDirection == 1)
                    {
                        sb.Append(entityId + "-" + map[x, y, z].direction + ",");
                    }
                    else
                    {
                        sb.Append(entityId + ",");
                    }
                }
            }
        }
        sb.Append(string.Format("{0}-{1}-{2}", SizeX, SizeY, SizeZ));


        var strMap = sb.ToString();
        //Logger.Log("Map");
        //Logger.Log(strMap);
        return strMap;
    }

    /// <summary>
    /// クリアする場合
    /// </summary>
    public void OnClear()
    {
        if (TransferGate != null)
        {
            TransferGate.gameObject.SetActive(true);
            PlayerCtl.E.Character.ShowArrow(TransferGate.transform);
        }
    }

    [Serializable]
    public struct MapCellData
    {
        public int entityID; // エンティティID
        public int direction; // 向き
    }
}

/// <summary>
/// マップタイプ
/// </summary>
public enum MapType
{
    /// <summary>
    /// ホーム
    /// </summary>
    Home = 1,

    /// <summary>
    /// チュートリアル
    /// </summary>
    /// 
    Guide = 2,
    /// <summary>
    /// 冒険
    /// </summary>
    Brave = 3, 

    /// <summary>
    /// フレンドホーム
    /// </summary>
    FriendHome = 4,

    /// <summary>
    /// 市場
    /// </summary>
    Market = 5,

    /// <summary>
    /// イベント
    /// </summary>
    Event = 7,

    /// <summary>
    /// テスト
    /// </summary>
    Test = 99,
}