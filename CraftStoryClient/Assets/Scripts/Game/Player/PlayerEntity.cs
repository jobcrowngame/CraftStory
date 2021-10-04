﻿using UnityEngine;
using SimpleInputNamespace;
using System;

/// <summary>
/// プレイヤーエンティティ
/// </summary>
public class PlayerEntity : CharacterEntity
{
    private CharacterController controller; // プレイヤーコンソール
    private Vector3 moveDirection = Vector3.zero; // 移動する値
    private PlayerBehaviorType beforBehaveior = PlayerBehaviorType.Waiting; // 前のプレイヤーの行動

    public override void Init()
    {
        base.Init();
        
        controller = GetComponent<CharacterController>();
    }

    public void Update()
    {
        if (Behavior.Type == PlayerBehaviorType.Jump && controller.isGrounded)
            Behavior.Type = beforBehaveior;

        if (moveing)
        {
            PlayerMove();
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="x">偏差X</param>
    /// <param name="y">偏差Y</param>
    public void Move(float x, float y)
    {
        // 壊す場合、移動出来ない
        if (Behavior.Type == PlayerBehaviorType.Breack)
            return;

        // マップデータ、カメラコンソールが初期化していない場合、移動できない
        if (DataMng.E.MapData == null || PlayerCtl.E.CameraCtl == null)
            return;

        var angle1 = GetAngleFromV2(new Vector2(x, y).normalized);
        var angle2 = PlayerCtl.E.CameraCtl.GetEulerAngleY;
        var newVec = GetV2FromAngle(angle1 + angle2);

        //キャラクターの移動と回転
        if (controller.isGrounded && Behavior.Type != PlayerBehaviorType.Jump)
        {
            if (x != 0 || y != 0)
            {
                moveDirection.x = newVec.x * SettingMng.MoveSpeed;
                moveDirection.z = newVec.y * SettingMng.MoveSpeed;
                Behavior.Type = PlayerBehaviorType.Run;
            }
            else
            {
                moveDirection.x = 0;
                moveDirection.z = 0;
                Behavior.Type = PlayerBehaviorType.Waiting;
            }

            moveDirection.y = 0;
        }

        if (x != 0 || y != 0)
            Model.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        // 重力
        moveDirection.y -= SettingMng.Gravity * Time.deltaTime;

        // マップ範囲外に出ないようにする
        if (MoveBoundaryCheckPosX(moveDirection.x))
            moveDirection.x = 0;
        if (MoveBoundaryCheckPosZ(moveDirection.z))
            moveDirection.z = 0;

        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime);

        if (transform.position.y < -10)
        {
            transform.position = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5);
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    public void Jump()
    {
        if (Behavior.Type != PlayerBehaviorType.Jump)
        {
            beforBehaveior = Behavior.Type;

            moveDirection.y = SettingMng.JumpSpeed;
            Behavior.Type = PlayerBehaviorType.Jump;
        }
    }

    /// <summary>
    /// 角度から座標変換
    /// </summary>
    private Vector2 GetV2FromAngle(float angle)
    {
        float radian = Mathf.PI * angle / 180.0f; // 度数法(度)から弧度法(ラジアン)に変換
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian));
    }
    /// <summary>
    /// 座標から角度変換
    /// </summary>
    private float GetAngleFromV2(Vector2 v)
    {
        return -Vector2.SignedAngle(new Vector2(0, 1), v);
    }

    /// <summary>
    /// 移動範囲境界判断　X
    /// </summary>
    private bool MoveBoundaryCheckPosX(float posX)
    {
        try
        {
            if (transform.position.x < SettingMng.MoveBoundaryOffset
                && transform.position.x + posX < transform.position.x)
                return true;

            if (transform.position.x > DataMng.E.MapData.Config.SizeX - SettingMng.MoveBoundaryOffset
                && transform.position.x + posX > transform.position.x)
                return true;

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
        
    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    private bool MoveBoundaryCheckPosZ(float posZ)
    {
        try
        {
            if (transform.position.z < SettingMng.MoveBoundaryOffset
                && transform.position.z + posZ < transform.position.z)
                return true;

            if (transform.position.z > DataMng.E.MapData.Config.SizeZ - SettingMng.MoveBoundaryOffset
                && transform.position.z + posZ > transform.position.z)
                return true;

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
    }

    /// <summary>
    /// モジュールアクティブによって調整
    /// </summary>
    public void IsModelActive(bool b)
    {
        controller.radius = b ? 0.40f : 0.01f;
        controller.height = b ? 1.1f : 1f;
    }


    Vector3 targetPos;
    bool moveing;
    float offsetDistance;
    Action moveCallback;

    /// <summary>
    /// 移動する
    /// </summary>
    /// <param name="pos">目標座標</param>
    /// <param name="callback">目標まで到着後のアクション</param>
    public void PlayerMoveTo(Vector3 pos, float offset, Action callback)
    {
        targetPos = pos;
        offsetDistance = offset;
        moveCallback = callback;
        moveing = true;

        var dir = (targetPos - transform.position).normalized;

        Debug.LogWarningFormat("Dir:{0}", dir);
    }
    public void PlayerMove()
    {
        if (Vector3.Distance(transform.position, targetPos) > offsetDistance)
        {
            var dir = (targetPos - transform.position).normalized;
            Move(dir.x, dir.z);
        }
        else
        {
            if (moveCallback != null)
            {
                moveCallback();
            }

            moveing = false;
        }
    }
}