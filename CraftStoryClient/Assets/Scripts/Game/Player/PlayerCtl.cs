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
    private MapBlock clickingBlock;
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
        if(joystick != null) playerEntity.Move(joystick);
    }

    public PlayerEntity AddPlayerEntity()
    {
        var resource = Resources.Load("Prefabs/Game/Character/Player") as GameObject;
        if (resource == null)
            return null;

        var pos = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5);
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
                    switch (entity.Type)
                    {
                        case ItemType.Workbench: 
                        case ItemType.Kamado:
                            var ui = UICtl.E.OpenUI<CraftUI>(UIType.Craft) as CraftUI;
                            ui.SetType((ItemType)entity.Data.Config.Type);
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
                    Lock = true;
                    CreateBlock(selectItem.Config().ReferenceID, obj, Vector3Int.CeilToInt(pos));
                    PlayerEntity.Behavior.Type = PlayerBehaviorType.Create;

                    StartCoroutine(UnLock());
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
                    BuilderPencil.UseBlueprint(Vector3Int.CeilToInt(pos), selectItem.relationData);
                    break;

                case ItemType.Workbench:
                case ItemType.Kamado:
                    CreateResources(selectItem.Config().ReferenceID, obj, Vector3Int.CeilToInt(pos));
                    PlayerEntity.Behavior.Type = PlayerBehaviorType.Create;
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
                PlayerEntity.Behavior.Type = PlayerBehaviorType.Breack;
                cell.OnClicking(time);
            }
        }

        var recource = collider.GetComponent<EntityResources>();
        if (recource != null)
        {
            if (clickingResource == null || clickingResource != recource)
            {
                clickingResource = recource;
                clickingResource.CancelClicking();
            }
            else
            {
                PlayerEntity.Behavior.Type = PlayerBehaviorType.Breack;
                recource.OnClicking(time);
            }
        }
    }

    private void CreateBlock(int blockID, GameObject collider, Vector3Int pos)
    {
        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(pos, blockID);
        Logger.Log("Create block " + pos);
    }
    private void CreateResources(int resourcesId, GameObject collider, Vector3Int pos)
    {
        WorldMng.E.MapCtl.CreateResources(pos, resourcesId);
        Logger.Log("Create block " + pos);
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

    private System.Collections.IEnumerator UnLock()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerCtl.E.Lock = false;
    }
}