using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BlueprintPreviewUI : UIBase
{
    Button BlueprintPreviewCloseBtn { get => FindChiled<Button>("BlueprintPreviewCloseBtn"); }
    Button BlueprintPreviewPlussBtn { get => FindChiled<Button>("BlueprintPreviewPlussBtn"); }
    Button BlueprintPreviewMinusBtn { get => FindChiled<Button>("BlueprintPreviewMinusBtn"); }

    public override void Init()
    {
        base.Init();

        BlueprintPreviewCloseBtn.onClick.AddListener(Close);
        BlueprintPreviewPlussBtn.onClick.AddListener(PlayerCtl.E.BlueprintPreviewCtl.OnClickPlussBtn);
        BlueprintPreviewMinusBtn.onClick.AddListener(PlayerCtl.E.BlueprintPreviewCtl.OnClickMinusBtn);
    }
    public override void Open()
    {
        base.Open();
        PlayerCtl.E.BlueprintPreviewCtl.Show();
    }
    public override void Close()
    {
        base.Close();
        PlayerCtl.E.BlueprintPreviewCtl.Show(false);
        UICtl.E.OpenUI<HomeUI>(UIType.Home);
        UICtl.E.OpenUI<ShopUI>(UIType.Shop);
    }

    public void SetData(int blueprintId)
    {
        var config = ConfigMng.E.Blueprint[blueprintId];
        var data = new BlueprintData(config.Data);

        PlayerCtl.E.BlueprintPreviewCtl.CreateBlock(data);
    }
}