using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class BlueprintPreviewUI : UIBase
{
    Button BlueprintPreviewCloseBtn { get => FindChiled<Button>("BlueprintPreviewCloseBtn"); }
    MyButton BlueprintPreviewPlussBtn { get => FindChiled<MyButton>("BlueprintPreviewPlussBtn"); }
    MyButton BlueprintPreviewMinusBtn { get => FindChiled<MyButton>("BlueprintPreviewMinusBtn"); }
    Button PhotographBtn { get => FindChiled<Button>("PhotographBtn"); }
    Slider Bar { get => FindChiled<Slider>("Slider"); }
    Image PhotographImg { get => FindChiled<Image>("PhotographImg"); }

    UIBase beforUI;
    float fadeInTimeStep = 0.05f;
    int renderTextureType = 1;
    Action<Texture2D> onPhotographCallback;

    /// <summary>
    /// 撮影してるフラグ
    /// </summary>
    bool IsPhotographing
    {
        get => mIsPhotographing;
        set
        {
            mIsPhotographing = value;

            Bar.gameObject.SetActive(!value);
            BlueprintPreviewCloseBtn.gameObject.SetActive(!value);
            BlueprintPreviewPlussBtn.gameObject.SetActive(!value);
            BlueprintPreviewMinusBtn.gameObject.SetActive(!value);
            PhotographBtn.gameObject.SetActive(value);
        }
    }
    private bool mIsPhotographing = false;

    public override void Init(object data)
    {
        base.Init(data);
        BlueprintPreviewLG.E.Init(this);

        BlueprintPreviewCloseBtn.onClick.AddListener(Close);
        BlueprintPreviewPlussBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(1); });
        BlueprintPreviewMinusBtn.AddClickingListener(()=> { PlayerCtl.E.BlueprintPreviewCtl.ChangeCameraPos(-1); });
        PhotographBtn.onClick.AddListener(()=> 
        { 
            IsPhotographing = true;
            var renderTexture = PlayerCtl.E.BlueprintPreviewCtl.GetRenderTexture(renderTextureType);
            var texture = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            if (onPhotographCallback != null)
            {
                onPhotographCallback(texture);
                onPhotographCallback = null;
            }

            StartCoroutine(PhotographIE());
        });
    }
    public override void Open(object data)
    {
        base.Open(data);
        renderTextureType = (int)data;

        PlayerCtl.E.BlueprintPreviewCtl.Show();

        WorldMng.E.GameTimeCtl.Active = false;
        DataMng.E.RuntimeData.IsPreviev = true;
        IsPhotographing = false;

        PhotographImg.gameObject.SetActive(false);
        PhotographBtn.gameObject.SetActive(renderTextureType != 0);
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

    /// <summary>
    /// スクリーンショットする場合のイベント
    /// </summary>
    /// <param name="callback"></param>
    public void AddListenerOnPhotographCallback(Action<Texture2D> callback)
    {
        onPhotographCallback = callback;
    }

    public void SetBarValue(float v)
    {
        Bar.value = v;
    }

    IEnumerator PhotographIE()
    {
        PhotographImg.gameObject.SetActive(true);

        //　Colorのアルファを0.1ずつ下げていく
        for (var i = 1f; i > 0; i -= fadeInTimeStep)
        {
            PhotographImg.color = new Color(255, 255, 255, i);
            //　指定秒数待つ
            yield return new WaitForSeconds(fadeInTimeStep);
        }

        PhotographImg.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        Close();
    }
}