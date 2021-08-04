using System;
using System.Collections.Generic;
using JsonConfigData;
using SimpleInputNamespace;
using UnityEngine;

public class PlayerCtl : MonoBehaviour
{
    public static PlayerCtl E
    {
        get
        {
            if (entity == null)
            {
                entity = UICtl.E.CreateGlobalObject<PlayerCtl>();
                entity.Init();
            }

            return entity;
        }
    }
    private static PlayerCtl entity;

    public bool Lock { get; set; }

    public PlayerEntity PlayerEntity { get => playerEntity; }
    private PlayerEntity playerEntity;

    public Joystick Joystick { get => joystick; set => joystick = value; }
    private Joystick joystick;

    public ScreenDraggingCtl ScreenDraggingCtl { get => screenDraggingCtl; set => screenDraggingCtl = value; }
    private ScreenDraggingCtl screenDraggingCtl;

    public BuilderPencil BuilderPencil { get => builderPencil; }
    private BuilderPencil builderPencil;

    public ItemData SelectItem 
    {
        get => selectItem; 
        set => selectItem = value;
    }
    private ItemData selectItem;
    private EntityBase clickingEntity;
    private EntityResources clickingResource;

    public CameraCtl CameraCtl
    {
        get => cameraCtl;
        set
        {
            cameraCtl = value;
            cameraCtl.Init();
        }
    }
    private CameraCtl cameraCtl;

    public BlueprintPreviewCtl BlueprintPreviewCtl
    {
        get => blueprintPreviewCtl;
        set
        {
            blueprintPreviewCtl = value;
        }
    }
    private BlueprintPreviewCtl blueprintPreviewCtl;

    public void Init()
    {
        builderPencil = new BuilderPencil();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerEntity.Jump();
        }

        if (joystick != null && !Lock)
            PlayerEntity.Move(joystick.xAxis.value, joystick.yAxis.value);
    }

    public PlayerEntity AddPlayerEntity()
    {
        var resource = Resources.Load("Prefabs/Game/Character/Player") as GameObject;
        if (resource == null)
            return null;

        Vector3 pos = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5);
        pos = MapCtl.FixEntityPos(DataMng.E.MapData, pos, DataMng.E.MapData.Config.CreatePosOffset);
        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
            return null;

        playerEntity = obj.GetComponent<PlayerEntity>();
        if (playerEntity == null)
        {
            Logger.Error("not find CharacterEntity component");
            return null;
        }

        playerEntity.Init();

        return playerEntity;
    }

    public void Jump()
    {
        PlayerEntity.Jump();
    }

    public void ChangeSelectItem(ItemData item)
    {
        builderPencil.CancelCreateBlueprint();
        builderPencil.CancelUserBlueprint();

        selectItem = item;

        PlayerEntity.Behavior.SelectItemType = (selectItem == null)
            ? ItemType.None
            : (ItemType)selectItem.Config().Type;
    }
    public void OnClick(GameObject collider, Vector3 pos, DirectionType dType)
    {
        if (DataMng.E.MapData.IsHome)
        {
            if (selectItem == null)
            {
                if (collider != null)
                {
                    var entity = collider.GetComponent<EntityBase>();
                    switch (entity.Type)
                    {
                        case EntityType.Workbench:
                        case EntityType.Kamado:
                            var ui = UICtl.E.OpenUI<CraftUI>(UIType.Craft);
                            ui.SetType(entity.Type);
                            break;

                        case EntityType.Door:
                            entity.OnClick();
                            break;
                    }
                }

                return;
            }
            else
            {
                switch ((ItemType)selectItem.Config().Type)
                {
                    case ItemType.Block:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos));
                        PlayerEntity.Behavior.Type = PlayerBehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.Workbench:
                    case ItemType.Kamado:
                    case ItemType.Door:
                        Lock = true;
                        var objdType = CommonFunction.GetCreateEntityDirection(pos);
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos), objdType);
                        PlayerEntity.Behavior.Type = PlayerBehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;

                    case ItemType.BuilderPencil:
                        var newPos = collider.transform.position;
                        newPos.y += 1;

                        if (builderPencil.IsStart)
                            builderPencil.Start(newPos);
                        else
                            builderPencil.End(newPos);
                        break;

                    case ItemType.Blueprint:
                        BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.relationData);
                        break;

                    case ItemType.Torch:
                        Lock = true;
                        CreateEntity(collider, selectItem.Config().ReferenceID, Vector3Int.CeilToInt(pos), dType);
                        PlayerEntity.Behavior.Type = PlayerBehaviorType.Create;

                        StartCoroutine(UnLock());
                        break;
                }
            }
        }
        else
        {
            if (collider != null)
            {
                var entity = collider.GetComponent<EntityBase>();
                switch (entity.Type)
                {
                    case EntityType.Resources:
                        var effect = EffectMng.E.AddEffect<EffectBase>(pos, EffectType.ResourcesDestroy);
                        effect.Init();

                        AdventureCtl.E.AddBonus(entity.EConfig.BonusID);
                        WorldMng.E.MapCtl.DeleteEntity(entity);
                        break;

                    case EntityType.TreasureBox:
                        entity.OnClick();
                        break;
                }
            }

               
        }
    }
    public void OnClicking(float time, GameObject collider)
    {
        if (collider == null)
            return;

        if (DataMng.E.MapData.IsHome)
        {
            var baseCell = collider.GetComponent<EntityBase>();
            if (baseCell != null)
            {
                if (baseCell.EConfig.CanDestroy == 0)
                    return;

                if (clickingEntity == null || clickingEntity != baseCell)
                {
                    clickingEntity = baseCell;
                    clickingEntity.CancelClicking();
                }
                else
                {
                    PlayerEntity.Behavior.Type = PlayerBehaviorType.Breack;
                    baseCell.OnClicking(time);
                }
            }
        }
    }

    private void CreateEntity(GameObject collider, int entityId, Vector3Int pos, DirectionType dType = DirectionType.up)
    {
        var cell = collider.GetComponent<EntityBase>();
        if (cell == null)
            return;

        if (cell.Type == EntityType.Door
            || cell.Type == EntityType.Workbench
            || cell.Type == EntityType.Kamado
            || cell.Type == EntityType.Torch
            || cell.Type == EntityType.Obstacle)
        {
            CommonFunction.ShowHintBar(19);
            return;
        }

        WorldMng.E.MapCtl.CreateEntity(entityId, pos, dType);
    }

    public void ConsumableSelectItem(int count = 1)
    {
        DataMng.E.ConsumableItemByGUID(selectItem.id, count);
    }
    public bool ConsumableItems(Dictionary<int, int> items)
    {
        bool ret = false;

        foreach (var item in items)
        {
            ret = DataMng.E.CheckConsumableItem(item.Key, item.Value);

            if (ret == false)
                break;
        }

        foreach (var item in items)
        {
            ret = DataMng.E.ConsumableItem(item.Key, item.Value);

            if (ret == false)
                break;
        }

        return ret;
    }

    private System.Collections.IEnumerator UnLock()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerCtl.E.Lock = false;
    }
}