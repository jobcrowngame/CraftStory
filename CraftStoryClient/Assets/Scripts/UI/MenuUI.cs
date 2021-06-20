using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn;
    Button CraftBtn;
    Button AdventureBtn;

    public override void Init()
    {
        base.Init();
        MenuLG.E.Init(this);
        InitUI();
    }

    private void InitUI()
    {

        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(() => { Close(); });

        CraftBtn = FindChiled<Button>("CraftBtn");
        CraftBtn.onClick.AddListener(() => { UICtl.E.OpenUI<CraftUI>(UIType.Craft); });

        AdventureBtn = FindChiled<Button>("AdventureBtn");
        DataMng.E.MapData.TransferGate = new EntityData(1000, EntityType.TransferGate);
        AdventureBtn.onClick.AddListener(CommonFunction.GoToNextScene);
    }
}