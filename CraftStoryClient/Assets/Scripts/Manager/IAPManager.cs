using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    private IAPStore iapStore;

    public void Init()
    {
        iapStore = new IAPStore();

        Logger.E.Log("初期化開始");

        try
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct("craftstory_120", ProductType.Consumable);
            builder.AddProduct("craftstory_980", ProductType.Consumable);
            builder.AddProduct("craftstory_1960", ProductType.Consumable);
            builder.AddProduct("craftstory_3060", ProductType.Consumable);
            builder.AddProduct("craftstory_4900", ProductType.Consumable);
            builder.AddProduct("craftstory_limit_490", ProductType.Consumable);
            builder.AddProduct("craftstory_limit_1480", ProductType.Consumable);
            builder.AddProduct("craftstory_limit_4400", ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }
        catch (System.Exception ex)
        {
            Logger.E.Error(ex.Message);
        }
    }

    /// <summary>
    /// Unity IAP が購入処理を行える場合に呼び出されます
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Logger.E.Log("OnInitialized");

        this.controller = controller;
        this.extensions = extensions;

        Logger.E.Log("UnityPurchasing Init OK! 初期化完了");
    }

    /// <summary>
    ///  Unity IAP 回復不可能な初期エラーに遭遇したときに呼び出されます。
    ///
    /// これは、インターネットが使用できない場合は呼び出されず、
    /// インターネットが使用可能になるまで初期化を試みます。
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Logger.E.Error("UnityPurchasing Init Fail! 初期化失敗.\n" + error);
    }

    /// <summary>
    /// 購入が終了したときに呼び出されます。
    ///
    ///  OnInitialized() 後、いつでも呼び出される場合があります。
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Logger.E.Log("購入成功.");
        Logger.E.Log("[ID]" + e.purchasedProduct.transactionID);
        Logger.E.Log("[receipt]" + e.purchasedProduct.receipt);
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 購入が失敗したときに呼び出されます。
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Logger.E.Error("購入失敗.");
    }

    // 購入処理を開始するために、ユーザーが '購入' ボタン
    // を押すと、関数が呼び出されます。
    public void OnPurchaseClicked(string productId)
    {
        Logger.E.Log("購入開始." + productId);
        controller.InitiatePurchase(productId);
    }
}