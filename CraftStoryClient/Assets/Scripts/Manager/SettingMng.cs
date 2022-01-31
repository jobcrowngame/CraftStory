using UnityEngine;

/// <summary>
/// ゲーム内の設定
/// </summary>
public class SettingMng : Single<SettingMng>
{
    // ローカル化
    public const bool UseLocalData = true;

    // 自動セーブタイム
    public const int AutoSaveDataTime = 300;

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
    public const float MoveSpeed = 0.08f;

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

    /// <summary>
    /// 冒険エリアでBuffを更新する
    /// </summary>
    public const int CreateAdventureBuffStep = 30;

    /// <summary>
    /// いいね場合、ポイント追加数
    /// </summary>
    public const int GoodAddPointCount = 3;

    /// <summary>
    /// 最大ロックオン距離
    /// </summary>
    public const float MaxLockUnDistance = 20;

    /// <summary>
    /// 最大空腹度
    /// </summary>
    public const int MaxHunger = 100;

    /// <summary>
    /// ジャンプで空腹度消耗
    /// </summary>
    public const int JumCostHumger = 0;

    /// <summary>
    /// 時間による空腹度消耗Step
    /// </summary>
    public const int CostHumgerTimer = 25;

    #region エリアマップ

    /// <summary>
    /// エリアマップのサイズ
    /// </summary>
    public static int AreaMapSize { get => DataMng.E.UserData.IsDeterioration ? mAreaMapSizeDeterioration : mAreaMapSize; }
    private const int mAreaMapSize = 30;
    private const int mAreaMapSizeDeterioration = 40;

    /// <summary>
    /// エリアの規模
    /// </summary>
    public const int AreaMapScaleX = 333;
    public const int AreaMapScaleZ = 333;

    /// <summary>
    /// エリアマップの最大高さ
    /// </summary>
    public const int AreaMapV3Y = 100;

    /// <summary>
    /// エリアマップで敵が生成する半径
    /// </summary>
    public const int AreaMapCreateMonsterRange = 35;

    /// <summary>
    /// エリアマップで敵が追いかける距離
    /// </summary>
    public const int AreaMapChaseRange = 25;

    /// <summary>
    /// エリアマップで敵が生成する最大数
    /// </summary>
    public const int AreaMapCreateMonsterMaxCount = 3;
    public const int AreaMapCreateMonsterNightMaxCount = 30; // 夜

    /// <summary>
    /// エリアマップで敵が生成する間隔（秒）
    /// </summary>
    public const int AreaMapCreateMonsterInterval = 30;
    public const int AreaMapCreateMonsterNightInterval = 5; // 夜

    /// <summary>
    /// エリアマップで敵がデスポーン時間（秒）
    /// </summary>
    public const int AreaMapMonsterDespawnTime = 60;

    /// <summary>
    /// エリアマップで敵がデスポーン距離
    /// </summary>
    public const int AreaMapMonsterDespawnRange = 50;

    /// <summary>
    /// 松明影響範囲
    /// </summary>
    public const int TorchRange = 7;

    // 空腹時HP減少割合
    public const float HungerDamageRatio = 0.01f;

    // 空腹時HP減少間隔(秒)
    public const int HungerDamageIntervalSec = 3;

    #endregion
}