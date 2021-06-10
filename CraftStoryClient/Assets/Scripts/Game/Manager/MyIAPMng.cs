using UnityEngine;
using UnityEngine.Purchasing;

public class MyIAPManager : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    private IAPTest iap;

    public void Init(IAPTest iap)
    {
        this.iap = iap;

        iap.ShowMsg("�������J�n");

        try
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            //builder.AddProduct("100_gold_coins", ProductType.Consumable, new IDs
            //{
            //    {"100_gold_coins_google", GooglePlay.Name},
            //    {"100_gold_coins_mac", MacAppStore.Name}
            //});

            UnityPurchasing.Initialize(this, builder);
        }
        catch (System.Exception ex)
        {
            iap.ShowMsg(ex.Message);
        }
    }

    /// <summary>
    /// Unity IAP ���w���������s����ꍇ�ɌĂяo����܂�
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");

        this.controller = controller;
        this.extensions = extensions;

        iap.ShowMsg("UnityPurchasing Init OK! ����������");
    }

    /// <summary>
    ///  Unity IAP �񕜕s�\�ȏ����G���[�ɑ��������Ƃ��ɌĂяo����܂��B
    ///
    /// ����́A�C���^�[�l�b�g���g�p�ł��Ȃ��ꍇ�͌Ăяo���ꂸ�A
    /// �C���^�[�l�b�g���g�p�\�ɂȂ�܂ŏ����������݂܂��B
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        iap.ShowMsg("UnityPurchasing Init Fail! ���������s.\n" + error);
    }

    /// <summary>
    /// �w�����I�������Ƃ��ɌĂяo����܂��B
    ///
    ///  OnInitialized() ��A���ł��Ăяo�����ꍇ������܂��B
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        iap.ShowMsg("�w������.");
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// �w�������s�����Ƃ��ɌĂяo����܂��B
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        iap.ShowMsg("�w�����s.");
    }

    // �w���������J�n���邽�߂ɁA���[�U�[�� '�w��' �{�^��
    // �������ƁA�֐����Ăяo����܂��B
    public void OnPurchaseClicked(string productId)
    {
        iap.ShowMsg("�w���J�n." + productId);
        controller.InitiatePurchase(productId);
    }
}