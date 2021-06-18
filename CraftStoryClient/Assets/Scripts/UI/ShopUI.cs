using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    Transform itemGridRoot;
    Button CloseBtn;

    public override void Init()
    {
        base.Init();

        ShopLG.E.Init(this);

        InitUI();
    }

    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();

        ClearCell(itemGridRoot);
    }

    private void InitUI()
    {
        itemGridRoot = FindChiled("Content");

        CloseBtn = FindChiled<Button>("CloseBtn");
        CloseBtn.onClick.AddListener(() => { Close(); });
    }

}
