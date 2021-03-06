using UnityEngine;
using UnityEngine.UI;

public partial class HomeUI
{
    Transform battle { get => FindChiled("Battle"); }
    Transform PointGet { get => FindChiled("PointGet"); }

    Image hpBar { get => FindChiled<Image>("HpImg", battle); }
    Text hpText { get => FindChiled<Text>("HpText", battle); }
    MyText MonsterNumberLeft { get => FindChiled<MyText>("NumberLeft"); }

    public Transform Battle { get => FindChiled("Battle"); }
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

    /// <summary>
    /// 敵残り数を出す
    /// </summary>
    public void ShowMonsterNumberLeft()
    {
        MonsterNumberLeft.gameObject.SetActive(WorldMng.E.CharacterCtl.RemainingNumber > 0);
        UpdateNumberLeftMonster(WorldMng.E.CharacterCtl.RemainingNumber);
    }

    /// <summary>
    /// 残った敵数
    /// </summary>
    public void UpdateNumberLeftMonster(int count)
    {
        MonsterNumberLeft.text = string.Format("残り {0} 体", count);

        if (count == 0)
        {
            //MonsterNumberLeft.GetComponent<Animation>().Play("HomeNumberLeft");
            MonsterNumberLeft.text = string.Format("転送門へ移動しよう");
        }
    }

    /// <summary>
    /// ポイントゲット場合の表現
    /// </summary>
    public void ShowPointGet()
    {
        PointGet.GetComponent<Animation>().Play("HomePointGet");
    }

    /// <summary>
    /// ロックオン
    /// </summary>
    /// <param name="target">目標</param>
    public void LockUnTarget(Transform target)
    {
        PlayerCtl.E.CameraCtl.LockUnTarget(target);
    }
}