using LitJson;
using System.Collections.Generic;
using UnityEngine;

public class EntityTreasureBox : EntityBase
{
    public override void ClickingEnd()
    {
        base.ClickingEnd();

        Logger.Warning("EntityTreasureBox cliking end");
    }

    public override void OnClick()
    {
        base.OnClick();

        NWMng.E.GetRandomBonus((rp) => 
        {
            if (string.IsNullOrEmpty(rp.ToString()))
                Logger.Error("Bad bonusID " + EConfig.BonusID);
            else
            {
                var bonusList = JsonMapper.ToObject<List<BonusListCell>>(rp.ToJson());
                foreach (var cell in bonusList)
                {
                    for (int i = 0; i < cell.count; i++)
                    {
                        AdventureCtl.E.AddBonus(cell.bonus);
                    }
                }

                WorldMng.E.MapCtl.DeleteEntity(this);
            }
        }, EConfig.BonusID);
    }

    private struct BonusListCell
    {
        public int bonus { get; set; }
        public int count { get; set; }
    }
}