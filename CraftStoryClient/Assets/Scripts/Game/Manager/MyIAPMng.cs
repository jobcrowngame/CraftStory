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
        Debug.Log("UnityPurchasing 初期化完了");
    }

    public void Init(IAPTest iap)
    {
        this.iap = iap;
    }

    /// <summary>
    /// Unity IAP が購入処理を行える場合に呼び出されます
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");

        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    ///  Unity IAP 回復不可能な初期エラーに遭遇したときに呼び出されます。
    ///
    /// これは、インターネットが使用できない場合は呼び出されず、
    /// インターネットが使用可能になるまで初期化を試みます。
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed");
        iap.OnInitializeFailed();
    }

    /// <summary>
    /// 購入が終了したときに呼び出されます。
    ///
    ///  OnInitialized() 後、いつでも呼び出される場合があります。
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("ProcessPurchase");
        iap.ProcessPurchase();
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 購入が失敗したときに呼び出されます。
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("OnPurchaseFailed");
        iap.OnPurchaseFailed();
    }

    // 購入処理を開始するために、ユーザーが '購入' ボタン
    // を押すと、関数が呼び出されます。
    public void OnPurchaseClicked(string productId)
    {
        Debug.Log("OnPurchaseClicked " + productId);
        controller.InitiatePurchase(productId);
    }
}