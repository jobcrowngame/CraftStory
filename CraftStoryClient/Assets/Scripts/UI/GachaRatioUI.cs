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

        SetCell(pond.BonusList07, pond.Percent07, pond.Level07);
        SetCell(pond.BonusList06, pond.Percent06, pond.Level06);
        SetCell(pond.BonusList05, pond.Percent05, pond.Level05);
        SetCell(pond.BonusList04, pond.Percent04, pond.Level04);
        SetCell(pond.BonusList03, pond.Percent03, pond.Level03);
        SetCell(pond.BonusList02, pond.Percent02, pond.Level02);
        SetCell(pond.BonusList01, pond.Percent01, pond.Level01);
    }
    private void SetCell(string stringList, int percent, int rare)
    {
        if (stringList == "N")
            return;

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
