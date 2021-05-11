using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Inventory.Model;

public class BagUI : UIBase
{
    Button closeBtn;
    Transform itemGridRoot;

    public override void Init()
    {
        BagLG.E.Init(this);

        if (!InitUI())
        {
            MLog.Error("BagUI Init fail!");
        }
    }

    private bool InitUI()
    {
        var loginObj = CommonFunction.FindChiledByName(gameObject, "CloseBtn");
        if (loginObj != null)
            closeBtn = loginObj.GetComponent<Button>();
        else
            return false;

        var itemGridRootObj = CommonFunction.FindChiledByName(gameObject, "Content");
        if (itemGridRootObj != null)
            itemGridRoot = itemGridRootObj.transform;
        else
            return false;

        closeBtn.onClick.AddListener(()=> { Close(); });

        return true;
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

        cell.Init(item);
    }

    private void ClearItem()
    {
        foreach (Transform t in itemGridRoot)
        {
            GameObject.Destroy(t.gameObject);
        }
    }
}