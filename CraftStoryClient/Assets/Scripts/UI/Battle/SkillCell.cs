using UnityEngine;
using UnityEngine.UI;

public class SkillCell : UIBase
{
    Button btn { get => GetComponent<Button>(); }
    Image Icon { get => GetComponent<Image>(); }
    Image CDMask { get => FindChiled<Image>("CDMask"); }
    Text CD { get => FindChiled<Text>("CD"); }
    Image CanNotUse { get => FindChiled<Image>("CanNotUse"); }
    Image Lock { get => FindChiled<Image>("Lock"); }

    SkillData mSkill;
    SkillData baseSkill;

    private void Awake()
    {
        btn.onClick.AddListener(() =>
        {
            if (mSkill.IsCooling || PlayerCtl.E.Character.ShareCDIsCooling)
                return;

            PlayerCtl.E.UserSkill(mSkill);

            int nextSkillID = mSkill.Config.NextSkill;
            foreach (var item in PlayerCtl.E.Character.SkillList)
            {
                if (item.Config.ID == nextSkillID)
                {
                    Set(item);
                }
            }
        });
    }

    public void SetBase(SkillData skill)
    {
        if (skill == null)
        {
            IsNull();
            return;
        }

        baseSkill = skill;
        Set(skill);
        RefreshCD(0);
    }

    public void Set(SkillData skill)
    {
        mSkill = skill;
        mSkill.SetSkillCell(this);

        Icon.sprite = ReadResources<Sprite>(skill.Config.Icon);
    }

    /// <summary>
    /// �X�L�����Ȃ��ꍇ�A
    /// </summary>
    private void IsNull()
    {
        CD.text = "";

        // ���b�N����Ă�
        Lock.gameObject.SetActive(true);
    }

    public void RefreshCD(float curCD)
    {
        CDMask.fillAmount = curCD / mSkill.Config.CD;

        int cd = (int)curCD + 1;
        CD.text = curCD > 0 ? cd.ToString() : "";
    }

    /// <summary>
    /// �X�L���g�p�o���Ȃ�
    /// </summary>
    /// <param name="b"></param>
    public void CanNotUseSkill(bool b)
    {
        CanNotUse.gameObject.SetActive(b);
    }
}