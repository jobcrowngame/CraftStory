using UnityEngine;
using UnityEngine.UI;

public class Title2UI : UIBase
{
    Transform Coin1 { get => FindChiled("Coin1"); }
    Transform Coin2 { get => FindChiled("Coin2"); }
    Transform Coin3 { get => FindChiled("Coin3"); }
    Image Icon1 { get => FindChiled<Image>("Icon", Coin1); }
    Image Icon2 { get => FindChiled<Image>("Icon", Coin2); }
    Image Icon3 { get => FindChiled<Image>("Icon", Coin3); }
    Text Text1 { get => FindChiled<Text>("Text", Coin1); }
    Text Text2 { get => FindChiled<Text>("Text", Coin2); }
    Text Text3 { get => FindChiled<Text>("Text", Coin3); }

    public override void Init()
    {
        base.Init();

        Icon1.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9000].IconResourcesPath);
        Icon2.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9001].IconResourcesPath);
        Icon3.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);

        Text1.text = "0";
        Text2.text = "0";
        Text3.text = "0";

        Coin1.gameObject.SetActive(false);
        Coin2.gameObject.SetActive(false);
        Coin3.gameObject.SetActive(false);
    }

    /// <summary>
    /// コインを更新
    /// </summary>
    public void RefreshCoins()
    {
        Text1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Text2.text = DataMng.E.RuntimeData.Coin2.ToString();
        Text3.text = DataMng.E.RuntimeData.Coin3.ToString();
    }

    /// <summary>
    /// コインを表し
    /// </summary>
    /// <param name="index">コインインデックス</param>
    public void ShowCoin(int index)
    {
        if (index == 1) Coin1.gameObject.SetActive(true);
        if (index == 2) Coin2.gameObject.SetActive(true);
        if (index == 3) Coin3.gameObject.SetActive(true);
    }
}
