using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxUI : UIBase
{
    Text TitleText { get => FindChiled<Text>("TitleText"); }

    Transform LevelUp { get => FindChiled<Transform>("LevelUp"); }

    Transform Exp { get => FindChiled<Transform>("Exp"); }
    Image Icon { get => FindChiled<Image>("Icon"); }
    Text Name { get => FindChiled<Text>("Name"); }
    Text Lv { get => FindChiled<Text>("Lv"); }
    Text AddExp { get => FindChiled<Text>("AddExp"); }
    Slider Slider { get => FindChiled<Slider>("Slider"); }

    Image MultiBonus { get => FindChiled<Image>("MultiBonus"); }
    Button Bonus3XBtn { get => FindChiled<Button>("Bonus3XBtn"); }
    Transform Bonus3XLabel { get => FindChiled<Transform>("Bonus3XLabel"); }
    Transform AdvertisingLabel { get => FindChiled<Transform>("AdvertisingLabel"); }
    Button OKBtn { get => FindChiled<Button>("OKBtn"); }
    Button AdvertisingBtn { get => FindChiled<Button>("AdvertisingBtn"); }

    Camera CameraforRT { get => FindChiled<Camera>("Camera for RT"); }
    Transform itemGridRoot;
    List<IconItemCell> cells;
    Action okBtnCallBack;

    float addedExp;

    string Bonus3XMsg = @"あつめたアイテムが３倍に増え、
「素材３倍チケット」が１枚消費されます。

素材３倍チケットを使用しますか？";

    public override void Init()
    {
        base.Init();

        GiftBoxLG.E.Init(this);

        AdvertisingBtn.gameObject.SetActive(true);
        AdvertisingLabel.gameObject.SetActive(false);

        AdvertisingBtn.onClick.AddListener(() =>
        {
            AudioMng.E.StopBGM();
            Bonus3XBtn.gameObject.SetActive(false);
            Bonus3XLabel.gameObject.SetActive(false);
            AdvertisingBtn.gameObject.SetActive(false);
            AdvertisingLabel.gameObject.SetActive(false);
            GoogleMobileAdsMng.E.ShowReawrd(() =>
            {
                int count = AdventureCtl.E.BonusList.Count;
                for (int i = 0; i < count; i++)
                {
                    AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                }
                StartDoubleBonus();
            });
        });

        ItemData ticket = DataMng.E.GetItemByItemId(9005);
        if (ticket != null)
        {
            Bonus3XBtn.gameObject.SetActive(true);
            Bonus3XBtn.onClick.AddListener(() =>
            {
               
                CommonFunction.ShowHintBox(Bonus3XMsg, () =>
                {
                    Bonus3XBtn.gameObject.SetActive(false);
                    Bonus3XLabel.gameObject.SetActive(false);
                    AdvertisingBtn.gameObject.SetActive(false);
                    AdvertisingLabel.gameObject.SetActive(false);
                    DataMng.E.ConsumableItem(9005);
                    int count = AdventureCtl.E.BonusList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                        AdventureCtl.E.BonusList.Add(AdventureCtl.E.BonusList[i]);
                    }
                    StartTripleBonus();
                }, ()=> { });
            });
            Bonus3XLabel.gameObject.SetActive(false);
        }
        else
        {
            Bonus3XBtn.gameObject.SetActive(false);
            Bonus3XLabel.gameObject.SetActive(true);
        }

        OKBtn.onClick.AddListener(() =>
        {
            Bonus3XBtn.gameObject.SetActive(false);
            Bonus3XLabel.gameObject.SetActive(false);
            AdvertisingBtn.gameObject.SetActive(false);
            AdvertisingLabel.gameObject.SetActive(false);
            OKBtn.gameObject.SetActive(false);

            if (DataMng.E.RuntimeData.MapType == MapType.Guide)
            {
                GuideLG.E.Next();
                PlayerCtl.E.Lock = false;
                ClearCell(itemGridRoot);

                StartCoroutine(StartAnimIE());
            }
            else
            {
                if (AdventureCtl.E.CurExp > 0)
                {
                    NWMng.E.AddExp((rp) =>
                    {
                        NWMng.E.ClearAdventure((rp) =>
                        {
                            if (okBtnCallBack != null)
                            {
                                PlayerCtl.E.Lock = false;
                                ClearCell(itemGridRoot);

                                StartCoroutine(StartAnimIE());
                            }
                        }, AdventureCtl.E.BonusList);
                    }, AdventureCtl.E.CurExp);
                }
                else
                {
                    okBtnCallBack();
                }
            }
        });

        itemGridRoot = FindChiled("Content");
    }

    public override void Open()
    {
        base.Open();

        PlayerCtl.E.Lock = true;
        MultiBonus.gameObject.SetActive(false);
        LevelUp.gameObject.SetActive(false);
        Exp.gameObject.SetActive(false);

        TitleText.text = "あつめたアイテム";
        addedExp = AdventureCtl.E.CurExp;

        //Icon.sprite = ReadResources<Sprite>("");
        Name.text = DataMng.E.RuntimeData.NickName;
        Lv.text = "Lv." + DataMng.E.RuntimeData.Lv;
        AddExp.text = "+" + addedExp;
        Slider.value = DataMng.E.RuntimeData.Exp / (float)ConfigMng.E.Character[DataMng.E.RuntimeData.Lv].LvUpExp;

#if UNITY_ANDROID
        AdvertisingBtn.gameObject.SetActive(false);
#endif
    }

    public override void Destroy()
    {
        CameraforRT.targetTexture = null;

        base.Destroy();
    }

    private IEnumerator StartAnimIE()
    {
        TitleText.text = "獲得した経験値";

        // Expバーを表示
        Exp.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        var lvUpExp = ConfigMng.E.Character[DataMng.E.RuntimeData.Lv].LvUpExp;
        var step = DataMng.E.RuntimeData.Exp + addedExp < lvUpExp ?
               addedExp / 50f :
               (lvUpExp - DataMng.E.RuntimeData.Exp) / 50f;

        // 経験値加算
        for (int i = 0; i < 50; i++)
        {
            if (addedExp <= 0 || DataMng.E.RuntimeData.Exp >= lvUpExp)
                break;
            
            addedExp -= step;
            DataMng.E.RuntimeData.Exp += step;

            AddExp.text = "+" + (int)addedExp;
            Slider.value = DataMng.E.RuntimeData.Exp / (float)lvUpExp;

            yield return new WaitForSeconds(0.02f);
        }

        // レベルアップなかった場合、残りのExpあると加算
        if (DataMng.E.RuntimeData.Exp + addedExp < lvUpExp && addedExp > 0)
        {
            DataMng.E.RuntimeData.Exp += addedExp;
            addedExp -= addedExp;

            AddExp.text = "+" + (int)addedExp;
            Slider.value = DataMng.E.RuntimeData.Exp / (float)lvUpExp;
        }

        // レベルアップした場合
        if (DataMng.E.RuntimeData.Exp + addedExp >= lvUpExp)
        {
            DataMng.E.RuntimeData.Exp -= lvUpExp;
            StartCoroutine(LevelUpIE());
        }
        else
        {
            okBtnCallBack();
        }
    }
    private IEnumerator LevelUpIE()
    {
        DataMng.E.RuntimeData.Lv++;
        Lv.text = "Lv." + DataMng.E.RuntimeData.Lv;
        Slider.value = 0;

        Lv.GetComponent<Animation>().Play("LevelUpText");
        yield return new WaitForSeconds(0.5f);

        LevelUp.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        LevelUp.gameObject.SetActive(false);

        StartCoroutine(StartAnimIE());
    }

    public void AddBonus(List<int> bonus)
    {
        Dictionary<int, int> items = CommonFunction.GetItemsByBonus(bonus);
        cells = new List<IconItemCell>();
        ClearCell(itemGridRoot);

        foreach (var item in items)
        {
            AddItem(item.Key, item.Value);
        }
    }
    private void AddItem(int itemID, int count)
    {
        if (itemID < 0)
            return;

        var cell = AddCell<IconItemCell>("Prefabs/UI/IconItem", itemGridRoot);
        if (cell != null)
        {
            cell.Add(ConfigMng.E.Item[itemID], count);
            cells.Add(cell);
        }
    }

    /// <summary>
    /// 2倍ボーナスボタンイベント
    /// </summary>
    private void StartDoubleBonus()
    {
        Bonus3XBtn.gameObject.SetActive(false);
        Bonus3XLabel.gameObject.SetActive(false);
        AdvertisingBtn.gameObject.SetActive(false);
        AdvertisingLabel.gameObject.SetActive(false);
        MultiBonus.gameObject.SetActive(true);
        MultiBonus.sprite = ReadResources<Sprite>("Textures/brave_2d_001"); ;

        foreach (var item in cells)
        {
            item.StartMultiAnim(2);
        }
    }

    /// <summary>
    /// 3倍ボーナスボタンイベント
    /// </summary>
    private void StartTripleBonus()
    {
        Bonus3XBtn.gameObject.SetActive(false);
        Bonus3XLabel.gameObject.SetActive(false);
        AdvertisingBtn.gameObject.SetActive(false);
        AdvertisingLabel.gameObject.SetActive(false);
        MultiBonus.gameObject.SetActive(true);
        MultiBonus.sprite = ReadResources<Sprite>("Textures/brave_2d_002"); ;

        foreach (var item in cells)
        {
            item.StartMultiAnim(3);
        }
    }

    /// <summary>
    /// OKボタンのイベント
    /// </summary>
    /// <param name="action"></param>
    public void SetCallBack(Action action)
    {
        okBtnCallBack = action;
    }
}
