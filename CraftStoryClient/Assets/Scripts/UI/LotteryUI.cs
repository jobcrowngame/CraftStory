using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryUI : UIBase
{
    Button CloseBtn;
    Button OneBtn;
    Button TenBtn;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        LotteryLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        CloseBtn = FindChiled<Button>("CloseBtn");
        if (CloseBtn != null) CloseBtn.onClick.AddListener(() => { Close(); });

        OneBtn = FindChiled<Button>("OneBtn");
        if(OneBtn != null) OneBtn.onClick.AddListener(() => { LotteryLG.E.LotteryOne(); });

        TenBtn = FindChiled<Button>("TenBtn");
        if (TenBtn != null) TenBtn.onClick.AddListener(() => { LotteryLG.E.LotteryTen(); });
    }
}
