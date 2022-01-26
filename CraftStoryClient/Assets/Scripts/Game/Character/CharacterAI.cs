
using UnityEngine;

public class CharacterAI
{
    CharacterMonster mCharacter;
    CharacterBase target;

    public bool InCombat { get; private set; }
    public CharacterBase Target { get => target; }

    float dazeTime = 0;
    bool mGotoCreatedPos;
    public bool Active { get; set; }

    public CharacterAI(CharacterMonster character)
    {
        mCharacter = character;
        Active = true;
    }

    public void Update()
    {
        if (!Active)
            return;

        // 死んだら何もしない
        if (mCharacter.Behavior == BehaviorType.Did ||
            mCharacter.Behavior == BehaviorType.Hit)
            return;

        // デスポーンチェック
        mCharacter.CheckDespawn(CheckDespawnRange());

        if (InCombat)
        {
            if (target == null || target.IsDied)
                FindTarget();
            else
            {
                // 追いかける距離以外になると、やめる
                if (CheckOutChaseRange())
                {
                    mGotoCreatedPos = true;
                    InCombat = false;
                    return;
                }

                if (CheckInAttackRange())
                    mCharacter.Attack();
                else
                {
                    // 目標があり、他の動作をしてない場合
                    if (Target != null && !mCharacter.CanNotChangeBehavior())
                        mCharacter.MoveToTargetPos(Target.transform.position);
                }
            }
        }
        else
        {
            FindTarget();
        }
    }

    /// <summary>
    /// 目標を探す
    /// </summary>
    public void FindTarget()
    {
        if (TargetInSecurityRange())
        {
            if (!InCombat)
            {
                // 助を呼ぶ
                mCharacter.CallForHelp();

                // 戦闘状態になる
                InCombat = true;

                // 生成座標に戻るをやめる
                mGotoCreatedPos = false;
            }
        }
        else
        {
            InCombat = false;

            // 生成された座標に戻る
            if (mGotoCreatedPos)
            {
                mCharacter.MoveToTargetPos(mCharacter.CreatedPos);

                // 生成された座標にいる場合、止める
                if (mCharacter.InCreatedPos())
                {
                    mGotoCreatedPos = false;
                    mCharacter.StopMove();
                }

                return;
            }

            if (mCharacter.Parameter.RandomMoveOnWait == 0)
            {
                mCharacter.DazeAction();
            }
            else
            {

                if (dazeTime <= 0)
                {
                    var random = Random.Range(0, 100);
                    if (random < 50)
                    {
                        mCharacter.RandomMove();
                    }
                    else
                    {
                        mCharacter.DazeAction();
                    }

                    // 動作の時間を分ける為 -2,+2offsetを加算
                    dazeTime = SettingMng.DazeTime + Random.Range(-2, 2);
                }
                else
                {
                    dazeTime -= Time.deltaTime;
                }
            }
        }
    }

    /// <summary>
    /// 目標が死んだ場合
    /// </summary>
    public void OnTargetDied()
    {
        target = null;
        mCharacter.Behavior = BehaviorType.Waiting;
    }

    /// <summary>
    /// 目標が警備範囲内にあるかのチェック
    /// </summary>
    /// <returns></returns>
    public bool TargetInSecurityRange()
    {
        target = CharacterCtl.E.FindTargetInSecurityRange(CharacterBase.CharacterGroup.Player, 
            mCharacter.transform.position, mCharacter.Parameter.SecurityRange);

        return target != null;
    }

    /// <summary>
    /// 攻撃範囲内かのチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInAttackRange()
    {
        var pointS = mCharacter.transform.position;
        var pointE = target.transform.position;
        var dis = Vector3.Distance(pointS, pointE);

        // 攻撃範囲内のスキルを選ぶ
        float maxDistance = 0;
        foreach (var skill in mCharacter.SkillList)
        {
            if (skill.IsCooling)
                continue;

            if (skill.distance > maxDistance)
            {
                maxDistance = skill.distance;
            }
        }

        // 使用できるスキルがない場合、最小距離まで行くと移動しない
        if (maxDistance == 0)
        {
            maxDistance = SettingMng.MinDistance;
        }

        return Mathf.Abs(dis) <= maxDistance;
    }

    public bool CheckOutChaseRange()
    {
        if (Target == null || mCharacter == null)
            return true;
        
        var dis = Mathf.Abs( Vector3.Distance(Target.transform.position, mCharacter.transform.position));
        return dis > SettingMng.AreaMapChaseRange;
    }

    /// <summary>
    /// デスポーン範囲以外のチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckDespawnRange()
    {
        var player = CharacterCtl.E.getPlayer();
        var dis = Mathf.Abs(Vector3.Distance(player.transform.position, mCharacter.transform.position));
        return dis > SettingMng.AreaMapMonsterDespawnRange;
    }

    /// <summary>
    /// ターゲットを与える
    /// </summary>
    /// <param name="_target">ターゲット</param>
    public void SetTarget(CharacterBase _target)
    {
        target = _target;
        InCombat = true;
    }
}