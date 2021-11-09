using JsonConfigData;
using System;
using System.Collections;

using UnityEngine;

public class EquipSkillCell : UIBase
{
    MyButton MyButton { get => GetComponent<MyButton>(); }

    int skillId;
    Skill config { get => ConfigMng.E.Skill[skillId]; }

    private void Awake()
    {
        MyButton.AddClickListener((i) =>
        {
            if (skillId <= 0)
                return;

            UICtl.E.OpenUI<SkillExplanationUI>(UIType.SkillExplanation, UIOpenType.OnCloseDestroyObj, skillId);
        });
    }

    public void Set(int skillId)
    {
        this.skillId = skillId;

        if (skillId <= 0)
        {
            MyButton.SetIcon(ConfigMng.E.Skill[skillId].Icon);
            return;
        }

        MyButton.SetIcon(config.Icon);
    }

    public void ShowNewAddAnimation(int skillId)
    {
        StartCoroutine(ShowNewAddAnimationIE(skillId));
    }

    IEnumerator ShowNewAddAnimationIE(int skillId)
    {
        var effect = EffectMng.E.AddUIEffect<EffectBase>(EquipListLG.E.UI.transform, transform.position, EffectType.Gacha);
        effect.Init();

        yield return new WaitForSeconds(1);

        Set(skillId);
    }
}
