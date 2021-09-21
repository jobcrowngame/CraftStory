
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopMyShopItemCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text NickName { get => FindChiled<Text>("NickName"); }
    Text ItemName { get => FindChiled<Text>("ItemName"); }
    Text Time { get => FindChiled<Text>("Time"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }
    Button PreviewBtn { get => FindChiled<Button>("PreviewBtn"); }
    Text BuyBtnText { get => FindChiled<Text>("Text", BuyBtn.transform); }
    Image BtnCostIcon { get => FindChiled<Image>("Image", BuyBtn.transform); }

    MyShopItem data;

    public void Set(MyShopItem data)
    {
        Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[data.itemId].IconResourcesPath);
        NickName.text = data.nickName;
        ItemName.text = data.newName;
        BuyBtnText.text = data.price.ToString();
        BtnCostIcon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);

        PreviewBtn.gameObject.SetActive(true);
        BtnCostIcon.gameObject.SetActive(true);

        BuyBtn.onClick.AddListener(() =>
        {
            string msg = string.Format(@"ポイントを「{0}」消費して、
この設計図を購入しますか？", data.price);

            CommonFunction.ShowHintBox(msg, () =>
            {
                if (DataMng.E.RuntimeData.Coin3 < data.price)
                {
                    CommonFunction.ShowHintBar(15);
                    return;
                }

                NWMng.E.BuyMyShopItem((rp) =>
                {
                    DataMng.E.RuntimeData.Coin3 -= data.price;
                    ShopLG.E.UI.RefreshCoins();

                    CommonFunction.ShowHintBar(17);
                }, data.myshopid);
            }, () => { });
        });
        PreviewBtn.onClick.AddListener(() =>
        {
            NWMng.E.OpenPreview(data.myshopid, (data) =>
            {
                var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
                ui.SetData(data, ShopLG.E.UI);
            });
        });

        data.created_at = data.created_at.AddDays(7);

        this.data = data;
        StartCoroutine(RefreshTime(data));
    }

    private IEnumerator RefreshTime(MyShopItem data)
    {
        while (true)
        {
            TimeSpan t = data.created_at - DateTime.Now;
            Time.text = string.Format("{0}日{1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);
            yield return new WaitForSeconds(1);
        }
    }
}