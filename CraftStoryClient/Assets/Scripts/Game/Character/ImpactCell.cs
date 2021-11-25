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

        if (target != null && config.Effect != "N")
        {
            EffectMng.E.AddBattleEffectHaveParent(config.Effect, 3, target.transform);
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
            case ImpactType.Debuffer:
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

            // 即死ぬ
            case ImpactType.DieInstantly:
                int random = Random.Range(0, 100);
                if (random <= config.PercentDamage)
                {
                    // Effect 追加
                    if (config.Effect2 != "N")
                        EffectMng.E.AddBattleEffectHaveParent(config.Effect2, 3, target.transform);

                    target.AddDamage(attacker, target.Parameter.CurHP);
                }
                break;

            // フリーズ
            case ImpactType.Freeze:
                random = Random.Range(0, 100);
                if (random <= config.PercentDamage)
                {
                    // Effect 追加
                    if (config.Effect2 != "N")
                        EffectMng.E.AddBattleEffectHaveParent(config.Effect2, 3, target.transform);

                    target.Hit(config.TargetFreezeTime);
                }
                break;

            // フリーズ
            case ImpactType.MoveSpeedUp:
                target.MoveSpeed *= config.PercentDamage * 0.01f;
                break;

            default: Logger.Warning("未知のImpact " + config.Type); break;
        }

        count--;

        if (count > 0)
        {
            timer = config.Interval;
        }
        else
        {
            switch ((ImpactType)config.Type)
            {
                // フリーズ
                case ImpactType.MoveSpeedUp:
                    target.MoveSpeed = 1;
                    break;

                default: break;
            }

            target.RemoveImpact(this);
        }
    }


    public enum ImpactType
    {
        /// <summary>
        /// ダメージを与える
        /// </summary>
        AddDamage = 1,

        /// <summary>
        /// デバフ
        /// </summary>
        Debuffer = 2,

        /// <summary>
        /// 即死ぬ
        /// </summary>
        DieInstantly = 3,

        /// <summary>
        /// フリーズ（freeze）
        /// </summary>
        Freeze = 4,

        /// <summary>
        /// 回復（HP）
        /// </summary>
        Recovery = 5,

        /// <summary>
        /// ダメージアップ
        /// </summary>
        DamageUp = 10,

        /// <summary>
        /// 移動スピードアップ
        /// </summary>
        MoveSpeedUp = 101,
    }
}