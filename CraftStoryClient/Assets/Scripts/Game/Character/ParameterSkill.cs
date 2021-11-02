using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ParameterSkill
{
    public int HP { get; set; }
    public int FixDamage { get; set; }
    public int PercentDamage { get; set; }
    public int Defense { get; set; }

    Parameter par;

    public ParameterSkill(Parameter par)
    {
        this.par = par;
    }

    public void AddSkill(SkillData skill, bool isUp = true)
    {
        // ステータスアップスキルがある場合
        if ((SkillData.SkillType)skill.Config.Type != SkillData.SkillType.ParameterUp)
            return;

        foreach (var imp in skill.Impacts)
        {
            var config = ConfigMng.E.Impact[int.Parse(imp)];
            var percentDamage = config.PercentDamage > 0 ? config.PercentDamage : 0;

            switch ((ImpactCell.ImpactType)config.Type)
            {
                case ImpactCell.ImpactType.DamageUp:
                    PercentDamage += percentDamage;
                    FixDamage += config.FixtDamage;
                    break;
            }
        }
    }

    public void RemoveSkill(SkillData skill)
    {
        // ステータスアップスキルがある場合
        if ((SkillData.SkillType)skill.Config.Type != SkillData.SkillType.ParameterUp)
            return;

        foreach (var imp in skill.Impacts)
        {
            var config = ConfigMng.E.Impact[int.Parse(imp)];
            var percentDamage = config.PercentDamage > 0 ? config.PercentDamage : 0;

            switch ((ImpactCell.ImpactType)config.Type)
            {
                case ImpactCell.ImpactType.DamageUp:
                    PercentDamage -= percentDamage;
                    FixDamage -= config.FixtDamage;
                    break;
            }
        }
    }
}