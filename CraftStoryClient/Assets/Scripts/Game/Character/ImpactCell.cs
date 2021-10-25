using JsonConfigData;
using UnityEngine;

public class ImpactCell
{
    CharacterBase target;
    CharacterBase attacker;
    int impactId;
    float timer;

    int count;

    Impact config { get => ConfigMng.E.Impact[impactId]; }

    public ImpactCell(CharacterBase target, CharacterBase attacker, int impactId)
    {
        this.target = target;
        this.attacker = attacker;
        this.impactId = impactId;

        count = config.Count;
        timer = config.Delay;

        if (config.Effect != "N")
        {
            EffectMng.E.AddBattleEffect(config.Effect, 3, target.transform, target.transform);
        }
    }

    public void OnUpdate()
    {
        timer -= Time.deltaTime;

        // 時間が切ると実行する
        if (timer <= 0)
            Active();
    }

    private void Active()
    {
        int damage = 0;

        switch ((ImpactType)config.Type)
        {
            case ImpactType.AddDamage:
                // ダメージ計算
                damage = BattleCalculationCtl.CalculationDamage(attacker, target, impactId);
                target.AddDamage(attacker, damage);
                target.Hit(config.TargetFreezeTime);
                break;

            case ImpactType.Recovery:
                // 回復量計算
                damage = BattleCalculationCtl.CalculationRecoveryValue(attacker, impactId);
                target.Recovery(damage);
                break;

            default:
                break;
        }

        count--;

        if (count > 0)
        {
            timer += config.Interval;
        }
        else
        {
            target.RemoveImpact(this);
        }
    }


    private enum ImpactType
    {
        /// <summary>
        /// ダメージを与える
        /// </summary>
        AddDamage = 1,

        /// <summary>
        /// 回復（HP）
        /// </summary>
        Recovery = 5,
    }
}