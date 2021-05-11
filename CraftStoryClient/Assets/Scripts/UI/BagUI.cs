using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2Money.Model;

public class BagUI : UIBase
{
    Text moneyText;
    Transform itemGridRoot;
    Button closeBtn;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        BagLG.E.Init(this);

        InitUI();
        RefreshMoney();
    }

    private void InitUI()
    {
        moneyText = FindChiled<Text>("Money");
        itemGridRoot = FindChiled("Content");

        closeBtn = FindChiled<Button>("CloseBtn");
        closeBtn.onClick.AddListener(()=> { Close(); });
    }

    public override void Open()
    {
        base.Open();

        BagLG.E.GetItemList();
    }
    public override void Close()
    {
        base.Close();

        ClearItem();
    }

    public void AddItems(List<EzItemSet> itemList)
    {
        foreach (var item in itemList)
        {
            AddItem(item);
        }
    }
    private void AddItem(EzItemSet item)
    {
        var resources = CommonFunction.ReadResourcesPrefab("Prefabs/UI/Item");

        var obj = GameObject.Instantiate(resources, itemGridRoot);
        if (obj == null)
            return;

        var cell = obj.GetComponent<ItemCell>();
        if (cell == null)
            return;

        cell.Add(item);
    }

    private void ClearItem()
    {
        foreach (Transform t in itemGridRoot)
        {
            GameObject.Destroy(t.gameObject);
        }
    }

    private void RefreshMoney()
    {
        BagLG.E.GetMoney(0);
    }
    public void RefreshMoneyResponse(EzWallet item)
    {
        moneyText.text = (item.Free + item.Paid).ToString();
    }
}