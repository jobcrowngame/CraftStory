using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクタエンティティ
/// </summary>
public class CharacterBase : MonoBehaviour
{
    /// <summary>
    /// モジュール
    /// </summary>
    private Transform model;
    public Transform Model { get => model; }

    /// <summary>
    /// パラメータ
    /// </summary>
    public Parameter Parameter { get; private set; }

    // プレイヤーコンソール
    public CharacterController Controller { get; private set; }

    /// <summary>
    /// 行動変換
    /// </summary>
    public BehaviorType Behavior
    {
        get => mBehavior;
        set
        {
            // 同じValue場合、スキップ
            if (mBehavior == value)
                return;

            // 死んだ後、他の動作に変更できません。
            if (Behavior == BehaviorType.Did)
                return;

            mBehavior = value;
            OnBehaviorChange(value);
        }
    }
    public BehaviorType mBehavior = BehaviorType.Waiting;

    public CharacterCamp Camp { get; set; }

    public HpUIBase HpCtl { get; private set; }

    /// <summary>
    /// スキルDic
    /// </summary>
    public List<SkillData> SkillList = new List<SkillData>();

    // 移動する値
    public Vector3 moveDirection = Vector3.zero;
    // 前の行動
    protected BehaviorType beforBehavior;

    private float ShareCD 
    {
        get { return mShareCD; }
        set
        {
            if (mShareCD <= 0)
                return;
            
            mShareCD = value;

            if (mShareCD <= 0)
            {
                ShareCDIsReady();
            }
        }
    }
    private float mShareCD = 0;
    public bool ShareCDIsCooling { get => ShareCD > 0; }

    private float FreezeTime = 0;

    private void Update()
    {
        OnUpdate();

        // 死んだら何もしない
        if (Behavior == BehaviorType.Did)
            return;

        // 重力
        if (Controller.isGrounded)
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.y -= SettingMng.Gravity * Time.deltaTime;
        }

        // マップ範囲外に出ないようにする
        if (MoveBoundaryCheckPosX(moveDirection.x))
            moveDirection.x = 0;
        if (MoveBoundaryCheckPosZ(moveDirection.z))
            moveDirection.z = 0;

        // 移動
        Controller.Move(moveDirection);

        // ジャンプ状態で地面に落ちるとジャンプ前の行動になる
        if (Behavior == BehaviorType.Jump && Controller.isGrounded)
            Behavior = beforBehavior;
    }
    public virtual void OnUpdate()
    {
        // 共有CD時間
        ShareCD -= Time.deltaTime;

        foreach (var skill in SkillList)
        {
            skill.OnUpdate();
        }

        if (Behavior == BehaviorType.Hit)
        {
            FreezeTime -= Time.deltaTime;

            if (FreezeTime < 0)
            {
                FreezeTime = 0;
                Behavior = BehaviorType.Waiting;
                Debug.Log("回復した。");
            }
        }
    }

    public virtual void Init(int characterId, CharacterCamp camp)
    {
        model = CommonFunction.FindChiledByName(transform, "Model").transform;
        Controller = GetComponent<CharacterController>();
        Camp = camp;

        // パラメータ初期化
        Parameter = new Parameter(characterId);
        Parameter.Init();

        AddSkills(Parameter.Skills);

        Behavior = BehaviorType.Waiting;
    }

    public virtual void OnClick() { }

    /// <summary>
    /// スキルを追加
    /// </summary>
    public void AddSkills(string skills)
    {
        if (skills == "N")
            return;

        var skillArr = skills.Split(',');
        foreach (var item in skillArr)
        {
            SkillList.Add(new SkillData(int.Parse(item)));
        }
    }

    /// <summary>
    /// スキル削除
    /// </summary>
    /// <param name="skills"></param>
    public void RemoveSkills(string skills)
    {
        if (skills == "N")
            return;

        var skillArr = skills.Split(',');
        foreach (var item in skillArr)
        {
            SkillList.Remove(new SkillData(int.Parse(item)));
        }
    }

    /// <summary>
    /// 行動変換する場合
    /// </summary>
    /// <param name="stage">アニメステージ</param>
    public virtual void OnBehaviorChange(BehaviorType behavior) { }

    /// <summary>
    /// HPバーを設定
    /// </summary>
    /// <param name="hpbar"></param>
    /// <param name="lockUpCamera">ずっとカメラを向かう</param>
    public void SetHpBar(HpUIBase hpbar, bool lockUpCamera = true)
    {
        HpCtl = hpbar;
        HpCtl.IsLockUpCamera(lockUpCamera);
        HpCtl.Init(Parameter);
    }

    /// <summary>
    /// 角度によって回転
    /// </summary>
    /// <param name="angle"></param>
    public void Rotation(Transform target)
    {

    }
    public void Rotation(Vector2 direction)
    {
        var angle = CommonFunction.Vector2ToAngle(direction);
        Model.rotation = Quaternion.Euler(new Vector3(0, -angle + 90, 0));
    }

    /// <summary>
    /// 移動停止
    /// </summary>
    public void StopMove()
    {
        moveDirection = Vector3.zero;
    }

    #region 戦闘

    /// <summary>
    /// 共有CD回復完了
    /// </summary>
    private void ShareCDIsReady()
    {
        Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// 行動を変更できないのチェック
    /// </summary>
    /// <returns></returns>
    public bool CanNotChangeBehavior()
    {
        return Behavior == BehaviorType.CallForHelp
            || Behavior == BehaviorType.ReadyAttack
            || Behavior == BehaviorType.Attack1
            || Behavior == BehaviorType.Attack2
            || Behavior == BehaviorType.Hit
            || Behavior == BehaviorType.Did
            || Behavior == BehaviorType.Breack;
    }

    /// <summary>
    /// スキルを使用
    /// </summary>
    /// <param name="skillId">スキルID</param>
    /// <param name="target">目標</param>
    public void StartUseSkill(SkillData skill, CharacterBase selectTarget = null)
    {
        // 攻撃中、スキル冷却中、共有CD冷却中場合、スキップ
        if (CanNotChangeBehavior()
            || skill.IsCooling 
            || ShareCDIsCooling)
            return;

        // 向きを調整
        if (selectTarget != null)
        {
            var direction = GetTargetDircetion(selectTarget.transform);
            Rotation(direction);
        }

        // 共有CD冷却になる
        ShareCD = SettingMng.ShareCD;

        // スキルを使用
        skill.Use();

        StartCoroutine(UseSkillIE(skill, selectTarget));
    }
    private IEnumerator UseSkillIE(SkillData skill, CharacterBase selectTarget = null)
    {
        Behavior = BehaviorType.ReadyAttack;

        // 準備時間を待つ
        yield return new WaitForSeconds(skill.Config.ReadyTime);

        Behavior = (BehaviorType)skill.Config.Animation;

        // 少し後ダメージを与る
        yield return new WaitForSeconds(0.3f);

        // 範囲攻撃
        switch ((SkillData.SkillType)skill.Config.Type)
        {
            case SkillData.SkillType.RangeAttack:
                // 攻撃範囲内の目標
                var targets = CharacterCtl.E.FindCharacterInAttackRange(transform, skill.Config.Distance);
                foreach (var target in targets)
                {
                    // 死んだやつは目標以外にする
                    if (target.Behavior == BehaviorType.Did)
                        continue;

                    var targetDir = CharacterCtl.E.CalculationDir(target.transform.position, transform.position);
                    var angle = Vector2.Angle(targetDir, GetMeDirection());

                    if (angle * 2 <= skill.Config.RangeAngle)
                    {
                        foreach (var impact in skill.Impacts)
                        {
                            int impactId = int.Parse(impact);
                            var impactConfig = ConfigMng.E.Impact[impactId];

                            // 目標が違う場合、スキップ
                            if (target.Camp != (CharacterCamp)impactConfig.Target)
                                continue;

                            Logger.Warning("{0} を攻撃した下。", target);

                            switch ((ImpactType)impactConfig.Type)
                            {
                                case ImpactType.AddDamage:
                                    HitDamage(target, this, impactId, skill.Config.TargetFreezeTime);
                                    //target.HitDamage(this, impactId, skill.Config.TargetFreezeTime);
                                    break;

                                default: Logger.Error("Not find impact type {0}", (ImpactType)impactConfig.Type); break;
                            }

                        }
                    }
                }
                break;

            case SkillData.SkillType.SingleAttack:
                // 目標がない、目標が死んだ場合、対象外
                if (selectTarget != null && selectTarget.Behavior != BehaviorType.Did)
                {
                    foreach (var impact in skill.Impacts)
                    {
                        int impactId = int.Parse(impact);
                        var impactConfig = ConfigMng.E.Impact[impactId];

                        Logger.Warning("{0} を攻撃した下。", selectTarget);

                        switch ((ImpactType)impactConfig.Type)
                        {
                            case ImpactType.AddDamage:
                                HitDamage(selectTarget, this, impactId, skill.Config.TargetFreezeTime);
                                //selectTarget.HitDamage(this, impactId, skill.Config.TargetFreezeTime);
                                break;

                            default: Logger.Error("Not find impact type {0}", (ImpactType)impactConfig.Type); break;
                        }
                    }
                }
                else
                {
                    Logger.Warning("目標を見つけません。");
                }
                break;

            case SkillData.SkillType.MagicBool:
                break;
            default:
                break;
        }

        // 攻撃後の凍結
        yield return new WaitForSeconds(skill.Config.ProcessTime);

        Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="attacker">攻撃者</param>
    /// <param name="impactId">インパクト</param>
    /// <param name="freezeTime">凍結時間</param>
    private void HitDamage(CharacterBase target, CharacterBase attacker, int impactId, float freezeTime)
    {
        // ダメージ計算
        int damage = BattleCalculationCtl.CalculationDamage(attacker, target, impactId);
        if (damage <= 0)
            damage = 1;

        Logger.Log("{1} が {0} のダメージを受けました。", damage, target.gameObject.name);

        // HP計算
        target.Parameter.CurHP -= damage;
        if (target.HpCtl != null) 
            target.HpCtl.OnValueChange(-damage);

        // 死んだ場合
        if (target.Parameter.CurHP <= 0)
        {
            target.Died();
            attacker.TargetDied();
        }
        else
        {
            // ターゲットが移動できなくなる
            target.moveDirection = Vector3.zero;

            target.Freeze(freezeTime);
        }
    }

    /// <summary>
    /// 死んだ
    /// </summary>
    protected virtual void Died()
    {
        Logger.Log("{0}が死んだ。", gameObject.name);

        StopMove();
        Behavior = BehaviorType.Did;

        HpCtl.OnDide();
        Controller.enabled = false;

        StopAllCoroutines();
    }

    /// <summary>
    /// 凍結する
    /// </summary>
    /// <param name="time"></param>
    protected virtual void Freeze(float time)
    {
        if (Behavior == BehaviorType.Hit)
            return;

        StopMove();
        Behavior = BehaviorType.Hit;
        FreezeTime = time;
    }

    protected virtual void TargetDied() {}



    #endregion
    #region 装備

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="equipmentId"></param>
    public void EquipEquipment(int equipmentId)
    {
        // スキル追加
        AddSkills(ConfigMng.E.Equipment[equipmentId].Skill);

        // 装備ステータス追加
        Parameter.Equipment.AddEquiptment(equipmentId);
    }

    /// <summary>
    /// 装備を消す
    /// </summary>
    /// <param name="equipmentId"></param>
    public void RemoveEquipment(int equipmentId)
    {
        // スキル削除
        RemoveSkills(ConfigMng.E.Equipment[equipmentId].Skill);

        // 装備ステータス削除
        Parameter.Equipment.RemoveEquipment(equipmentId);
    }

    #endregion
    #region Common

    /// <summary>
    /// 向きのベクトルをゲット
    /// </summary>
    /// <returns></returns>
    private Vector2 GetMeDirection()
    {
        return CommonFunction.AngleToVector2(Model.eulerAngles.y);
    }

    /// <summary>
    /// 目標に向かう向きをゲット
    /// </summary>
    /// <param name="target">目標</param>
    /// <returns></returns>
    public Vector2 GetTargetDircetion(Transform target)
    {
        return CommonFunction.GetDirection(target.position, transform.position).normalized;
    }

    /// <summary>
    /// 目標までの距離をゲット
    /// </summary>
    /// <param name="target">目標</param>
    /// <returns></returns>
    public float GetTargetDistance(Transform target)
    {
        return Mathf.Abs(Vector3.Distance(target.position, transform.position));
    }

    /// <summary>
    /// 移動範囲境界判断　X
    /// </summary>
    protected bool MoveBoundaryCheckPosX(float posX)
    {
        try
        {
            if (transform.position.x < SettingMng.MoveBoundaryOffset
                && transform.position.x + posX < transform.position.x)
                return true;

            if (transform.position.x > DataMng.E.MapData.Config.SizeX - SettingMng.MoveBoundaryOffset
                && transform.position.x + posX > transform.position.x)
                return true;

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return false;
        }

    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    protected bool MoveBoundaryCheckPosZ(float posZ)
    {
        try
        {
            if (transform.position.z < SettingMng.MoveBoundaryOffset
                && transform.position.z + posZ < transform.position.z)
                return true;

            if (transform.position.z > DataMng.E.MapData.Config.SizeZ - SettingMng.MoveBoundaryOffset
                && transform.position.z + posZ > transform.position.z)
                return true;

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
            return false;
        }
    }

    #endregion

    #region Emum

    /// <summary>
    /// キャンプ
    /// </summary>
    public enum CharacterCamp
    {
        /// <summary>
        /// キャラクター
        /// </summary>
        Player = 1,

        /// <summary>
        /// モンスター
        /// </summary>
        Monster,
    }
}

/// <summary>
/// 行動タイプ
/// </summary>
public enum BehaviorType
{
    Nono = 0,
    Waiting, // 何もしない
    Run, // 走る
    Create, // 作る
    Breack, // 壊す
    Jump, // ジャンプ


    /// <summary>
    /// 動作01
    /// </summary>
    observe01 = 70,

    /// <summary>
    /// 助を探す
    /// </summary>
    CallForHelp = 71,

    /// <summary>
    /// 攻撃準備
    /// </summary>
    ReadyAttack = 79,

    /// <summary>
    /// 攻撃
    /// </summary>
    Attack1 = 80,

    /// <summary>
    /// 攻撃2
    /// </summary>
    Attack2 = 81,

    /// <summary>
    /// 攻撃受ける
    /// </summary>
    Hit = 90,

    /// <summary>
    /// 死ぬ
    /// </summary>
    Did　= 99,
}

public enum ImpactType
{
    AddDamage = 1,
}

#endregion