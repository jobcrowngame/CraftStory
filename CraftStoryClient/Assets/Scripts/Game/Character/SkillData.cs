using JsonConfigData;

using System;
using UnityEngine;

public class SkillData
{
    public float distance { get => Config.Distance; }

    public bool IsCooling { get => curCooling > 0; }
    private float curCooling;

    // インパクト
    public string[] Impacts { get => Config.Impact.Split(','); }

    public Skill Config { get; private set; }

    SkillCell mSkillCell;

    public SkillData(int skillId)
    {
        Config = ConfigMng.E.GetSkillById(skillId);
    }

    public void OnUpdate()
    {
        if (IsCooling)
        {
            curCooling -= Time.deltaTime;

            if (mSkillCell != null)
            {
                mSkillCell.RefreshCD(curCooling);
            }
        }
    }

    public void Use()
    {
        curCooling = Config.CD;
    }

    public void DecreaseCD(float time)
    {
        curCooling -= time;
    }

    public void SetSkillCell(SkillCell skillCell)
    {
        mSkillCell = skillCell;
    }

    public enum SkillType
    {
        RangeAttack = 1,
        SingleAttack = 2,
        MagicBool = 3,
    }
}