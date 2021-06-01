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
    private int selectItemID;
    private Item selectItemConfig
    {
        get 
        {
            if (selectItemID < 1)
            {
                Debug.LogError("not find item " + selectItemID);
                return null;
            }
            return ConfigMng.E.Item[selectItemID]; 
        }
    } 
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
    }

    public PlayerEntity AddEntity()
    {
        var resource = Resources.Load("Prefabs/Game/Character/Player") as GameObject;
        if (resource == null)
            return null;

        var pos = WorldMng.E.MapCtl.GetRandomGroundPos();
        var posX = DataMng.E.CurrentMapConfig.PlayerPosX;
        var posZ = DataMng.E.CurrentMapConfig.PlayerPosZ;

        if (posX > 0) pos.x = posX;
        if (posZ > 0) pos.z = posZ;

        pos.y += 5;
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

    public void ChangeSelectItem(int blockId)
    {
        selectItemID = blockId;

        if (selectItemID == 0)
        {
            motionType = PlayerMotionType.None;
            return;
        }

        switch ((ItemType)selectItemConfig.Type)
        {
            case ItemType.Block:
                motionType = PlayerMotionType.SelectBlock;
                break;

            case ItemType.BuilderPencil:
                motionType = PlayerMotionType.SelectBuilderPencil;
                break;
        }
    }
    public void OnClick(GameObject obj, Vector3 pos)
    {
        if (selectItemID == 0)
            return;

        switch ((ItemType)selectItemConfig.Type)
        {
            case ItemType.Block:
                CreateBlock(selectItemConfig.ReferenceID, obj, new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z));
                break;

            case ItemType.BuilderPencil:
                var newPos = obj.transform.position;
                newPos.y += 1;

                if (builderPencil.IsStart)
                    builderPencil.Start(newPos);
                else
                    builderPencil.End(newPos);
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

    private void CreateBlock(int blockID, GameObject collider, Vector3Int pos)
    {
        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(pos, blockID);

        Debug.Log("Create block " + pos);
    }

    enum PlayerMotionType
    {
        None = 0,
        SelectBlock,
        SelectBuilderPencil,
    }
}