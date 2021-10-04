using UnityEngine;

/// <summary>
/// キャラクタエンティティ
/// </summary>
public class CharacterEntity : MonoBehaviour
{
    // モジュール
    private Transform model;
    public Transform Model { get => model; }
    private Animator animator;

    // ブロック壊す場合のエフェクト
    private Transform deleteEffect;

    // モジュールのアクティブ
    public bool IsActive
    {
        get => model.gameObject.activeSelf;
        set
        {
            if (model.gameObject.activeSelf == value)
                return;
            
            model.gameObject.SetActive(value);

            Behavior.Type = value
                ? PlayerBehaviorType.Run
                : PlayerBehaviorType.None;
        }
    }

    /// <summary>
    /// プレイヤー行動
    /// </summary>
    public PlayerBehavior Behavior
    {
        get
        {
            if (playerBehavior == null)
                playerBehavior = new PlayerBehavior();

            return playerBehavior;
        }
    }
    private PlayerBehavior playerBehavior;

    public virtual void Init()
    {
        model = CommonFunction.FindChiledByName(transform, "Model").transform;
        animator = model.GetComponent<Animator>();
        Behavior.Type = PlayerBehaviorType.Waiting;

        deleteEffect = CommonFunction.FindChiledByName(transform, "DeleteEffect").transform;
    }

    /// <summary>
    /// 行動変換
    /// </summary>
    /// <param name="stage">アニメステージ</param>
    public void EntityBehaviorChange(int stage)
    {
        if (animator == null)
        {
            Logger.Error("not find animator");
            return;
        }

        animator.SetInteger("State", stage);
    }

    /// <summary>
    /// ブロック壊すエフェクトアクティブ
    /// </summary>
    /// <param name="b"></param>
    public void ShowDestroyEffect(bool b = true)
    {
        if (deleteEffect != null)
        {
            deleteEffect.gameObject.SetActive(b);
        }
    }

    /// <summary>
    /// 角度によって回転
    /// </summary>
    /// <param name="angle"></param>
    public void Rotation(Vector2 direction)
    {
        var angle = CommonFunction.Vector2ToAngle(direction);
        Model.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0));
    }




    /// <summary>
    /// 座標から角度変換
    /// </summary>
    private float GetAngleFromV2(Vector2 v)
    {
        return -Vector2.SignedAngle(new Vector2(0, 1), v);
    }
}