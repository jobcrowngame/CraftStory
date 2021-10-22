

public class BattleCalculationCtl
{
    /// <summary>
    /// ダメージの計算
    /// </summary>
    /// <param name="attacker">攻撃者</param>
    /// <param name="target">ターゲット</param>
    /// <param name="impactId">インパクトID</param>
    /// <returns></returns>
    public static int CalculationDamage(CharacterBase attacker, CharacterBase target, int impactId)
    {
        int damage = attacker.Parameter.Damage;
        var impactCfg = ConfigMng.E.Impact[impactId];

        var skillPercentDamage = damage * (impactCfg.PercentDamage * 0.01f);
        var skillFixtDamage = impactCfg.FixtDamage > 0 ? impactCfg.FixtDamage : 0;
        var skillDamage = skillPercentDamage + skillFixtDamage;

        int defense = target.Parameter.Defense;

        // (攻撃力* (10 + スキル補正値) -防御力* 5) * 0.3
        int endDamage = (int)((damage * (10 + skillDamage) - defense * 5) * 0.3f);
        if (endDamage <= 0)
            endDamage = 1;

        return endDamage;
    }

    /// <summary>
    /// 回復量を計算
    /// </summary>
    /// <param name="attacker">攻撃者</param>
    /// <param name="impactId">インパクトID</param>
    /// <returns></returns>
    public static int CalculationRecoveryValue(CharacterBase attacker, int impactId)
    {
        int damage = attacker.Parameter.Damage;
        var impactCfg = ConfigMng.E.Impact[impactId];

        var skillPercentDamage = damage * (impactCfg.PercentDamage * 0.01f);
        var skillFixtDamage = impactCfg.FixtDamage > 0 ? impactCfg.FixtDamage : 0;
        var skillDamage = skillPercentDamage + skillFixtDamage;

        return (int)skillDamage;
    }
}