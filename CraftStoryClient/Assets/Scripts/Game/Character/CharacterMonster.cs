using System.Collections;

using UnityEngine;


public class CharacterMonster : CharacterBase
{
    Animator animator { get => CommonFunction.FindChiledByName<Animator>(transform, "Model"); }
    CharacterAI ai;
    CharacterBase Target { get => ai.Target; }

    public override void Init(int characterId, CharacterGroup camp)
    {
        base.Init(characterId, camp);

        Behavior = BehaviorType.Waiting;
        ai = new CharacterAI(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (ai != null && Behavior != BehaviorType.Did)
        {
            ai.Update();
        }
    }

    public override void OnClick()
    {
        base.OnClick();

        PlayerCtl.E.CameraCtl.CancelLockUn();
        PlayerCtl.E.Character.Target = this;

        var playerPos = WorldMng.E.CharacterCtl.getPlayer().transform.position;
        var distance = Vector3.Distance(playerPos, transform.position);

        Debug.Log(distance);

        if (distance < SettingMng.MaxLockUnDistance)
        {
            HomeLG.E.UI.LockUnTarget(transform);
            HpCtl.OnLockUn(true);
        }
    }

    public override void OnBehaviorChange(BehaviorType behavior)
    {
        base.OnBehaviorChange(behavior);

        //Logger.Log("Monster Behavior: " + behavior);

        animator.SetInteger("State", (int)behavior);
    }

    protected override void Died()
    {
        base.Died();

        // モンスターを倒す場合、ボーナスが手に入る
        string[] pondIds = Parameter.PondIds.Split(',');
        for (int i = 0; i < pondIds.Length; i++)
        {
            foreach (var item in CommonFunction.GetBonusListByPondId(int.Parse(pondIds[i])))
            {
                AdventureCtl.E.AddBonus(item);
            }
        }

        // 経験値手に入る
        AdventureCtl.E.AddExp(Parameter.AddExp);

        // Exp手に入る演出
        PlayerCtl.E.AddExp(Parameter.AddExp);

        WorldMng.E.CharacterCtl.MonsterDied();

        GuideLG.E.NextOnCreateBlock();

        StartCoroutine(DiedIE());
    }

    protected override void TargetDied()
    {
        base.TargetDied();

        // 目標が死んだ場合
        ai.OnTargetDied();
    }

    IEnumerator DiedIE()
    {
        yield return new WaitForSeconds(0.8f);

        EffectMng.E.AddBattleEffect("Die", 3, transform);

        Model.gameObject.SetActive(false);

        yield return new WaitForSeconds(10f);
        GameObject.Destroy(gameObject);
    }

    /// <summary>
    /// ランダムで移動
    /// </summary>
    public void RandomMove()
    {
        var randomAngle = Random.Range(0, 360);
        var dir = CommonFunction.AngleToVector2(randomAngle);

        MoveToTarget(dir);
        StartCoroutine(RandomMoveIE());
    }
    IEnumerator RandomMoveIE()
    {
        yield return new WaitForSeconds(3f);
        StopMove();
        Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// デイズアクション
    /// </summary>
    public void DazeAction()
    {
        StartCoroutine(DazeActionIE());
    }
    IEnumerator DazeActionIE()
    {
        Behavior = BehaviorType.observe01;
        yield return new WaitForSeconds(Parameter.DazeTime);
        if (Parameter.RandomMoveOnWait == 1)
        {
            Behavior = BehaviorType.Waiting;
        }
    }

    /// <summary>
    /// 助けを呼びます
    /// </summary>
    public void CallForHelp()
    {
        StartCoroutine(CallForHelpID(1));
    }
    IEnumerator CallForHelpID(float time)
    {
        // 行動変更
        Behavior = BehaviorType.CallForHelp;

        // 向きを調整
        var direction = GetDircetion(ai.Target.transform);
        Rotation(direction);

        // 助け呼ぶ範囲内のモンスター
        var monsterList = WorldMng.E.CharacterCtl.FindTargetListInSecurityRange(Group, transform.position, Parameter.CallForHelpRange);
        foreach (CharacterMonster item in monsterList)
        {
            if (item.Parameter.RespondToHelp == 1)
            {
                item.ai.SetTarget(Target);
            }
        }

        yield return new WaitForSeconds(time);

        Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// 目標を目指して移動
    /// </summary>
    public void MoveToTarget()
    {
        // 目標がない場合、スキップ
        if (Target == null)
            return;

        // 他の動作をしてる場合、スキップ
        if (CanNotChangeBehavior())
            return;

        // 向きを調整
        var direction = GetDircetion(Target.transform);
        Rotation(direction);
        MoveToTarget(direction);
    }
    public void MoveToTarget(Vector2 direction)
    {
        if (Behavior != BehaviorType.Run) Behavior = BehaviorType.Run;

        // 移動遷移量設定
        moveDirection.x = direction.x * Parameter.MoveSpeed * Time.deltaTime;
        moveDirection.z = direction.y * Parameter.MoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 攻撃
    /// </summary>
    public void Attack()
    {
        // 共有CD冷却中場合、スキップ
        if (ShareCDIsCooling)
            return;

        foreach (var skill in SkillList)
        {
            StopMove();
            Behavior = BehaviorType.Waiting;
            StartUseSkill(CharacterGroup.Player, skill, Target);
        }
    }
}