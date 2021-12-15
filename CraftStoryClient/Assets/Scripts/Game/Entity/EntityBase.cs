using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンティティベース
/// </summary>
public class EntityBase : MonoBehaviour
{
    private int id; // エンティティID
    private Vector3Int pos; // 座標
    public Direction direction; // 向き
    private float clickingTime; // タッチした時間

    public int EntityID { get => id; set => id = value; } // エンティティID
    public Entity EConfig { get => ConfigMng.E.Entity[id]; } // エンティティ設定ファイル
    public Vector3Int Pos { get => pos; set => pos = value; } // 座標
    public EntityType Type { get => (EntityType)EConfig.Type; } // エンティティタイプ
    public Direction Direction { get => direction; set => direction = value; } // 向き

    public List<Vector3Int> ObstacleList = new List<Vector3Int>();

    public virtual void Init() { }
    public virtual void OnClick() { }
    /// <summary>
    /// 長い時間クリック
    /// </summary>
    /// <param name="time"></param>
    public virtual void OnClicking(float time)
    {
        if (clickingTime == 0)
            EffectMng.E.AddDestroyEffect(transform.position);

        clickingTime += time;

        if (clickingTime > EConfig.DestroyTime)
        {
            clickingTime = 0;

            ClickingEnd();

            EffectMng.E.RemoveDestroyEffect();
        }
    }
    /// <summary>
    /// 長い時間クリックキャンセル
    /// </summary>
    public virtual void CancelClicking() { clickingTime = 0; }
    /// <summary>
    /// 長い時間クリック終了
    /// </summary>
    public virtual void ClickingEnd() { }
    /// <summary>
    /// タッチした向き
    /// </summary>
    /// <param name="tType"></param>
    public virtual void SetDirection(Direction tType) { }

    /// <summary>
    /// エンティティを壊す場合のロジック
    /// </summary>
    public void OnDestroyEntity()
    {
        int itemId = EConfig.ItemID;
        int count = 1;
        // 壊したブロックを手に入る
        NWMng.E.AddItem(null, itemId, count);
        // ローカルのアイテム数変更
        DataMng.E.AddItem(itemId, count);
        // エンティティインスタンスを削除
        WorldMng.E.MapCtl.DeleteEntity(this);
        // 削除Effectを追加
        var effect = EffectMng.E.AddEffect<EffectBase>(transform.position, EffectType.BlockDestroyEnd);
        effect.Init();
    }
}

public enum EntityType
{
    None = 0,　// 空
    Block = 1,// 一般ブロック
    Block2 = 2, // 半透明ブロック
    Flower = 3, // 花
    BigFlower = 4, // 巨大花
    Grass = 5, // 草
    Firm = 6, // 畑

    /// <summary>
    /// 農作物
    /// </summary>
    Crops = 7,

    /// <summary>
    /// 向きあるブロック
    /// </summary>
    Block3 = 8,

    /// <summary>
    /// 水ブロック
    /// </summary>
    Block4 = 9,

    Block99 = 99, // 壊れないブロック
    Resources = 100, // 
    TreasureBox = 110,
    Workbench = 1000,
    Kamado = 1001,

    /// <summary>
    /// 掲示板
    /// </summary>
    Mission = 1002,

    /// <summary>
    /// 商店
    /// </summary>
    ChargeShop = 1003,

    /// <summary>
    /// ガチャ商店
    /// </summary>
    GachaShop = 1004,

    /// <summary>
    /// 資材商店
    /// </summary>
    ResourceShop = 1005,

    /// <summary>
    /// 設計図ショップ
    /// </summary>
    BlueprintShop = 1006,

    /// <summary>
    /// ギフトショップ
    /// </summary>
    GiftShop = 1007,
    Door = 2000,

    Bed = 2001,

    /// <summary>
    /// 松明
    /// </summary>
    Torch = 2100,

    /// <summary>
    /// ランタン
    /// </summary>
    Lanthanum = 2101,

    /// <summary>
    /// 普通のEntity
    /// </summary>
    DefaltEntity = 3000,

    /// <summary>
    /// 普通のEntity（表面と判断しない）
    /// </summary>
    DefaltSurfaceEntity = 3001,

    /// <summary>
    /// 向きがある普通のEntity
    /// </summary>
    HaveDirectionEntity = 3002,

    /// <summary>
    /// 向きがある普通のEntity（表面と判断しない）
    /// </summary>
    HaveDirectionSurfaceEntity = 3003,

    /// <summary>
    /// 爆弾
    /// </summary>
    Blast = 4000,

    TransferGate = 9999,
    Obstacle = 10000,
}