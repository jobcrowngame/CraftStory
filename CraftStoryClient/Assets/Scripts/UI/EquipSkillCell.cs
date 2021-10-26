﻿using JsonConfigData;

public class EquipSkillCell : UIBase
{
    MyButton MyButton { get => GetComponent<MyButton>(); }

    int skillId;
    Skill config { get => ConfigMng.E.Skill[skillId]; }

    private void Awake()
    {
        MyButton.AddClickListener((i) =>
        {
            UICtl.E.OpenUI<SkillExplanationUI>(UIType.SkillExplanation, UIOpenType.OnCloseDestroyObj, skillId);
        });
    }

    public void Set(int skillId)
    {
        if (skillId <= 0)
        {
            MyButton.SetIcon(ConfigMng.E.Skill[0].Icon);
            return;
        }

        this.skillId = skillId;

        MyButton.SetIcon(config.Icon);
    }
}
