using UnityEngine.UI;

public class BlueprintPreviewUI : UIBase
{
    Button BlueprintPreviewCloseBtn { get => FindChiled<Button>("BlueprintPreviewCloseBtn"); }
    MyButton BlueprintPreviewPlussBtn { get => FindChiled<MyButton>("BlueprintPreviewPlussBtn"); }
    MyButton BlueprintPreviewMinusBtn { get => FindChiled<MyButton>("BlueprintPreviewMinusBtn"); }
    Slider Bar { get => FindChiled<Slider>("Slider"); }

    UIBase beforUI;

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

        WorldMng.E.GameTimeCtl.Active = false;
        DataMng.E.RuntimeData.IsPreviev = true;
    }
    public override void Close()
    {
        base.Close();
        PlayerCtl.E.BlueprintPreviewCtl.Show(false);

        HomeLG.E.UI.Open();
        if (beforUI != null) beforUI.Open();

        WorldMng.E.GameTimeCtl.Active = true;
        DataMng.E.RuntimeData.IsPreviev = false;
    }

    public void SetData(int blueprintId, UIBase beforUI)
    {
        var config = ConfigMng.E.Blueprint[blueprintId];
        var data = new BlueprintData(config.Data);

        PlayerCtl.E.BlueprintPreviewCtl.CreateBlock(data);

        this.beforUI = beforUI;
    }
    public void SetData(string jsonData, UIBase beforUI)
    {
        var data = new BlueprintData(jsonData);
        PlayerCtl.E.BlueprintPreviewCtl.CreateBlock(data);

        this.beforUI = beforUI;
    }

    public void SetBarValue(float v)
    {
        Bar.value = v;
    }
}