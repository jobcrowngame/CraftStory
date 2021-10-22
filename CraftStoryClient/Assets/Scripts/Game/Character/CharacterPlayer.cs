using UnityEngine;
using SimpleInputNamespace;
using System;

/// <summary>
/// プレイヤーエンティティ
/// </summary>
public partial class CharacterPlayer : CharacterBase
{

    // ブロック壊す場合のエフェクト
    private Transform deleteEffect;

    // アニメション
    private Animator animator { get => CommonFunction.FindChiledByName<Animator>(transform, "Model"); }

    public CharacterBase Target
    {
        get => mTarget;
        set
        {
            mTarget = value;
        }
    }
    private CharacterBase mTarget;

    // モジュールのアクティブ
    public bool ShowModel
    {
        get => Model.gameObject.activeSelf;
        set
        {
            if (Model.gameObject.activeSelf == value)
                return;

            Model.gameObject.SetActive(value);

            Behavior = value
                ? BehaviorType.Run
                : BehaviorType.Waiting;
        }
    }

    public override void Init(int characterId, CharacterCamp camp)
    {
        base.Init(characterId, camp);
        deleteEffect = CommonFunction.FindChiledByName(transform, "DeleteEffect").transform;

        Behavior = BehaviorType.Waiting;

        // 装備する
        PlayerCtl.E.EquipEquipments();
    }

    public override void OnBehaviorChange(BehaviorType behavior)
    {
        base.OnBehaviorChange(behavior);

        Logger.Log("Player Behavior: " + behavior);

        PlayerCtl.E.Character.ShowDestroyEffect(behavior == BehaviorType.Breack);

        // Breack以外ならDestroyEffectを削除する
        if (behavior != BehaviorType.Breack)
        {
            EffectMng.E.RemoveDestroyEffect();
        }

        animator.SetInteger("State", (int)behavior);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (moveing)
        {
            PlayerMove();
        }
    }

    protected override void TargetDied()
    {
        base.TargetDied();

        PlayerCtl.E.CameraCtl.CancelLockUn();
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(float x, float y)
    {
        if (DataMng.E.MapData == null || PlayerCtl.E.CameraCtl == null)
            return;

        // 動作変更制限
        if (CanNotChangeBehavior())
            return;

        Behavior = BehaviorType.Run;

        var angle1 = 360 - CommonFunction.Vector2ToAngle(new Vector2(x, y).normalized) + 90;
        var angle2 = PlayerCtl.E.CameraCtl.GetEulerAngleY;
        var newVec = CommonFunction.AngleToVector2(angle1 + angle2);

        //キャラクターの移動と回転
        if (Controller.isGrounded && Behavior != BehaviorType.Jump)
        {
            if (x != 0 || y != 0)
            {
                moveDirection.x = newVec.x * SettingMng.MoveSpeed * Time.deltaTime;
                moveDirection.z = newVec.y * SettingMng.MoveSpeed * Time.deltaTime;
            }
            else
            {
                moveDirection.x = 0;
                moveDirection.z = 0;
            }

            moveDirection.y = 0;
        }

        if (x != 0 || y != 0)
            Model.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

        //moveDirection = transform.TransformDirection(moveDirection);

        if (transform.position.y < -10)
        {
            transform.position = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5);
        }
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    public void Jump()
    {
        if (Controller.isGrounded)
        {
            // ジャンプ前の行動を記録
            beforBehavior = Behavior;

            moveDirection.y = SettingMng.JumpSpeed;
            Behavior = BehaviorType.Jump;
        }
    }

    /// <summary>
    /// モジュールアクティブによって調整
    /// </summary>
    public void IsModelActive(bool b)
    {
        Controller.radius = b ? 0.40f : 0.01f;
        Controller.height = b ? 1.1f : 1f;
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


    Vector3 targetPos;
    bool moveing;
    float offsetDistance;
    Action moveCallback;

    /// <summary>
    /// 移動する
    /// </summary>
    /// <param name="pos">目標座標</param>
    /// <param name="callback">目標まで到着後のアクション</param>
    public void PlayerMoveTo(Vector3 pos, float offset, Action callback)
    {
        targetPos = pos;
        offsetDistance = offset;
        moveCallback = callback;
        moveing = true;
        PlayerCtl.E.Lock = true;
    }
    public void PlayerMove()
    {
        if (Vector3.Distance(transform.position, targetPos) > offsetDistance)
        {
            var dir = (targetPos - transform.position).normalized;
            Move(dir.x, dir.z);
        }
        else
        {
            if (moveCallback != null)
            {
                moveCallback();
            }

            StopMove();
            Behavior = BehaviorType.Waiting;
            moveing = false;
            PlayerCtl.E.Lock = false;
        }
    }

    /// <summary>
    /// 武器を装備しているかのチェック
    /// </summary>
    /// <returns></returns>
    public bool IsEquipedEquipment()
    {
        return SkillList.Count > 0;
    }
}