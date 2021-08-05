using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPMng : Single<IAPMng>, IStoreListener
{
    private IStoreController m_Controller;
    private IAppleExtensions m_AppleExtensions;
    private ITransactionHistoryExtensions m_TransactionHistoryExtensions;
    private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;

    public override void Init()
    {
        Logger.Log("初期化開始");

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
            builder.AddProduct("craftstory_subsc_980", ProductType.Consumable);
            builder.AddProduct("craftstory_subsc_1960", ProductType.Consumable);
            builder.AddProduct("craftstory_subsc_3060", ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);

            string receipt = builder.Configure<IAppleConfiguration>().appReceipt;
            Logger.Log(receipt);

            bool canMakePayments = builder.Configure<IAppleConfiguration>().canMakePayments;
            Logger.Log(canMakePayments);
        }
        catch (System.Exception ex)
        {
            Logger.Error(ex.Message);
        }
    }

    /// <summary>
    /// This will be called when Unity IAP has finished initialising.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Logger.Log("OnInitialized");

        m_Controller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();
        m_TransactionHistoryExtensions = extensions.GetExtension<ITransactionHistoryExtensions>();
        m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();


        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        //ShowSubscription();


        extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(product =>
        {
            Logger.Log("RegisterPurchaseDeferredListener" + product.definition.id);
        });

        extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        {
            if (result)
            {
                // なにかがリストアされたというわけではありません
                // 単にリストアの処理が終了したということです
                Logger.Log("リストア成功");
            }
            else
            {
                // リストア失敗
                Logger.Log("リストア失敗");
            }
        });

        extensions.GetExtension<IAppleExtensions>().RefreshAppReceipt(receipt =>
        {
            // リクエストが成功したら、このハンドラーが呼び出されます
            // レシートは最新の App Store レシート
            Logger.Log(receipt);
            Logger.Log("successCallback");
        },
        () =>
        {
            // リクエストが失敗したら、このハンドラーが呼び出されます。
            // 例えば、ネットワークが使用不可であったり、
            // ユーザーが誤ったパスワードを入力した場合など。
            Logger.Log("RefreshAppReceipt Error");
        });

        Logger.Log("UnityPurchasing Init OK! 初期化完了");
    }

    /// <summary>
    ///  Unity IAP 回復不可能な初期エラーに遭遇したときに呼び出されます。
    ///
    /// これは、インターネットが使用できない場合は呼び出されず、
    /// インターネットが使用可能になるまで初期化を試みます。
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Logger.Error("UnityPurchasing Init Fail! 初期化失敗.\n" + error);
            NWMng.E.ShowClientLog("UnityPurchasing Init Fail! 初期化失敗.\n" + error);
    }

    /// <summary>
    /// 購入が終了したときに呼び出されます。
    ///
    ///  OnInitialized() 後、いつでも呼び出される場合があります。
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Logger.Log("購入成功.");
        Product product = e.purchasedProduct;

        Logger.Log(e.purchasedProduct.metadata.isoCurrencyCode);
        Logger.Log("[ID]" + e.purchasedProduct.transactionID);

        var productId = product.definition.storeSpecificId;
        var receiptId = e.purchasedProduct.transactionID;
        var receipt = product.receipt;

        NWMng.E.ShowClientLog(string.Format("購入成功:Acc = {0}, productId = {1}, ProductType = {2}", 
            DataMng.E.UserData.Account, productId, product.definition.type));

        if (product.definition.type == ProductType.Consumable)
        {
            if (productId == "craftstory_subsc_980"
                || productId == "craftstory_subsc_1960"
                || productId == "craftstory_subsc_3060")
            {
                var config = ConfigMng.E.GetShopByName(productId);
                if (config != null)
                {
                    NWMng.E.BuySubscription((rp) =>
                    {
                        NWMng.E.GetSubscriptionInfo(() =>
                        {
                            if (ShopLG.E.UI != null) ShopLG.E.UI.RefreshSubscription();

                            NWMng.E.GetNewEmailCount(() =>
                            {
                                HomeLG.E.UI.RefreshRedPoint();
                            });
                        });

                        m_Controller.ConfirmPendingPurchase(product);
                    }, config.ID);
                }
            }
            else
            {
                NWMng.E.Charge((rp) =>
                {
                    NWMng.E.GetCoins((rp) =>
                    {
                        DataMng.GetCoins(rp);
                        if (ShopLG.E.UI != null) ShopLG.E.UI.RefreshCoins();

                        
                    });

                    m_Controller.ConfirmPendingPurchase(product);
                }, productId, receiptId);
            }
           
        }
        else if (product.definition.type == ProductType.Subscription)
        {
           
        }

        NWMng.E.ShowClientLog(receipt);


        //ShowSubscription();



        return PurchaseProcessingResult.Pending;
    }

    /// <summary>
    /// 購入が失敗したときに呼び出されます。
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Logger.Warning("購入失敗.");

        if (p == PurchaseFailureReason.PurchasingUnavailable)
        {
            // デバイス設定で IAP が無効である場合があります。
            Logger.Error("PurchaseFailureReason.PurchasingUnavailable");
            NWMng.E.ShowClientLog("PurchaseFailureReason.PurchasingUnavailable");
        }
    }

    // 購入処理を開始するために、ユーザーが '購入' ボタン
    // を押すと、関数が呼び出されます。
    public void OnPurchaseClicked(string productId)
    {
        Logger.Log("購入開始." + productId);
        m_Controller.InitiatePurchase(productId);
    }




    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    ///
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Logger.Log("Purchase deferred: " + item.definition.id);
    }

    private bool checkIfProductIsAvailableForSubscriptionManager(string receipt)
    {
        var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
        if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
        {
            Logger.Log("The product receipt does not contain enough information");
            return false;
        }
        var store = (string)receipt_wrapper["Store"];
        var payload = (string)receipt_wrapper["Payload"];

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            Logger.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            Logger.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Logger.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }

    public void ShowSubscription()
    {
        Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();
        Logger.Warning("---------Start show IntroductoryPriceDictionary");
        foreach (var item in introductory_info_dict.Keys)
        {
            Logger.Log("Key:{0}, Value:{1}", item, introductory_info_dict[item]);
        }
        Logger.Warning("---------End show IntroductoryPriceDictionary");

        foreach (var item in m_Controller.products.all)
        {
            // this is the usage of SubscriptionManager class
            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    if (checkIfProductIsAvailableForSubscriptionManager(item.receipt))
                    {
                        string intro_json = (introductory_info_dict == null
                            || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId))
                                ? null
                                : introductory_info_dict[item.definition.storeSpecificId];

                        SubscriptionManager p = new SubscriptionManager(item, intro_json);
                        SubscriptionInfo info = p.getSubscriptionInfo();
                        Logger.Log("product id is: " + info.getProductId());
                        Logger.Log("purchase date is: " + info.getPurchaseDate());
                        Logger.Log("subscription next billing date is: " + info.getExpireDate());
                        Logger.Log("is subscribed? " + info.isSubscribed().ToString());
                        Logger.Log("is expired? " + info.isExpired().ToString());
                        Logger.Log("is cancelled? " + info.isCancelled());
                        Logger.Log("product is in free trial peroid? " + info.isFreeTrial());
                        Logger.Log("product is auto renewing? " + info.isAutoRenewing());
                        Logger.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                        Logger.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                        Logger.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                        Logger.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                        Logger.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                    }
                    else
                    {
                        Logger.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                    }
                }
                else
                {
                    Logger.Log("the product is not a subscription product");
                }
            }
            else
            {
                Logger.Log("the product should have a valid receipt");
            }
        }
    }
}