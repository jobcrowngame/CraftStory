
using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintDetailsUI : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    MyText ItemName { get => FindChiled<MyText>("ItemName"); }
    MyText UserName { get => FindChiled<MyText>("UserName"); }
    MyText Time { get => FindChiled<MyText>("Time"); }
    MyText Price { get => FindChiled<MyText>("Price"); }
    MyText GoodNum { get => FindChiled<MyText>("GoodNum"); }
    Button BuyBtn { get => FindChiled<Button>("BuyBtn"); }
    Button GoodBtn { get => FindChiled<Button>("Button"); }
    Button PreviewBtn { get => Icon.GetComponent<Button>(); }
    Button Mask { get => transform.GetComponent<Button>(); }

    Animation anim { get => Icon.GetComponent<Animation>(); }

    MyShopItem data;

    public string TargetAcc { get => data.targetAcc; }

    public override void Init(object inData)
    {
        base.Init(data);

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
                    CommonFunction.ShowHintBar(17);

                    ShopBlueprintLG.E.UI.RefreshCoin();
                    HomeLG.E.UI.RefreshCoins();
                }, data.myshopid);
            }, () => { });
        });

        GoodBtn.onClick.AddListener(OnClickGoodBtn);

        PreviewBtn.onClick.AddListener(() =>
        {
            NWMng.E.OpenPreview(data.myshopid, (data) =>
            {
                var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose, 0);
                ui.SetData(data, ShopBlueprintLG.E.UI);
            });
        });

        Mask.onClick.AddListener(Close);
    }

    public override void Open(object inData)
    {
        base.Open(inData);
        data = (MyShopItem)inData;

        Icon.sprite = null;

        // アイコン
        AWSS3Mng.E.DownLoadTexture2D(Icon, this.data.icon);

        ItemName.text = data.newName;
        UserName.text = data.nickName;
        Price.text = data.price.ToString();
        GoodNum.text = data.goodNum.ToString();

        data.created_at = data.created_at.AddDays(7);

        TimeZoneMng.E.AddTimerEvent03(Timer);
    }

    public override void Close()
    {
        base.Close();

        TimeZoneMng.E.RemoveTimerEvent03(Timer);
    }

    public void OnClickGoodBtn()
    {
        if (DataMng.E.RuntimeData.UseGoodNum >= 3)
        {
            CommonFunction.ShowHintBar(1047001);
            return;
        }

        NWMng.E.MyShopGoodEvent((rp) =>
        {
            data.goodNum++;
            GoodNum.text = data.goodNum.ToString();


            DataMng.E.RuntimeData.UseGoodNum++;
            DataMng.E.RuntimeData.Coin3 += SettingMng.GoodAddPointCount;

            ShopBlueprintLG.E.UI.RefreshGoodNum(data.targetAcc);
            ShopBlueprintLG.E.UI.RefreshCoin();

            // 他のユーザーいいねするタスク
            TaskMng.E.AddMainTaskCount(7);

            ShowGoodAnim();
        }, data.targetAcc);
    }

    private void Timer()
    {
        TimeSpan t = data.created_at - DateTime.Now;
        Time.text = t.Days > 0 ?
                string.Format("残り時間: {0}日", t.Days) :
                string.Format("残り時間: {1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);
    }

    private void ShowGoodAnim()
    {
        if (anim != null && !anim.isPlaying)
        {
            anim.Play("ShopBlueprintGoodBtn");
        }
    }
}