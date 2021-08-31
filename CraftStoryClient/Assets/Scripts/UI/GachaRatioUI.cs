using UnityEngine;
using UnityEngine.UI;

public class GachaRatioUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Transform CellParent { get => FindChiled("Content"); }

    public override void Init()
    {
        base.Init();
        GachaRatioLG.E.Init(this);

        Title.SetTitle("ガチャ提供割合");
        Title.SetOnClose(() => { Close(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);
    }
    public void Set(int id)
    {
        var config = ConfigMng.E.Gacha[id];
        Des.text = config.Des;

        SetCell(id);
    }

    private void SetCell(int id)
    {
        var config = ConfigMng.E.Gacha[id];
        var pond = ConfigMng.E.RandomBonusPond[config.PondId];

        SetCell(pond.BonusList01, pond.Percent01, 1);
        SetCell(pond.BonusList02, pond.Percent02, 2);
        SetCell(pond.BonusList03, pond.Percent03, 3);
    }
    private void SetCell(string stringList, int percent, int rare)
    {
        var list = stringList.Split(',');

        for (int i = 0; i < list.Length; i++)
        {
            var cell = AddCell<GachaRatioCell>("Prefabs/UI/GachaRatioCell", CellParent);
            if (cell != null)
            {
                cell.Set(int.Parse(list[i]), percent / list.Length, rare);
            }
        }
    }
}
