using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShopUI : UIBase
{
    TitleUI title;
    MyShopCell[] cells;

    public override void Init()
    {
        base.Init();
        MyShopLG.E.Init(this);

        title = FindChiled<TitleUI>("Title");
        title.SetTitle("マイショップ");
        title.SetOnClose(() => { Close(); });
        title.EnActiveCoin(2);
        title.EnActiveCoin(3);

        var cellParent = FindChiled("Btns");
        cells = new MyShopCell[cellParent.childCount];
        for (int i = 0; i < cellParent.childCount; i++)
        {
            cells[i] = cellParent.GetChild(i).gameObject.AddComponent<MyShopCell>();
        }

    }
    public override void Open()
    {
        base.Open();
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Init(i + 1);
        }
        title.RefreshCoins();
    }
}
