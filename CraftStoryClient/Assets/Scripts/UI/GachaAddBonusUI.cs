using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GachaAddBonusUI : UIBase
{
    Transform Items { get => FindChiled("Items"); }
    Transform Parent { get => FindChiled("Content"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }

    Transform CardFlipping { get => FindChiled("CardFlippingBtn"); }
    Button CardFlippingBtn { get => CardFlipping.GetComponent<Button>(); }

    Bonus bonus;
    int gachaId;

    public override void Init()
    {
        base.Init();
        GachaAddBonusLG.E.Init(this);

        OkBtn.onClick.AddListener(()=> 
        {
            UICtl.E.OpenUI<RouletteUI>(UIType.Roulette, UIOpenType.None, gachaId);
            Close();
        });
        CardFlippingBtn.onClick.AddListener(() =>
        {
            Items.gameObject.SetActive(true);
            CardFlipping.gameObject.SetActive(false);
        });
    }

    public override void Open()
    {
        base.Open();

        Items.gameObject.SetActive(false);

        GachaAddBonusLG.E.OnOpen();

        StartCoroutine(CutInIE());
    }

    public void Set(int index, int gachaId)
    {
        ShowOkBtn(false);

        //Logger.Log("index:{0} gachaId:{1}", index, gachaId);

        this.gachaId = gachaId;
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

    IEnumerator CutInIE()
    {
        yield return new WaitForSeconds(2f);
        CardFlipping.gameObject.SetActive(true);

        AudioMng.E.ShowSE("cutinSE");
    }
}
