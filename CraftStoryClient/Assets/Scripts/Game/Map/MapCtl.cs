using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// マップコンソール
/// </summary>
public class MapCtl
{
    private MapDataFactory mapFactory; // マップ工場
    private Transform mapCellParent; // ブロック親
    private Transform builderPencilParent; // 建物親
    private Transform effectParent; // エフェクト親

    public Transform CellParent { get => mapCellParent; }
    private CombineMeshObj CombineMeshObj { get => CellParent.GetComponent<CombineMeshObj>(); }
    public Transform EffectParent { get => effectParent; }

    private Dictionary<Vector3Int, EntityCrops> cropsList;

    public MapCtl()
    {
        mapFactory = new MapDataFactory();
        cropsList = new Dictionary<Vector3Int, EntityCrops>();

        TimeZoneMng.E.AddTimerEvent03(Update1S);
    }

    private void Update1S()
    {
        foreach (var item in cropsList.Values)
        {
            item.Update1S();
        }
    }

    public void AddCrops(Vector3Int pos, EntityCrops entity)
    {
        cropsList[pos] = entity;
    }
    public void RemoveCrops(Vector3Int pos)
    {
        cropsList.Remove(pos);
        DataMng.E.UserData.CropsTimers.Remove(CommonFunction.Vector3ToString(pos));
    }

    public void ClearCrops()
    {
        cropsList.Clear();
    }

    /// <summary>
    /// マップを生成
    /// </summary>
    public void CreateMap()
    {
        DataMng.E.SetMapData(NowLoadingLG.E.NextMapID);

        mapCellParent = new GameObject("Ground", typeof(CombineMeshObj)).transform;
        effectParent = new GameObject("Effects").transform;

        var startTime = DateTime.Now;

        InstantiateEntitys(DataMng.E.MapData);

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("map 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="mData"></param>
    private void InstantiateEntitys(MapData mData)
    {
        for (int y = 0; y < mData.GetMapSize().y; y++)
        {
            for (int z = 0; z < mData.GetMapSize().z; z++)
            {
                for (int x = 0; x < mData.GetMapSize().x; x++)
                {

#if UNITY_EDITOR
                    if (mData.Map[x, y, z].entityID == 10000)
                    {
                        //CommonFunction.Instantiate<EntityBlock>("Prefabs/Game/Block/WaterBlock", WorldMng.E.MapCtl.CellParent, new Vector3(x, y, z));
                    }
#endif

                    if (mData.Map[x, y, z].entityID == 0 || mData.Map[x, y, z].entityID == 10000)
                        continue;

                    var site = new Vector3Int(x, y, z);
                    if (CheckBlockIsSurface(mData, site))
                    {
                        // ガイドの転送門の場合
                        if (DataMng.E.RuntimeData.MapType == MapType.Guide && mData.Map[x, y, z].entityID == 9999)
                        {
                            GuideLG.E.TransferGateSite = site;
                        }
                        else
                        {
                            DataMng.E.MapData.Add(mData.Map[x, y, z], site);
                        }
                    }
                }
            }
        }

        CombineMesh();
    }

    /// <summary>
    /// 半透明エンティティをインスタンス
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="startPos"></param>
    public void InstantiateTransparenEntitys(BlueprintData blueprint, Vector3Int startPos)
    {
        var shader = Shader.Find("SemiTransparent");
        builderPencilParent = new GameObject("BuilderPancil").transform;
        builderPencilParent.position = startPos;

        foreach (var item in blueprint.blocks)
        {
            // Obstacleなら処理しない
            if ((EntityType)ConfigMng.E.Entity[item.id].Type == EntityType.Obstacle)
                continue;

            var entity = MapData.InstantiateEntity(new MapData.MapCellData() { entityID = item.id, direction = item.direction }, builderPencilParent, item.GetPos(), false);
            entity.transform.localPosition = item.GetPos();

            // 向きによって回転
            if (entity.EConfig.HaveDirection == 1)
            {
                var angle = CommonFunction.GetCreateEntityAngleByDirection((Direction)item.direction);
                entity.transform.localRotation = Quaternion.Euler(0, angle, 0);
            }

            // 半透明場合、Collider を Enabled する
            var collider = entity.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            // サブも同じ
            foreach (Transform cell in entity.transform)
            {
                var cellCollider = cell.GetComponent<BoxCollider>();
                if (cellCollider != null)
                {
                    cellCollider.enabled = false;
                }
            }

            var config = ConfigMng.E.Entity[item.id];
            if (shader != null)
            {
                // 被ってるかのフラグ
                bool IsDuplicate = false;
                // エンティティのPos
                var entityPos = Vector3Int.CeilToInt(entity.transform.position);

                // エンティティ本体が被ってるかのチェック
                IsDuplicate = !DataMng.E.MapData.IsNull(Vector3Int.CeilToInt(entityPos));

                // エンティティサイズが１以上の場合、関連Pos
                var list = GetEntityPosListByDirection(item.id, entityPos, (Direction)item.direction);

                // 関連Posが被ってるかのチェック
                foreach (var pos in list)
                {
                    IsDuplicate = !DataMng.E.MapData.IsNull(Vector3Int.CeilToInt(pos));
                    if (IsDuplicate)
                    {
                        // 設計図が被ってる
                        blueprint.IsDuplicate = true;
                        break;
                    }
                }

                // 重複したらこの設計図重複してる状態にする
                if (IsDuplicate)
                {
                    blueprint.IsDuplicate = true;
                }

                // サブObjectをゲット
                List<GameObject> childs = new List<GameObject>();
                CommonFunction.GetAllChiled(entity.transform, ref childs);

                // サブObjectがない場合
                if (childs.Count == 0)
                {
                    var render = entity.GetComponent<Renderer>();
                    if (render != null)
                    {
                        // 半透明シェーダーに差し替え
                        render.material.shader = shader;
                        render.material.color = IsDuplicate ? new Color(1, 0, 0, 0.5f) : new Color(1, 1, 1, 0.5f);
                    }
                }
                // サブObjectがある場合
                else
                {
                    foreach (var cell in childs)
                    {
                        var render = cell.GetComponent<Renderer>();
                        if (render == null)
                            continue;

                        // 半透明シェーダーに差し替え
                        render.material.shader = shader;
                        render.material.color = IsDuplicate ? new Color(1, 0, 0, 0.5f) : new Color(1, 1, 1, 0.5f);
                    }
                }
            }
        }
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="buildPos"></param>
    public void InstantiateEntitys(BlueprintData blueprint, Vector3Int buildPos)
    {
        if (blueprint == null)
        {
            Logger.Error("blueprint is null");
            return;
        }

        foreach (var item in blueprint.blocks)
        {
            var entity = DataMng.E.MapData.Add(new MapData.MapCellData() { entityID = item.id, direction = item.direction }
                , CommonFunction.Vector3Sum(item.GetPos(), buildPos));
            //CheckNextToEntitys(item.GetPos());
        }

        // メッシュを結合
        WorldMng.E.MapCtl.CombineMesh();
    }
    /// <summary>
    /// エンティティを作成
    /// </summary>
    /// <param name="entityId">エンティティID</param>
    /// <param name="pos">座標</param>
    /// <param name="dType">向き</param>
    /// <returns></returns>
    public EntityBase CreateEntity(int entityId, Vector3Int pos, Direction dType)
    {
        // マップエリア以外ならエラーメッセージを出す。
        if (IsOutRange(DataMng.E.MapData, pos))
        {
            if (pos.y > DataMng.E.MapData.GetMapSize().y - 1)
            {
                CommonFunction.ShowHintBar(7);
            }
            return null;
        }

        // 作成できるかのチェック
        if (!CheckCanCreate(DataMng.E.MapData, entityId, pos, dType))
        {
            CommonFunction.ShowHintBar(19);
            return null;
        }

        var entity = DataMng.E.MapData.Add(new MapData.MapCellData() { entityID = entityId, direction = (int)dType }, pos);

        CheckNextToEntitys(pos);

        PlayerCtl.E.UseSelectItem();

        return entity;
    }

    /// <summary>
    /// エンティティを削除
    /// </summary>
    /// <param name="entity"></param>
    public void DeleteEntity(EntityBase entity)
    {
        //Logger.Warning("DeleteEntity by entity {0}:{1}", entity.gameObject.name, entity.Pos);

        DataMng.E.MapData.RemoveEntity(entity);
        CheckNextToEntitys(entity.Pos);
        CombineMesh();
    }
    public void DeleteEntity(Vector3Int pos)
    {
        EntityBase entity = DataMng.E.MapData.GetEntity(pos);
        if (entity != null)
        {
            DataMng.E.MapData.RemoveEntity(entity);
            //Logger.Warning("DeleteEntity {0}:{1} ", entity.gameObject.name, pos);
        }
    }

    public void RemoveMesh(EntityBase entity)
    {
        CellParent.GetComponent<CombineMeshObj>().RemoveMesh(entity.EntityID, entity.Pos);
    }
    public void CombineMesh()
    {
        if (CellParent == null)
        {
            Logger.Warning("CellParent is null");
            return;
        }

        CombineMeshObj.Combine();
    }
    public void ClearMesh()
    {
        if (CellParent == null)
        {
            Logger.Warning("CellParent is null");
            return;
        }

        CombineMeshObj.Clear();
    }

    /// <summary>
    /// 設計図エンティティを削除
    /// </summary>
    public void DeleteBuilderPencil()
    {
        if(builderPencilParent != null) GameObject.Destroy(builderPencilParent.gameObject);
    }

    /// <summary>
    /// 隣のエンティティをチェック
    /// </summary>
    public void CheckNextToEntitys(Vector3Int pos)
    {
        List<Vector3Int> entitys = new List<Vector3Int>();

        entitys.Add(new Vector3Int(pos.x - 1, pos.y, pos.z));
        entitys.Add(new Vector3Int(pos.x + 1, pos.y, pos.z));
        entitys.Add(new Vector3Int(pos.x, pos.y - 1, pos.z));
        entitys.Add(new Vector3Int(pos.x, pos.y + 1, pos.z));
        entitys.Add(new Vector3Int(pos.x, pos.y, pos.z - 1));
        entitys.Add(new Vector3Int(pos.x, pos.y, pos.z + 1));

        for (int i = 0; i < entitys.Count; i++)
        {
            if (IsOutRange(DataMng.E.MapData, entitys[i]))
                continue;

            DataMng.E.MapData.OnSurface(entitys[i], CheckBlockIsSurface(DataMng.E.MapData, entitys[i]));
        }
    }

    /// <summary>
    /// マップIDによってマップを生成
    /// </summary>
    /// <param name="mapId"></param>
    /// <returns></returns>
    public MapData CreateMapData(int mapId)
    {
        return mapFactory.CreateMapData(mapId);
    }

#region Static Function

    /// <summary>
    /// 作成できるかのチェック
    /// </summary>
    public static bool CheckCanCreate(MapData mdata, int entityId, Vector3Int pos, Direction dType)
    {
        // 被ってるかをチェック
        if (mdata.Map[pos.x, pos.y, pos.z].entityID > 0)
            return false;

        var config = ConfigMng.E.Entity[entityId];
        // 大きいエンティティの範囲チェック
        if (config.ScaleX > 1 || config.ScaleY > 1 || config.ScaleZ > 1)
        {
            var list = GetEntityPosListByDirection(entityId, pos, dType);
            foreach (var item in list)
            {
                // マップ外ならNG
                if (IsOutRange(mdata, item))
                    return false;

                // 作りたい範囲内に他のブロックがある場合、NG
                if (mdata.Map[item.x, item.y, item.z].entityID > 0)
                    return false;
            }
        }

        // サスペンションのチェック
        var downEntityPos = new Vector3Int(pos.x, pos.y - 1, pos.z);
        var downEntityId = DataMng.E.MapData.Map[downEntityPos.x, downEntityPos.y, downEntityPos.z].entityID;

        // 農作物の場合、下のブロックが畑のブロックならOK
        if ((EntityType)config.Type == EntityType.Crops && (EntityType)ConfigMng.E.Entity[downEntityId].Type != EntityType.Firm)
            return false;

        if (downEntityId <= 0)
        {
            return config.CanSuspension == 1;
        }

        return true;
    }

    /// <summary>
    /// 大きさが１以上の場合
    /// 向きによってエンティティリストをゲット
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="pos"></param>
    /// <param name="dType"></param>
    /// <returns></returns>
    public static List<Vector3Int> GetEntityPosListByDirection(int entityId, Vector3Int pos, Direction dType)
    {
        var config = ConfigMng.E.Entity[entityId];
        List<Vector3Int> posList = new List<Vector3Int>();
        switch (dType)
        {
            case Direction.up:
            case Direction.down:
            case Direction.foward:
                for (int x = 0; x < config.ScaleX; x++)
                {
                    for (int z = 0; z < config.ScaleZ; z++)
                    {
                        for (int y = 0; y < config.ScaleY; y++)
                        {
                            if (x == 0 && y == 0 && z == 0) continue;
                            posList.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z + z));
                        }
                    }
                }
                break;

            case Direction.back:
                for (int x = 0; x > -config.ScaleX; x--)
                {
                    for (int z = 0; z > -config.ScaleZ; z--)
                    {
                        for (int y = 0; y < config.ScaleY; y++)
                        {
                            if (x == 0 && y == 0 && z == 0) continue;
                            posList.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z + z));
                        }
                    }
                }
                break;

            case Direction.left:
                for (int y = 0; y < config.ScaleY; y++)
                {
                    for (int x = 0; x > -config.ScaleZ; x--)
                    {
                        for (int z = 0; z < config.ScaleX; z++)
                        {

                            if (x == 0 && y == 0 && z == 0) continue;
                            posList.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z + z));
                        }
                    }
                }
                break;

            case Direction.right:
                for (int y = 0; y < config.ScaleY; y++)
                {
                    for (int x = 0; x < config.ScaleZ; x++)
                    {
                        for (int z = 0; z > -config.ScaleX; z--)
                        {
                            if (x == 0 && y == 0 && z == 0) continue;
                            posList.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z + z));
                        }
                    }
                }
                break;
        }

        return posList;
    }

    /// <summary>
    /// offsetによってエンティティ座標を修正
    /// </summary>
    /// <param name="mData"></param>
    /// <param name="pos"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Vector3 FixEntityPos(MapData mData, Vector3 pos, int offset)
    {
        if (pos.x < offset)
            pos.x = offset;

        if (pos.x > mData.GetMapSize().x - offset)
            pos.x = mData.GetMapSize().x - offset;

        if (pos.z < offset)
            pos.z = offset;

        if (pos.z > mData.GetMapSize().z - offset)
            pos.z = mData.GetMapSize().z - offset;

        pos.y = GetVertexY(mData, (int)pos.x, (int)pos.z);

        return pos;
    }
    /// <summary>
    /// 座標X、Zによって地面Yの座標をゲット
    /// </summary>
    /// <param name="mapData"></param>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    /// <param name="offsetY">偏位量</param>
    /// <returns></returns>
    public static Vector3Int GetGroundPos(MapData mapData, int posX, int posZ, float offsetY = 0, int CreatePosOffset = 3)
    {
        try
        {
            if (posX < 0) posX = UnityEngine.Random.Range(CreatePosOffset, mapData.SizeX - CreatePosOffset);
            if (posZ < 0) posZ = UnityEngine.Random.Range(CreatePosOffset, mapData.SizeZ - CreatePosOffset);

            int posY = GetVertexY(mapData, posX, posZ) + (int)offsetY;

            Vector3Int newPos = new Vector3Int(posX, posY, posZ);
            if (!CheckCreatePos(mapData, newPos))
            {
                // 生成できない座標の場合、５回ループして新しいランダム座標を取得
                for (int k = 0; k < 100; k++)
                {
                    posX = UnityEngine.Random.Range(CreatePosOffset, mapData.SizeX - CreatePosOffset);
                    posZ = UnityEngine.Random.Range(CreatePosOffset, mapData.SizeZ - CreatePosOffset);
                    posY = GetVertexY(mapData, posX, posZ) + (int)offsetY;
                    newPos = new Vector3Int(posX, posY, posZ);
                    newPos = Vector3Int.CeilToInt(FixEntityPos(mapData, newPos, CreatePosOffset));

                    // ゲットした座標に他のEntityがある場合、スキップ
                    if (mapData.Map[newPos.x, newPos.y, newPos.z].entityID > 0)
                        continue;

                    if (CheckCreatePos(mapData, newPos))
                        break;
                }
            }

            return newPos;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return Vector3Int.zero;
        }
    }
    private static int GetVertexY(MapData mapData, int x, int z)
    {
        int y = 0;
        for (int i = mapData.SizeY - 1; i >= 0; i--)
        {
            if (mapData.Map[x, i, z].entityID == 0)
                continue;

            y = i + 1;
            break;
        }
        return y;
    }

    /// <summary>
    /// 生成できるかのチェック
    /// </summary>
    /// <param name="pos">座標</param>
    /// <returns></returns>
    private static bool CheckCreatePos(MapData mapData, Vector3 pos)
    {
        Vector3 downEntityPos = new Vector3(pos.x, pos.y - 1, pos.z);
        if (IsOutRange(mapData, Vector3Int.CeilToInt(downEntityPos))){
            return false;
        }

        var downEntity = mapData.Map[(int)downEntityPos.x, (int)downEntityPos.y, (int)downEntityPos.z];
        return ConfigMng.E.Entity[downEntity.entityID].CanPut == 1;
    }

    /// <summary>
    /// ブロックが表面にあるかチェック
    /// </summary>
    /// <param name="mapData"></param>
    /// <param name="site"></param>
    /// <returns></returns>
    private static bool CheckBlockIsSurface(MapData mapData, Vector3Int site)
    {
        var isSurface = IsSurface(mapData, new Vector3Int(site.x - 1, site.y, site.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(site.x + 1, site.y, site.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(site.x, site.y - 1, site.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(site.x, site.y + 1, site.z));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(site.x, site.y, site.z - 1));
        if (!isSurface) isSurface = IsSurface(mapData, new Vector3Int(site.x, site.y, site.z + 1));
        return isSurface;
    }
    /// <summary>
    /// 表面であるかをチェック
    /// </summary>
    private static bool IsSurface(MapData mapData, Vector3Int pos)
    {
        if (pos.y > mapData.GetMapSize().y - 1)
            return true;

        if (IsOutRange(mapData, pos))
            return false;

        return mapData.Map[pos.x, pos.y, pos.z].entityID == 0 
            || mapData.Map[pos.x, pos.y, pos.z].entityID == (int)EntityType.Obstacle
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Block2
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Block3
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Crops
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Workbench
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Kamado
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Resources
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Door
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Bed
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Torch
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.TreasureBox
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Flower
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.BigFlower
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Grass
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Mission
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.ChargeShop
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.GachaShop
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.ResourceShop
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.BlueprintShop
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.GiftShop
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.DefaltSurfaceEntity
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.HaveDirectionSurfaceEntity
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Blast
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.TransferGate;
    }
    /// <summary>
    /// マップの最大サイズ外
    /// </summary>
    public static bool IsOutRange(MapData mapData, Vector3Int pos)
    {
        return pos.x < 0 || pos.x > mapData.GetMapSize().x - 1
            || pos.y < 0 || pos.y > mapData.GetMapSize().y - 1
            || pos.z < 0 || pos.z > mapData.GetMapSize().z - 1;
    }

#endregion
}
