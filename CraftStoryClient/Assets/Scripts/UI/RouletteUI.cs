using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

public class RouletteUI : UIBase
{
    TitleUI Title { get => FindChiled<TitleUI>("Title"); }
    Image RouletteBG { get => FindChiled<Image>("RouletteBG"); }
    Button StartBtn { get => FindChiled<Button>("StartBtn"); }

    int gachaId;
    int index;
    float speed;
    bool start;
    bool end = false;

    public override void Init()
    {
        base.Init();
        RouletteLG.E.Init(this);

        Title.SetTitle("ルーレット");
        Title.SetOnClose(() => { Close(); });
        Title.EnActiveCoin(1);
        Title.EnActiveCoin(2);
        Title.EnActiveCoin(3);

        StartBtn.onClick.AddListener(StartRoulette);
    }

    public override void Open()
    {
        base.Open();
        StartBtn.enabled = true;
        Title.CloseBtnEnable(false);
        RouletteBG.transform.eulerAngles = Vector3.zero;
    }

    private void Update()
    {
        if (start)
        {
            RouletteBG.transform.Rotate(Vector3.back, speed * Time.deltaTime);

            if (speed > 0)
            {
                speed -= Time.deltaTime * 100;
            }
            else
            {
                speed = 0;
            }

            if (speed < 268 && !end)
            {
                speed = 268;
                end = CheckIndex();
            }
        }
    }
    public void Set(int index, int gachaId)
    {
        this.gachaId = gachaId;
        this.index = index;

        var config = ConfigMng.E.Roulette[ConfigMng.E.Gacha[gachaId].Roulette];
        string[] rouletteCells = config.CellList.Split(',');

        for (int i = 0; i < rouletteCells.Length; i++)
        {
            var cell = FindChiled<RouletteItemCell>(string.Format("GameObject ({0})", i + 1));
            if (cell != null)
            {
                var cellConfig = ConfigMng.E.RouletteCell[int.Parse(rouletteCells[rouletteCells.Length - i - 1])];
                var bonusCofnig = ConfigMng.E.Bonus[cellConfig.Bonus];
                cell.Set(bonusCofnig.Bonus1, bonusCofnig.BonusCount1);
            }
        }
    }

    private void StartRoulette()
    {
        StartBtn.enabled = false;
        Title.CloseBtnEnable(true);

        speed = 1000;
        start = true;
        end = false;
    }

    private bool CheckIndex()
    {
        var angles = RouletteBG.transform.eulerAngles.z;
        var min = 36 * (index - 1);
        var max = 36 * index;
        return angles >= min && angles < max;
    }
}
