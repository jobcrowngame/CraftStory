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
        /// <summary>
        /// 範囲攻撃
        /// </summary>
        RangeAttack = 1,

        /// <summary>
        /// 単体攻撃
        /// </summary>
        SingleAttack = 2,

        /// <summary>
        /// 遠距離範囲攻撃
        /// </summary>
        RangedRangeAttack = 3,

        /// <summary>
        /// ビーム
        /// </summary>
        Beam = 4,

        /// <summary>
        /// 回復
        /// </summary>
        Recovery = 5,

        /// <summary>
        /// ステータスアップ（被動）
        /// </summary>
        ParameterUp = 6,
    }
}