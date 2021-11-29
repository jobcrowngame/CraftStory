﻿using UnityEngine;

/// <summary>
/// ゲーム内の設定
/// </summary>
public class SettingMng : Single<SettingMng>
{
    // カメラ角度制限　最大
    public const float LookUpAngleMax = 80f;

    // カメラ角度制限　最小
    public const float LookUpAngleMin = 320f;

    // カメラ距離制限　最大
    public const float CameraDistanseMax = -10f;

    // キャラクターモジュール隠れる距離
    public const float EnactiveCharacterModelDistanse = -2f;

    // カメラスピード
    public const float CamRotSpeed = 0.5f;

    // 移動範囲制限偏差
    public const int MoveBoundaryOffset = 3;

    // 移動スピード
    public const float MoveSpeed = 0.1f;

    // ジャンプ力
    public const float JumpSpeed = 0.2f;

    //重力の大きさ
    public const float Gravity = 0.01f;

    //マイショップレベルアップコスト1
    public const int MyShopCostLv1 = 100;

    //マイショップレベルアップコスト2
    public const int MyShopCostLv2 = 500;

    // ゲーム内一日秒数
    public const int GameDaySeconds = 60 * 18;

    // Skybox　明るさ最小
    public const float MinAmbientIntensity = 0.8f;

    /// <summary>
    /// フレンド最大数
    /// </summary>
    public const int MaxFriendCount = 100;

    /// <summary>
    /// NPCと　話す距離
    /// </summary>
    public const float NPCTTalkDistance = 3;

    /// <summary>
    /// 共有CD
    /// </summary>
    public const float ShareCD = 0.5f;

    /// <summary>
    /// モンスターが近くまで移動する最小距離
    /// </summary>
    public const float MinDistance = 3f;

    /// <summary>
    /// デイズアクション時間
    /// </summary>
    public const float DazeTime = 10;

    /// <summary>
    /// ダメージObjectが削除される時間
    /// </summary>
    public const float DamageDestroyTime = 3;
}