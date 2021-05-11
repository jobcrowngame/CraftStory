using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gs2.Unity.Gs2Exchange.Model;

public class GiftBoxUI : UIBase
{
    Button OKBtn;
    Transform itemGridRoot;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        GiftBoxLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        OKBtn = FindChiled<Button>("OKBtn");
        OKBtn.onClick.AddListener(() => { Close(); });

        itemGridRoot = FindChiled("Content");
    }

    public void Add(EzRateModel response)
    {
        MLog.Log(response.Name);
    }

    //public void AddItems(List<EzItemSet> itemList)
    //{
    //    foreach (var item in itemList)
    //    {
    //        AddItem(item);
    //    }
    //}
    //private void AddItem(EzItemSet item)
    //{
    //    var resources = CommonFunction.ReadResourcesPrefab("Prefabs/UI/Item");

    //    var obj = GameObject.Instantiate(resources, itemGridRoot);
    //    if (obj == null)
    //        return;

    //    var cell = obj.GetComponent<ItemCell>();
    //    if (cell == null)
    //        return;

    //    cell.Init(item);
    //}
}
