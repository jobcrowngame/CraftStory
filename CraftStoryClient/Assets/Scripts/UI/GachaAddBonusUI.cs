using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class GachaAddBonusUI : UIBase
{
    Text Title { get => FindChiled<Text>("Title"); }
    Transform Parent { get => FindChiled("Content"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }

    Bonus bonus;

    public override void Init()
    {
        base.Init();
        GachaAddBonusLG.E.Init(this);

        Title.text = "1枚だけめくって、おまけを手に入れよう！";

        OkBtn.onClick.AddListener(()=> 
        {
            UICtl.E.OpenUI<RouletteUI>(UIType.Roulette, UIOpenType.None, 1);
            Close();
        });
    }

    public override void Open()
    {
        base.Open();
        GachaAddBonusLG.E.OnOpen();
    }

    public void Set(int index, int gachaId)
    {
        ShowOkBtn(false);

        var gachaConfig = ConfigMng.E.Gacha[gachaId];
        var rouletteConfig = ConfigMng.E.Roulette[gachaConfig.Roulette];
        var list = rouletteConfig.CellList.Split(',');
        int rouletteCellId = int.Parse(list[index - 1]);
        int bonusId = ConfigMng.E.RouletteCell[rouletteCellId].Bonus;
        bonus = ConfigMng.E.Bonus[bonusId];

        AddCells();
    }

    private void AddCells()
    {
        ClearCell(Parent);
        for (int i = 0; i < 6; i++)
        {
            var cell = AddCell<GachaAddBonusCell>("Prefabs/UI/GachaAddBonusCell", Parent);
            if (cell != null)
            {
                cell.Init();
                cell.Add(ConfigMng.E.Item[bonus.Bonus1], bonus.BonusCount1);
            }
        }
    }

    public void ShowOkBtn(bool b = true)
    {
        OkBtn.gameObject.SetActive(b);
    }
}
