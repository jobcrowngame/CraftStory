using UnityEngine;
using UnityEngine.UI;

public class BlueprintReNameUI : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Button PhotographBtn { get => FindChiled<Button>("PhotographBtn"); }
    InputField input { get => FindChiled<InputField>("InputField"); }

    public string mapData { get; set; }

    public override void Init()
    {
        base.Init();
        BlueprintReNameLG.E.Init(this);

        OkBtn.onClick.AddListener(OnClickOK);
        CancelBtn.onClick.AddListener(Close);
        PhotographBtn.onClick.AddListener(OnClickPhotographBtn);
        input.onEndEdit.AddListener((r) =>
        {
            if (string.IsNullOrEmpty(input.text))
            {
                CommonFunction.ShowHintBar(3);
                return;
            }

            GuideLG.E.Next();
        });
    }

    public override void Open()
    {
        base.Open();

        BlueprintReNameLG.E.UIStep = 0;
        if (BlueprintReNameLG.E.PhotographTexture != null)
        {
            SetIcon(BlueprintReNameLG.E.PhotographTexture);
            BlueprintReNameLG.E.UIStep++;
        }
    }

    public override void Close()
    {
        base.Close();

        BlueprintReNameLG.E.PhotographTexture = null;
        Icon.sprite = null;
    }

    /// <summary>
    /// 設計図データ
    /// </summary>
    /// <param name="msg"></param>
    public void SetMapData(string msg)
    {
        mapData = msg;
    }

    /// <summary>
    /// アイコンを設定
    /// </summary>
    /// <param name="texture"></param>
    public void SetIcon(Texture2D texture)
    {
        Icon.sprite = CommonFunction.Texture2dToSprite(texture);
    }

    /// <summary>
    /// UI　ステップが変更される場合、
    /// </summary>
    /// <param name="step"></param>
    public void OnStepChange(int step)
    {
        OkBtn.gameObject.SetActive(BlueprintReNameLG.E.UIStep > 0);
        CancelBtn.gameObject.SetActive(BlueprintReNameLG.E.UIStep > 0);
        PhotographBtn.gameObject.SetActive(BlueprintReNameLG.E.UIStep == 0);
    }

    public void OnClickOK()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            CommonFunction.ShowHintBar(3);
            return;
        }

        if (BlueprintReNameLG.E.PhotographTexture == null)
        {
            return;
        }

        string fileName = CommonFunction.GetTextureName();
        AWSS3Mng.E.UploadTexture2D(BlueprintReNameLG.E.PhotographTexture, fileName);

        DataMng.E.AddBlueprint(3002, 1, input.text, mapData, fileName,() =>
        {
            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                DataMng.E.RemoveItemByGuid(PlayerCtl.E.SelectItem.id, 1);
                PlayerCtl.E.SelectItem = null;
                Close();

                GuideLG.E.Next();
            }
            else
            {
                NWMng.E.RemoveItem((rp) =>
                {
                    NWMng.E.GetItems(() =>
                    {
                        PlayerCtl.E.SelectItem = null;
                        Close();
                    });
                }, PlayerCtl.E.SelectItem.itemId, 1);
            }
        });
    }
    public void OnClickPhotographBtn()
    {
        var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
        ui.SetData(mapData, BlueprintReNameLG.E.UI);
        ui.ShowPhotographBtn();
    }
}
