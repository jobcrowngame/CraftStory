using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// プレイヤーエンティティ
/// </summary>
public partial class CharacterPlayer : CharacterBase
{

    // ブロック壊す場合のエフェクト
    private Transform deleteEffect;

    private Transform weaponL;
    private Transform weaponR;

    // アニメション
    private Animator animator;

    public CharacterBase Target
    {
        get => mTarget;
        set
        {
            mTarget = value;
        }
    }
    private CharacterBase mTarget;

    private Transform Arrow;
    private Transform ArrowTarget;


    public bool IsGrand;

    // モジュールのアクティブ
    public bool ShowModel
    {
        get => Model.gameObject.activeSelf;
        set
        {
            if (Model.gameObject.activeSelf == value)
                return;

            Model.gameObject.SetActive(value);

            if (!IsDied)
            {
                Behavior = Behavior;
            }
        }
    }

    public Transform FollowPoint { get => followPoint; }
    private Transform followPoint;

    private void Awake()
    {
        animator = CommonFunction.FindChiledByName<Animator>(transform, "Model");
        Arrow = CommonFunction.FindChiledByName(transform, "Arrow").transform;

        followPoint = CommonFunction.FindChiledByName(transform, "FollowPoint").transform;
    }

    public override void Init(int characterId, CharacterCamp camp)
    {
        base.Init(characterId, camp);
        deleteEffect = CommonFunction.FindChiledByName(transform, "DeleteEffect").transform;

        weaponL = CommonFunction.FindChiledByName(transform, "Weapon_L").transform;
        weaponR = CommonFunction.FindChiledByName(transform, "Weapon_R").transform;

        // 装備する
        PlayerCtl.E.EquipEquipments();

        Behavior = BehaviorType.Waiting;

        ShowArrow(null, false);

        // レベル10になるタスク
        if (DataMng.E.RuntimeData.Lv >= 10)
            TaskMng.E.AddMainTaskCount(10);
    }

    public override void OnBehaviorChange(BehaviorType behavior)
    {
        base.OnBehaviorChange(behavior);

        if (PlayerCtl.E.Character == null)
            return;

        Logger.Log("Player Behavior: " + behavior);

        PlayerCtl.E.Character.ShowDestroyEffect(behavior == BehaviorType.Breack);

        // Breack以外ならDestroyEffectを削除する
        if (behavior != BehaviorType.Breack)
        {
            EffectMng.E.RemoveDestroyEffect();
        }

        if (animator != null) animator.SetInteger("State", (int)behavior);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        IsGrand = Controller.isGrounded;

        if (moveing)
        {
            PlayerMove();
        }

        if (ArrowTarget != null)
        {
            var start = new Vector2(transform.position.x, transform.position.z);
            var end = new Vector2(ArrowTarget.position.x, ArrowTarget.position.z);
            Arrow.eulerAngles = new Vector3(0,270 - CommonFunction.Vector2ToAngle(start - end),0);
        }

        // 落ちた場合、遷移
        if (transform.position.y < -10)
        {
            Controller.enabled = false;
            transform.position = MapCtl.GetGroundPos(DataMng.E.MapData, DataMng.E.MapData.Config.PlayerPosX, DataMng.E.MapData.Config.PlayerPosZ, 5, DataMng.E.MapData.Config.CreatePosOffset);
            Controller.enabled = true;
        }
    }

    /// <summary>
    /// 目標が死んだ場合
    /// </summary>
    protected override void TargetDied()
    {
        base.TargetDied();

        PlayerCtl.E.CameraCtl.CancelLockUn();
    }

    protected override void Died()
    {
        base.Died();

        StartCoroutine(OnDied());
    }
    IEnumerator OnDied()
    {
        yield return new WaitForSeconds(4);
        CommonFunction.GoToNextScene(100);
    }
    

    public override void EquipEquipment(ItemEquipmentData equipmentData)
    {
        base.EquipEquipment(equipmentData);

        if (string.IsNullOrEmpty(equipmentData.equipmentConfig.ResourcesPath) || equipmentData.equipmentConfig.ResourcesPath == "N")
            return;

        Transform parent = null;
        switch ((ItemType)equipmentData.Config().Type)
        {
            case ItemType.Weapon:
                // どの手に装備するか
                parent = equipmentData.equipmentConfig.LeftEquipment == 1 ? weaponL : weaponR;
                break;

            case ItemType.Armor:
                break;

            default:
                break;
        }

        if (parent != null)
        {
            CommonFunction.ClearCell(weaponL);
            CommonFunction.ClearCell(weaponR);

            // オブジェクトをインスタンス
            var obj = CommonFunction.Instantiate(equipmentData.equipmentConfig.ResourcesPath, parent, Vector3.zero);
            if (obj != null)
            {
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localEulerAngles = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(float x, float y)
    {
        if (DataMng.E.MapData == null || PlayerCtl.E.CameraCtl == null || IsDied)
            return;

        // 動作変更制限
        if (CanNotChangeBehavior())
            return;

        if (Behavior != BehaviorType.Run) Behavior = BehaviorType.Run;

        var angle1 = 360 - CommonFunction.Vector2ToAngle(new Vector2(x, y).normalized) + 90;
        var angle2 = PlayerCtl.E.CameraCtl.GetEulerAngleY;
        var newVec = CommonFunction.AngleToVector2(angle1 + angle2);

        //キャラクターの移動と回転
        moveDirection.x = newVec.x * SettingMng.MoveSpeed;
        moveDirection.z = newVec.y * SettingMng.MoveSpeed;

        if (x != 0 || y != 0)
            Model.rotation = Quaternion.Euler(new Vector3(0, angle1 + angle2, 0));

       
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

    /// <summary>
    /// 矢印
    /// </summary>
    /// <param name="b"></param>
    public void ShowArrow(Transform target, bool b = true)
    {
        Arrow.gameObject.SetActive(b);
        ArrowTarget = target;
    }

    Vector3 targetPos;
    bool moveing;
    float offsetDistance;
    Action moveCallback;

    public Vector2 newDir;

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
            var dir = GetDircetion(targetPos);
            var angle = CommonFunction.Vector2ToAngle(dir);
            var cameraAngle = PlayerCtl.E.CameraCtl.GetEulerAngleY;

            newDir = CommonFunction.AngleToVector2(90 - angle - cameraAngle);

            Rotation(newDir);
            Move(newDir.x, newDir.y);
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

    /// <summary>
    /// Exp追加
    /// </summary>
    public void AddExp(int exp)
    {
        HpCtl.AddExp(exp);
    }
}