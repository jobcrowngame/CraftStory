using UnityEngine;
using SimpleInputNamespace;

class PlayerEntity : CharacterEntity
{
    public static PlayerEntity E;

    private CharacterController controller;
    private CameraCtl cameraCtl;

    public float speed = 6.0F;       //歩行速度
    public float jumpSpeed = 8.0F;   //ジャンプ力
    public float gravity = 20.0F;    //重力の大きさ

    private Vector3 moveDirection = Vector3.zero;
    private float h, v;

    private BlockData selectBlock;
    private GameObject selectBlokCube;

    public Joystick joystick;

    private Transform playerModel;
    public ScreenDraggingCtl screenDraggingCtl;

    private MapBlock selectMapCell;

    public override void Init()
    {
        base.Init();
        
        E = this;

        controller = GetComponent<CharacterController>();
        cameraCtl = Camera.main.GetComponent<CameraCtl>();
        playerModel = CommonFunction.FindChiledByName(transform, "PlayerModel").transform;

        cameraCtl.Init();

        ChangeSelectBlock(1001);
    }

    private void Update()
    {
        if (SettingMng.E.MouseCursorLocked)
        {
            h = Input.GetAxis("Horizontal");    //左右矢印キーの値(-1.0~1.0)
            v = Input.GetAxis("Vertical");      //上下矢印キーの値(-1.0~1.0)

            Move(h, v);
        }
        else
        {
            if (joystick != null)
                Move(joystick.xAxis.value, joystick.yAxis.value);
        }

        controller.Move(new Vector3(0, -gravity * Time.deltaTime, 0) * Time.deltaTime);
    }

    public void Move(float h, float v)
    {
        if (h == 0 || v == 0)
            return;

        this.h = h;
        this.v = v;

        var angle1 = GetAngleFromV2(new Vector2(h, v).normalized);
        var angle2 = cameraCtl.GetEulerAngleY;
        var vec = GetV2FromAngle(angle1 + angle2);

        //キャラクターの移動と回転
        if (controller.isGrounded)
        {
            moveDirection = speed * new Vector3(vec.x, 0, vec.y);
            moveDirection = transform.TransformDirection(moveDirection);
        }

        playerModel.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        if (WorldMng.E.MapCtl.OutOfMapRangeX(transform.position.x + moveDirection.x * Time.deltaTime))
            moveDirection.x = 0;
        if (WorldMng.E.MapCtl.OutOfMapRangeZ(transform.position.z + moveDirection.z * Time.deltaTime))
            moveDirection.z = 0;

        controller.Move(moveDirection * Time.deltaTime);
    }
    public void CameraRotate(float x, float y)
    {
        cameraCtl.CameraRotate(x, y);
    }

    private Vector2 GetV2FromAngle(float angle)
    {
        float radian = Mathf.PI * angle / 180.0f; // 度数法(度)から弧度法(ラジアン)に変換
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }
    private float GetAngleFromV2(Vector2 v)
    {
        return -Vector2.SignedAngle(new Vector2(0,1), v);
    }
   
    public void PlayerModelActive(bool b)
    {
        playerModel.gameObject.SetActive(b);
    }

    public void ChangeSelectBlock(int blockId)
    {
        BlockData bData = new BlockData(ConfigMng.E.BlockConfig[blockId].ID);
        selectBlock = bData;
    }

    public void CreateCube(GameObject collider, Vector3Int pos)
    {
        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        if (selectBlock == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(pos, selectBlock);
    }

    public void OnClicking(float time, GameObject collider)
    {
        if (collider == null)
            return;

        var cell = collider.GetComponent<MapBlock>();
        if (cell == null)
            return;

        if (selectMapCell == null || selectMapCell != cell)
        {
            selectMapCell = cell;
            selectMapCell.CancelClicking();
        }
        else
            cell.OnClicking(time);
    }
}