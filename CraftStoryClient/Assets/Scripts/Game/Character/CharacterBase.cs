using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクタエンティティ
/// </summary>
public class CharacterBase : MonoBehaviour
{
    #region パラメータ

    /// <summary>
    /// モジュール
    /// </summary>
    private Transform model;
    public Transform Model { get => CommonFunction.FindChiledByName(transform, "Model").transform; }

    public Transform fowardObj;
    public Transform FowardObj { get => fowardObj; }

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
            //if (mBehavior == value)
            //    return;

            //// 死んだ後、他の動作に変更できません。
            //if (Behavior == BehaviorType.Did)
            //    return;

            mBehavior = value;
            OnBehaviorChange(value);
        }
    }
    public BehaviorType mBehavior = BehaviorType.Waiting;

    public CharacterGroup Group { get; set; }

    public float MoveSpeed { get; set; }

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

    public bool IsDied { get => Parameter.CurHP <= 0; }

    private float ShareCD = 0;
    public bool ShareCDIsCooling { get => ShareCD > 0; }

    public float FreezeTime = 0;

    protected bool ForcedRotate = false;
    /// <summary>
    /// スキル使用中
    /// </summary>
    public bool SkillUsing = false;

    #endregion

    public virtual void OnUpdate()
    {
        //if (DataMng.E.MapData == null)
        //    return;

        // 死んだら何もしない
        if (Behavior == BehaviorType.Did)
            return;

        // 重力
        moveDirection.y -= SettingMng.Gravity;

        // マップ範囲外に出ないようにする
        if (IsMapAreaOutX(moveDirection.x))
            moveDirection.x = 0;
        if (IsMapAreaOutZ(moveDirection.z))
            moveDirection.z = 0;

        // 移動
        if (Controller != null && Controller.enabled == true)
            Controller.Move(moveDirection);

        if (Controller != null && Controller.isGrounded)
            moveDirection.y = 0;

        // ジャンプ状態で地面に落ちるとジャンプ前の行動になる
        if (Controller != null && Behavior == BehaviorType.Jump && Controller.isGrounded)
            Behavior = beforBehavior;

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
                Behavior = BehaviorType.Waiting;
        }

        // ImpactのUpdate
        for (int i = 0; i < impactList.Count; i++)
        {
            impactList[i].OnUpdate();
        }
    }

    public virtual void Init(int characterId, CharacterGroup camp)
    {
        var fObj = CommonFunction.FindChiledByName(transform, "FowardObj");
        if(fObj != null) fowardObj = fObj.transform;

        Controller = GetComponent<CharacterController>();
        Group = camp;
        MoveSpeed = 1;

        // パラメータ初期化
        Parameter = new Parameter(characterId);
        AddSkills(Parameter.Skills);

        if (Parameter.RandomMoveOnWait == 1)
        {
            Behavior = BehaviorType.Waiting;
        }
        else
        {
            Behavior = BehaviorType.observe01;
        }
    }

    public virtual void OnClick() { }

    /// <summary>
    /// スキルを追加
    /// </summary>
    public void AddSkills(string skills)
    {
        if (string.IsNullOrEmpty(skills) || skills == "N")
            return;

        var skillArr = skills.Split(',');
        foreach (var item in skillArr)
        {
            var skill = new SkillData(int.Parse(item));
            SkillList.Add(skill);
            Parameter.skillPar.AddSkill(skill);
        }
    }
    /// <summary>
    /// スキル削除
    /// </summary>
    /// <param name="skills"></param>
    public void RemoveSkills(string skills)
    {
        if (skills == "N"　|| string.IsNullOrEmpty(skills))
            return;

        var skillArr = skills.Split(',');
        foreach (var skillStr in skillArr)
        {
            for (int i = 0; i < SkillList.Count; i++)
            {
                if (int.Parse(skillStr) == SkillList[i].Config.ID)
                {
                    Parameter.skillPar.RemoveSkill(SkillList[i]);
                    SkillList.Remove(SkillList[i]);
                    break;
                }
            }
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
    public void RefreshHpBar()
    {
        HpCtl.RefreshHpBar();
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
    /// 行動を変更できないのチェック
    /// </summary>
    /// <returns></returns>
    public bool CanNotChangeBehavior()
    {
        return Behavior == BehaviorType.CallForHelp
            || Behavior == BehaviorType.Attack80
            || Behavior == BehaviorType.Attack81
            || Behavior == BehaviorType.Attack82
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
    public void StartUseSkill(CharacterGroup targetGroup, SkillData skill, CharacterBase selectTarget = null)
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
            if (selectTarget != null && GetTargetDistance(selectTarget.transform) > skill.distance)
            {
                CommonFunction.ShowHintBar(32);
                return;
            }
        }

        // 共有CD冷却になる
        ShareCD = SettingMng.ShareCD;

        // スキルを使用
        skill.Use();

        // ジャンプ以外はスキル中状態にする
        if ((SkillData.SkillType)skill.Config.Type != SkillData.SkillType.Jump)
        {
            SkillUsing = true;
            StopMove();
        }

        StartCoroutine(UseSkillIE(targetGroup, skill, selectTarget));
    }
    private IEnumerator UseSkillIE(CharacterGroup targetGroup, SkillData skill, CharacterBase selectTarget = null)
    {
        // 目標がある場合、向きを調整
        if (selectTarget != null && 
            (SkillData.SkillType)skill.Config.Type != SkillData.SkillType.Jump &&
            (SkillData.SkillType)skill.Config.Type != SkillData.SkillType.MoveSpeedUp)
        {
            var direction = GetDircetion(selectTarget.transform);
            Rotation(direction);
        }

        if (skill.Config.ReadyTime > 0)
        {
            Behavior = (BehaviorType)skill.Config.ReadyAnimation;

            // Effect 追加
            if (skill.Config.ReadyEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.ReadyEffect, skill.Config.ReadyEffectTime, Model.transform);

            // Effect 追加
            if (selectTarget != null && skill.Config.ReadyTargetEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.ReadyTargetEffect, skill.Config.ReadyTargetEffectTime, selectTarget.Model.transform);

            // 準備時間を待つ
            yield return new WaitForSeconds(skill.Config.ReadyTime);
        }

        if ((SkillData.SkillType)skill.Config.Type != SkillData.SkillType.Jump)
        {
            Behavior = (BehaviorType)skill.Config.Animation;
        }

        switch ((SkillData.SkillType)skill.Config.Type)
        {
            // 範囲攻撃
            case SkillData.SkillType.RangeAttack:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(RangeAttack(skill, targetGroup));
                break;

            // 単体攻撃
            case SkillData.SkillType.SingleAttack:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(SingleAttackIE(skill, selectTarget));
                break;

            // 遠距離範囲攻撃
            case SkillData.SkillType.RangedRangeAttack:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(RangedRangeAttackIE(skill, selectTarget, targetGroup));
                break;

            // ビーム
            case SkillData.SkillType.Beam:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffectHaveParent(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(BeamIE(skill, targetGroup));
                break;

            // 自分の回復
            case SkillData.SkillType.Recovery:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                foreach (var impact in skill.Impacts)
                {
                    AddImpact(this, this, int.Parse(impact));
                }

                SkillUsing = false;
                break;

            case SkillData.SkillType.RangeRecovery:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(RangeRecoveryIE(skill, targetGroup));
                break;

            // ジャンプ
            case SkillData.SkillType.Jump:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                Jump();
                SkillUsing = false;
                break;

            // ジャンプ
            case SkillData.SkillType.MoveSpeedUp:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffectHaveParent(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                for (int i = 0; i < skill.Impacts.Length; i++)
                {
                    if (skill.Impacts[0] == "N")
                        continue;

                    AddImpact(this, this, int.Parse(skill.Impacts[0]));
                }
                SkillUsing = false;
                break;

            case SkillData.SkillType.FullMapAttack:
                // Effect 追加
                if (skill.Config.AttackerEffect != "N")
                    EffectMng.E.AddBattleEffect(skill.Config.AttackerEffect, skill.Config.AttackerEffectTime, Model.transform);

                StartCoroutine(FullMapAttack(skill, targetGroup));
                break;

            default:
                break;
        }

        // 攻撃後の凍結
        yield return new WaitForSeconds(skill.Config.ProcessTime);

        if ((SkillData.SkillType)skill.Config.Type == SkillData.SkillType.Jump)
        {
            Behavior = beforBehavior;
        }
        else
        {
            Behavior = BehaviorType.Waiting;
        }
    }

    private IEnumerator RangeAttack(SkillData skill,  CharacterGroup targetGroup)
    {
        // 複数のダメージImpact
        for (int i = 0; i < skill.Config.AttackCount; i++)
        {
            RangeAttackAddImpact(skill, targetGroup, skill.Impacts);
            yield return new WaitForSeconds(skill.Config.Interval);
        }

        // 一回のImpact
        RangeAttackAddImpact(skill, targetGroup, skill.OneceImpacts);

        SkillUsing = false;
    }
    private void RangeAttackAddImpact(SkillData skill, CharacterGroup targetGroup, string[] impacts)
    {
        // 目標を探す
        var targets = CharacterCtl.E.FindCharacterInRange(transform, skill.Config.Distance, targetGroup);
        foreach (var target in targets)
        {
            // 不存在、死んだ目標はスキップ
            if (target == null || target.IsDied)
                break;

            var targetDir = CharacterCtl.E.CalculationDir(target.transform.position, transform.position);
            var angle = Vector2.Angle(targetDir, GetMeDirection());

            if (angle * 2 <= skill.Config.RangeAngle)
            {
                foreach (var impact in impacts)
                {
                    if (impact == "N")
                        continue;

                    target.AddImpact(target, this, int.Parse(impact));
                }
            }
        }
    }
   
    private IEnumerator SingleAttackIE(SkillData skill, CharacterBase mainTarget)
    {
        // 目標がない、目標が死んだ場合、対象外
        if (mainTarget != null)
        {
            int attackCount = skill.Config.AttackCount;
            float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

            // Effect 追加
            if (skill.Config.TargetEffect != "N")
            {
                var effect = EffectMng.E.AddBattleEffect(skill.Config.TargetEffect, skill.Config.TargetEffectTime, mainTarget.Model.transform.position, Model.transform.rotation);

                // Effectのパレットセット
                if (skill.Config.TargetEffectParentInModel == 1)
                {
                    effect.transform.SetParent(mainTarget.transform);
                }
            }

            // ディリーダメージを与える
            yield return new WaitForSeconds(skill.Config.DelayDamageTime);

            while (attackCount > 0)
            {
                SingleAttackIEAddImpact(skill, mainTarget, skill.Impacts);

                 attackCount--;
                yield return new WaitForSeconds(interval);
            }

            SingleAttackIEAddImpact(skill, mainTarget, skill.OneceImpacts);
        }

        SkillUsing = false;
    }
    private void SingleAttackIEAddImpact(SkillData skill, CharacterBase target, string[] impacts)
    {
        if (target == null || target.IsDied)
            return;

        foreach (var impact in impacts)
        {
            if (impact == "N")
                continue;
            
            target.AddImpact(target, this, int.Parse(impact));
        }
    }

    private IEnumerator RangedRangeAttackIE(SkillData skill, CharacterBase mainTarget, CharacterGroup targetGroup)
    {
        if (mainTarget != null)
        {
            int attackCount = skill.Config.AttackCount;
            float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

            // Effect 追加
            if (skill.Config.TargetEffect != "N")
                EffectMng.E.AddBattleEffect(skill.Config.TargetEffect, skill.Config.TargetEffectTime, mainTarget.Model.transform);

            // ディリーダメージを与える
            yield return new WaitForSeconds(skill.Config.DelayDamageTime);


            while (attackCount > 0)
            {
                RangedRangeAttackIEAddImpact(skill, mainTarget, targetGroup, skill.Impacts);

                attackCount--;
                yield return new WaitForSeconds(interval);
            }

            // 一回のImpact
            RangedRangeAttackIEAddImpact(skill, mainTarget, targetGroup, skill.OneceImpacts);
        }

        SkillUsing = false;
    }
    private void RangedRangeAttackIEAddImpact(SkillData skill, CharacterBase mainTarget, CharacterGroup targetGroup, string[] impacts)
    {
        var targets = CharacterCtl.E.FindCharacterInRange(mainTarget.transform.position, skill.Config.Radius, targetGroup);
        foreach (var target in targets)
        {
            if (target == null || target.IsDied)
                break;

            foreach (var impact in impacts)
            {
                if (impact == "N")
                    continue;

                target.AddImpact(target, this, int.Parse(impact));
            }
        }
    }

    private IEnumerator BeamIE(SkillData skill, CharacterGroup targetGroup)
    {
        int attackCount = skill.Config.AttackCount;
        float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

        // ディリーダメージを与える
        yield return new WaitForSeconds(skill.Config.DelayDamageTime);

        ForcedRotate = true;

        while (attackCount > 0)
        {
            BeamIEAddImpact(skill, targetGroup, skill.Impacts);

            attackCount--;
            yield return new WaitForSeconds(interval);
        }

        ForcedRotate = false;
        SkillUsing = false;

        // 一回のImpact
        BeamIEAddImpact(skill, targetGroup, skill.OneceImpacts);
    }
    private void BeamIEAddImpact(SkillData skill, CharacterGroup targetGroup, string[] impacts)
    {
        var targets = CharacterCtl.E.FindCharacterInRect(this, skill.Config.Distance, skill.Config.Radius, targetGroup);
        foreach (var target in targets)
        {
            if (target == null || target.IsDied)
                continue;

            foreach (var impact in impacts)
            {
                if (impact == "N")
                    continue;

                target.AddImpact(target, this, int.Parse(impact));
            }
        }
    }

    private IEnumerator RangeRecoveryIE(SkillData skill, CharacterGroup targetGroup)
    {
        int attackCount = skill.Config.AttackCount;
        float interval = skill.Config.Interval > 0 ? skill.Config.Interval : 0;

        // ディリーダメージを与える
        yield return new WaitForSeconds(skill.Config.DelayDamageTime);


        while (attackCount > 0)
        {
            RangeRecoveryIEAddImpact(skill, targetGroup, skill.Impacts);

            attackCount--;
            yield return new WaitForSeconds(interval);
        }

        SkillUsing = false;

        // 一回のImpact
        RangeRecoveryIEAddImpact(skill, targetGroup, skill.OneceImpacts);
    }
    private void RangeRecoveryIEAddImpact(SkillData skill, CharacterGroup targetGroup, string[] impacts)
    {
        // 目標を探す
        var targets = CharacterCtl.E.FindCharacterInRange(transform.position, skill.Config.Radius, targetGroup);
        foreach (var target in targets)
        {
            if (target == null || target.IsDied)
                break;

            foreach (var impact in impacts)
            {
                if (impact == "N")
                    continue;

                target.AddImpact(target, this, int.Parse(impact));
            }
        }
    }

    private IEnumerator FullMapAttack(SkillData skill, CharacterGroup targetGroup)
    {
        // 複数のダメージImpact
        for (int i = 0; i < skill.Config.AttackCount; i++)
        {
            FullMapAttackAddImpact(skill, targetGroup, skill.Impacts);
            yield return new WaitForSeconds(skill.Config.Interval);
        }

        // 一回のImpact
        FullMapAttackAddImpact(skill, targetGroup, skill.OneceImpacts);

        SkillUsing = false;
    }
    private void FullMapAttackAddImpact(SkillData skill, CharacterGroup targetGroup, string[] impacts)
    {
        // 目標を探す
        var targets = CharacterCtl.E.FindCharacterAll(targetGroup);
        foreach (var target in targets)
        {
            // 不存在、死んだ目標はスキップ
            if (target == null || target.IsDied)
                break;

            foreach (var impact in impacts)
            {
                if (impact == "N")
                    continue;

                target.AddImpact(target, this, int.Parse(impact));
            }
        }
    }



    /// <summary>
    /// インパクトの追加
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attacker"></param>
    /// <param name="impactId"></param>
    public void AddImpact(CharacterBase target, CharacterBase attacker, int impactId)
    {
        impactList.Add(new ImpactCell(target, attacker, impactId));
    }

    /// <summary>
    /// 死んだ
    /// </summary>
    protected virtual void Died()
    {
        StopMove();
        Behavior = BehaviorType.Did;

        HpCtl.OnDide();
        Controller.enabled = false;

        StopAllCoroutines();
    }

    public void AddDamageRatio(CharacterBase attacker, float ratio)
    {
        AddDamage(attacker, Mathf.FloorToInt(Parameter.AllHP * ratio));
    }

    /// <summary>
    /// 強制殺す
    /// </summary>
    public virtual void Kill() { }

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

        //Logger.Log("{1} が {0} のダメージを受けました。", damage, gameObject.name);

        // HP計算
        Parameter.CurHP -= damage;

        // ガイドの場合、70%以下にさせない
        if (DataMng.E.RuntimeData.MapType == MapType.Guide &&
            Group == CharacterGroup.Player &&
            Parameter.CurHP < (Parameter.AllHP * 0.7))
        {
            Parameter.CurHP = (int)(Parameter.AllHP * 0.7);
        }

        // 死んだ場合
        if (Parameter.CurHP <= 0)
        {
            Parameter.CurHP = 0;

            Died();
            if (attacker != null) 
            { 
                attacker.TargetDied(); 
            }
        }

        if (HpCtl != null)
            HpCtl.OnValueChange(-damage);
    }

    /// <summary>
    /// 回復
    /// </summary>
    /// <param name="damage"></param>
    public void Recovery(int damage)
    {
        // 回復後のHPが最大HPより多い場合、最大までの偏差を回復
        if (damage > Parameter.AllHP - Parameter.CurHP)
            damage = Parameter.AllHP - Parameter.CurHP;

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
        if (time <= 0)
            return;

        if (time > FreezeTime)
            FreezeTime = time;


        if (!CanNotChangeBehavior())
        {
            StopMove();
            Behavior = BehaviorType.Hit;
        }
    }

    protected virtual void TargetDied() {}

    /// <summary>
    /// 復活
    /// </summary>
    public virtual void Resurrection()
    {
        Parameter.CurHP = Parameter.AllHP;
        Controller.enabled = true;
        Behavior = BehaviorType.Waiting;

        if (HpCtl != null)
            HpCtl.OnResurrection();
    }

    /// <summary>
    /// ジャンプ
    /// </summary>
    public void Jump()
    {
        if (Controller.isGrounded && !SkillUsing)
        {
            // ジャンプ前の行動を記録
            beforBehavior = Behavior;

            moveDirection.y = SettingMng.JumpSpeed;
            Behavior = BehaviorType.Jump;
        }
    }

    public void AddFireBool(CharacterBase target)
    {
        var fireBool = CommonFunction.Instantiate<FireBool>("Prefabs/Battle/FireBool", null, transform.position);
        fireBool.SetInfo(this, target, 1);
    }

    #endregion
    #region 装備

    /// <summary>
    /// 装備する
    /// </summary>
    /// <param name="equipmentId"></param>
    public virtual void EquipEquipment(ItemEquipmentData equipmentData)
    {
        // デフォルトスキル追加
        AddSkills(ConfigMng.E.Equipment[equipmentData.equipmentConfig.ID].Skill);
        // ゲットスキル追加
        AddSkills(equipmentData.AttachSkillsStr);

        // 装備ステータス追加
        Parameter.Equipment.AddEquiptment(equipmentData.equipmentConfig.ID);

        // Hpバーを更新
        RefreshHpBar();
    }

    /// <summary>
    /// 装備を消す
    /// </summary>
    /// <param name="equipmentId"></param>
    public virtual void RemoveEquipment(ItemEquipmentData equipmentData)
    {
        // デフォルトスキル削除
        RemoveSkills(ConfigMng.E.Equipment[equipmentData.equipmentConfig.ID].Skill);

        // ゲットスキル削除
        RemoveSkills(equipmentData.AttachSkillsStr);

        // 装備ステータス削除
        Parameter.Equipment.RemoveEquipment(equipmentData.equipmentConfig.ID);

        // Hpバーを更新
        RefreshHpBar();
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
    public Vector2 GetDircetion(Transform target)
    {
        return GetDircetion(target.position);
    }
    protected Vector2 GetDircetion(Vector3 target)
    {
        return CommonFunction.GetDirection(target, transform.position).normalized;
    }

    public Vector3 Get()
    {
        return FowardObj.position - transform.position;
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
    protected bool IsMapAreaOutX(float posX)
    {
        try
        {
            if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            {
                return MapMng.E.IsMapAreaOutX(transform.position.x, moveDirection.x);
            }
            else
            {
                if (transform.position.x < SettingMng.MoveBoundaryOffset
                && transform.position.x + posX < transform.position.x)
                    return true;

                if (transform.position.x > DataMng.E.MapData.Config.SizeX - SettingMng.MoveBoundaryOffset
                    && transform.position.x + posX > transform.position.x)
                    return true;

                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex.Message);
            return false;
        }

    }
    /// <summary>
    /// 移動範囲境界判断　Y
    /// </summary>
    protected bool IsMapAreaOutZ(float posZ)
    {
        try
        {
            if (DataMng.E.RuntimeData.MapType == MapType.AreaMap)
            {
                return MapMng.E.IsMapAreaOutZ(transform.position.z, moveDirection.z);
            }
            else
            {
                if (transform.position.z < SettingMng.MoveBoundaryOffset
                && transform.position.z + posZ < transform.position.z)
                    return true;

                if (transform.position.z > DataMng.E.MapData.Config.SizeZ - SettingMng.MoveBoundaryOffset
                    && transform.position.z + posZ > transform.position.z)
                    return true;

                return false;
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex.Message);
            return false;
        }
    }

    #endregion
    #region Emum

    /// <summary>
    /// キャラクタグループ
    /// </summary>
    public enum CharacterGroup
    {
        /// <summary>
        /// キャラクター
        /// </summary>
        Player = 1,

        /// <summary>
        /// モンスター
        /// </summary>
        Monster,

        /// <summary>
        /// 妖精
        /// </summary>
        Fairy,
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

    /// <summary>
    /// 壊す
    /// </summary>
    Breack = 4,

    Jump = 5, // ジャンプ

    /// <summary>
    /// 料理を食べる
    /// </summary>
    EatFood = 6,

    /// <summary>
    /// 凍結
    /// </summary>
    Freeze = 10,

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