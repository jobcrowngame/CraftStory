using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MapMng
{
    private static Transform builderPencilParent; // 建物親

    public Transform MapParent { get; private set; }
    public Transform EffectParent { get; private set; }
    public MapEntitysPool MapPool { get; set; }
    public MapInstance[,] MapInstanceArr { get => mapInstanceArr; }

    MapInstance[,] mapInstanceArr;
    private int loadAreaRange = 2;

    private Dictionary<Vector3Int, EntityCrops> cropsList = new Dictionary<Vector3Int, EntityCrops>();
    private Dictionary<Vector3Int, EntityCrops> areaMapCropsList = new Dictionary<Vector3Int, EntityCrops>();

    public MapMng()
    {
        TimeZoneMng.E.AddTimerEvent03(Update1S);
    }

    public int IndexX
    {
        get => DataMng.E.UserData.AreaIndexX;
        set
        {
            if (value == DataMng.E.UserData.AreaIndexX)
                return;

            AreaChangeX(DataMng.E.UserData.AreaIndexX, value);

            DataMng.E.UserData.AreaIndexX = value;
        }
    }
    public int IndexZ
    {
        get => DataMng.E.UserData.AreaIndexZ;
        set
        {
            if (value == DataMng.E.UserData.AreaIndexZ)
                return;

            AreaChangeZ(DataMng.E.UserData.AreaIndexZ, value);

            DataMng.E.UserData.AreaIndexZ = value;
        }
    }

    // マップサイズ
    private int minX, maxX, minZ, maxZ, maxY;
    public Vector3Int MapSize 
    {
        get 
        {
            if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            {
                return new Vector3Int(maxX, mapInstanceArr[0, 0].Data.GetMapSize().y, maxZ);
            }
            else
            {
                return DataMng.E.MapData.GetMapSize();
            }
        }
    }

    public void Init()
    {
        MapTool.Clear();

        MapPool = new GameObject("MapPool").AddComponent<MapEntitysPool>();
        MapPool.Init();

        MapParent = new GameObject("MapParent").transform;
        EffectParent = new GameObject("Effects").transform;

        minX = SettingMng.MoveBoundaryOffset;
        minZ = SettingMng.MoveBoundaryOffset;

        mapInstanceArr = new MapInstance[SettingMng.AreaMapScaleX, SettingMng.AreaMapScaleZ];
        foreach (var area in ConfigMng.E.MapArea.Values)
        {
            var areaInstance = new GameObject("Ground_" + area.ID, typeof(MapInstance)).GetComponent<MapInstance>();
            areaInstance.transform.SetParent(MapParent);
            areaInstance.Init(area.ID);

            mapInstanceArr[area.OffsetX, area.OffsetZ] = areaInstance;

            // マップサイズX
            if (area.OffsetX * SettingMng.AreaMapSize + SettingMng.AreaMapSize > maxX) maxX 
                    = area.OffsetX * SettingMng.AreaMapSize + SettingMng.AreaMapSize;

            // マップサイズZ
            if (area.OffsetZ * SettingMng.AreaMapSize + SettingMng.AreaMapSize > maxZ) maxZ 
                    = area.OffsetZ * SettingMng.AreaMapSize + SettingMng.AreaMapSize;
        }

        AreaInit(DataMng.E.UserData.PlayerPositionX, DataMng.E.UserData.PlayerPositionZ);

        // 設計図プレイビューコンソールObjectを追加
        if (DataMng.E.RuntimeData.MapType == MapType.Home ||
            DataMng.E.RuntimeData.MapType == MapType.AreaMap ||
            DataMng.E.RuntimeData.MapType == MapType.Market ||
            DataMng.E.RuntimeData.MapType == MapType.Guide)
        {
            PlayerCtl.E.BlueprintPreviewCtl = BlueprintPreviewCtl.Instantiate();
        }
    }

    public void ClearMesh()
    {
        if (MapInstanceArr == null)
            return;

        foreach (var item in MapInstanceArr)
        {
            item.CombineMeshCtl.Clear();
        }
    }



    /// <summary>
    /// エリア変更
    /// </summary>
    public void AreaChangeX(int from, int to)
    {
        Logger.Log("Area map index change X: {0}➡{1}", from, to);

        for (int z = IndexZ - loadAreaRange; z <= IndexZ + loadAreaRange; z++)
        {
            for (int x = from - loadAreaRange; x <= from + loadAreaRange; x++)
            {
                if (x < 0 || z < 0 || x >= SettingMng.AreaMapScaleX || z >= SettingMng.AreaMapScaleX)
                    continue;

                mapInstanceArr[x, z].Active = false;
            }
        }

        for (int z = IndexZ - loadAreaRange; z <= IndexZ + loadAreaRange; z++)
        {
            for (int x = to - loadAreaRange; x <= to + loadAreaRange; x++)
            {
                if (x < 0 || z < 0 || x >= SettingMng.AreaMapScaleX || z >= SettingMng.AreaMapScaleX)
                    continue;

                mapInstanceArr[x, z].Active = true;
            }
        }

        foreach (var item in mapInstanceArr)
        {
            item.Execution();
        }
    }
    public void AreaChangeZ(int from, int to)
    {
        Logger.Log("Area map index change Z: {0}➡{1}", from, to);

        for (int x = IndexX - loadAreaRange; x <= IndexX + loadAreaRange; x++)
        {
            for (int z = from - loadAreaRange; z <= from + loadAreaRange; z++)
            {
                if (x < 0 || z < 0 || x >= SettingMng.AreaMapScaleZ || z >= SettingMng.AreaMapScaleZ)
                    continue;

                mapInstanceArr[x, z].Active = false;
            }
        }

        for (int x = IndexX - loadAreaRange; x <= IndexX + loadAreaRange; x++)
        {
            for (int z = to - loadAreaRange; z <= to + loadAreaRange; z++)
            {
                if (x < 0 || z < 0 || x >= SettingMng.AreaMapScaleZ || z >= SettingMng.AreaMapScaleZ)
                    continue;

                mapInstanceArr[x, z].Active = true;
            }
        }

        foreach (var item in mapInstanceArr)
        {
            item.Execution();
        }
    }
    public void AreaInit(int posX, int posZ)
    {
        int indexX = posX / SettingMng.AreaMapSize;
        int indexZ = posZ / SettingMng.AreaMapSize;

        if (indexX < 0 || indexZ < 0 || indexX >= SettingMng.AreaMapScaleX || indexZ >= SettingMng.AreaMapScaleZ)
        {
            Logger.Warning("bad index X,Y [{0},{1}]", IndexX, IndexZ);
            return;
        }
        else
        {
            DataMng.E.UserData.AreaIndexX = indexX;
            DataMng.E.UserData.AreaIndexZ = indexZ;

            for (int z = -loadAreaRange; z <= loadAreaRange; z++)
            {
                for (int x = -loadAreaRange; x <= loadAreaRange; x++)
                {
                    if (indexX + x < 0 || indexZ + z < 0 || indexX + x >= SettingMng.AreaMapScaleX || indexZ + z >= SettingMng.AreaMapScaleZ)
                        continue;

                    mapInstanceArr[indexX + x, indexZ + z].Active = true;
                    mapInstanceArr[indexX + x, indexZ + z].Execution(false);
                }
            }
        }
    }

    ///// <summary>
    ///// マップを追加
    ///// </summary>
    ///// <param name="areaId"></param>
    //private void AddMapDataToAreaDic(int areaId)
    //{
    //    var areaInstance = new GameObject("Ground", typeof(MapInstance)).GetComponent<MapInstance>();
    //    areaInstance.transform.SetParent(MapParent);
    //    areaInstance.Init(areaId);

    //    mapDic[areaId] = areaInstance;

    //    var config = ConfigMng.E.MapArea[areaId];
    //    if (config.OffsetX < minX) minX = config.OffsetX;
    //    if (config.OffsetX + 50 > maxX) maxX = config.OffsetX + 50;
    //    if (config.OffsetZ < minZ) minZ = config.OffsetZ;
    //    if (config.OffsetZ + 50 > maxZ) maxZ = config.OffsetZ + 50;
    //}

    ///// <summary>
    ///// マップを削除
    ///// </summary>
    ///// <param name="areaId"></param>
    //private void RemoveMapDataFromAreaDic(int areaId)
    //{
    //    mapDic[areaId].DestroyInstance();
    //    mapDic.Remove(areaId);
    //}

    public Vector3Int GetPlayerGroundPos(int offsetY = 3)
    {
        int x = DataMng.E.UserData.PlayerPositionX;
        int z = DataMng.E.UserData.PlayerPositionZ;
        return GetGroundPos(x, z, offsetY);
    }

    /// <summary>
    /// 移動範囲境界判断　X
    /// </summary>
    public bool IsMapAreaOutX(float cur, float add)
    {
        try
        {
            return cur+ add > maxX - SettingMng.MoveBoundaryOffset || cur + add <= minX;
        }
        catch (System.Exception ex)
        {
            Logger.Warning(ex.Message);
            return true;
        }

    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    public bool IsMapAreaOutZ(float cur, float add)
    {
        try
        {
            return cur + add > maxZ - SettingMng.MoveBoundaryOffset || cur + add <= minZ;
        }
        catch (System.Exception ex)
        {
            Logger.Warning(ex.Message);
            return true;
        }
    }

    public void CheckArea(Transform playerTrans)
    {
        IndexX = (int)(playerTrans.position.x / SettingMng.AreaMapSize);
        IndexZ = (int)(playerTrans.position.z / SettingMng.AreaMapSize);

        DataMng.E.UserData.PlayerPositionX = (int)(playerTrans.position.x);
        DataMng.E.UserData.PlayerPositionZ = (int)(playerTrans.position.z);
    }

    public void SaveData()
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap || mapInstanceArr == null)
            return;

        foreach (var item in mapInstanceArr)
        {
            item.SaveData();
        }
    }

    public EntityBase CreateEntity(int entityId, Vector3Int pos, Direction dType)
    {
        // マップエリア以外ならエラーメッセージを出す。
        if (IsOutRange(WorldMng.E.MapMng.MapSize, pos))
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

        var cell = GetMapCell(pos);
        if (cell != null)
        {
            cell.SetData(new MapData.MapCellData() { entityID = entityId, direction = (int)dType });
            cell.InstanceObj();
        }

        PlayerCtl.E.UseSelectItem();

        cell.Map.CombineMesh();

        return cell.Entity;
    }

    public List<MapCell> GetTorchs()
    {
        List<MapCell> list = new List<MapCell>();
        foreach (var map in mapInstanceArr)
        {
            list.AddRange(map.TorchDic);
        }

        return list;
    }

    public void DeleteEntity(EntityBase entity)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            var cell = GetMapCell(entity.WorldPos);
            cell.Map.OnDestroyEntity(cell);
            cell.Map.CombineMesh();
        }
        else
        {
            WorldMng.E.MapCtl.DeleteEntity(entity);
        }
    }

    private void Update1S()
    {
        foreach (var item in cropsList.Values)
        {
            if (item == null || item.IsDestroy)
                continue;
            
            item.Update1S();
        }

        foreach (var item in areaMapCropsList.Values)
        {
            if (item == null || item.IsDestroy)
                continue;

            item.Update1S();
        }
    }
    public void AddCrops(Vector3Int pos, EntityCrops entity)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            areaMapCropsList[pos] = entity;
        }
        else
        {
            cropsList[pos] = entity;
        }
    }
    public void RemoveCrops(Vector3Int pos)
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            areaMapCropsList.Remove(pos);
            DataMng.E.UserData.CropsTimers.Remove(CommonFunction.Vector3ToString(pos));
        }
        else
        {
            cropsList.Remove(pos);
            DataMng.E.UserData.CropsTimers.Remove(CommonFunction.Vector3ToString(pos));
        }
    }
    public void ClearCrops()
    {
        areaMapCropsList.Clear();
        cropsList.Clear();
    }

    #region static function

    public static MapCell GetMapCell(Vector3Int worldPosition)
    {
        if (IsOutRange(WorldMng.E.MapMng.MapSize, worldPosition))
            return null;

        int indexX = worldPosition.x / SettingMng.AreaMapSize;
        int indexZ = worldPosition.z / SettingMng.AreaMapSize;

        int localPosX = worldPosition.x % SettingMng.AreaMapSize;
        int localPosZ = worldPosition.z % SettingMng.AreaMapSize;

        return WorldMng.E.MapMng.mapInstanceArr[indexX, indexZ].GetCell(new Vector3Int(localPosX, worldPosition.y, localPosZ));
    }
    public static MapData.MapCellData GetMapCellData(Vector3Int worldPosition)
    {
        try
        {
            if (IsOutRange(WorldMng.E.MapMng.MapSize, worldPosition))
                return new MapData.MapCellData() { entityID = -1 };

            int indexX = worldPosition.x / SettingMng.AreaMapSize;
            int indexZ = worldPosition.z / SettingMng.AreaMapSize;

            int localPosX = worldPosition.x % SettingMng.AreaMapSize;
            int localPosZ = worldPosition.z % SettingMng.AreaMapSize;

            if (indexX > SettingMng.AreaMapScaleX - 1 || indexZ > SettingMng.AreaMapScaleZ - 1)
                return new MapData.MapCellData() { entityID = -1 };

            return WorldMng.E.MapMng.mapInstanceArr[indexX, indexZ].GetCellData(new Vector3Int(localPosX, worldPosition.y, localPosZ));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            throw;
        }
    }
    public static MapInstance GetMapInstance(Vector3Int worldPosition)
    {
        int indexX = worldPosition.x / SettingMng.AreaMapSize;
        int indexZ = worldPosition.z / SettingMng.AreaMapSize;
        return WorldMng.E.MapMng.MapInstanceArr[indexX, indexZ];
    }

    public static void ActiveMapCell(Vector3Int worldPosition)
    {
        var cell = GetMapCell(worldPosition);
        if (cell != null)
        {
            cell.InstanceObj();
        }
    }
    public static bool CheckAroundIsSurface(MapCell cell)
    {
        if (cell == null)
            return false;

        bool isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x + 1, cell.WorldPosition.y, cell.WorldPosition.z));
        if (!isSurface) isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x - 1, cell.WorldPosition.y, cell.WorldPosition.z));
        if (!isSurface) isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x, cell.WorldPosition.y + 1, cell.WorldPosition.z));
        if (!isSurface) isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x, cell.WorldPosition.y - 1, cell.WorldPosition.z));
        if (!isSurface) isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x, cell.WorldPosition.y, cell.WorldPosition.z + 1));
        if (!isSurface) isSurface = CheckAroundIsSurface(new Vector3Int(cell.WorldPosition.x, cell.WorldPosition.y, cell.WorldPosition.z - 1));
        return isSurface;
    }
    private static bool CheckAroundIsSurface(Vector3Int WorldPosition)
    {
        try
        {
            if (IsOutRange(WorldMng.E.MapMng.MapSize, WorldPosition))
                return false;

            var cellData = GetMapCellData(WorldPosition);
            if (cellData.entityID < 0)
                return false;

            return IsSurface(cellData.entityID);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return false;
        }
    }

    public static bool IsSurface(int entityId)
    {
        var entityType = ConfigMng.E.Entity[entityId].Type;

        return entityId == 0
            || entityType == (int)EntityType.Block2
            || entityType == (int)EntityType.Block3
            || entityType == (int)EntityType.Block4
            || entityType == (int)EntityType.Block5
            || entityType == (int)EntityType.Block6
            || entityType == (int)EntityType.Seed
            || entityType == (int)EntityType.TreeSeeds
            || entityType == (int)EntityType.Workbench
            || entityType == (int)EntityType.Kamado
            || entityType == (int)EntityType.EquipmentWorkbench
            || entityType == (int)EntityType.CookingTable
            || entityType == (int)EntityType.Resources
            || entityType == (int)EntityType.Door
            || entityType == (int)EntityType.Bed
            || entityType == (int)EntityType.Torch
            || entityType == (int)EntityType.TreasureBox
            || entityType == (int)EntityType.Flower
            || entityType == (int)EntityType.BigFlower
            || entityType == (int)EntityType.Grass
            || entityType == (int)EntityType.Mission
            || entityType == (int)EntityType.ChargeShop
            || entityType == (int)EntityType.GachaShop
            || entityType == (int)EntityType.ResourceShop
            || entityType == (int)EntityType.BlueprintShop
            || entityType == (int)EntityType.GiftShop
            || entityType == (int)EntityType.DefaltSurfaceEntity
            || entityType == (int)EntityType.HaveDirectionSurfaceEntity
            || entityType == (int)EntityType.Blast
            || entityType == (int)EntityType.TransferGate;
    }

    public static bool IsOutRange(Vector3Int mapSize, Vector3Int worldPos, int offset = 0)
    {
        return worldPos.x < 0 + offset || worldPos.x > mapSize.x - 1 - offset
            || worldPos.y < 0 + offset || worldPos.y > mapSize.y - 1 - offset
            || worldPos.z < 0 + offset || worldPos.z > mapSize.z - 1 - offset;
    }
    public static bool IsOutRange(Vector3Int worldPos)
    {
        return worldPos.x < 0 || worldPos.x > GetMapSize().x - 1
            || worldPos.y < 0 || worldPos.y > GetMapSize().y - 1
            || worldPos.z < 0 || worldPos.z > GetMapSize().z - 1;
    }

    /// <summary>
    /// 座標X、Zによって地面Yの座標をゲット
    /// </summary>
    /// <param name="mapData"></param>
    /// <param name="posX"></param>
    /// <param name="posZ"></param>
    /// <param name="offsetY">偏位量</param>
    /// <returns></returns>
    public static Vector3Int GetGroundPos(int posX, int posZ, float offsetY = 0, int CreatePosOffset = 3)
    {
        try
        {
            if (posX < 0) posX = UnityEngine.Random.Range(CreatePosOffset, WorldMng.E.MapMng.MapSize.x - CreatePosOffset);
            if (posZ < 0) posZ = UnityEngine.Random.Range(CreatePosOffset, WorldMng.E.MapMng.MapSize.z - CreatePosOffset);

            int posY = GetVertexY(posX, posZ) + (int)offsetY;

            Vector3Int newPos = new Vector3Int(posX, posY, posZ);
            if (!CheckCreatePos(newPos))
            {
                // 生成できない座標の場合、ｋ回ループして新しいランダム座標を取得
                for (int k = 0; k < 100; k++)
                {
                    posX = UnityEngine.Random.Range(CreatePosOffset, WorldMng.E.MapMng.MapSize.x - CreatePosOffset);
                    posZ = UnityEngine.Random.Range(CreatePosOffset, WorldMng.E.MapMng.MapSize.z - CreatePosOffset);
                    posY = GetVertexY(posX, posZ) + (int)offsetY;
                    newPos = new Vector3Int(posX, posY, posZ);

                    // ゲットした座標に他のEntityがある場合、スキップ
                    if (GetMapCell(newPos) == null || GetMapCell(newPos).Data.entityID > 0)
                        continue;

                    if (CheckCreatePos(newPos))
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
    private static int GetVertexY(int x, int z)
    {
        if (DataMng.E.RuntimeData.MapType != MapType.AreaMap)
        {
            return GetVertexY(DataMng.E.MapData, x, z);
        }

        int y = 0;
        for (int i = WorldMng.E.MapMng.MapSize.y - 1; i >= 0; i--)
        {
            var cell = GetMapCell(new Vector3Int(x, i, z));
            if (cell == null || cell.EntityId == 0)
                continue;

            y = i + 1;
            break;
        }
        return y;
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
    private static bool CheckCreatePos(Vector3 pos)
    {
        Vector3 downEntityPos = new Vector3(pos.x, pos.y - 1, pos.z);
        if (IsOutRange(WorldMng.E.MapMng.MapSize, Vector3Int.CeilToInt(downEntityPos), 3))
        {
            return false;
        }

        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            var downEntity = GetMapCell(Vector3Int.CeilToInt(downEntityPos));
            return downEntity == null ? false : ConfigMng.E.Entity[downEntity.Data.entityID].CanPut == 1;
        }
        else
        {
            var downEntity = DataMng.E.MapData.Map[(int)downEntityPos.x, (int)downEntityPos.y, (int)downEntityPos.z];
            return ConfigMng.E.Entity[downEntity.entityID].CanPut == 1;
        }
    }

    public static bool CheckCanCreate(MapData mdata, int entityId, Vector3Int pos, Direction dType)
    {
        var cell = GetMapCell(pos);

        // 被ってるかをチェック
        if (cell.EntityId > 0)
            return false;

        var config = ConfigMng.E.Entity[entityId];
        // 大きいエンティティの範囲チェック
        if (config.ScaleX > 1 || config.ScaleY > 1 || config.ScaleZ > 1)
        {
            var list = GetEntityPosListByDirection(entityId, pos, dType);
            foreach (var item in list)
            {
                // マップ外ならNG
                if (IsOutRange(WorldMng.E.MapMng.MapSize, item))
                    return false;

                // 作りたい範囲内に他のブロックがある場合、NG
                if (GetMapCell(item).EntityId > 0)
                    return false;
            }
        }

        // サスペンションのチェック
        var downEntityPos = new Vector3Int(pos.x, pos.y - 1, pos.z);
        var downCell = GetMapCell(downEntityPos);
        if (downCell == null)
            return false;

        // 農作物の場合、下のブロックが畑のブロックならOK
        if ((EntityType)config.Type == EntityType.Seed && downCell.Type != EntityType.Firm)
            return false;

        if ((EntityType)config.Type == EntityType.TreeSeeds && downCell.EntityId != 1005 && downCell.EntityId != 1006)
            return false;

        if (downCell.EntityId <= 0)
        {
            return config.CanSuspension == 1;
        }

        return true;
    }

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

    public static Vector3Int GetMapSize()
    {
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            return WorldMng.E.MapMng.MapSize;
        }
        else
        {
            return DataMng.E.MapData.GetMapSize();
        }
    }

    public static MapData.MapCellData GetMapDataByPosition(Vector3Int worldPos)
    {
        if (IsOutRange(worldPos))
            return new MapData.MapCellData();

        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            var cell = GetMapCell(worldPos);
            return cell.Data;
        }
        else
        {
            return DataMng.E.MapData.Map[worldPos.x, worldPos.y, worldPos.z];
        }
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="buildPos"></param>
    public static void InstantiateEntitys(BlueprintData blueprint, Vector3Int buildPos)
    {
        if (blueprint == null)
        {
            Logger.Error("blueprint is null");
            return;
        }

        Dictionary<int, MapInstance> mapDic = new Dictionary<int, MapInstance>();

        foreach (var item in blueprint.blocks)
        {
            var newPos = CommonFunction.Vector3Sum(item.GetPos(), buildPos);
            var newEntityData = new MapData.MapCellData() { entityID = item.id, direction = item.direction };

            if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            {
                var cell = GetMapCell(newPos);
                cell.SetData(newEntityData);
                cell.InstanceObj();

                // エリアが違う場合、影響エリアを記録
                if (!mapDic.ContainsKey(cell.Map.AreaID))
                    mapDic[cell.Map.AreaID] = cell.Map;
            }
            else
            {
                DataMng.E.MapData.Add(newEntityData, newPos);
            }
        }

            // メッシュを結合
        if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
        {
            foreach (var item in mapDic.Values)
            {
                item.CombineMesh();
            }
        }
        else
        {
            WorldMng.E.MapCtl.CombineMesh();
        }
    }
    /// <summary>
    /// 半透明エンティティをインスタンス
    /// </summary>
    /// <param name="blueprint"></param>
    /// <param name="startWorldPos"></param>
    public static void InstantiateTransparenEntitys(BlueprintData blueprint, Vector3Int startWorldPos)
    {
        var shader = Shader.Find("SemiTransparent");
        builderPencilParent = new GameObject("BuilderPancil").transform;
        builderPencilParent.position = startWorldPos;
        blueprint.State = BlueprintData.BlueprintState.None;

        foreach (var item in blueprint.blocks)
        {
            var entityConfig = ConfigMng.E.Entity[item.id];

            // Obstacleなら処理しない
            if ((EntityType)entityConfig.Type == EntityType.Obstacle)
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

            // 半透明場合、Collider を Enabled する
            var meshCollider = entity.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.enabled = false;
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

            // エンティティのPos
            var entityPos = Vector3Int.CeilToInt(entity.transform.position);
            if (blueprint.State == BlueprintData.BlueprintState.None)
            {
                CheckPos(blueprint, entityPos);
            }

            // エンティティサイズが１以上の場合、関連Pos
            var list = GetEntityPosListByDirection(item.id, entityPos, (Direction)item.direction);
            foreach (var pos in list)
            {
                if (blueprint.State == BlueprintData.BlueprintState.None)
                {
                    CheckPos(blueprint, pos);
                }
            }

            // シェーダー差し替え
            if (shader != null)
            {
                // サブObjectをゲット
                List<GameObject> childs = new List<GameObject>();
                CommonFunction.GetAllChiled(entity.transform, ref childs);
                bool IsDuplicate = GetMapDataByPosition(entity.WorldPos).entityID != 0;

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
    /// 設計図エンティティを削除
    /// </summary>
    public static void DeleteBuilderPencil()
    {
        if (builderPencilParent != null) GameObject.Destroy(builderPencilParent.gameObject);
    }

    private static void CheckPos(BlueprintData blueprint, Vector3Int pos)
    {
        // 最大高さをこえた場合
        if (blueprint.State == BlueprintData.BlueprintState.None && pos.y >= GetMapSize().y)
        {
            blueprint.State = BlueprintData.BlueprintState.TooHigh;
        }

        // マップサイズ範囲外の場合
        if (blueprint.State == BlueprintData.BlueprintState.None && IsOutRange(pos))
        {
            blueprint.State = BlueprintData.BlueprintState.IsOutRange;
        }

        // エンティティ本体が被ってるかのチェック
        if (blueprint.State == BlueprintData.BlueprintState.None && GetMapDataByPosition(pos).entityID != 0)
        {
            blueprint.State = BlueprintData.BlueprintState.IsDuplicate;
        }
    }

    #endregion
}