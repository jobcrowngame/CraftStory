using JsonConfigData;
using UnityEngine;

/// <summary>
/// エンティティベース
/// </summary>
public class EntityBase : MonoBehaviour
{
    private int id; // エンティティID
    private Vector3Int pos; // 座標
    private Direction direction; // 向き
    private float clickingTime; // タッチした時間

    public int EntityID { get => id; set => id = value; } // エンティティID
    public Entity EConfig { get => ConfigMng.E.Entity[id]; } // エンティティ設定ファイル
    public Vector3Int Pos { get => pos; set => pos = value; } // 座標
    public EntityType Type { get => (EntityType)EConfig.Type; } // エンティティタイプ
    public Direction Direction { get => direction; set => direction = value; } // 向き

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
}

public enum EntityType
{
    None = 0,　// 空
    Block = 1,// 一般ブロック
    Block2 = 2, // 半透明ブロック
    Block99 = 99, // 壊れないブロック
    Resources = 100, // 
    TreasureBox = 110,
    Workbench = 1000,
    Kamado = 1001,
    Door = 2000,
    Torch = 2100,
    TransferGate = 9999,
    Obstacle = 10000,
}