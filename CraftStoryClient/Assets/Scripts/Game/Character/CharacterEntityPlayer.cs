using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class CharacterEntityPlayer : CharacterEntity
{
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

    private MapCellType selectMapCellType;
    private GameObject selectBlokCube;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        var mainCameraObj = CommonFunction.FindChiledByName(gameObject.transform, "Main Camera");
        cameraCtl = mainCameraObj.GetComponent<MainCameraCtl>();
    }

    // Update is called once per frame
    void Update()
    {

        h = Input.GetAxis("Horizontal");    //左右矢印キーの値(-1.0~1.0)
        v = Input.GetAxis("Vertical");      //上下矢印キーの値(-1.0~1.0)
        mX = Input.GetAxis("Mouse X");      //マウスの左右移動量(-1.0~1.0)
        mY = Input.GetAxis("Mouse Y");      //マウスの上下移動量(-1.0~1.0)

        lookUpAngle = Camera.main.transform.eulerAngles.x - 180 + camRotSpeed * mY;
        //Debug.LogFormat("angle:{0},  cameraAngleX:{1},  mY:{2}", lookUpAngle, Camera.main.transform.eulerAngles.x, mY * camRotSpeed);
        //if (Mathf.Abs(lookUpAngle) > 120 || Mathf.Abs(lookUpAngle) > 180)
            Camera.main.transform.Rotate(new Vector3(-camRotSpeed * mY, 0, 0));


        //キャラクターの移動と回転
        if (controller.isGrounded)
        {
            moveDirection = speed * new Vector3(h, 0, v);
            moveDirection = transform.TransformDirection(moveDirection);
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;
        }

        gameObject.transform.Rotate(new Vector3(0, rotateSpeed * mX, 0));

        //if (controller.isGrounded)
        //{
        //    moveDirection = new Vector3(h, 0, v);
        //    moveDirection = transform.TransformDirection(moveDirection);
        //    moveDirection *= speed;

        //    if (Input.GetButton("Jump"))
        //        moveDirection.y = jumpSpeed;
        //}

        //if (controller.isGrounded)
        //{
        //    gameObject.transform.Rotate(new Vector3(0, rotateSpeed * h, 0));
        //    moveDirection = speed * v * gameObject.transform.forward;

        //    if (Input.GetButton("Jump"))
        //        moveDirection.y = jumpSpeed;
        //}

        ////キャラクターの移動お回転
        //if (controller.isGrounded)
        //{
        //    moveDirection = speed * new Vector3(h, 0, v);                               //移動方向を決定
        //    moveDirection = transform.TransformDirection(moveDirection);         //ベクトルをローカル座標からグローバル座標へ変換

        //    if (Input.GetButton("Jump"))
        //        moveDirection.y = jumpSpeed;
        //}
        //gameObject.transform.Rotate(new Vector3(0, rotateSpeed * mX, 0));



        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
            CreateCube();
        if (Input.GetMouseButtonDown(1))
            DestroyCube();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeSelectMapCellType(MapCellType.Black);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeSelectMapCellType(MapCellType.Blue);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeSelectMapCellType(MapCellType.Red);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeSelectMapCellType(MapCellType.Green);

        //if (Input.GetKey(KeyCode.UpArrow))
        //    E.ViewingAngleChange(-Vector3.right);
        //if (Input.GetKey(KeyCode.DownArrow))
        //    E.ViewingAngleChange(Vector3.right);
        //if (Input.GetKey(KeyCode.LeftArrow))
        //    E.ViewingAngleChange(-Vector3.up);
        //if (Input.GetKey(KeyCode.RightArrow))
        //    E.ViewingAngleChange(Vector3.up);

    }//Update()

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
        WorldMng.E.MapCtl.CreateCell(cameraCtl.HitPos, selectMapCellType);
    }
    public void DestroyCube()
    {
        if (cameraCtl.HitObj == null)
            return;

        var cell = cameraCtl.HitObj.GetComponent<MapCell>();
        if (cell == null)
            return;

        Destroy(cell.gameObject);
    }
}