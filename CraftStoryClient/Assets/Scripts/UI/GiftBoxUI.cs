using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxUI : UIBase
{
    Image MultiBonus { get => FindChiled<Image>("MultiBonus"); }
    Button Bonus3XBtn { get => FindChiled<Button>("Bonus3XBtn"); }
    Transform Bonus3XLabel { get => FindChiled<Transform>("Bonus3XLabel"); }
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

                //NWMng.E.ClearAdventure((rp) =>
                //{
                //    StartDoubleBonus();
                //}, AdventureCtl.E.BonusList);
                StartDoubleBonus();
            });
        });
        ItemData ticket = DataMng.E.GetItemByItemId(9005);
        if (ticket != null)
        {
            Bonus3XBtn.gameObject.SetActive(true);
            Bonus3XBtn.onClick.AddListener(() =>
            {
                DataMng.E.ConsumableItem(9005);
                int count = AdventureCtl.E.BonusList.Count;
                for (int i = 0; i < count; i++)
                {
                    AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                    AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                }
                //NWMng.E.ClearAdventure((rp) =>
                //{
                //    StartTripleBonus();
                //}, AdventureCtl.E.BonusList);
                StartTripleBonus();
            });
            Bonus3XLabel.gameObject.SetActive(false);
        }
        else
        {
            Bonus3XBtn.gameObject.SetActive(false);
            Bonus3XLabel.gameObject.SetActive(true);
        }
        OKBtn.onClick.AddListener(() => 
        {
            NWMng.E.AddExp((rp) => 
            {
                DataMng.E.RuntimeData.Lv = (int)rp["lv"];
                DataMng.E.RuntimeData.Exp = (int)rp["exp"];

                // レベルアップ表現があると追加
                // ...
                // ...

                NWMng.E.ClearAdventure((rp) =>
                {
                    if (okBtnCallBack != null)
                    {
                        PlayerCtl.E.Lock = false;
                        ClearCell(itemGridRoot);

                        okBtnCallBack();
                    }
                }, AdventureCtl.E.BonusList, DataMng.E.MapData.Config.Floor);
            }, AdventureCtl.E.CurExp);
        });

        itemGridRoot = FindChiled("Content");
    }

    public override void Open()
    {
        base.Open();

        PlayerCtl.E.Lock = true;
        MultiBonus.gameObject.SetActive(false);
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
        Bonus3XBtn.gameObject.SetActive(false);
        Bonus3XLabel.gameObject.SetActive(false);
        AdvertisingBtn.gameObject.SetActive(false);
        MultiBonus.gameObject.SetActive(true);
        MultiBonus.sprite = ReadResources<Sprite>("Textures/brave_2d_001"); ;

        foreach (var item in cells)
        {
            item.StartMultiAnim(2);
        }
    }

    /// <summary>
    /// 3倍ボーナスボタンイベント
    /// </summary>
    private void StartTripleBonus()
    {
        Bonus3XBtn.gameObject.SetActive(false);
        Bonus3XLabel.gameObject.SetActive(false);
        AdvertisingBtn.gameObject.SetActive(false);
        MultiBonus.gameObject.SetActive(true);
        MultiBonus.sprite = ReadResources<Sprite>("Textures/brave_2d_002"); ;

        foreach (var item in cells)
        {
            item.StartMultiAnim(3);
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
