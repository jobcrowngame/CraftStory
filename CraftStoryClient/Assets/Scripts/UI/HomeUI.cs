using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Button BagBtn;
    Button LotteryBtn;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        HomeLG.E.Init(this);

        InitUI();
    }

    private void InitUI()
    {
        BagBtn = FindChiled<Button>("BagBtn");
        BagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        LotteryBtn = FindChiled<Button>("LotteryBtn");
        LotteryBtn.onClick.AddListener(() => { UICtl.E.OpenUI<LotteryUI>(UIType.Lottery); });
    }
}
