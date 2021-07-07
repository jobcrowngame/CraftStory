using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MyShopCell : UIBase
{
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Time;
    Button UnlockBtn { get => FindChiled<Button>("UnlockBtn"); }
    Text UnlockBtnText { get => FindChiled<Text>("Text", UnlockBtn.transform); }
    Button PreviewBtn { get => FindChiled<Button>("PreviewBtn"); }

    MyShopItem item;
    bool OpenTimer = true;

    private int Index 
    {
        get => index;
        set
        {
            index = value;
            item = new MyShopItem();

            switch (value)
            {
                case 1:
                    UnlockBtn.gameObject.SetActive(false);
                    break;

                case 2:
                    UnlockBtn.gameObject.SetActive(DataMng.E.RuntimeData.MyShop.myShopLv < 1);
                    UnlockBtnText.text = SettingMng.E.MyShopCostLv1 + "個";
                    break;

                case 3:
                    UnlockBtn.gameObject.SetActive(DataMng.E.RuntimeData.MyShop.myShopLv < 2);
                    UnlockBtnText.text = SettingMng.E.MyShopCostLv2 + "個";
                    break;
            }

            item = DataMng.E.RuntimeData.MyShop.myShopItem[value - 1];
            if ((DateTime.Now - item.created_at).Days >= 7)
            {
                DataMng.E.RuntimeData.MyShop.myShopItem[value - 1] = new MyShopItem();
                item = DataMng.E.RuntimeData.MyShop.myShopItem[value - 1];
            }

            item.created_at = item.created_at.AddDays(7);

            Icon.sprite = ReadResources<Sprite>(ConfigMng.E.Item[item.itemId].IconResourcesPath);

            if (item.itemId > 0)
            {
                RefreshTime();
                PreviewBtn.gameObject.SetActive(true);
            }
            else
            {
                Time.text = "";
            }
        }
    }
    private int index;

    private void Awake()
    {
        Time = FindChiled<Text>("Time");

        UnlockBtn.onClick.AddListener(() =>
        {
            int cost = DataMng.E.RuntimeData.MyShop.myShopLv == 0 ? SettingMng.E.MyShopCostLv1 : SettingMng.E.MyShopCostLv2;
            string msg = string.Format("{0}{1}個消耗して解放しますか。", "クラフトシード", cost);
            CommonFunction.ShowHintBox(null, msg, () =>
            {
                MyShopLG.E.UpdateMyShopLevel();
            }, () => { });
        });
        PreviewBtn.onClick.AddListener(() =>
        {
            var ui = UICtl.E.OpenUI<BlueprintPreviewUI>(UIType.BlueprintPreview, UIOpenType.AllClose);
            ui.SetData(item.data);
        });
        Icon.transform.GetComponent<Button>().onClick.AddListener(() => 
        {
            if (DataMng.E.RuntimeData.MyShop.myShopLv < Index - 1)
            {
                CommonFunction.ShowHintBar(13);
                return;
            }

            if (DataMng.E.RuntimeData.MyShop.myShopItem[Index - 1].itemId == 0)
            {
                MyShopSelectItemLG.E.Index = Index;
                UICtl.E.OpenUI<MyShopSelectItemUI>(UIType.MyShopSelectItem);
            }
            else
            {
                CommonFunction.ShowHintBar(12);
            }
        });
    }

    public void Init(int index)
    {
        Index = index;

        if (item.itemId > 0)
        {
            StartCoroutine(RefreshTime());
        }
    }
    private IEnumerator RefreshTime()
    {
        while (OpenTimer)
        {
            TimeSpan t = item.created_at - DateTime.Now;
            Time.text = string.Format("{0}日{1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);
            yield return new WaitForSeconds(1);
        }
    }
}