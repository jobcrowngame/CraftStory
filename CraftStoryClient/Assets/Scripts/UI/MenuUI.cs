using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn;
    //Button CraftBtn;
    Button AdventureBtn;
    Button ShopBtn;

    public override void Init()
    {
        base.Init();
        MenuLG.E.Init(this);

        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(() => { Close(); });

        //CraftBtn = FindChiled<Button>("CraftBtn");
        //CraftBtn.onClick.AddListener(() => { UICtl.E.OpenUI<CraftUI>(UIType.Craft); });

        AdventureBtn = FindChiled<Button>("AdventureBtn");
        DataMng.E.MapData.TransferGate = new EntityData(1000, ItemType.TransferGate);
        AdventureBtn.onClick.AddListener(CommonFunction.GoToNextScene);

        ShopBtn = FindChiled<Button>("ShopBtn");
        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });
    }
}