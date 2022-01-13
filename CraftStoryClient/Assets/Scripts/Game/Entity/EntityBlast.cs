
using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlast : EntityBase
{
    Blast config;
    float curTime;
    Dictionary<int, MapInstance> areaList = new Dictionary<int, MapInstance>();

    public void Set(int entityId)
    {
        foreach (var item in ConfigMng.E.Blast.Values)
        {
            if (item.EntityID == entityId)
            {
                config = item;
                StartTimer();
                break;
            }
        }
    }

    private void StartTimer()
    {
        TimeZoneMng.E.AddTimerEvent01(Update02S);
    }
    private void EndTimer()
    {
        TimeZoneMng.E.RemoveTimerEvent01(Update02S);
        Burst();
    }

    private void Update02S()
    {
        curTime += 0.2f;

        if (curTime >= config.Timer)
        {
            EndTimer();
        }
    }

    private void Burst()
    {
        // add effect
        var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, config.Effect);
        effect.Init(3);

        DestroyEntitys(config.Radius);

        // 自分を壊す
        DestroyObject();
    }

    private void DestroyEntitys(int radius)
    {
        areaList.Clear();

        Dictionary<int, int> addItems = new Dictionary<int, int>();

        var posList = GetDestroyPosList(radius);
        foreach (var pos in posList)
        {
            var entityId = GetEntityIdByPos(pos);
            // 手にいるアイテムを記録
            if (entityId > 0)
            {
                var config = ConfigMng.E.Entity[entityId];
                if (config.ItemID > 0 && (EntityType)config.Type != EntityType.Blast)
                {
                    if (addItems.ContainsKey(config.ItemID))
                    {
                        addItems[config.ItemID] += 1;
                    }
                    else
                    {
                        addItems[config.ItemID] = 1;
                    }
                    // 農業物の場合
                }
                else if ((EntityType)config.Type == EntityType.Crops)
                {
                    EntityCrops entity = (EntityCrops)DataMng.E.MapData.GetEntity(pos);
                    var cropsConfig = ConfigMng.E.GetCropsByEntityID(entity.EConfig.ID);
                    int addItemID = 0;
                    int count = 0;

                    // 追加されるアイテムを決まる
                    if (entity.IsState3)
                    {
                        addItemID = cropsConfig.DestroyAddItem;
                        count = cropsConfig.Count;
                    }
                    else
                    {
                        addItemID = cropsConfig.ItemID;
                        count = 1;
                    }

                    // アイテムリストに追加
                    if (addItems.ContainsKey(addItemID))
                    {
                        addItems[addItemID] += count;
                    }
                    else
                    {
                        addItems[addItemID] = count;
                    }

                    entity.OnRemoveCropsEntity();
                }
            }
        }

        // マップデータ削除
        DestroyObj(radius, posList);

        CombineMesh();
        HomeLG.E.AddItems(addItems);
        foreach (var item in addItems)
        {
            DataMng.E.AddItem(item.Key, item.Value);
        }

        if (HomeLG.E.UI != null) HomeLG.E.UI.RefreshItemBtns();
    }

    private List<Vector3Int> GetDestroyPosList(int radius)
    {
        List<Vector3Int> posList = new List<Vector3Int>();

        var startPos = Vector3Int.CeilToInt(transform.position);

        // 爆破範囲判定
        int minY = config.Type == 1 ? -radius : 0;

        for (int y = minY; y <= radius; y++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    int indexX = startPos.x + x;
                    int indexY = startPos.y + y;
                    int indexZ = startPos.z + z;

                    // 範囲以外ならスキップ
                    var newPos = new Vector3Int(indexX, indexY, indexZ);
                    if (PosIsOutRange(newPos))
                        continue;

                    posList.Add(newPos);
                }
            }
        }

        return posList;
    }

    private bool PosIsOutRange(Vector3Int pos)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            return MapMng.IsOutRange(WorldMng.E.MapMng.MapSize, pos);
        }
        else
        {
            // マップ以外のPos場合、スキップ
            if (pos.x < 0 || pos.x >= DataMng.E.MapData.SizeX ||
                pos.y < 1 || pos.y >= DataMng.E.MapData.SizeY ||
                pos.z < 0 || pos.z >= DataMng.E.MapData.SizeZ ||
                DataMng.E.MapData.Map[pos.x, pos.y, pos.z].entityID == 10000)
                return true;
        }

        return false;
    }

    private int GetEntityIdByPos(Vector3Int pos)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            var cell = MapMng.GetMapCell(pos);
            if (cell == null)
            {
                Logger.Error("Not find MapCell " + pos);
                return 0;
            }

            // 影響エリアを記録
            if (!areaList.ContainsKey(cell.Map.AreaID))
                areaList[cell.Map.AreaID] = cell.Map;

            return cell.EntityId;
        }
        else
        {
            return DataMng.E.MapData.Map[pos.x, pos.y, pos.z].entityID;
        }
    }

    /// <summary>
    /// マップデータ削除
    /// </summary>
    /// <param name="pos"></param>
    private void DestroyObj(int radius, List<Vector3Int> posList)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            Dictionary<int, MapInstance> mapList = new Dictionary<int, MapInstance>();

            foreach (var pos in posList)
            {
                var cell = MapMng.GetMapCell(pos);
                cell.Map.OnDestroyEntity(cell);

                if (!mapList.ContainsKey(cell.Map.AreaID))
                    mapList[cell.Map.AreaID] = cell.Map;
            }

            foreach (var pos in posList)
            {
                MapMng.GetMapCell(pos).ActiveAroundBlock();
            }

            foreach (var item in mapList.Values)
            {
                item.CombineMesh();
            }
        }
        else
        {
            foreach (var pos in posList)
            {
                DataMng.E.MapData.Map[pos.x, pos.y, pos.z] = new MapData.MapCellData() { entityID = 0, direction = 0 };
            }

            foreach (var pos in posList)
            {
                // entityを削除
                WorldMng.E.MapCtl.DeleteEntity(pos);

                // 爆破周りのブロックをチェック
                WorldMng.E.MapCtl.CheckNextToEntitys(pos);
            }
        }
    }

    private void CombineMesh()
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            foreach (var item in areaList.Values)
            {
                item.CombineMesh();
            }
        }
        else
        {
            WorldMng.E.MapCtl.CombineMesh();
        }
    }
}