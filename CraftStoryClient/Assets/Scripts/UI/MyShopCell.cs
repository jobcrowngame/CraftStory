using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyShopCell : UIBase
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
                    break;

                case 2:
                    UnLock.gameObject.SetActive(DataMng.E.MyShop.myShopLv < 1);
                    UnLockBtnText.text = SettingMng.E.MyShopCostLv1 + "個";
                    break;

                case 3:
                    UnLock.gameObject.SetActive(DataMng.E.MyShop.myShopLv < 2);
                    UnLockBtnText.text = SettingMng.E.MyShopCostLv2 + "個";
                    break;
            }

            myShopItem = DataMng.E.MyShop.MyShopItem[value - 1];
            if (myShopItem.itemId > 0 && !IsTimeOut())
            {
                if ((DateTime.Now - myShopItem.created_at).Days >= 7)
                {
                    DataMng.E.MyShop.MyShopItem[value - 1] = new MyShopItem();
                    myShopItem = DataMng.E.MyShop.MyShopItem[value - 1];
                }

                myShopItem.created_at = myShopItem.created_at.AddDays(7);

                NewNameText.text = myShopItem.newName;
                Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[myShopItem.itemId].IconResourcesPath);

                RefreshTime();
                PreviewBtn.gameObject.SetActive(true);
                LoadBtn.gameObject.SetActive(true);
                Time.gameObject.SetActive(true);
                NewNameBG.gameObject.SetActive(true);
            }
            else
            {
                Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[0].IconResourcesPath); ;

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
                            MyShopLG.E.UI.RefreshUI();
                            OpenTimer = false;
                        }, myShopItem.site, 0);
                    }
                }, () => { });
            }
        });
        PreviewBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
            ui.SetData(myShopItem.data, MyShopLG.E.UI);
        });
        Icon.transform.GetComponent<Button>().onClick.AddListener(() => 
        {
            if (DataMng.E.MyShop.myShopLv < Index - 1)
            {
                int cost = DataMng.E.MyShop.myShopLv == 0
                    ? SettingMng.E.MyShopCostLv1
                    : SettingMng.E.MyShopCostLv2;

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
                        MyShopLG.E.UpdateMyShopLevel(cost);
                        CommonFunction.ShowHintBar(13);
                    }
                }, () => { });
            }
            else
            {
                if (DataMng.E.MyShop.MyShopItem[Index - 1].itemId == 0)
                {
                    MyShopSelectItemLG.E.Index = Index;

                    string msg = string.Format(@"マイショップに設計図をアップロードすると
7日間全ユーザーへ公開されます。

7日間以内にアップロードを取り下げることも
できますが、クラフトシードを「100個」消費します");

                    CommonFunction.ShowHintBox(null, msg, () =>
                    {
                        UICtl.E.OpenUI<MyShopSelectItemUI>(UIType.MyShopSelectItem);
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

    public void Init(int index)
    {
        Index = index;

        if (myShopItem.itemId > 0)
        {
            OpenTimer = true;
            StartCoroutine(RefreshTime());
        }
    }
    private IEnumerator RefreshTime()
    {
        while (OpenTimer)
        {
            TimeSpan t = myShopItem.created_at - DateTime.Now;
            if (IsTimeOut())
            {
                NWMng.E.LoadBlueprint((rp) =>
                {
                    DataMng.E.MyShop.MyShopItem[Index - 1] = new MyShopItem();
                    MyShopLG.E.UI.RefreshUI();
                    OpenTimer = false;
                }, myShopItem.site, 1);
            }
            else
            {
                Time.text = string.Format("{0}日{1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);

            }

            yield return new WaitForSeconds(1);
        }
    }
    private bool IsTimeOut()
    {
        TimeSpan t = myShopItem.created_at - DateTime.Now;
        return t.TotalSeconds < 0;
    }
}