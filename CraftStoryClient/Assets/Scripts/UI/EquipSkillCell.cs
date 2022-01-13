using JsonConfigData;
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

            if (DataMng.E.MapData.Config.MapType == (int)MapType.Guide)
            {
                GuideLG.E.Next();
            }
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
        var effect = EffectMng.E.AddUIEffect<EffectBase>(transform, Vector3.zero, EffectType.Gacha);
        effect.Init();

        yield return new WaitForSeconds(1);

        Set(skillId);
    }
}
