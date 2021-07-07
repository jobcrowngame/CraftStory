using UnityEngine.UI;

public class BlueprintPreviewUI : UIBase
{
    Button BlueprintPreviewCloseBtn { get => FindChiled<Button>("BlueprintPreviewCloseBtn"); }
    MyButton BlueprintPreviewPlussBtn { get => FindChiled<MyButton>("BlueprintPreviewPlussBtn"); }
    MyButton BlueprintPreviewMinusBtn { get => FindChiled<MyButton>("BlueprintPreviewMinusBtn"); }
    Slider Bar { get => FindChiled<Slider>("Slider"); }

    public override void Init()
    {
        base.Init();
        BlueprintPreviewLG.E.Init(this);

        BlueprintPreviewCloseBtn.onClick.AddListener(Close);
        BlueprintPreviewPlussBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(1); });
        BlueprintPreviewMinusBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(-1); });
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
    public void SetData(string jsonData)
    {
        var data = new BlueprintData(jsonData);
        PlayerCtl.E.BlueprintPreviewCtl.CreateBlock(data);
    }

    public void SetBarValue(float v)
    {
        Bar.value = v;
    }
}