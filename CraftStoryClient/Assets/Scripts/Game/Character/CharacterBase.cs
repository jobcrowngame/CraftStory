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

    private List<ImpactCell> impactList = new List<ImpactCell>();

    // 移動する値
    public Vector3 moveDirection = Vector3.zero;
    // 前の行動
    protected BehaviorType beforBehavior;

    private bool IsDied { get => Parameter.CurHP <= 0; }

    private float ShareCD = 0;
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
                Debug.Log("回復した。");
                Behavior = BehaviorType.Waiting;
                FreezeTime = 0;
            }
        }

        // ImpactのUpdate
        for (int i = 0; i < impactList.Count; i++)
        {
            impactList[i].OnUpdate();
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
            || Behavior == BehaviorType.Attack80
            || Behavior == BehaviorType.Attack81
            || Behavior == BehaviorType.Hit
            || Behavior == BehaviorType.Did
            || Behavior == BehaviorType.ReadyAttack01
            || Behavior == BehaviorType.ReadyAttack02
            || Behavior == BehaviorType.ReadyAttack03
            || Behavior == BehaviorType.ReadyAttack04
            || Behavior == BehaviorType.Skill_100
            || Behavior == BehaviorType.Skill_101
            || Behavior == BehaviorType.Skill_102
            || Behavior == BehaviorType.Skill_103
            || Behavior == BehaviorType.Skill_104
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

        if ((SkillData.SkillType)skill.Config.Type == SkillData.SkillType.SingleAttack
            || (SkillData.SkillType)skill.Config.Type == SkillData.SkillType.RangedRangeAttack)
        {
            // 範囲以外の場合、使用できない
            if (GetTargetDistance(selectTarget.transform) > skill.distance)
            {
                CommonFunction.ShowHintBar(32);
                return;
            }
        }

        // 共有CD冷却になる
        ShareCD = SettingMng.ShareCD;

        // スキルを使用
        skill.Use();

        StartCoroutine(UseSkillIE(skill, selectTarget));
    }
    private IEnumerator UseSkillIE(SkillData skill, CharacterBase selectTarget = null)
    {

        if (skill.Config.ReadyTime > 0)
        {
            Behavior = (BehaviorType)skill.Config.ReadyAnimation;

            // Effect 追加
            if (skill.Config.ReadyEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.ReadyEffect, skill.Config.ReadyEffectTime, transform.position);

            // Effect 追加
            if (selectTarget != null && skill.Config.ReadyTargetEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.ReadyTargetEffect, skill.Config.ReadyTargetEffectTime, selectTarget.transform.position);

            // 準備時間を待つ
            yield return new WaitForSeconds(skill.Config.ReadyTime);
        }

        Behavior = (BehaviorType)skill.Config.Animation;

        // Effect 追加
        if (skill.Config.AttackerEffect != "N")
            EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, transform.position);

        // 目標のキャンプ
        var targetCamp = Camp == CharacterCamp.Monster ? CharacterCamp.Player : CharacterCamp.Monster;

        switch ((SkillData.SkillType)skill.Config.Type)
        {
            // 範囲攻撃
            case SkillData.SkillType.RangeAttack:
                // 向きを調整
                if (selectTarget != null)
                {
                    var direction = GetTargetDircetion(selectTarget.transform);
                    Rotation(direction);
                }

                // 攻撃範囲内の目標
                var targets = CharacterCtl.E.FindCharacterInRange(transform, skill.Config.Distance, targetCamp);
                foreach (var target in targets)
                {
                    var targetDir = CharacterCtl.E.CalculationDir(target.transform.position, transform.position);
                    var angle = Vector2.Angle(targetDir, GetMeDirection());

                    if (angle * 2 <= skill.Config.RangeAngle)
                    {
                        foreach (var impact in skill.Impacts)
                        {
                            target.AddImpact(target, this, int.Parse(impact));
                        }
                    }
                }
                break;

            // 単体攻撃
            case SkillData.SkillType.SingleAttack:
                StartCoroutine(SingleAttackIE(skill, selectTarget));
                break;

            // 遠距離範囲攻撃
            case SkillData.SkillType.RangedRangeAttack:
                StartCoroutine(RangedRangeAttackIE(skill, selectTarget, targetCamp));
                break;

            // ビーム
            case SkillData.SkillType.Beam:
                break;

            // 自分の回復
            case SkillData.SkillType.Recovery:
                foreach (var impact in skill.Impacts)
                {
                    AddImpact(this, this, int.Parse(impact));
                }
                break;

            default:
                break;
        }

        // 攻撃後の凍結
        yield return new WaitForSeconds(skill.Config.ProcessTime);

        Behavior = BehaviorType.Waiting;
    }

    private IEnumerator RangedRangeAttackIE(SkillData skill, CharacterBase mainTarget, CharacterCamp targetCamp)
    {
        if (mainTarget != null)
        {
            // 向きを調整
            var direction = GetTargetDircetion(mainTarget.transform);
            Rotation(direction);

            int attackCount = skill.Config.AttackCount;
            float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

            // Effect 追加
            if (skill.Config.TargetEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.TargetEffect, skill.Config.TargetEffectTime, transform.position);

            while (attackCount > 0)
            {
                var targets = CharacterCtl.E.FindCharacterInRange(mainTarget.transform, skill.Config.Distance, targetCamp);
                foreach (var target in targets)
                {
                    foreach (var impact in skill.Impacts)
                    {
                        target.AddImpact(target, this, int.Parse(impact));
                    }
                }

                attackCount--;
                yield return new WaitForSeconds(interval);
            }
        }
    }
    private IEnumerator SingleAttackIE(SkillData skill, CharacterBase mainTarget)
    {
        // 目標がない、目標が死んだ場合、対象外
        if (mainTarget != null)
        {
            // 向きを調整
            var direction = GetTargetDircetion(mainTarget.transform);
            Rotation(direction);

            int attackCount = skill.Config.AttackCount;
            float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

            // Effect 追加
            if (skill.Config.TargetEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.TargetEffect, skill.Config.TargetEffectTime, transform.position);

            if (CharacterCtl.E.InDistance(skill.distance, transform, mainTarget.transform))
            {
                while (attackCount > 0)
                {
                    for (int i = 0; i < skill.Impacts.Length; i++)
                    {
                        mainTarget.AddImpact(mainTarget, this, int.Parse(skill.Impacts[i]));
                    }

                    attackCount--;
                    yield return new WaitForSeconds(interval);
                }
            }
        }
    }

    /// <summary>
    /// インパクトの追加
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attacker"></param>
    /// <param name="impactId"></param>
    private void AddImpact(CharacterBase target, CharacterBase attacker, int impactId)
    {
        impactList.Add(new ImpactCell(target, attacker, impactId));
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
    /// ダメージを与える
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="damage"></param>
    public void AddDamage(CharacterBase attacker, int damage)
    {
        if (IsDied)
        {
            Logger.Log("僕は死にました。");
            return;
        }

        Logger.Log("{1} が {0} のダメージを受けました。", damage, gameObject.name);

        // HP計算
        Parameter.CurHP -= damage;
        if (HpCtl != null)
            HpCtl.OnValueChange(-damage);

        // 死んだ場合
        if (Parameter.CurHP <= 0)
        {
            Died();
            attacker.TargetDied();
        }
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="damage"></param>
    public void Recovery(int damage)
    {
        // 回復後のHPが最大HPより多い場合、最大までの偏差を回復
        if (damage > Parameter.MaxHP - Parameter.CurHP)
            damage = Parameter.MaxHP - Parameter.CurHP;

        // HP計算
        Parameter.CurHP += damage;

        if (HpCtl != null)
            HpCtl.OnValueChange(damage);
    }

    /// <summary>
    /// インパクトを削除
    /// </summary>
    public void RemoveImpact(ImpactCell impact)
    {
        impactList.Remove(impact);
    }

    /// <summary>
    /// 凍結する
    /// </summary>
    /// <param name="time"></param>
    public virtual void Hit(float time)
    {
        if (CanNotChangeBehavior() || time <= 0)
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
    /// ステータススキル準備
    /// </summary>
    ReadyAttack04 = 76,

    /// <summary>
    /// 攻撃準備（弓）
    /// </summary>
    ReadyAttack03 = 77,

    /// <summary>
    /// 攻撃準備（魔法）
    /// </summary>
    ReadyAttack02 = 78,

    /// <summary>
    /// 攻撃準備（剣）
    /// </summary>
    ReadyAttack01 = 79,

    /// <summary>
    /// 攻撃
    /// </summary>
    Attack80 = 80,

    /// <summary>
    /// 攻撃2
    /// </summary>
    Attack81 = 81,

    /// <summary>
    /// 攻撃2
    /// </summary>
    Attack82 = 82,

    /// <summary>
    /// 攻撃受ける
    /// </summary>
    Hit = 90,

    /// <summary>
    /// 死ぬ
    /// </summary>
    Did　= 99,

    /// <summary>
    /// スキル
    /// </summary>
    Skill_100 = 100,

    /// <summary>
    /// ため斬り
    /// </summary>
    Skill_101 = 101,

    /// <summary>
    /// 魔法陣、メテオ
    /// </summary>
    Skill_102 = 102,

    /// <summary>
    /// 連打、矢雨
    /// </summary>
    Skill_103 = 103,

    /// <summary>
    /// ステータススキル
    /// </summary>
    Skill_104 = 104,
}
#endregion