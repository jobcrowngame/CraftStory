using UnityEngine;
using SimpleInputNamespace;

class PlayerEntity : CharacterEntity
{
    public static PlayerEntity E;

    private MainCameraCtl cameraCtl;
    private CharacterController controller;

    public float speed = 6.0F;       //歩行速度
    public float jumpSpeed = 8.0F;   //ジャンプ力
    public float gravity = 20.0F;    //重力の大きさ
    public float rotateSpeed = 3.0F;    //回転速度
    public float camRotSpeed = 5.0f;    //視点の上下スピード

    private Vector3 moveDirection = Vector3.zero;
    private float h, v;
    private float mX, mY;
    private float lookUpAngle;

    private BlockData selectBlock;
    private GameObject selectBlokCube;

    public Joystick joystick;
    public ScreenDraggingCtl screenDraggingCtl;

    private MapBlock selectMapCell;

    private void Awake()
    {
        E = this;

        controller = GetComponent<CharacterController>();

        var camera = Camera.main;
        cameraCtl = camera.GetComponent<MainCameraCtl>();
    }

    private void Start()
    {
        cameraCtl.SetParent(transform);

        ChangeSelectBlock(1001);
    }

    private void Update()
    {
        if (SettingMng.E.MouseCursorLocked)
        {
            h = Input.GetAxis("Horizontal");    //左右矢印キーの値(-1.0~1.0)
            v = Input.GetAxis("Vertical");      //上下矢印キーの値(-1.0~1.0)
            mX = Input.GetAxis("Mouse X");      //マウスの左右移動量(-1.0~1.0)
            mY = Input.GetAxis("Mouse Y");      //マウスの上下移動量(-1.0~1.0)

            Move(h, v);
            Rotate(mX, mY);

            if (controller.isGrounded)
            {
                if (Input.GetButton("Jump"))
                    moveDirection.y = jumpSpeed;
            }

            if (Input.GetMouseButtonDown(0))
                CreateCube();
            if (Input.GetMouseButtonDown(1))
                DestroyCube();
        }
        else
        {
            if (joystick != null)
                Move(joystick.xAxis.value, joystick.yAxis.value);
        }
    }//Update()

    public void Move(float h, float v)
    {
        this.h = h;
        this.v = v;

        //キャラクターの移動と回転
        if (controller.isGrounded)
        {
            moveDirection = speed * new Vector3(h, 0, v);
            moveDirection = transform.TransformDirection(moveDirection);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
    public void Rotate(float mx, float my)
    {
        mX = mx;
        mY = my;

        lookUpAngle = Camera.main.transform.eulerAngles.x - 180 + camRotSpeed * mY;
        //Debug.LogFormat("angle:{0},  cameraAngleX:{1},  mY:{2}", lookUpAngle, Camera.main.transform.eulerAngles.x, mY * camRotSpeed);
        //if (Mathf.Abs(lookUpAngle) > 120 || Mathf.Abs(lookUpAngle) > 180)
        Camera.main.transform.Rotate(new Vector3(-camRotSpeed * mY, 0, 0));
        gameObject.transform.Rotate(new Vector3(0, rotateSpeed * mX, 0));
    }

    public void ChangeSelectBlock(int blockId)
    {
        BlockData bData = new BlockData(ConfigMng.E.BlockConfig[blockId].ID);
        selectBlock = bData;
    }

    public void CreateCube()
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapBlock>();
        if (cell == null)
            return;

        if (selectBlock == null)
            return;

        WorldMng.E.MapCtl.CreateBlock(cameraCtl.HitPos, selectBlock);
    }
    public void DestroyCube()
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapBlock>();
        if (cell == null)
            return;

        cell.Delete();
    }
    public void OnClicking(float time)
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapBlock>();
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