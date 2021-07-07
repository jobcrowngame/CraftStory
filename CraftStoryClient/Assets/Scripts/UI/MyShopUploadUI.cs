using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShopUploadUI : UIBase
{
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    InputField InputField { get => FindChiled<InputField>("InputField"); }

    ItemData itemData;
    int Index;

    public override void Init()
    {
        base.Init();
        MyShopUploadLG.E.Init(this);

        OKBtn.onClick.AddListener(() => 
        {
            if (itemData == null)
                return;

            if (string.IsNullOrEmpty(InputField.text))
            {
                CommonFunction.ShowHintBar(14);
                return;
            }

            NWMng.E.UploadBlueprintToMyShop((rp) =>
            {
                //DataMng.E.ConsumableItemByGUID((int)rp["itemGuid"]);

                if (!string.IsNullOrEmpty(rp["myShopItems"].ToString()))
                {
                    List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp["myShopItems"].ToJson());
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        DataMng.E.RuntimeData.MyShop.myShopItem[i] = shopItems[i];
                    }
                }

                MyShopLG.E.UI.RefreshUI();
            }, itemData.id, Index, int.Parse(InputField.text));

            Close();
        });
        CancelBtn.onClick.AddListener(Close);
    }
    public override void Open()
    {
        base.Open();

        InputField.text = "";
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
}
