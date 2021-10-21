using UnityEngine;
using UnityEngine.UI;

public class BlueprintReNameUI : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Button OkBtn { get => FindChiled<Button>("OkBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Button PhotographBtn { get => FindChiled<Button>("PhotographBtn"); }
    InputField input { get => FindChiled<InputField>("InputField"); }

    public string mapData { get; set; }
    Texture2D texture;

    public override void Init()
    {
        base.Init();
        BlueprintReNameLG.E.Init(this);

        OkBtn.onClick.AddListener(OnClickOK);
        CancelBtn.onClick.AddListener(Close);
        CloseBtn.onClick.AddListener(Close);
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

    public override void Close()
    {
        base.Close();

        Icon.sprite = null;
        PhotographMode();
    }

    /// <summary>
    /// 設計図データ
    /// </summary>
    /// <param name="msg"></param>
    public void SetMapData(string msg)
    {
        mapData = msg;
    }

    public void OnClickOK()
    {
        if (string.IsNullOrEmpty(input.text))
        {
            CommonFunction.ShowHintBar(3);
            return;
        }

        string fileName = CommonFunction.GetTextureName();

        // 設計図アイテムを追加
        DataMng.E.AddBlueprint(3002, 1, input.text, mapData, fileName, () =>
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
                // S3にテクスチャをアップロード
                AWSS3Mng.E.UploadTexture2D(texture, fileName);

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

    /// <summary>
    /// 撮影ボタンクリック
    /// </summary>
    public void OnClickPhotographBtn()
    {
        var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose, 1);
        ui.SetData(mapData, BlueprintReNameLG.E.UI);
        ui.AddListenerOnPhotographCallback((texture) => 
        {
            this.texture = texture;
            Icon.sprite = CommonFunction.Texture2dToSprite(texture);

            PhotographMode(false);
        });

        GuideLG.E.Next();
    }

    /// <summary>
    /// 撮影モードの場合
    /// </summary>
    private void PhotographMode(bool b = true)
    {
        OkBtn.gameObject.SetActive(!b);
        CancelBtn.gameObject.SetActive(!b);
        PhotographBtn.gameObject.SetActive(b);
    }
}
