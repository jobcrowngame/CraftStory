using UnityEngine;

public class SettingMng : Single<SettingMng>
{
    private bool mouseCursorLocked = false;
    public bool MouseCursorLocked
    {
        get { return mouseCursorLocked; }
        set
        {
            mouseCursorLocked = value;

            // Mouse Cursor
            Cursor.lockState = value ?
                CursorLockMode.Locked :
                CursorLockMode.None;
        }
    }

    public float LookUpAngleMax { get => lookUpAngleMax; }
    private float lookUpAngleMax = 80f;

    public float LookUpAngleMin { get => lookUpAngleMin; }
    private float lookUpAngleMin = 320f;

    public float CameraDistanseMax { get => cameraDistanseMax; }
    private float cameraDistanseMax = -10f;

    public float EnactiveCharacterModelDistanse { get => enactiveCharacterModelDistanse; }
    private float enactiveCharacterModelDistanse = -3f;

    public float CamRotSpeed { get => camRotSpeed; }
    private float camRotSpeed = 0.5f;    //視点の上下スピード

    public float RotateSpeed { get => rotateSpeed; }
    private float rotateSpeed = 3.0F;    //回転速度

    public float CameraPullSpeed { get => cameraPullSpeed; }
    private float cameraPullSpeed = 10f;

    public float CameraPushSpeed { get => cameraPushSpeed; }
    private float cameraPushSpeed = 10f;

    public int CreateEntityPosOffSet { get => createEntityPosOffSet; }
    private int createEntityPosOffSet = 3;
}
