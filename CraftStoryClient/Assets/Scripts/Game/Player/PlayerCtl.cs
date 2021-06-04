﻿using System;
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

    public PlayerEntity PlayerEntity { get => playerEntity; }
    private PlayerEntity playerEntity;

    public Joystick Joystick { get => joystick; set => joystick = value; }
    private Joystick joystick;

    public ScreenDraggingCtl ScreenDraggingCtl { get => screenDraggingCtl; set => screenDraggingCtl = value; }
    private ScreenDraggingCtl screenDraggingCtl;

    public BuilderPencil BuilderPencil { get => builderPencil; }
    private BuilderPencil builderPencil;

    PlayerMotionType motionType;
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

    public void Init()
    {
        builderPencil = new BuilderPencil();
        motionType = PlayerMotionType.None;
    }

    private void Update()
    {
        if (joystick != null && !joystick.IsWaiting)
        {
            playerEntity.Move(joystick.xAxis.value, joystick.yAxis.value);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            AddItem(3000, 1);
        }
    }

    public PlayerEntity AddEntity()
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

        if (selectItem == null)
        {
            motionType = PlayerMotionType.None;
            return;
        }

        switch ((ItemType)selectItem.Config.Type)
        {
            case ItemType.Block:
                motionType = PlayerMotionType.SelectBlock;
                break;

            case ItemType.BuilderPencil:
                motionType = PlayerMotionType.SelectBuilderPencil;
                break;

            case ItemType.Blueprint:
                motionType = PlayerMotionType.SelectBlueprint;
                break;
        }
    }
    public void OnClick(GameObject obj, Vector3 pos)
    {
        if (selectItem == null)
            return;

        switch ((ItemType)selectItem.Config.Type)
        {
            case ItemType.Block:
                CreateBlock(selectItem.Config.ReferenceID, obj, Vector3Int.CeilToInt(pos));
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
                BuilderPencil.UserBlueprint(Vector3Int.CeilToInt(pos), selectItem.Data);
                break;
        }
    }
    public void OnClicking(float time, GameObject collider)
    {
        if (collider == null)
            return;

        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        if (clickingBlock == null || clickingBlock != cell)
        {
            clickingBlock = cell;
            clickingBlock.CancelClicking();
        }
        else
            cell.OnClicking(time);
    }

    /// <summary>
    /// アイテム手にいる
    /// </summary>
    public void AddItem(int itemID, int count, object data = null)
    {
        DataMng.E.AddItem(itemID, data, count);

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.RefreshItemBtns();
    }
    /// <summary>
    /// 消耗アイテム
    /// </summary>
    public void ConsumableItem(int itemID, int count = 1)
    {
        selectItem.Count -= count;

        if (selectItem.Count == 0)
        {
            DataMng.E.Items.Remove(selectItem);
            selectItem = null;
            ItemBtn.Select(null);
        }

        var homeUI = UICtl.E.GetUI<HomeUI>(UIType.Home);
        if (homeUI != null) homeUI.RefreshItemBtns();
    }

    public void ConsumableSelectItem(int count = 1)
    {
        ConsumableItem(selectItem.ItemID, count);
    }

    private void CreateBlock(int blockID, GameObject collider, Vector3Int pos)
    {
        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(pos, blockID);
        ConsumableSelectItem();

        Debug.Log("Create block " + pos);
    }
    

    enum PlayerMotionType
    {
        None = 0,
        SelectBlock,
        SelectBuilderPencil,
        SelectBlueprint,
    }
}