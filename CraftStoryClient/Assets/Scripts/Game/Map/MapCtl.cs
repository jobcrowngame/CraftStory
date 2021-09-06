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
    public Transform EffectParent { get => effectParent; }

    public MapCtl()
    {
        mapFactory = new MapDataFactory();
    }

    /// <summary>
    /// マップを生成
    /// </summary>
    public void CreateMap()
    {
        DataMng.E.SetMapData(NowLoadingLG.E.NextMapID);

        mapCellParent = new GameObject("Ground").transform;
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
                        CommonFunction.Instantiate<EntityBlock>("Prefabs/Game/Block/WaterBlock", WorldMng.E.MapCtl.CellParent, new Vector3(x,y,z));
                    }
#endif

                    if (mData.Map[x, y, z].entityID == 0 || mData.Map[x, y, z].entityID == 10000)
                        continue;

                    var site = new Vector3Int(x, y, z);
                    if (CheckBlockIsSurface(mData, site))
                    {
                        DataMng.E.MapData.Add(mData.Map[x, y, z], site);
                    }
                }
            }
        }
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
            var entity = MapData.InstantiateEntity(new MapData.MapCellData() { entityID = item.id, direction = item.direction }, builderPencilParent, item.GetPos());
            entity.transform.localPosition = item.GetPos();

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
                if ((EntityType)config.Type == EntityType.Workbench
                    || (EntityType)config.Type == EntityType.Kamado
                    || (EntityType)config.Type == EntityType.Door
                    || (EntityType)config.Type == EntityType.Torch
                    || (EntityType)config.Type == EntityType.Flower
                    || (EntityType)config.Type == EntityType.BigFlower
                    || (EntityType)config.Type == EntityType.Grass
                    || (EntityType)config.Type == EntityType.Obstacle)
                {
                    List<GameObject> childs = new List<GameObject>();
                    CommonFunction.GetAllChiled(entity.transform, ref childs);
                    foreach (var cell in childs)
                    {
                        var render = cell.GetComponent<Renderer>();
                        if (render == null)
                            continue;

                        render.material.shader = shader;

                        // 重複されるかをチェック
                        if (DataMng.E.MapData.IsNull(Vector3Int.CeilToInt(cell.transform.position)))
                        {
                            render.material.color = new Color(1, 1, 1, 0.5f);
                        }
                        else
                        {
                            render.material.color = new Color(1, 0, 0, 0.5f);
                            blueprint.IsDuplicate = true;
                        }
                    }
                }
                else
                {
                    var render = entity.GetComponent<Renderer>();
                    if (render != null)
                    {
                        render.material.shader = shader;

                        // 重複されるかをチェック
                        if (DataMng.E.MapData.IsNull(Vector3Int.CeilToInt(entity.transform.position)))
                        {

                            render.material.color = new Color(1, 1, 1, 0.5f);
                        }
                        else
                        {
                            render.material.color = new Color(1, 0, 0, 0.5f);
                            blueprint.IsDuplicate = true;
                        }
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

        PlayerCtl.E.UseItem();

        return entity;
    }

    /// <summary>
    /// エンティティを削除
    /// </summary>
    /// <param name="entity"></param>
    public void DeleteEntity(EntityBase entity)
    {
        DataMng.E.MapData.Remove(entity);
        CheckNextToEntitys(entity.Pos);
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
    private void CheckNextToEntitys(Vector3Int pos)
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

            DataMng.E.MapData.IsSurface(entitys[i], CheckBlockIsSurface(DataMng.E.MapData, entitys[i]));
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

        // 大きいエンティティの範囲チェック
        var config = ConfigMng.E.Entity[entityId];
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

            case Direction.right:
                for (int x = 0; x < config.ScaleZ; x++)
                {
                    for (int z = 0; z > -config.ScaleX; z--)
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
                for (int x = 0; x > -config.ScaleZ; x--)
                {
                    for (int z = 0; z < config.ScaleX; z++)
                    {
                        for (int y = 0; y < config.ScaleY; y++)
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
    public static Vector3Int GetGroundPos(MapData mapData, int posX, int posZ, float offsetY = 0)
    {
        if (posX < 0) posX = UnityEngine.Random.Range(0, mapData.SizeX);
        if (posZ < 0) posZ = UnityEngine.Random.Range(0, mapData.SizeZ);

        int posY = 0;
        for (int i = mapData.SizeY - 1; i >= 0; i--)
        {
            if (mapData.Map[posX, i, posZ].entityID == 0)
                continue;

            posY = (int)(i + 1 + offsetY);
            break;
        }

        Vector3Int newPos = new Vector3Int(posX, posY, posZ);
        if (!CheckCreatePos(mapData, newPos))
        {
            // 生成できない座標の場合、５回ループして新しいランダム座標を取得
            for (int k = 0; k < 5; k++)
            {
                posX = UnityEngine.Random.Range(0, mapData.SizeX);
                posZ = UnityEngine.Random.Range(0, mapData.SizeZ);

                posY = 0;
                for (int i = mapData.SizeY - 1; i >= 0; i--)
                {
                    if (mapData.Map[posX, i, posZ].entityID == 0)
                        continue;

                    posY = (int)(i + 1 + offsetY);
                    break;
                }

                newPos = new Vector3Int(posX, posY, posZ);
                if (CheckCreatePos(mapData, newPos))
                    break;
            }
        }

        return newPos;
    }
    /// <summary>
    /// 生成できるかのチェック
    /// </summary>
    /// <param name="pos">座標</param>
    /// <returns></returns>
    private static bool CheckCreatePos(MapData mapData, Vector3 pos)
    {
        Vector3 downEntityPos = new Vector3(pos.x, pos.y - 1, pos.z);
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
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Workbench
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Kamado
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Resources
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Door
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Torch
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.TreasureBox
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Flower
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.BigFlower
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.Grass
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
