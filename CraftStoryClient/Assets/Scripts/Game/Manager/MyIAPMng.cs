using UnityEngine;
using UnityEngine.Purchasing;

public class MyIAPManager : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;
    private IAPTest iap;

    public MyIAPManager()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("100_gold_coins", ProductType.Consumable, new IDs
        {
            {"100_gold_coins_google", GooglePlay.Name},
            {"100_gold_coins_mac", MacAppStore.Name}
        });

        UnityPurchasing.Initialize(this, builder);
        Debug.Log("UnityPurchasing ����������");
    }

    public void Init(IAPTest iap)
    {
        this.iap = iap;
    }

    /// <summary>
    /// Unity IAP ���w���������s����ꍇ�ɌĂяo����܂�
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");

        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    ///  Unity IAP �񕜕s�\�ȏ����G���[�ɑ��������Ƃ��ɌĂяo����܂��B
    ///
    /// ����́A�C���^�[�l�b�g���g�p�ł��Ȃ��ꍇ�͌Ăяo���ꂸ�A
    /// �C���^�[�l�b�g���g�p�\�ɂȂ�܂ŏ����������݂܂��B
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed");
        iap.OnInitializeFailed();
    }

    /// <summary>
    /// �w�����I�������Ƃ��ɌĂяo����܂��B
    ///
    ///  OnInitialized() ��A���ł��Ăяo�����ꍇ������܂��B
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase");
        iap.ProcessPurchase();
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// �w�������s�����Ƃ��ɌĂяo����܂��B
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("OnPurchaseFailed");
        iap.OnPurchaseFailed();
    }

    // �w���������J�n���邽�߂ɁA���[�U�[�� '�w��' �{�^��
    // �������ƁA�֐����Ăяo����܂��B
    public void OnPurchaseClicked(string productId)
    {
        Debug.Log("OnPurchaseClicked " + productId);
        controller.InitiatePurchase(productId);
    }
}