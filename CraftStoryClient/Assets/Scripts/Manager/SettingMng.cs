using UnityEngine;

/// <summary>
/// ゲーム内の設定
/// </summary>
public class SettingMng : Single<SettingMng>
{
    // カメラ角度制限　最大
    public float LookUpAngleMax { get => lookUpAngleMax; }
    private float lookUpAngleMax = 80f;

    // カメラ角度制限　最小
    public float LookUpAngleMin { get => lookUpAngleMin; }
    private float lookUpAngleMin = 320f;

    // カメラ距離制限　最大
    public float CameraDistanseMax { get => cameraDistanseMax; }
    private float cameraDistanseMax = -10f;

    // キャラクターモジュール隠れる距離
    public float EnactiveCharacterModelDistanse { get => enactiveCharacterModelDistanse; }
    private float enactiveCharacterModelDistanse = -2f;

    // カメラスピード
    public float CamRotSpeed { get => camRotSpeed; }
    private float camRotSpeed = 0.5f;    //視点の上下スピード

    // 移動範囲制限偏差
    public int MoveBoundaryOffset { get => moveBoundaryOffset; }
    private int moveBoundaryOffset = 3;

    // 移動スピード
    public float MoveSpeed { get => moveSpeed; }
    private float moveSpeed = 4F;       //歩行速度

    // ジャンプ力
    public float JumpSpeed { get => jumpSpeed; }
    private float jumpSpeed = 7F;   //ジャンプ力

    // 重力
    public float Gravity { get => gravity; }
    private float gravity = 20F;    //重力の大きさ

    public int MyShopCostLv1 { get => myShopCostLv1; }
    private int myShopCostLv1 = 100;    //マイショップレベルアップコスト1

    public int MyShopCostLv2 { get => myShopCostLv2; }
    private int myShopCostLv2 = 500;    //マイショップレベルアップコスト2

    // ゲーム内一日秒数
    public int GameDaySeconds { get => mGameDaySeconds; }
    private int mGameDaySeconds = 60 * 10;

    // Skybox　明るさ最小
    public float MinAmbientIntensity { get => mMinAmbientIntensity; }
    private float mMinAmbientIntensity = 0.6f;

    /// <summary>
    /// フレンド最大数
    /// </summary>
    public int MaxFriendCount { get => mMaxFriendCount; }
    private int mMaxFriendCount = 100;
}