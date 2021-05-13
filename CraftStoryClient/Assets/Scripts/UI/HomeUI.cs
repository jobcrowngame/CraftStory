using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Button BagBtn;
    Button LotteryBtn;
    Button ShopBtn;

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

        ShopBtn = FindChiled<Button>("ShopBtn");
        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });
    }
}
