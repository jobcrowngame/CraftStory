using UnityEngine;
using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn;
    Button CraftBtn;

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
        CraftBtn.onClick.AddListener(() => { Debug.Log("CraftBtn"); });
    }
}