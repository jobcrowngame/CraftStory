using System;
using System.Collections.Generic;
using JsonConfigData;
using Newtonsoft.Json;
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

    public PlayerEntity PlayerEntity { get => playerEntity; }
    private PlayerEntity playerEntity;

    public Joystick Joystick { get => joystick; set => joystick = value; }
    private Joystick joystick;

    public ScreenDraggingCtl ScreenDraggingCtl { get => screenDraggingCtl; set => screenDraggingCtl = value; }
    private ScreenDraggingCtl screenDraggingCtl;

    public BuilderPencil BuilderPencil { get => builderPencil; }
    private BuilderPencil builderPencil;

    private ItemData selectItem;
    private MapBlock clickingBlock;

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

    private bool lockCtl;

    public void Init()
    {
        builderPencil = new BuilderPencil();
    }

    private void Update()
    {
        playerEntity.Move(joystick);

        if (Input.GetKeyDown(KeyCode.F1))
        {
            DataMng.E.AddItem(3000, 1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            UICtl.E.OpenUI<ChargeUI>(UIType.Charge);
        }
    }

    public PlayerEntity AddPlayerEntity()
    {
        var resource = Resources.Load("Prefabs/Game/Character/Player") as GameObject;
        if (resource == null)
            return null;

        var pos = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5);
        pos = MapCtl.FixEntityPos(DataMng.E.MapData, pos, DataMng.E.CurrentMapConfig.CreatePosOffset);
        var obj = GameObject.Instantiate(resource, pos, Quaternion.identity);
        if (obj == null)
            return null;

        playerEntity = obj.GetComponent<PlayerEntity>();
        if (playerEntity == null)
        {
            Debug.LogError("not find CharacterEntity component");
            return null;
        }

        playerEntity.Init();

        return playerEntity;
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
    public void OnClick(GameObject obj, Vector3 pos)
    {
        if (selectItem == null)
        {
            if (obj != null)
            {
                var entity = obj.GetComponent<EntityResources>();
                if (entity != null)
                {
                    switch (entity.EntityType)
                    {
                        case EntityType.Tree:
                            break;
                        case EntityType.Rock:
                            break;
                        case EntityType.TransferGate:
                            break;
                        case EntityType.Craft: UICtl.E.OpenUI<CraftUI>(UIType.Craft); break;
                        default:
                            break;
                    }
                }
            }

            return;
        }
        else
        {
            switch ((ItemType)selectItem.Config().Type)
            {
                case ItemType.Block:
                    CreateBlock(selectItem.Config().ReferenceID, obj, Vector3Int.CeilToInt(pos));
                    break;

                case ItemType.BuilderPencil:
                    var newPos = obj.transform.position;
                    newPos.y += 1;

                    if (builderPencil.IsStart)
                        builderPencil.Start(newPos);
                    else
                        builderPencil.End(newPos);
                    break;

                case ItemType.Blueprint:
                    BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.Data);
                    break;
            }
        }
    }
    public void OnClicking(float time, GameObject collider)
    {
        if (collider == null)
            return;

        var cell = collider.GetComponent<MapBlock>();
        if (cell != null && DataMng.E.MapData.IsHome)
        {
            if (clickingBlock == null || clickingBlock != cell)
            {
                clickingBlock = cell;
                clickingBlock.CancelClicking();
            }
            else
            {
                cell.OnClicking(time);
            }
        }


    }

    private void CreateBlock(int blockID, GameObject collider, Vector3Int pos)
    {
        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(pos, blockID);
        ConsumableSelectItem();

        Lock();
        PlayerEntity.Behavior.Type = PlayerBehaviorType.CreateBlock;

        Debug.Log("Create block " + pos);
    }

    public void ConsumableSelectItem(int count = 1)
    {
        DataMng.E.ConsumableSelectItem(selectItem.id, count);
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

    public void Lock()
    {
        lockCtl = true;
    }
    public void UnLock()
    {
        lockCtl = false;
    }
    public bool IsLocked()
    {
        return lockCtl;
    }
}