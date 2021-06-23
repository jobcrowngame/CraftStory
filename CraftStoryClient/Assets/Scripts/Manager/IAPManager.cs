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

            string receipt = builder.Configure<IAppleConfiguration>().appReceipt;
            Debug.Log(receipt);

            bool canMakePayments = builder.Configure<IAppleConfiguration>().canMakePayments;
            Debug.Log(canMakePayments);
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

        foreach (var product in controller.products.all)
        {
            Logger.E.Log("[ProductId]" + product.definition.storeSpecificId);
            Logger.E.Log("[Title]" + product.metadata.localizedTitle);
        }

        extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(product => {
            Logger.E.Log("RegisterPurchaseDeferredListener" + product.definition.id);
        });

        extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result => {
            if (result)
            {
                // なにかがリストアされたというわけではありません
                // 単にリストアの処理が終了したということです
                Logger.E.Log("リストア成功");
            }
            else
            {
                // リストア失敗
                Logger.E.Log("リストア失敗");
            }
        });

        extensions.GetExtension<IAppleExtensions>().RefreshAppReceipt(receipt => {
            // リクエストが成功したら、このハンドラーが呼び出されます
            // レシートは最新の App Store レシート
            Logger.E.Log(receipt);
            Logger.E.Log("successCallback");
        },
        () => {
            // リクエストが失敗したら、このハンドラーが呼び出されます。
            // 例えば、ネットワークが使用不可であったり、
            // ユーザーが誤ったパスワードを入力した場合など。
            Logger.E.Log("RefreshAppReceipt Error");
        });

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
        Product product = e.purchasedProduct;

        Logger.E.Log(e.purchasedProduct.metadata.isoCurrencyCode);
        Logger.E.Log("[ID]" + e.purchasedProduct.transactionID);

        var productId = product.definition.storeSpecificId;
        var receiptId = e.purchasedProduct.transactionID;
        var receipt = product.receipt;

        NWMng.E.Charge((rp) => 
        {
            controller.ConfirmPendingPurchase(product);

            NWMng.E.GetCoins((rp) =>
            {
                DataMng.GetCoins(rp[0]);
                if (ShopLG.E.UI != null) ShopLG.E.UI.Refresh();
            });
        }, productId, receiptId);

        return PurchaseProcessingResult.Pending;
    }

    /// <summary>
    /// 購入が失敗したときに呼び出されます。
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Logger.E.Error("購入失敗.");

        if (p == PurchaseFailureReason.PurchasingUnavailable)
        {
            // デバイス設定で IAP が無効である場合があります。
            Logger.E.Error("PurchaseFailureReason.PurchasingUnavailable");
        }
    }

    // 購入処理を開始するために、ユーザーが '購入' ボタン
    // を押すと、関数が呼び出されます。
    public void OnPurchaseClicked(string productId)
    {
        Logger.E.Log("購入開始." + productId);
        controller.InitiatePurchase(productId);
    }
}