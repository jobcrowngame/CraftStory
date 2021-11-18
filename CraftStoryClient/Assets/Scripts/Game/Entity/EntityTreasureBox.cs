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

    private System.Collections.IEnumerator OnClickIE(JsonData rp)
    {
        if (anim != null)
            anim.Play("object_3d_065--01_Open");

        effect.gameObject.SetActive(true);

        yield return new WaitForSeconds(3);

        if (!string.IsNullOrEmpty(rp.ToString()))
        {
            var bonusList = JsonMapper.ToObject<List<BonusListCell>>(rp.ToJson());
            foreach (var cell in bonusList)
            {
                for (int i = 0; i < cell.count; i++)
                {
                    AdventureCtl.E.AddBonus(cell.bonus);
                }
            }
        }

        WorldMng.E.MapCtl.DeleteEntity(this);
    }
}