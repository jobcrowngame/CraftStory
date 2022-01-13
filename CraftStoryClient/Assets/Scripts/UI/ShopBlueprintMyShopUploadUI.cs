using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintMyShopUploadUI : UIBase
{
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Dropdown Dropdown { get => FindChiled<Dropdown>("Dropdown"); }

    ItemData itemData;
    int Index;

    public override void Init()
    {
        base.Init();

        Dropdown.options.Clear();
        Dropdown.AddOptions(new List<string>
        {
            "110ポイント(100ポイント獲得)",
            "330ポイント(300ポイント獲得)",
            "550ポイント(500ポイント獲得)",
            "1100ポイント(1000ポイント獲得)",
            "2200ポイント(2000ポイント獲得)",
            "5500ポイント(5000ポイント獲得)",
            "11000ポイント(10000ポイント獲得)"
        });
        Dropdown.value = 0;

        OKBtn.onClick.AddListener(() => 
        {
            if (itemData == null)
                return;

            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                CommonFunction.ShowHintBar(16);
                GuideLG.E.Next();
            }
            else
            {
                if (ShopBlueprintMyShopSelectItemLG.E.SelectItem != null && ShopBlueprintMyShopSelectItemLG.E.SelectItem.ItemData != null)
                {
                    // 設計図データをゲットしてプレビューでスクリーンショットする
                    NWMng.E.GetBlueprintPreviewDataByItemGuid((rp) =>
                    {
                        // スクリーンショットに遷移
                        var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose, 2);
                        ui.SetData((string)rp, ShopBlueprintLG.E.UI);
                        ui.AddListenerOnPhotographCallback((texture) =>
                        {
                            string textureName = CommonFunction.GetTextureName();
                            AWSS3Mng.E.UploadTexture2D(texture, textureName, ()=> 
                            {
                                NWMng.E.UploadBlueprintToMyShop((rp) =>
                                {
                                    NWMng.E.GetMyshopInfo(() =>
                                    {
                                        ShopBlueprintLG.E.UI.RefreshMyShopWindow();
                                        CommonFunction.ShowHintBar(16);

                                        // 設計図アップロード案内の表示
                                        //NWMng.E.GetTotalUploadBlueprintCount((rp) =>
                                        //{
                                        //    int count = int.Parse(rp.ToString());
                                        //    Image img = GameObject.Find("BluePrintHintImage").GetComponent<Image>();
                                        //    img.color = new Color(1f, 1f, 1f, count == 0 ? 1 : 1 / 256f);
                                        //    if (count == 1)
                                        //    {
                                        //        DataMng.E.RuntimeData.NewEmailCount++;
                                        //        HomeLG.E.UI.RefreshRedPoint();
                                        //        UICtl.E.OpenUI<ChatUI>(UIType.Chat, UIOpenType.OnCloseDestroyObj, 124);
                                        //    }
                                        //});

                                    });
                                }, itemData.id, Index, GetPrice(), textureName);
                            });
                        });

                        // 設計図アップロードタスク
                        TaskMng.E.AddMainTaskCount(6);
                    }, ShopBlueprintMyShopSelectItemLG.E.SelectItem.ItemData.id);
                }
            }

            Close();
        });
        CancelBtn.onClick.AddListener(Close);
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }

    public void SetItemData(ItemData data, int index)
    {
        itemData = data;
        Index = index;
    }

    private int GetPrice()
    {
        switch (Dropdown.value)
        {
            case 0: return 110;
            case 1: return 330;
            case 2: return 550;
            case 3: return 1100;
            case 4: return 2200;
            case 5: return 5500;
            case 6: return 11000;
        }
        return 0;
    }
}
