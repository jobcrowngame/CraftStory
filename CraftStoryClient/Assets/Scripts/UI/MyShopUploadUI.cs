using LitJson;
using System.Collections.Generic;
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

            NWMng.E.UploadBlueprintToMyShop((rp) =>
            {
                //DataMng.E.ConsumableItemByGUID((int)rp["itemGuid"]);

                DataMng.E.MyShop.Clear();
                if (!string.IsNullOrEmpty(rp["myShopItems"].ToString()))
                {
                    List<MyShopItem> shopItems = JsonMapper.ToObject<List<MyShopItem>>(rp["myShopItems"].ToJson());
                    for (int i = 0; i < shopItems.Count; i++)
                    {
                        DataMng.E.MyShop.myShopItem[i] = shopItems[i];
                    }
                }

                MyShopLG.E.UI.RefreshUI();

                CommonFunction.ShowHintBar(16);
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
