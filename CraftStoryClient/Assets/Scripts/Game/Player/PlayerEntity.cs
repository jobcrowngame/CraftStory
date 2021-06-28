﻿using UnityEngine;
using SimpleInputNamespace;

public class PlayerEntity : CharacterEntity
{
    private CharacterController controller;

    private Vector3 moveDirection = Vector3.zero;
    private float x, y;

    private bool modelActive;

    public override void Init()
    {
        base.Init();
        
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        controller.Move(new Vector3(0, -SettingMng.E.Gravity * Time.deltaTime, 0) * Time.deltaTime);
    }

    public override void ModelActive(bool b)
    {
        if (modelActive == b)
            return;

        modelActive = b;

        if (b!) Behavior.Type = PlayerBehaviorType.None;
        
        base.ModelActive(b);

        if (b) Behavior.Type = PlayerBehaviorType.Run;
    }
    public void Move(Joystick joystick)
    {
        if (joystick == null || PlayerCtl.E.Lock)
            return;

        if (Behavior.Type == PlayerBehaviorType.Create
            || Behavior.Type == PlayerBehaviorType.Breack)
            return;

        if (joystick.IsWaiting)
        {
            Behavior.Type = PlayerBehaviorType.Waiting;
            return;
        }

        Behavior.Type = PlayerBehaviorType.Run;

        x = joystick.xAxis.value;
        y = joystick.yAxis.value;

        var angle1 = GetAngleFromV2(new Vector2(x, y).normalized);
        var angle2 = PlayerCtl.E.CameraCtl.GetEulerAngleY;
        var vec = GetV2FromAngle(angle1 + angle2);

        //キャラクターの移動と回転
        if (controller.isGrounded)
        {
            moveDirection = SettingMng.E.MoveSpeed * new Vector3(vec.x, 0, vec.y);
            moveDirection = transform.TransformDirection(moveDirection);
        }

        Model.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        if (MoveBoundaryCheckPosX(moveDirection.x))
            moveDirection.x = 0;
        if (MoveBoundaryCheckPosZ(moveDirection.z))
            moveDirection.z = 0;

        controller.Move(moveDirection * Time.deltaTime);
    }
    private Vector2 GetV2FromAngle(float angle)
    {
        float radian = Mathf.PI * angle / 180.0f; // 度数法(度)から弧度法(ラジアン)に変換
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }
    private float GetAngleFromV2(Vector2 v)
    {
        return -Vector2.SignedAngle(new Vector2(0, 1), v);
    }

    /// <summary>
    /// 移動範囲境界判断　X
    /// </summary>
    private bool MoveBoundaryCheckPosX(float posX)
    {
        if (transform.position.x < SettingMng.E.MoveBoundaryOffset 
            && transform.position.x + posX < transform.position.x)
                return true;

        if (transform.position.x > DataMng.E.MapData.Config.SizeX - SettingMng.E.MoveBoundaryOffset
            && transform.position.x + posX > transform.position.x)
            return true;

        return false;
    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    private bool MoveBoundaryCheckPosZ(float posZ)
    {
        if (transform.position.z < SettingMng.E.MoveBoundaryOffset
           && transform.position.z + posZ < transform.position.z)
            return true;

        if (transform.position.z > DataMng.E.MapData.Config.SizeZ - SettingMng.E.MoveBoundaryOffset
            && transform.position.z + posZ > transform.position.z)
            return true;

        return false;
    }

    public void Wait()
    {
        Logger.Log("Wait");
    }
}