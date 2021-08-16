using JsonConfigData;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaBonusUI : UIBase
{
    Transform Parent { get => FindChiled("Content"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }

    private int gachaId;
    private int index;

    public override void Init()
    {
        base.Init();
        GachaBonusLG.E.Init(this);

        OkBtn.onClick.AddListener(()=> 
        {
            var ui = UICtl.E.OpenUI<RouletteUI>(UIType.Roulette);
            ui.Set(index, gachaId);
            Close();

            Logger.Warning(index.ToString());
        });
    }
    public void Set(ShopLG.GachaResponse result, int gachaId)
    {
        ClearCell(Parent);
        foreach (var item in result.bonusList)
        {
            var cell = AddCell<IconItemCell>("Prefabs/UI/IconItem", Parent);
            if (cell != null)
            {
                var config = ConfigMng.E.Bonus[item.bonusId];

                cell.Init();
                cell.Add(ConfigMng.E.Item[config.Bonus1], config.BonusCount1);
            }
        }

        index = result.index;
        this.gachaId = gachaId;
    }
}