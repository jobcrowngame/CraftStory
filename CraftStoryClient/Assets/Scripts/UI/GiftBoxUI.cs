using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2Showcase.Model;

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
        OKBtn.onClick.AddListener(() => { Close(); });

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

    public void AddItems(List<ItemData> items)
    {
        for (int i = 0; i < items.Count; i++)
        {

        }
    }
    public void AddItem(EzRateModel r)
    {
        //AddItem(r);
    }
    private void AddItem(string itemName, int count)
    {
        var cell = AddCell<IconItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Add(itemName, count);
        }
    }
}
