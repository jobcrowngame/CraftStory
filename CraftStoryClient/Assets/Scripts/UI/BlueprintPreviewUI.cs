using UnityEngine.UI;

public class BlueprintPreviewUI : UIBase
{
    Button BlueprintPreviewCloseBtn { get => FindChiled<Button>("BlueprintPreviewCloseBtn"); }
    MyButton BlueprintPreviewPlussBtn { get => FindChiled<MyButton>("BlueprintPreviewPlussBtn"); }
    MyButton BlueprintPreviewMinusBtn { get => FindChiled<MyButton>("BlueprintPreviewMinusBtn"); }
    MyButton PhotographBtn { get => FindChiled<MyButton>("PhotographBtn"); }
    Slider Bar { get => FindChiled<Slider>("Slider"); }

    UIBase beforUI;

    /// <summary>
    /// 撮影してるフラグ
    /// </summary>
    bool IsPhotographing
    {
        get => mIsPhotographing;
        set
        {
            mIsPhotographing = value;

            Bar.gameObject.SetActive(!IsPhotographing);
            BlueprintPreviewPlussBtn.gameObject.SetActive(!IsPhotographing);
            BlueprintPreviewMinusBtn.gameObject.SetActive(!IsPhotographing);
            PhotographBtn.gameObject.SetActive(IsPhotographing);
        }
    }
    private bool mIsPhotographing = false;

    public override void Init()
    {
        base.Init();
        BlueprintPreviewLG.E.Init(this);

        BlueprintPreviewCloseBtn.onClick.AddListener(()=>
        {
            if (IsPhotographing)
                IsPhotographing = false;
            else
                Close();
        });
        BlueprintPreviewPlussBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(1); });
        BlueprintPreviewMinusBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(-1); });
        PhotographBtn.onClick.AddListener(()=> { IsPhotographing = true; });
    }
    public override void Open()
    {
        base.Open();
        PlayerCtl.E.BlueprintPreviewCtl.Show();

        WorldMng.E.GameTimeCtl.Active = false;
        DataMng.E.RuntimeData.IsPreviev = true;

        IsPhotographing = false;
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

        PlayerCtl.E.BlueprintPreviewCtl.CreateEntity(data);

        this.beforUI = beforUI;
    }
    public void SetData(string jsonData, UIBase beforUI)
    {
        var data = new BlueprintData(jsonData);
        PlayerCtl.E.BlueprintPreviewCtl.CreateEntity(data);

        this.beforUI = beforUI;
    }

    public void ShowPhotographBtn()
    {
        PhotographBtn.gameObject.SetActive(true);
    }

    public void SetBarValue(float v)
    {
        Bar.value = v;
    }
}