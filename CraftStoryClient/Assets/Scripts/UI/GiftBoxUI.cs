using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxUI : UIBase
{
    Button OKBtn;
    Transform itemGridRoot;

    public override void Init()
    {
        base.Init();

        GiftBoxLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        OKBtn = FindChiled<Button>("OKBtn");
        OKBtn.onClick.AddListener(() => 
        {
            Close();
            CommonFunction.GoToNextScene();
        });

        itemGridRoot = FindChiled("Content");
    }

    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
    }

    public void AddBonus(List<int> bonus)
    {
        for (int i = 0; i < bonus.Count; i++)
        {
            var config = ConfigMng.E.Bonus[bonus[i]];
            AddItem(config.Bonus1, config.BonusCount1);
        }
    }
    private void AddItem(int itemID, int count)
    {
        if (itemID < 0)
            return;

        var cell = AddCell<IconItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Add(ConfigMng.E.Item[itemID], count);
        }
    }
}
