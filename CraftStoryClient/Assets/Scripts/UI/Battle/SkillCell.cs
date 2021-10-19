using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCell : UIBase
{
    Button btn { get => GetComponent<Button>(); }
    Image Icon { get => GetComponent<Image>(); }
    Image Mask { get => FindChiled<Image>("Mask"); }
    Text CD { get => FindChiled<Text>("CD"); }

    SkillData mSkill;

    public void Set(SkillData skill)
    {
        mSkill = skill;
        mSkill.SetSkillCell(this);

        Icon.sprite = ReadResources<Sprite>(skill.Config.Icon);

        btn.onClick.AddListener(() => 
        {
            if (mSkill.IsCooling)
                return;

            PlayerCtl.E.UserSkill(mSkill);
        });

        RefreshCD(0);
    }

    public void RefreshCD(float curCD)
    {
        Mask.fillAmount = curCD / mSkill.Config.CD;

        int cd = (int)curCD + 1;
        CD.text = curCD > 0 ? cd.ToString() : "";
    }
}
