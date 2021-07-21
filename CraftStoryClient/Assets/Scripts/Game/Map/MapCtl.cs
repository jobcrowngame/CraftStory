using JsonConfigData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapCtl
{
    private MapDataFactory mapFactory;
    private Transform mapCellParent;
    private Transform builderPencilParent;
    private Transform effectParent;

    public Transform CellParent { get => mapCellParent; }
    public Transform EffectParent { get => effectParent; }

    public MapCtl()
    {
        mapFactory = new MapDataFactory();
    }

    public void CreateMap(int mapID)
    {
        // ホームデータがない場合デフォルトデータを作る
        if (mapID == 100 && DataMng.E.HomeData == null)
            DataMng.E.HomeData = mapFactory.CreateMapData(mapID);

        DataMng.E.MapData = mapID == 100
            ? DataMng.E.HomeData
            : mapFactory.CreateMapData(mapID);

        mapCellParent = new GameObject("Ground").transform;
        effectParent = new GameObject("Effects").transform;

        var startTime = DateTime.Now;

        InstantiateEntitys(DataMng.E.MapData);

        TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks - startTime.Ticks);
        Logger.Log("map 生成するに {0} かかりました。", elapsedSpan.TotalMilliseconds);
    }
    private void InstantiateEntitys(MapData mData)
    {
        try
        {
            for (int y = 0; y < mData.MapSize.y; y++)
            {
                for (int z = 0; z < mData.MapSize.z; z++)
                {
                    for (int x = 0; x < mData.MapSize.x; x++)
                    {
                        var site = new Vector3Int(x, y, z);
                        if (CheckBlockIsSurface(mData, site))
                        {
                            DataMng.E.MapData.Add(mData.Map[x, y, z], site);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    public void InstantiateTransparenEntitys(BlueprintData blueprint, Vector3Int startPos)
    {
        var shader = Shader.Find("SemiTransparent");
        builderPencilParent = new GameObject("BuilderPancil").transform;
        builderPencilParent.position = startPos;

        foreach (var item in blueprint.blocks)
        {
            var entity = MapData.InstantiateEntity(new MapData.MapCellData() { entityID = item.id, direction = item.direction }, builderPencilParent, item.GetPos());
            entity.transform.localPosition = item.GetPos();

            entity.GetComponent<BoxCollider>().enabled = false;
            var config = ConfigMng.E.Entity[item.id];

            if (shader != null)
            {
                if ((EntityType)config.Type == EntityType.Workbench
                    || (EntityType)config.Type == EntityType.Kamado
                    || (EntityType)config.Type == EntityType.Door
                    || (EntityType)config.Type == EntityType.Torch
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
    public EntityBase CreateEntity(int entityId, Vector3Int pos, DirectionType dType)
    {
        // マップエリア以外ならエラーメッセージを出す。
        if (IsOutRange(DataMng.E.MapData, pos))
        {
            if (pos.y > DataMng.E.MapData.MapSize.y - 1)
            {
                CommonFunction.ShowHintBar(7);
            }
            return null;
        }

        var entity = DataMng.E.MapData.Add(new MapData.MapCellData() { entityID = entityId, direction = (int)dType }, pos);

        CheckNextToEntitys(pos);

        PlayerCtl.E.ConsumableSelectItem();

        return entity;
    }

    public void DeleteEntity(EntityBase entity)
    {
        DataMng.E.MapData.Remove(entity.Pos);
        CheckNextToEntitys(entity.Pos);
    }
    public void DeleteBuilderPencil()
    {
        if(builderPencilParent != null) GameObject.Destroy(builderPencilParent.gameObject);
    }

    /// <summary>
    /// 隣のエンティティをゲット
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

            if (CheckBlockIsSurface(DataMng.E.MapData, entitys[i]))
            {
                var entity = DataMng.E.MapData.GetEntity(entitys[i]);
                if (entity == null)
                {
                    DataMng.E.MapData.Add(entitys[i]);
                }
            }
            else
                DataMng.E.MapData.DeleteObj(entitys[i]);
        }
    }

    #region Static Function

    public static Vector3 FixEntityPos(MapData mData, Vector3 pos, int offset)
    {
        if (pos.x < offset)
            pos.x = offset;

        if (pos.x > mData.MapSize.x - offset)
            pos.x = mData.MapSize.x - offset;

        if (pos.z < offset)
            pos.z = offset;

        if (pos.z > mData.MapSize.z - offset)
            pos.z = mData.MapSize.z - offset;

        return pos;
    }

    public static Vector3Int GetGroundPos(MapData mapData, int posX, int posZ, float offsetY = 0)
    {
        if (posX < 0) posX = UnityEngine.Random.Range(0, mapData.Config.SizeX);
        if (posZ < 0) posZ = UnityEngine.Random.Range(0, mapData.Config.SizeZ);

        int posY = 0;
        for (int i = mapData.Config.SizeY - 1; i >= 0; i--)
        {
            if (mapData.Map[posX, i, posZ].entityID == 0)
                continue;

            posY = (int)(i + 1 + offsetY - 0.5f);
            break;
        }

        return new Vector3Int(posX, posY, posZ);
    }
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
            || ConfigMng.E.Entity[mapData.Map[pos.x, pos.y, pos.z].entityID].Type == (int)EntityType.TransferGate;
    }
    /// <summary>
    /// マップの最大サイズ外
    /// </summary>
    public static bool IsOutRange(MapData mapData, Vector3Int pos)
    {
        return pos.x < 0 || pos.x > mapData.MapSize.x - 1
            || pos.y < 0 || pos.y > mapData.MapSize.y - 1
            || pos.z < 0 || pos.z > mapData.MapSize.z - 1;
    }

    #endregion
}
