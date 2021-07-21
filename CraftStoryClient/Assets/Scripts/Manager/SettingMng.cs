using UnityEngine;

public class SettingMng : Single<SettingMng>
{
    public float LookUpAngleMax { get => lookUpAngleMax; }
    private float lookUpAngleMax = 80f;

    public float LookUpAngleMin { get => lookUpAngleMin; }
    private float lookUpAngleMin = 320f;

    public float CameraDistanseMax { get => cameraDistanseMax; }
    private float cameraDistanseMax = -10f;

    public float EnactiveCharacterModelDistanse { get => enactiveCharacterModelDistanse; }
    private float enactiveCharacterModelDistanse = -2f;

    public float CamRotSpeed { get => camRotSpeed; }
    private float camRotSpeed = 0.5f;    //視点の上下スピード

    public float RotateSpeed { get => rotateSpeed; }
    private float rotateSpeed = 3.0F;    //回転速度

    public float CameraPullSpeed { get => cameraPullSpeed; }
    private float cameraPullSpeed = 10f;

    public float CameraPushSpeed { get => cameraPushSpeed; }
    private float cameraPushSpeed = 50f;

    public int MoveBoundaryOffset { get => moveBoundaryOffset; }
    private int moveBoundaryOffset = 3;

    public float MoveSpeed { get => moveSpeed; }
    private float moveSpeed = 4.5F;       //歩行速度

    public float JumpSpeed { get => jumpSpeed; }
    private float jumpSpeed = 8.0F;   //ジャンプ力

    public float Gravity { get => gravity; }
    private float gravity = 400.0F;    //重力の大きさ

    public int MyShopCostLv1 { get => myShopCostLv1; }
    private int myShopCostLv1 = 100;    //マイショップレベルアップコスト1

    public int MyShopCostLv2 { get => myShopCostLv2; }
    private int myShopCostLv2 = 500;    //マイショップレベルアップコスト2

    // ゲーム内一日秒数
    public int GameDaySeconds { get => mGameDaySeconds; }
    private int mGameDaySeconds = 6 * 5;

    public float MinAmbientIntensity { get => mMinAmbientIntensity; }
    private float mMinAmbientIntensity = 0.6f;
}