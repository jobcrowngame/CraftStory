using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラコンソール
/// </summary>
public class CameraCtl : MonoBehaviour
{
    private float mX, mY;
    private float lookUpAngle;

    private Transform cameraRotateX;
    private Transform cameraRotateY;

    //private float triggerExitTime;
    private RaycastHit _cacheRaycastHit;

    private float CameraPosZ
    {
        get => cameraPosZ;
        set
        {
            cameraPosZ = value;
        }
    }
    private float cameraPosZ = -10;
    private float cameraPosMinZ = SettingMng.CameraDistanseMax;
    private float cameraPosMaxZ = 0;

    private float curMaxDistance = SettingMng.CameraDistanseMax;
    private float curMaxDistance2 = SettingMng.CameraDistanseMax;

    public float GetEulerAngleY { get => cameraRotateY.transform.localEulerAngles.y; }

    bool AutoCameraMove = false;
    bool isRightMove;
    float offsetAngle;
    float movedAngle;
    float offsetZ;
    Action cameraMoveOverCallback;

    float moveSpeed = 5;

    public void Init()
    {
        cameraRotateX = CommonFunction.FindChiledByName(PlayerCtl.E.PlayerEntity.transform, "X").transform;
        cameraRotateY = CommonFunction.FindChiledByName(PlayerCtl.E.PlayerEntity.transform, "Y").transform;

        transform.SetParent(cameraRotateX);
        transform.localPosition = new Vector3(0, 0, cameraPosZ);
        transform.localRotation = Quaternion.identity;

        //isBacking = false;
    }

    void Update()
    {
        if (PlayerCtl.E.PlayerEntity != null)
        {
            PlayerCtl.E.PlayerEntity.IsActive = transform.localPosition.z < SettingMng.EnactiveCharacterModelDistanse;
            PlayerCtl.E.PlayerEntity.IsModelActive(transform.localPosition.z < SettingMng.EnactiveCharacterModelDistanse);
        }

        if (AutoCameraMove)
        {
            StartCameraMove();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.GetType() == typeof(CharacterController))
            return;

        //triggerExitTime = Time.time;
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetType() == typeof(CharacterController))
            return;

        //triggerExitTime = Time.time;
    }

    public void CameraRotate(float mx, float my)
    {
        mX = mx * SettingMng.CamRotSpeed;
        mY = my * SettingMng.CamRotSpeed;

        lookUpAngle = cameraRotateX.transform.localRotation.eulerAngles.x - mx;

        if (lookUpAngle > SettingMng.LookUpAngleMax && lookUpAngle < SettingMng.LookUpAngleMin)
            return;

        if (lookUpAngle > SettingMng.LookUpAngleMin && lookUpAngle < 360)
        {
            float curV = 360 - lookUpAngle;
            float offset = curV > 360 - SettingMng.LookUpAngleMin ? 0 : 20;
            float maxV = 360 - SettingMng.LookUpAngleMin - offset;
            float v = Mathf.Sqrt(curV / maxV);
            v *= curMaxDistance;
            CameraPosZ = curMaxDistance - v;
            if (CameraPosZ > 0) CameraPosZ = 0;

            curMaxDistance2 = CameraPosZ;
            RefreshCameraPos();
        }
        else
        {
            curMaxDistance2 = SettingMng.CameraDistanseMax;
        }

        cameraRotateX.transform.Rotate(new Vector3(-mX, 0, 0));
        cameraRotateY.transform.Rotate(new Vector3(0, mY, 0));
    }

    public void ChangeCameraPos(float v)
    {
        //if (IsBlock())
        //    return;

        CameraPosZ += v * 0.3f;

        if (CameraPosZ <= cameraPosMinZ)
        {
            CameraPosZ = cameraPosMinZ;
        }

        if (CameraPosZ >= cameraPosMaxZ)
        {
            CameraPosZ = cameraPosMaxZ;
        }

        if (CameraPosZ < curMaxDistance2)
        {
            CameraPosZ = curMaxDistance2;
        }

        curMaxDistance = CameraPosZ;
        RefreshCameraPos();
    }
    public void RefreshCameraPos()
    {
        transform.localPosition = new Vector3(0, 0.8f, CameraPosZ);
    }

    private bool IsBlock()
    {
        if (transform.localPosition.z > -1f)
            return false;

        Vector2 pos = new Vector2(Screen.width / 2, Screen.height / 2);
        var obj = RayCastHits(pos);
        if (obj == null)
            return false;

        if (obj.name == "Main Camera")
                return false;

        var cCtl = obj.GetComponent<CharacterController>();
        return cCtl == null;
    }
    /// <summary>
    /// Rayを飛ばしてチェック
    /// </summary>
    private GameObject RayCastHits(Vector2 position)
    {
        // スクリーン座標を元にRayを取得
        var ray = Camera.main.ScreenPointToRay(position);

#if UNITY_EDITOR
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 5, false);
#endif

        if (!Physics.Raycast(ray, out _cacheRaycastHit))
            return null;

        return _cacheRaycastHit.collider.gameObject;
    }

    /// <summary>
    /// カメラがゆっくり回転
    /// </summary>
    /// <param name="direction">ターゲット向き</param>
    public void CameraslowlyRotateToTarget(Vector2 direction, Action callback)
    {
        cameraMoveOverCallback = callback;
        movedAngle = 0;
        PlayerCtl.E.Lock = true;
        AutoCameraMove = true;

        // カメラ向き
        Vector2 cameraDir = CommonFunction.AngleToVector2(GetEulerAngleY);

        // 回転する角度
        offsetAngle = Vector2.Angle(direction, cameraDir);

        // 回転向きを計算
        var newAngle = GetEulerAngleY + 90;
        Vector2 newDir = CommonFunction.AngleToVector2(newAngle);
        var newDot = Vector2.Dot(direction, newDir);

        offsetZ = CameraPosZ + 4;

        isRightMove = newDot < 0;
    }

    /// <summary>
    /// カメラの
    /// </summary>
    private void StartCameraMove()
    {
        // カメラYの変化
        var step = offsetAngle * Time.deltaTime * moveSpeed;
        if (step < 0.1f)
        {
            step = 0.1f;
        }
        movedAngle += step;
        cameraRotateY.transform.Rotate(new Vector3(0, isRightMove ? -step : step, 0));

        // カメラZの変化
        step = offsetZ * Time.deltaTime * moveSpeed;
        CameraPosZ += -step;
        RefreshCameraPos();

        if (movedAngle > offsetAngle)
        {
            AutoCameraMove = false;
            PlayerCtl.E.Lock = false;

            if (cameraMoveOverCallback != null)
            {
                cameraMoveOverCallback();
            }
        }
    }
}
