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
        Dictionary<int, int> items = new Dictionary<int, int>();
        for (int i = 0; i < bonus.Count; i++)
        {
            var config = ConfigMng.E.Bonus[bonus[i]];
            BonusToItems(items, config.Bonus1, config.BonusCount1);
            BonusToItems(items, config.Bonus2, config.BonusCount2);
            BonusToItems(items, config.Bonus3, config.BonusCount3);
            BonusToItems(items, config.Bonus4, config.BonusCount4);
            BonusToItems(items, config.Bonus5, config.BonusCount5);
            BonusToItems(items, config.Bonus6, config.BonusCount6);
        }

        foreach (var item in items)
        {
            AddItem(item.Key, item.Value);
        }
    }
    private void BonusToItems(Dictionary<int, int> items, int itemId, int count)
    {
        if (items.ContainsKey(itemId))
        {
            items[itemId] += count;
        }
        else
        {
            items[itemId] = count;
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
