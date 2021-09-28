
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintDetailsCell : UIBase
{
    Image Icon { get => transform.GetComponent<Image>(); }
    Text Title { get => FindChiled<Text>("Title"); }
    Text UserName { get => FindChiled<Text>("UserName"); }
    Text Price { get => FindChiled<Text>("Price"); }
    Text Time { get => FindChiled<Text>("Time"); }

    Button GoodBtn { get => FindChiled<Button>("Button"); }
    Text GoodNum { get => FindChiled<Text>("GoodNum"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }
    Button PreviewBtn { get => transform.GetComponent<Button>(); }

    MyShopItem data;

    public string TargetAcc { get => data.targetAcc; }

    public void Set(MyShopItem data)
    {
        if (data.itemId == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        AWSS3Mng.E.DownLoadTexture2D(Icon, data.icon);

        Title.text = data.newName;
        UserName.text = data.nickName;
        Price.text = data.price.ToString();
        GoodNum.text = data.goodNum.ToString();

        GoodBtn.onClick.AddListener(() =>
        {
            if (DataMng.E.RuntimeData.MyShopGoodNum >= 3)
            {
                CommonFunction.ShowHintBar(1047001);
                return;
            }

            NWMng.E.MyShopGoodEvent((rp) =>
            {
                ShopBlueprintLG.E.RefreshGoodNum(data.targetAcc);
                DataMng.E.RuntimeData.MyShopGoodNum++;
            }, data.targetAcc);
        });
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
    public void GoodNumberAdd()
    {
        data.goodNum++;
        GoodNum.text = data.goodNum.ToString();
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