using System.Collections;

using UnityEngine;


public class CharacterMonster : CharacterBase
{
    Animator animator { get => CommonFunction.FindChiledByName<Animator>(transform, "Model"); }
    CharacterAI ai;
    CharacterBase Target { get => ai.Target; }

    public override void Init(int characterId, CharacterCamp camp)
    {
        base.Init(characterId, camp);

        Behavior = BehaviorType.Waiting;
        ai = new CharacterAI(this);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (ai != null || Behavior != BehaviorType.Did)
        {
            ai.Update();
        }
    }

    public override void OnClick()
    {
        base.OnClick();

        PlayerCtl.E.Character.Target = this;
        HomeLG.E.UI.LockUnTarget(transform);
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
        foreach (var item in CommonFunction.GetBonusListByPondId(Parameter.PondId))
        {
            AdventureCtl.E.AddBonus(item);
        }

        // 経験値手に入る
        AdventureCtl.E.AddExp(Parameter.AddExp);

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
        Behavior = BehaviorType.Waiting;
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
        var direction = GetTargetDircetion(ai.Target.transform);
        Rotation(direction);

        // 助け呼ぶ範囲内のモンスター
        var monsterList = CharacterCtl.E.FindTargetListInSecurityRange(Camp, transform.position, Parameter.CallForHelpRange);
        foreach (CharacterMonster item in monsterList)
        {
            item.ai.SetTarget(Target);
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
        var direction = GetTargetDircetion(Target.transform);
        MoveToTarget(direction);
    }
    public void MoveToTarget(Vector2 direction)
    {
        Behavior = BehaviorType.Run;

        // 向きを調整
        Rotation(direction);

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
            StartUseSkill(skill, Target);
        }
    }
}