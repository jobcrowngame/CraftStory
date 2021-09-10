using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaRatioUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Text Des { get => FindChiled<Text>("Des"); }
    Transform CellParent { get => FindChiled("Content"); }
    Text Level1P { get => FindChiled<Text>("Level1P"); }
    Text Level2P { get => FindChiled<Text>("Level2P"); }
    Text Level3P { get => FindChiled<Text>("Level3P"); }

    List<GachaRatioCell> cellList = new List<GachaRatioCell>();

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
        Des.text = config.Des == "N" ? "" : config.Des;

        SetTitlePercent(id);
        SetCell(id);
    }

    private void SetTitlePercent(int id)
    {
        var config = ConfigMng.E.Gacha[id];
        var pond = ConfigMng.E.RandomBonusPond[config.PondId];

        int level1 = 0;
        int level2 = 0;
        int level3 = 0;
        ChangePercent(pond.Percent01, pond.Level01, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent02, pond.Level02, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent03, pond.Level03, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent04, pond.Level04, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent05, pond.Level05, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent06, pond.Level06, ref level1, ref level2, ref level3);
        ChangePercent(pond.Percent07, pond.Level07, ref level1, ref level2, ref level3);

        Level1P.text = level1 * 0.1f + "%";
        Level2P.text = level2 * 0.1f + "%";
        Level3P.text = level3 * 0.1f + "%";
    }
    private void ChangePercent(int percent, int rare, ref int level1, ref int level2, ref int level3)
    {
        switch (rare)
        {
            case 1: level1 += percent; break;
            case 2: level2 += percent; break;
            case 3: level3 += percent; break;
        }
    }

    private void SetCell(int id)
    {
        // サブを削除
        foreach (var item in cellList)
        {
            GameObject.Destroy(item.gameObject);
        }
        cellList.Clear();

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

        // サブを追加
        for (int i = 0; i < list.Length; i++)
        {
            var cell = AddCell<GachaRatioCell>("Prefabs/UI/GachaRatioCell", CellParent);
            if (cell != null)
            {
                cell.Set(int.Parse(list[i]), percent / list.Length, rare);
                cellList.Add(cell);
            }
        }
    }
}
