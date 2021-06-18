﻿using JsonConfigData;
using UnityEngine;
using UnityEngine.UI;

public class HomeItemBtn : UIBase
{
    private static HomeItemBtn curSelectBtn;
    public static HomeItemBtn CurSelectBtn
    {
        get => curSelectBtn;
        set
        {
            if (curSelectBtn == value)
            {
                if(curSelectBtn != null) curSelectBtn.OnSelected(false);
                curSelectBtn = null;
            }
            else
            {
                if (curSelectBtn != null) curSelectBtn.OnSelected(false);

                curSelectBtn = value;
                if (curSelectBtn != null) curSelectBtn.OnSelected();
            }

            PlayerCtl.E.ChangeSelectItem(curSelectBtn == null
                ? null
                : curSelectBtn.itemData);
        }
    }

    Image Icon;
    Text Count;
    Transform Selection;

    Button btn;

    private ItemData itemData;

    public int Index { get; set; }

    private void Awake()
    {
        Icon = FindChiled<Image>("Icon");
        Count = FindChiled<Text>("Count");
        Selection = FindChiled("Selection");

        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        OnSelected(false);
        CurSelectBtn = null;
    }
   

    private void OnClick()
    {
        if (itemData == null)
            return;

        CurSelectBtn = this;
    }
    private void OnSelected(bool b = true)
    {
        Selection.gameObject.SetActive(b);
    }

    public void Refresh()
    {
        itemData = DataMng.E.GetItemByEquipedSite(Index + 1);

        if (CurSelectBtn == this && CurSelectBtn.itemData == null)
            CurSelectBtn = null;

        if (itemData == null)
        {
            Icon.sprite = null;
            Count.text = "";
            OnSelected(false);
        }
        else
        {
            Icon.sprite = ReadResources<Sprite>(itemData.Config().IconResourcesPath);
            Count.text = itemData.Config().MaxCount == 1
                ? ""
                : "x" + itemData.count;
        }
    }
}

public enum ItemType
{
    None = 0,
    Block = 1,
    BuilderPencil = 50,
    Blueprint = 51,
}