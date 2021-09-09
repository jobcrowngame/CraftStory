using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ʃ^�C�g��
/// </summary>
public class TitleUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Text Title { get => FindChiled<Text>("TitleText"); }

    Text Coin1 { get => FindChiled<Text>("Coin1"); }
    Image Coin1Image { get => FindChiled<Image>("Image", Coin1.transform); }
    Text Coin2 { get => FindChiled<Text>("Coin2"); }
    Image Coin2Image { get => FindChiled<Image>("Image", Coin2.transform); }
    Text Coin3 { get => FindChiled<Text>("Coin3"); }
    Image Coin3Image { get => FindChiled<Image>("Image", Coin3.transform); }

    Action action;

    private void Awake()
    {
        CloseBtn.onClick.AddListener(()=> 
        {
            if (action != null)
                action();
        });

        Coin1.text = "0";
        Coin1Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9000].IconResourcesPath);
        Coin2.text = "0";
        Coin2Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9001].IconResourcesPath);
        Coin3.text = "0";
        Coin3Image.sprite = ReadResources<Sprite>(ConfigMng.E.Item[9002].IconResourcesPath);
    }

    /// <summary>
    /// �^�C�g�������Z�b�g
    /// </summary>
    /// <param name="title">�^�C�g��</param>
    public void SetTitle(string title)
    {
        Title.text = title;
    }

    /// <summary>
    /// �R�C�����X�V
    /// </summary>
    public void RefreshCoins()
    {
        Coin1.text = DataMng.E.RuntimeData.Coin1.ToString();
        Coin2.text = DataMng.E.RuntimeData.Coin2.ToString();
        Coin3.text = DataMng.E.RuntimeData.Coin3.ToString();
    }

    /// <summary>
    /// Close�{�^�����N���b�N����ꍇ�̃C�x���g
    /// </summary>
    /// <param name="action">�C�x���g</param>
    public void SetOnClose(Action action)
    {
        this.action = action;
    }

    /// <summary>
    /// �R�C�����B���
    /// </summary>
    /// <param name="index"></param>
    public void EnActiveCoin(int index)
    {
        if (index == 1) Coin1.gameObject.SetActive(false);
        if (index == 2) Coin2.gameObject.SetActive(false);
        if (index == 3) Coin3.gameObject.SetActive(false);
    }

    /// <summary>
    /// Close�{�^����enabled�ݒ�
    /// </summary>
    /// <param name="b"></param>
    public void CloseBtnEnable(bool b)
    {
        CloseBtn.enabled = b;
    }
}
