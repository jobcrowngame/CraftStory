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
                Close();
                DataMng.GetItems(rp);
                GoToNext();
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
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
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

    private void GoToNext()
    {
        PlayerCtl.E.Lock = false;
        int nextTransferGateID = 0;

        if (DataMng.E.MapData.Config.TransferGateID == 9999)
        {
            nextTransferGateID = NowLoadingLG.E.BeforTransferGateID;
        }
        else
        {
            nextTransferGateID = DataMng.E.MapData.Config.TransferGateID;
        }

        CommonFunction.GoToNextScene(nextTransferGateID);
        AdventureCtl.E.Clear();
    }
    private void StartDoubleBonus()
    {
        AdvertisingBtn.gameObject.SetActive(false);
        DoubleBonus.gameObject.SetActive(true);

        foreach (var item in cells)
        {
            item.StartDoubleAnim();
        }
    }
}
