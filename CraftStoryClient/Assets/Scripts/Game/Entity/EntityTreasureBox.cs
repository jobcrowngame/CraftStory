using LitJson;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝ボックス
/// </summary>
public class EntityTreasureBox : EntityBase
{
    Animation anim { get => CommonFunction.FindChiledByName<Animation>(transform, "Model"); }
    Transform effect { get => CommonFunction.FindChiledByName(transform, "TreasureBox").transform; }

    bool playing;

    public override void ClickingEnd()
    {
        base.ClickingEnd();

        Logger.Warning("EntityTreasureBox cliking end");
    }

    public override void OnClick()
    {
        base.OnClick();

        if (playing)
            return;

        playing = true;

        NWMng.E.GetRandomBonus((rp) =>
        {
            StartCoroutine(OnClickIE(rp));
        }, EConfig.BonusID);
    }

    private struct BonusListCell
    {
        public int bonus { get; set; }
        public int count { get; set; }
    }

    private System.Collections.IEnumerator OnClickIE(Dictionary<int, int> rp)
    {
        if (anim != null)
            anim.Play("object_3d_065--01_Open");

        effect.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        foreach (var dic in rp)
        {
            for (int i = 0; i < dic.Value; i++)
            {
                AdventureCtl.E.AddBonus(dic.Key);
            }
        }

        yield return new WaitForSeconds(2);

        WorldMng.E.MapCtl.DeleteEntity(this);
    }
}