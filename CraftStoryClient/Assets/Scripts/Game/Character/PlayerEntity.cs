﻿using UnityEngine;
using SimpleInputNamespace;

class PlayerEntity : CharacterEntity
{
    public static PlayerEntity E;

    private CharacterController controller;
    private Camera mainCamera;

    private Vector3 cameraOffset = new Vector3(0, 0, -10f);

    public float speed = 6.0F;       //歩行速度
    public float jumpSpeed = 8.0F;   //ジャンプ力
    public float gravity = 20.0F;    //重力の大きさ
    public float rotateSpeed = 3.0F;    //回転速度
    public float camRotSpeed = 5.0f;    //視点の上下スピード

    private Vector3 moveDirection = Vector3.zero;
    private float h, v;
    private float mX, mY;
    private float lookUpAngle, cameraPosZ;

    private BlockData selectBlock;
    private GameObject selectBlokCube;

    public Joystick joystick;
    private Transform cameraRotateX;
    private Transform cameraRotateY;
    private Transform playerModel;
    public ScreenDraggingCtl screenDraggingCtl;

    private MapBlock selectMapCell;

    public override void Init()
    {
        base.Init();

        E = this;
        controller = GetComponent<CharacterController>();

        cameraRotateX = CommonFunction.FindChiledByName(transform, "X").transform;
        cameraRotateY = CommonFunction.FindChiledByName(transform, "Y").transform;
        playerModel = CommonFunction.FindChiledByName(transform, "PlayerModel").transform;

        mainCamera = Camera.main;
        mainCamera.transform.SetParent(cameraRotateX);
        mainCamera.transform.localPosition = cameraOffset;
        mainCamera.transform.localRotation = Quaternion.identity;

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
            CameraRotate(mX, mY);

            //if (controller.isGrounded)
            //{
            //    if (Input.GetButton("Jump"))
            //        moveDirection.y = jumpSpeed;
            //}
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
        var angle2 = cameraRotateY.rotation.eulerAngles.y;
        var vec = GetV2FromAngle(angle1 + angle2);

        //キャラクターの移動と回転
        if (controller.isGrounded)
        {
            moveDirection = speed * new Vector3(vec.x, 0, vec.y);
            moveDirection = transform.TransformDirection(moveDirection);
        }

        playerModel.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        controller.Move(moveDirection * Time.deltaTime);
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

    public void CameraRotate(float mx, float my)
    {
        mX = mx;
        mY = my;

        lookUpAngle = cameraRotateX.transform.localRotation.eulerAngles.x + (-camRotSpeed * mY);

        if (lookUpAngle > 80 && lookUpAngle < 340)
            return;

        if (lookUpAngle > 340)
        {
            cameraPosZ = (360 - lookUpAngle) - 10;

            playerModel.gameObject.SetActive(cameraPosZ < -3);

            if (cameraPosZ > -1)
                cameraPosZ = -1;

            mainCamera.transform.localPosition = new Vector3(0, 0, cameraPosZ);
        }
        else
        {
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }

        cameraRotateX.transform.Rotate(new Vector3(-camRotSpeed * mY, 0, 0));
        cameraRotateY.transform.Rotate(new Vector3(0, rotateSpeed * mX, 0));
    }

    public void ChangeSelectBlock(int blockId)
    {
        BlockData bData = new BlockData(ConfigMng.E.BlockConfig[blockId].ID);
        selectBlock = bData;
    }

    public void CreateCube(GameObject collider, Vector3 pos)
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