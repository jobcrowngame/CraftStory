using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShopUploadUI : UIBase
{
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button CancelBtn { get => FindChiled<Button>("CancelBtn"); }
    Dropdown Dropdown { get => FindChiled<Dropdown>("Dropdown"); }

    ItemData itemData;
    int Index;

    public override void Init()
    {
        base.Init();
        MyShopUploadLG.E.Init(this);

        Dropdown.options.Clear();
        Dropdown.AddOptions(new List<string>
        {
            "100ポイント",
            "300ポイント",
            "500ポイント",
            "1000ポイント",
            "2000ポイント",
            "5000ポイント",
            "10000ポイント"
        });
        Dropdown.value = 0;

        OKBtn.onClick.AddListener(() => 
        {
            if (itemData == null)
                return;

            NWMng.E.UploadBlueprintToMyShop((rp) =>
            {
                //DataMng.E.ConsumableItemByGUID((int)rp["itemGuid"]);

                DataMng.E.RuntimeData.MyShop.Clear();
                if (!string.IsNullOrEmpty(rp["myShopItems"].ToString()))
                {
                    List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp["myShopItems"].ToJson());
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        DataMng.E.RuntimeData.MyShop.myShopItem[i] = shopItems[i];
                    }
                }

                MyShopLG.E.UI.RefreshUI();
            }, itemData.id, Index, GetPrice());

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
            case 0: return 100;
            case 1: return 300;
            case 2: return 500;
            case 3: return 1000;
            case 4: return 2000;
            case 5: return 5000;
            case 6: return 10000;
        }
        return 0;
    }
}
