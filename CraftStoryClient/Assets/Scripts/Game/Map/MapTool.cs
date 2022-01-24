using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MapTool
{
    static Dictionary<int, GameObject> blocks = new Dictionary<int, GameObject>();

    public static void Clear()
    {
        blocks.Clear();
    }

    /// <summary>
    /// エンティティをインスタンス
    /// </summary>
    /// <param name="entityCell">エンティティデータ</param>
    /// <param name="parent">親</param>
    /// <param name="pos">座標</param>
    /// <param name="isCombineMesh">メッシュ軽量化するか</param>
    /// <returns></returns>
    public static EntityBase InstantiateEntity(MapCell cell, Transform parent, Vector3Int pos, bool isCombineMesh = true)
    {
        try
        {
            // 設定ファイル
            EntityBase entity = null;
            GameObject obj = null;
            switch (cell.Type)
            {
                case EntityType.Obstacle:
                    break;

                case EntityType.Block:
                case EntityType.Block2:
                case EntityType.Block3:
                case EntityType.Block4:
                case EntityType.Block5:
                case EntityType.Block6:
                case EntityType.Block99:
                case EntityType.Firm:
                    if (isCombineMesh)
                    {
                        if (!blocks.TryGetValue(cell.EntityId, out obj))
                        {
                            obj = CommonFunction.Instantiate(cell.Config.Resources, null, pos);
                            obj.gameObject.SetActive(false);

                            // オブジェクトリスト
                            blocks[cell.EntityId] = obj;
                        }

                        var mesh = obj.GetComponent<MeshFilter>();
                        var render = obj.GetComponent<MeshRenderer>();

                        var combineMO = parent.GetComponent<CombineMeshCtl>();
                        if (combineMO != null)
                        {
                            combineMO.AddObj(cell.EntityId, mesh.mesh, render.material, pos, cell.Direction);
                        }

                        if (cell.Type == EntityType.Block5 ||
                            cell.Type == EntityType.Block6)
                        {
                            entity = CommonFunction.Instantiate<EntityBlock>(ConfigMng.E.Entity[1].Resources, parent, pos);
                        }
                        else
                        {
                            entity = CommonFunction.Instantiate<EntityBlock>(ConfigMng.E.Entity[0].Resources, parent, pos);

                            var collider = entity.GetComponent<MeshCollider>();
                            collider.sharedMesh = mesh.mesh;

                            // 水ブロックの場合、triggerにする
                            if (cell.Type == EntityType.Block4)
                            {
                                collider.convex = true;
                                collider.isTrigger = true;
                            }
                        }
                    }
                    else
                    {
                        entity = CommonFunction.Instantiate<EntityBlock>(cell.Config.Resources, parent, pos);
                    }
                    break;

                case EntityType.Grass:
                    entity = CommonFunction.Instantiate<EntityGrass>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Seed:
                case EntityType.TreeSeeds:
                    entity = CommonFunction.Instantiate<EntityCrops>(cell.Config.Resources, parent, pos);
                    MapMng.E.AddCrops(entity.WorldPos, (EntityCrops)entity);
                    break;

                case EntityType.Resources:
                    entity = CommonFunction.Instantiate<EntityResources>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.TransferGate:
                    entity = CommonFunction.Instantiate<EntityTransferGate>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Torch:
                    if (cell.Direction != Direction.down)
                    {
                        entity = CommonFunction.Instantiate<EntityTorch>(cell.Config.Resources, parent, pos);
                        entity.SetDirection(cell.Direction);
                    }
                    break;

                case EntityType.TreasureBox:
                    entity = CommonFunction.Instantiate<EntityTreasureBox>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Flower:
                case EntityType.BigFlower:
                    entity = CommonFunction.Instantiate<EntityFlowoer>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Workbench:
                case EntityType.Kamado:
                case EntityType.EquipmentWorkbench:
                case EntityType.CookingTable:
                case EntityType.Door:
                case EntityType.Mission:
                case EntityType.ChargeShop:
                case EntityType.GachaShop:
                case EntityType.ResourceShop:
                case EntityType.BlueprintShop:
                case EntityType.GiftShop:
                    entity = CommonFunction.Instantiate<EntityFunctionalObject>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.DefaltEntity:
                case EntityType.DefaltSurfaceEntity:
                    entity = CommonFunction.Instantiate<EntityDefalt>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.HaveDirectionEntity:
                case EntityType.HaveDirectionSurfaceEntity:
                    entity = CommonFunction.Instantiate<EntityHaveDirection>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Bed:
                    entity = CommonFunction.Instantiate<EntityBed>(cell.Config.Resources, parent, pos);
                    break;

                case EntityType.Blast:
                    var entityBlast = CommonFunction.Instantiate<EntityBlast>(cell.Config.Resources, parent, pos);
                    entityBlast.Set(cell.EntityId);
                    entity = entityBlast;
                    break;


                default: Logger.Error("not find entityType " + cell.Type); break;
            }

            if (entity != null)
            {
                entity.EntityID = cell.EntityId;
                entity.LocalPos = pos;
                entity.Direction = cell.Direction;
                entity.Init();

                if (entity.EConfig.HaveDirection == 1)
                {
                    var angle = CommonFunction.GetCreateEntityAngleByDirection(cell.Direction);
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


   
   
}
