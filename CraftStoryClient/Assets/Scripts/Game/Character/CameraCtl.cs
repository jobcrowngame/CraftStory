using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtl : MonoBehaviour
{
    private float mX, mY;
    private float lookUpAngle;

    private Transform cameraRotateX;
    private Transform cameraRotateY;

    private Vector3 minPos;
    private Vector3 maxPos;
    private float triggerExitTime;
    private RaycastHit _cacheRaycastHit;

    public float GetEulerAngleY { get => cameraRotateY.transform.rotation.eulerAngles.y; }

    public void Init()
    {
        minPos = Vector3.zero;
        maxPos = new Vector3(0, 0, SettingMng.E.CameraDistanseMax);

        cameraRotateX = CommonFunction.FindChiledByName(PlayerEntity.E.transform, "X").transform;
        cameraRotateY = CommonFunction.FindChiledByName(PlayerEntity.E.transform, "Y").transform;

        transform.SetParent(cameraRotateX);
        transform.localPosition = new Vector3(0, 0, SettingMng.E.CameraDistanseMax);
        transform.localRotation = Quaternion.identity;

        //isBacking = false;
    }

    void Update()
    {
        if (IsBlock())
        {
            CameraPull(false);
        }
        else
        {
            if (Time.time - triggerExitTime > 3 && transform.localPosition.z > SettingMng.E.CameraDistanseMax)
                CameraPull(true);
        }

        PlayerEntity.E.PlayerModelActive(transform.localPosition.z < SettingMng.E.EnactiveCharacterModelDistanse);
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.GetType() == typeof(CharacterController))
            return;

        triggerExitTime = Time.time;
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetType() == typeof(CharacterController))
            return;

        triggerExitTime = Time.time;
    }

    public void CameraRotate(float mx, float my)
    {
        mX = mx * SettingMng.E.CamRotSpeed;
        mY = my * SettingMng.E.CamRotSpeed;

        lookUpAngle = cameraRotateX.transform.localRotation.eulerAngles.x - mY;

        if (lookUpAngle > SettingMng.E.LookUpAngleMax && lookUpAngle < SettingMng.E.LookUpAngleMin)
            return;

        if (lookUpAngle > SettingMng.E.LookUpAngleMin && lookUpAngle < 360)
        {
            Debug.Log(lookUpAngle);
            CameraPull(false);
        }

        cameraRotateX.transform.Rotate(new Vector3(-mY, 0, 0));
        cameraRotateY.transform.Rotate(new Vector3(0, mX, 0));
    }
    private void CameraPull(bool b)
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, b ? maxPos : minPos, b ? 0.01f : 0.5f);
    }
    private bool IsBlock()
    {
        if (transform.localPosition.z > -1f)
            return false;

        Vector2 pos = new Vector2(Screen.width / 2, Screen.height / 2);
        var obj = RayCastHits(pos);
        if (obj == null)
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
}
