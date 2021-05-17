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
    public Vector3 cameraOffset = new Vector3(0, 0.7f, 0);

    private Vector3 moveDirection = Vector3.zero;
    private float h, v;
    private float mX, mY;
    private float lookUpAngle;

    private MapCellType selectMapCellType;
    private GameObject selectBlokCube;

    public Joystick joystick;
    public ScreenDraggingCtl screenDraggingCtl;

    private void Awake()
    {
        E = this;

        controller = GetComponent<CharacterController>();

        var camera = Camera.main;
        cameraCtl = camera.GetComponent<MainCameraCtl>();
        camera.transform.SetParent(transform);
        camera.transform.localPosition = Vector3.zero + cameraOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (SettingMng.E.MouseCursorLocked)
        {
            h = Input.GetAxis("Horizontal");    //左右矢印キーの値(-1.0~1.0)
            v = Input.GetAxis("Vertical");      //上下矢印キーの値(-1.0~1.0)
            mX = Input.GetAxis("Mouse X");      //マウスの左右移動量(-1.0~1.0)
            mY = Input.GetAxis("Mouse Y");      //マウスの上下移動量(-1.0~1.0)

            Move(h, v);
            Rotate(mX, mY);
        }
        else
        {
            if (joystick != null)
                Move(joystick.xAxis.value, joystick.yAxis.value);
        }

        if (Input.GetMouseButtonDown(0) && SettingMng.E.MouseCursorLocked)
            CreateCube();

        if (Input.GetMouseButtonDown(1))
            DestroyCube();
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
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
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

    public void ChangeSelectMapCellType(MapCellType mcType)
    {
        selectMapCellType = mcType;
    }

    //public void ViewingAngleChange(Vector3 offset)
    //{
    //    offset = offset * rotateSpeed;

    //    cameraCtl.transform.rotation = Quaternion.Euler(new Vector3(
    //        cameraCtl.transform.rotation.eulerAngles.x + offset.x,
    //        cameraCtl.transform.rotation.eulerAngles.y,
    //        cameraCtl.transform.rotation.eulerAngles.z + offset.z));

    //    transform.rotation = Quaternion.Euler(new Vector3(
    //        transform.rotation.eulerAngles.x,
    //        transform.rotation.eulerAngles.y + offset.y,
    //        transform.rotation.eulerAngles.z));
    //}

    public void CreateCube()
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapCell>();
        if (cell == null)
            return;

        Debug.Log("Create cube. " + cameraCtl.HitObj.transform.position);
        WorldMng.E.MapCtl.CreateMapCell(cameraCtl.HitPos, selectMapCellType);
    }
    public void DestroyCube()
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapCell>();
        if (cell == null)
            return;

        Debug.Log("Delete cube. " + cameraCtl.HitObj.transform.position);
        WorldMng.E.MapCtl.DeleteMapCell(cell);
    }
}