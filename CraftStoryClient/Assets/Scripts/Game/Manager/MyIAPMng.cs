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

        iap.ShowMsg("初期化開始");

        try
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("craftstory_120", ProductType.Consumable, new IDs
            {
                {"craftstory_120", AppleAppStore.Name},
            });
            builder.AddProduct("craftstory_free", ProductType.Consumable, new IDs
            {
                {"craftstory_free", AppleAppStore.Name}
            });

            UnityPurchasing.Initialize(this, builder);
        }
        catch (System.Exception ex)
        {
            iap.ShowMsg(ex.Message);
        }
    }

    /// <summary>
    /// Unity IAP が購入処理を行える場合に呼び出されます
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized");

        this.controller = controller;
        this.extensions = extensions;

        iap.ShowMsg("UnityPurchasing Init OK! 初期化完了");
    }

    /// <summary>
    ///  Unity IAP 回復不可能な初期エラーに遭遇したときに呼び出されます。
    ///
    /// これは、インターネットが使用できない場合は呼び出されず、
    /// インターネットが使用可能になるまで初期化を試みます。
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        iap.ShowMsg("UnityPurchasing Init Fail! 初期化失敗.\n" + error);
    }

    /// <summary>
    /// 購入が終了したときに呼び出されます。
    ///
    ///  OnInitialized() 後、いつでも呼び出される場合があります。
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        iap.ShowMsg("購入成功.");
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 購入が失敗したときに呼び出されます。
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        iap.ShowMsg("購入失敗.");
    }

    // 購入処理を開始するために、ユーザーが '購入' ボタン
    // を押すと、関数が呼び出されます。
    public void OnPurchaseClicked(string productId)
    {
        iap.ShowMsg("購入開始." + productId);
        controller.InitiatePurchase(productId);
    }
}