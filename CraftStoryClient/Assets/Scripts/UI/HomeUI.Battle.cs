using UnityEngine;
using UnityEngine.UI;

public partial class HomeUI
{
    Transform battle { get => FindChiled("Battle"); }
    Image hpBar { get => FindChiled<Image>("HpImg", battle); }
    Text hpText { get => FindChiled<Text>("HpText", battle); }

    Transform Battle { get => FindChiled("Battle"); }
    SkillCell[] skills;

    public void OnHpChange(float percent)
    {
        hpBar.fillAmount = percent;
        hpText.text = (percent * 100).ToString("F0") + "%";
    }

    /// <summary>
    /// スキル設定
    /// </summary>
    public void SetSkills()
    {
        int index = 0;
        for (int i = 0; i < Battle.GetChild(0).childCount; i++)
        {
            if (PlayerCtl.E.Character.SkillList.Count > index)
            {
                if (PlayerCtl.E.Character.SkillList[index].Config.CanEquipment != 1)
                {
                    i--;
                    index++;
                    continue;
                }

                skills[i].SetBase(PlayerCtl.E.Character.SkillList[index]);
            }
            else
            {
                skills[i].SetBase(null);
            }

            index++;
        }
    }
}