
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
        if (mCharacter.State == CharacterBase.StateType.Died)
            return;

        // 待てると次の動作を続けます
        if (mCharacter.State == CharacterBase.StateType.Waiting)
        {
            FindTarget();

            if (InCombat)
            {
                if (CheckInAttackRange())
                {
                    mCharacter.Attack();
                }
                else
                {
                    mCharacter.MoveToTarget();
                }
            }
        }
    }

    /// <summary>
    /// 目標を探す
    /// </summary>
    public void FindTarget()
    {
        if (InCombat)
            return;

        if (TargetInSecurityRange())
        {
            if (!InCombat)
                mCharacter.CallForHelp();

            // 戦闘状態になる
            InCombat = true;
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
    }

    /// <summary>
    /// 目標が警備範囲内にあるかのチェック
    /// </summary>
    /// <returns></returns>
    public bool TargetInSecurityRange()
    {
        target = CharacterCtl.E.FindTargetInSecurityRange(CharacterBase.CharacterCamp.Player, 
            mCharacter.transform.position, mCharacter.Parameter.SecurityRange);

        return target != null;
    }

    /// <summary>
    /// 攻撃範囲内かのチェック
    /// </summary>
    /// <returns></returns>
    public bool CheckInAttackRange()
    {
        if (target == null)
            return false;

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

        return Mathf.Abs(Vector3.Distance(mCharacter.transform.position, target.transform.position)) <= maxDistance;
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