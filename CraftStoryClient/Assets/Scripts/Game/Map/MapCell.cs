using JsonConfigData;
using System;
using UnityEngine;
using static MapData;

public class MapCell
{
    MapCellData data;
    Vector3Int localPosition;
    MapInstance map;

    EntityBase entity;

    public MapInstance Map { get => map; }
    public Vector3Int WorldPosition { get => 
        new Vector3Int(localPosition.x + map.OffsetX,
            localPosition.y,
            localPosition.z + map.OffsetZ);
    }
    public Vector3Int LocalPosition { get => localPosition; }

    public bool IsSurface { get => MapMng.IsSurface(data.entityID); }
    public MapCellData Data { get => data; }

    public Entity Config { get => ConfigMng.E.Entity[data.entityID]; }
    public EntityType Type { get => (EntityType)Config.Type; }
    public Direction Direction { get => (Direction)data.direction; }
    public int EntityId { get => data.entityID; }
    public EntityBase Entity 
    { 
        get => entity;
        set
        {
            entity = value;

            RefreshEntity();
        }
    }

    public MapCell(MapInstance map, MapCellData data, Vector3Int localPosition)
    {
        this.data = data;
        this.localPosition = localPosition;
        this.map = map;
    }

    public void SetData(MapCellData data)
    {
        this.data = data;

        Map.Data.SetData(data, LocalPosition);
    }

    private void RefreshEntity()
    {
        if (entity == null)
            return;

        entity.transform.SetParent(map.transform);
        entity.transform.localPosition = localPosition;
        entity.gameObject.SetActive(true);

        // スケールが１以上の場合、他の位置を障害物にする
        var obstacleList = MapTool.GetEntityPosListByDirection(EntityId, localPosition, Direction);
        foreach (var item in obstacleList)
        {
            map.Data.Map[item.x, item.y, item.z] = new MapCellData() { entityID = 10000 };

#if UNITY_EDITOR
            //CommonFunction.Instantiate<EntityBlock>("Prefabs/Game/Block/WaterBlock", WorldMng.E.MapCtl.CellParent, item);
#endif
        }

        // 転送門の場合、保存
        if (Type == EntityType.TransferGate)
        {
            map.TransferGate = entity;
        }
    }

    public int InstanceObj(string reName = "")
    {
        try
        {
            // エンティティが存在する場合、スキップ
            if (Entity != null)
                return 0;

            if (data.entityID < 1 || Type == EntityType.Obstacle)
                return 0;

            if (!MapMng.CheckAroundIsSurface(this))
                return 0;

            // エンティティをインスタンス
            entity = MapTool.InstantiateEntity(this, map.transform, localPosition);

            // 松明の場合、記録
            if (Type == EntityType.Torch)
                Map.TorchDic.Add(this);

            //Logger.Warning("Instance {0},{1}", EntityId, LocalPosition);
            return 1;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return 0;
        }
    }

    public void OnDestroy()
    {
        if (Entity != null) Entity.DestroyObject();
        data.entityID = 0;
        Map.Data.Map[LocalPosition.x, LocalPosition.y, LocalPosition.z].entityID = 0;
        Entity = null;

        // 松明の場合、削除
        if (Type == EntityType.Torch)
            Map.TorchDic.Remove(this);
    }

    public void ActiveAroundBlock()
    {
        // 周りのブロックをアクティブ
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x - 1, WorldPosition.y, WorldPosition.z));
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x + 1, WorldPosition.y, WorldPosition.z));
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x, WorldPosition.y - 1, WorldPosition.z));
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x, WorldPosition.y + 1, WorldPosition.z));
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x, WorldPosition.y, WorldPosition.z - 1));
        MapMng.ActiveMapCell(new Vector3Int(WorldPosition.x, WorldPosition.y, WorldPosition.z + 1));
    }
}