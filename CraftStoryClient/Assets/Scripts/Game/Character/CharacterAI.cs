
using UnityEngine;

public class CharacterAI
{
    CharacterMonster mCharacter;
    CharacterBase target;

    public bool InCombat { get; private set; }
    public CharacterBase Target { get => target; }

    float dazeTime = 0;

    public CharacterAI(CharacterMonster character)
    {
        mCharacter = character;
    }

    public void Update()
    {
        // 死んだら何もしない
        if (mCharacter.Behavior == BehaviorType.Did ||
            mCharacter.Behavior == BehaviorType.Hit)
            return;

        if (InCombat)
        {
            if (target == null || target.IsDied)
                FindTarget();
            else
            {
                if (CheckInAttackRange())
                    mCharacter.Attack();
                else
                {
                    mCharacter.MoveToTarget();
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
            }
        }
        else if (mCharacter.Parameter.RandomMoveOnWait == 0)
        {
            mCharacter.DazeAction();
        }
        else
        {
            InCombat = false;

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
                dazeTime = SettingMng.DazeTime + Random.Range(-2,2);
            }
            else
            {
                dazeTime -= Time.deltaTime;
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
        target = WorldMng.E.CharacterCtl.FindTargetInSecurityRange(CharacterBase.CharacterGroup.Player, 
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