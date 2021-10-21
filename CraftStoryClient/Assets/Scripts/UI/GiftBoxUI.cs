using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxUI : UIBase
{
    Transform DoubleBonus { get => FindChiled("DoubleBonus"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button AdvertisingBtn { get => FindChiled<Button>("AdvertisingBtn"); }
    Transform itemGridRoot;
    List<IconItemCell> cells;
    Action okBtnCallBack;

    public override void Init()
    {
        base.Init();

        GiftBoxLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        AdvertisingBtn.onClick.AddListener(() => 
        {
            GoogleMobileAdsMng.E.ShowReawrd(()=> 
            {
                int count = AdventureCtl.E.BonusList.Count;
                for (int i = 0; i < count; i++)
                {
                    AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                }

                NWMng.E.ClearAdventure((rp) =>
                {
                    StartDoubleBonus();
                }, AdventureCtl.E.BonusList);
            });
        });
        OKBtn.onClick.AddListener(() => 
        {
            NWMng.E.ClearAdventure((rp) =>
            {
                if (okBtnCallBack != null)
                {
                    PlayerCtl.E.Lock = false;
                    ClearCell(itemGridRoot);

                    okBtnCallBack();
                }
            }, AdventureCtl.E.BonusList);
        });

        itemGridRoot = FindChiled("Content");
    }

    public override void Open()
    {
        base.Open();

        PlayerCtl.E.Lock = true;
        DoubleBonus.gameObject.SetActive(false);
    }

    public void AddBonus(List<int> bonus)
    {
        Dictionary<int, int> items = CommonFunction.GetItemsByBonus(bonus);
        cells = new List<IconItemCell>();
        ClearCell(itemGridRoot);

        foreach (var item in items)
        {
            AddItem(item.Key, item.Value);
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
            cells.Add(cell);
        }
    }
    
    /// <summary>
    /// 2倍ボーナスボタンイベント
    /// </summary>
    private void StartDoubleBonus()
    {
        AdvertisingBtn.gameObject.SetActive(false);
        DoubleBonus.gameObject.SetActive(true);

        foreach (var item in cells)
        {
            item.StartDoubleAnim();
        }
    }

    /// <summary>
    /// OKボタンのイベント
    /// </summary>
    /// <param name="action"></param>
    public void SetCallBack(Action action)
    {
        okBtnCallBack = action;
    }
}
