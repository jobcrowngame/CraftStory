using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopBlueprintMyShopCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Transform NewNameBG { get => FindChiled("NewNameBG"); }
    Text NewNameText { get => FindChiled<Text>("Text", NewNameBG); }
    Text Time;
    Transform UnLock { get => FindChiled("UnLockBtn"); }
    Button LoadBtn { get => FindChiled<Button>("LoadBtn"); }
    Text UnLockBtnText { get => FindChiled<Text>("Text", UnLock.transform); }
    Button PreviewBtn { get => FindChiled<Button>("PreviewBtn"); }

    MyShopItem myShopItem;
    bool OpenTimer = true;

    private int Index 
    {
        get => index;
        set
        {
            index = value;
            myShopItem = new MyShopItem();

            switch (value)
            {
                case 1:
                    UnLock.gameObject.SetActive(false);
                    Icon.sprite = ReadResources<Sprite>("Textures/shop_2d_075");
                    break;

                case 2:
                    UnLock.gameObject.SetActive(DataMng.E.MyShop.myShopLv < 1);
                    UnLockBtnText.text = SettingMng.MyShopCostLv1 + "個";
                    Icon.sprite = ReadResources<Sprite>(DataMng.E.MyShop.myShopLv < 1 ? "Textures/shop_2d_076" : "Textures/shop_2d_075");
                    break;

                case 3:
                    UnLock.gameObject.SetActive(DataMng.E.MyShop.myShopLv < 2);
                    UnLockBtnText.text = SettingMng.MyShopCostLv2 + "個";
                    Icon.sprite = ReadResources<Sprite>(DataMng.E.MyShop.myShopLv < 2 ? "Textures/shop_2d_076" : "Textures/shop_2d_075");
                    break;
            }

            myShopItem = DataMng.E.MyShop.MyShopItem[value - 1];
            myShopItem.created_at = myShopItem.created_at.AddDays(7);

            if (myShopItem.itemId > 0 && !IsTimeOut())
            {
                NewNameText.text = myShopItem.newName;
                Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[myShopItem.itemId].IconResourcesPath);
                if (!string.IsNullOrEmpty(myShopItem.icon))
                {
                    AWSS3Mng.E.DownLoadTexture2D(Icon, myShopItem.icon);
                }

                PreviewBtn.gameObject.SetActive(true);
                LoadBtn.gameObject.SetActive(true);
                Time.gameObject.SetActive(true);
                NewNameBG.gameObject.SetActive(true);
            }
            else
            {
                PreviewBtn.gameObject.SetActive(false);
                LoadBtn.gameObject.SetActive(false);
                Time.gameObject.SetActive(false);
                NewNameBG.gameObject.SetActive(false);
            }
        }
    }
    private int index;

    private void Awake()
    {
        Time = FindChiled<Text>("Time");

        LoadBtn.onClick.AddListener(() =>
        {
            if (myShopItem.itemId > 0)
            {
                string msg = @"クラフトシード「100個」を消費して
アップロードを取り下げますか？";

                CommonFunction.ShowHintBox(msg, () =>
                {
                    if (DataMng.E.RuntimeData.Coin1 < 100)
                    {
                        CommonFunction.ShowHintBar(1010001);
                    }
                    else
                    {
                        NWMng.E.LoadBlueprint((rp) =>
                        {
                            DataMng.E.MyShop.MyShopItem[Index - 1] = new MyShopItem();
                            DataMng.E.RuntimeData.Coin1 -= 100;
                            ShopBlueprintLG.E.UI.RefreshMyShopWindow();
                            OpenTimer = false;
                            ShopBlueprintLG.E.UI.RefreshCoin();
                            HomeLG.E.UI.RefreshCoins();
                        }, myShopItem.site, 0);
                    }
                }, () => { });
            }
        });
        PreviewBtn.onClick.AddListener(() =>
        {
            NWMng.E.OpenPreview(myShopItem.myshopid, (data)=> 
            {
                var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose, 0);
                ui.SetData(data, ShopBlueprintLG.E.UI);
            });

        });
        Icon.transform.GetComponent<Button>().onClick.AddListener(() => 
        {
            if (DataMng.E.MyShop.myShopLv < Index - 1)
            {
                int cost = DataMng.E.MyShop.myShopLv == 0
                    ? SettingMng.MyShopCostLv1
                    : SettingMng.MyShopCostLv2;

                string msg = string.Format(@"クラフトシード「{0}個」を消費することで、
アップロード枠を開放することができます。

アップロード枠を開放すると、
複数の設計図を販売できるようになります。

アップロード枠を開放しますか？", cost);

                CommonFunction.ShowHintBox(null, msg, () =>
                {
                    if (DataMng.E.RuntimeData.Coin1 < cost)
                    {
                        CommonFunction.ShowHintBar(1010001);
                    }
                    else
                    {
                        ShopBlueprintLG.E.UpdateMyShopLevel(cost);
                        CommonFunction.ShowHintBar(13);
                    }
                }, () => { });
            }
            else
            {
                if (DataMng.E.MyShop.MyShopItem[Index - 1].itemId == 0)
                {
                    ShopBlueprintMyShopSelectItemLG.E.Index = Index;

                    string msg = string.Format(@"マイショップに設計図をアップロードすると
7日間全ユーザーへ公開されます。

7日間以内にアップロードを取り下げることも
できますが、クラフトシードを「100個」消費します");

                    CommonFunction.ShowHintBox(null, msg, () =>
                    {
                        UICtl.E.OpenUI<ShopBlueprintMyShopSelectItemUI>(UIType.ShopBlueprintMyShopSelectItem);
                        GuideLG.E.Next();
                    }, null, "button_2D_022");
                }
                else
                {
                    CommonFunction.ShowHintBar(12);
                }

                GuideLG.E.Next();
            }
        });
    }

    public override void Destroy()
    {
        base.Destroy();

        TimeZoneMng.E.RemoveTimerEvent03(Timer);
    }

    public void Init(int index)
    {
        Index = index;


        if (myShopItem.itemId > 0)
        {
            OpenTimer = true;
        }

        TimeZoneMng.E.AddTimerEvent03(Timer);
    }

    private void Timer()
    {
        if (OpenTimer)
        {
            TimeSpan t = myShopItem.created_at - DateTime.Now;
            if (IsTimeOut())
            {
                NWMng.E.LoadBlueprint((rp) =>
                {
                    DataMng.E.MyShop.MyShopItem[Index - 1] = new MyShopItem();
                    ShopBlueprintLG.E.UI.RefreshMyShopWindow();
                    OpenTimer = false;
                }, myShopItem.site, 1);
            }
            else
            {
                Time.text = t.Days > 0 ?
                    string.Format("残り時間: {0}日", t.Days) :
                    string.Format("残り時間: {1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);
            }
        }
    }

    private bool IsTimeOut()
    {
        TimeSpan t = myShopItem.created_at - DateTime.Now;
        return t.TotalSeconds < 0;
    }
}