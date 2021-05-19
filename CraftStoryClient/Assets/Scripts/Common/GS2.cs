using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Gs2.Core;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Util;
using Gs2.Unity.Gs2Inventory.Model;
using Gs2.Unity.Gs2Money.Model;
using Gs2.Unity.Gs2Exchange.Model;
using Gs2.Unity.Gs2Showcase.Model;
using Gs2.Core.Exception;
using Gs2.Unity;

public class GS2 : MonoBehaviour
{

    #region Common

    private static GS2 entity;

    private Client gs2;
    private Profile profile;
    private GameSession gameSession;

    public static GS2 E 
    { 
        get 
        {
            if (entity == null)
                entity = UICtl.E.CreateGlobalObject<GS2>();

            return entity;
        }
    }

    private void OnError(Exception e)
    {
        Debug.LogError(e.ToString());
    }

    public IEnumerator InitCoroutine()
    {
        Debug.Log("初期化 GS2");

        profile = new Gs2.Unity.Util.Profile(
           clientId: PublicPar.clientId,
           clientSecret: PublicPar.clientSecret,
           reopener: new Gs2BasicReopener()
       );

        AsyncResult<object> asyncResult = null;

        var current = profile.Initialize(
            r => { asyncResult = r; }
        );

        yield return current;

        // コルーチンの実行が終了した時点で、コールバックは必ず呼ばれています
        // クライアントの初期化に失敗した場合は終了
        if (asyncResult.Error != null)
        {
            OnError(asyncResult.Error);
            yield break;
        }

        gs2 = new Gs2.Unity.Client(profile);
    }

    #endregion
    #region Account

    public void CreateAccount(Action<string, string> response)
    {
        StartCoroutine(CreateAccountIE(response));
    }
    public IEnumerator CreateAccountIE(Action<string, string> response)
    {
        // アカウントを新規作成
        Debug.Log("アカウントを新規作成");

        AsyncResult<EzCreateResult> asyncResult = null;

        var current = gs2.Account.Create(
            r => { asyncResult = r; },
            PublicPar.accountNamespaceName
        );

        yield return current;

        // コルーチンの実行が終了した時点で、コールバックは必ず呼ばれています
        // アカウントが作成できなかった場合は終了
        if (asyncResult.Error != null)
        {
            OnError(asyncResult.Error);
            yield break;
        }

        // 作成したアカウント情報を取得
        var account = asyncResult.Result.Item;

        response(account.UserId, account.Password);
    }

    public void Login(Action<GameSession> response, string userid, string pw)
    {
        StartCoroutine(LoginIE(response, userid, pw));
    }
    private IEnumerator LoginIE(Action<GameSession> response, string id, string pw)
    {
        // ログイン
        Debug.Log("ログイン");

        AsyncResult<GameSession> asyncResult = null;
        var current = profile.Login(
           authenticator: new Gs2AccountAuthenticator(
               session: profile.Gs2Session,
               accountNamespaceName: PublicPar.accountNamespaceName,
               keyId: PublicPar.accountEncryptionKeyId,
               userId: id,
               password: pw
           ),
           r => { asyncResult = r; }
       );

        yield return current;

        // コルーチンの実行が終了した時点で、コールバックは必ず呼ばれています

        // ゲームセッションオブジェクトが作成できなかった場合は終了
        if (asyncResult.Error != null)
        {
            OnError(asyncResult.Error);
            yield break;
        }

        // ログイン状態を表すゲームセッションオブジェクトを取得
        gameSession = asyncResult.Result;

        response(gameSession);
    }

    #endregion
    #region Inventory

    public void GetInventory()
    {
        StartCoroutine(GetInventoryIE());
    }
    private IEnumerator GetInventoryIE()
    {
        yield return gs2.Inventory.GetInventory(r =>
        {
            if (r.Error != null)
            {
                // エラーが発生した場合に到達
                // r.Error は発生した例外オブジェクトが格納されている
                Debug.LogError(r.Error.Message);
            }
            else
            {
                Debug.Log(r.Result.Item.InventoryId); // string インベントリ
                Debug.Log(r.Result.Item.InventoryName); // string インベントリモデル名
                Debug.Log(r.Result.Item.CurrentInventoryCapacityUsage); // integer 現在のインベントリのキャパシティ使用量
                Debug.Log(r.Result.Item.CurrentInventoryMaxCapacity); // integer 現在のインベントリの最大キャパシティ
            }
        },
        gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
        PublicPar.inventoryNamespaceName,   // カテゴリー名
        PublicPar.inventoryName   // インベントリの種類名
        );
    }

    public void GetItem(string pageToken)
    {
        StartCoroutine(GetItemIE(pageToken));
    }
    private IEnumerator GetItemIE(string itemName)
    {
        yield return gs2.Inventory.GetItem(r =>
        {
            if (r.Error != null)
            {
                // エラーが発生した場合に到達
                // r.Error は発生した例外オブジェクトが格納されている
                Debug.LogError(r.Error.Message);
            }
            else
            {
                Debug.Log(r.Result.Items); // list[ItemSet] 有効期限ごとのアイテム所持数量のリスト
            }
        },
            gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
            PublicPar.inventoryNamespaceName,   // カテゴリー名
            PublicPar.inventoryName,   //  インベントリの種類名
            itemName   // データの取得を開始する位置を指定するトークン(オプション値)
        );
    }

    public void ListItems(Action<List<EzItemSet>> response, string pageToken, long limit)
    {
        StartCoroutine(ListItemsIE(response, pageToken, limit));
    }
    private IEnumerator ListItemsIE(Action<List<EzItemSet>> response, string pageToken, long limit)
    {
        yield return gs2.Inventory.ListItems( r => 
            {
                if (r.Error != null)
                {
                    // エラーが発生した場合に到達
                    // r.Error は発生した例外オブジェクトが格納されている
                    Debug.LogError(r.Error.Message);
                }
                else
                {
                    response(r.Result.Items);
                    //Debug.Log(r.Result.Items); // list[ItemSet] 有効期限ごとのアイテム所持数量のリスト
                    //Debug.Log(r.Result.NextPageToken); // string リストの続きを取得するためのページトークン
                }
            },
            gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
            PublicPar.inventoryNamespaceName,   // カテゴリー名
            PublicPar.inventoryName   //  インベントリの種類名
            //pageToken,   // データの取得を開始する位置を指定するトークン(オプション値)
            //limit   // データの取得件数(オプション値)
        );
    }

    public void Consume(string itemMasterName, long consumeCount, string itemName)
    {
        StartCoroutine(ConsumeIE(itemMasterName, consumeCount, itemName));
    }
    private IEnumerator ConsumeIE(string itemMasterName, long consumeCount, string itemName)
    {
        yield return gs2.Inventory.Consume(
            r => {
                if (r.Error != null)
                {
                    // エラーが発生した場合に到達
                    // r.Error は発生した例外オブジェクトが格納されている
                    Debug.LogError("Consume fail");
                    Debug.LogError(r.Error.Message);
                }
                else
                {
                    Debug.Log("Consume success");
                    BagLG.E.DecreaseRespones(r.Result.Items);
                    //Debug.Log(r.Result.Items); // list[ItemSet] 消費後の有効期限ごとのアイテム所持数量のリスト
                    //Debug.Log(r.Result.ItemModel.Name); // string アイテムモデルの種類名
                    //Debug.Log(r.Result.ItemModel.Metadata); // string アイテムモデルの種類のメタデータ
                    //Debug.Log(r.Result.ItemModel.StackingLimit); // long スタック可能な最大数量
                    //Debug.Log(r.Result.ItemModel.AllowMultipleStacks); // boolean スタック可能な最大数量を超えた時複数枠にアイテムを保管することを許すか
                    //Debug.Log(r.Result.ItemModel.SortValue); // integer 表示順番
                    //Debug.Log(r.Result.Inventory.InventoryId); // string インベントリ
                    //Debug.Log(r.Result.Inventory.InventoryName); // string インベントリモデル名
                    //Debug.Log(r.Result.Inventory.CurrentInventoryCapacityUsage); // integer 現在のインベントリのキャパシティ使用量
                    //Debug.Log(r.Result.Inventory.CurrentInventoryMaxCapacity); // integer 現在のインベントリの最大キャパシティ
                }
            },
            gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
            PublicPar.inventoryNamespaceName,   //  カテゴリー名
            PublicPar.inventoryName,   //  インベントリの名前
            itemMasterName,   //  アイテムマスターの名前
            consumeCount,   //  消費する量
            itemName // アイテムの名前
        );
    }

    #endregion
    #region Money

    /// <summary>
    /// 課金通貨を消費する
    /// </summary>
    public void WithdrawMoney(Action response, int slot, int count, bool paidOnly = false)
    {
        StartCoroutine(WithdrawMoneyIE(response, slot, count, paidOnly));
    }
    public IEnumerator WithdrawMoneyIE(Action response, int slot, int count, bool paidOnly = false)
    {
        yield return gs2.Money.Withdraw(
            r =>
            {
                if (r.Error != null)
                {
                    // エラーが発生した場合に到達
                    // r.Error は発生した例外オブジェクトが格納されている
                    Debug.LogError("Withdraw fail!!!");
                    Debug.LogError(r.Error.Message);
                }
                else
                {
                    response();
                    //Debug.Log(r.Result.Item.Slot); // integer スロット番号
                    //Debug.Log(r.Result.Item.Paid); // integer 有償課金通貨所持量
                    //Debug.Log(r.Result.Item.Free); // integer 無償課金通貨所持量
                    //Debug.Log(r.Result.Item.UpdatedAt); // long 最終更新日時
                    //Debug.Log(r.Result.Price); // float 消費した通貨の価格
                }
            },
            gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
            PublicPar.moneyNS,   //  ネームスペースの名前
            slot,   //  スロット番号
            count,   //  消費する課金通貨の数量
            paidOnly   //  有償課金通貨のみを対象とするか
        );
    }

    /// <summary>
    /// 課金通貨を在庫を取得
    /// </summary>
    public void GetMoney(Action<EzWallet> response, int slot)
    {
        StartCoroutine(GetMoneyIE(response, slot));
    }
    public IEnumerator GetMoneyIE(Action<EzWallet> response, int slot)
    {
        yield return gs2.Money.Get(
            r =>
            {
                if (r.Error != null)
                {
                    // エラーが発生した場合に到達
                    // r.Error は発生した例外オブジェクトが格納されている
                    Debug.LogError("Withdraw fail!!!");
                    Debug.LogError(r.Error.Message);
                }
                else
                {
                    response(r.Result.Item);
                    //Debug.Log(r.Result.Item.Slot); // integer スロット番号
                    //Debug.Log(r.Result.Item.Paid); // integer 有償課金通貨所持量
                    //Debug.Log(r.Result.Item.Free); // integer 無償課金通貨所持量
                    //Debug.Log(r.Result.Item.UpdatedAt); // long 最終更新日時
                    //Debug.Log(r.Result.Price); // float 消費した通貨の価格
                }
            },
            gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
            PublicPar.moneyNS,   //  ネームスペースの名前
            slot   //  スロット番号
        );
    }

    #endregion
    #region Exchange

    /// <summary>
    /// 交換
    /// </summary>
    public void Exchange(Action<EzRateModel> response, int count, string rateName, List<Gs2.Unity.Gs2Exchange.Model.EzConfig> config = null)
    {
        StartCoroutine(ExchangeIE(response, count, rateName, config));
    }
    private IEnumerator ExchangeIE(Action<EzRateModel> response, int count, string rateName, List<Gs2.Unity.Gs2Exchange.Model.EzConfig> config = null)
    {
        yield return gs2.Exchange.Exchange(
             r =>
             {
                 if (r.Error != null)
                 {
                     // エラーが発生した場合に到達
                     // r.Error は発生した例外オブジェクトが格納されている
                     Debug.LogError("Exchange fail!!!");
                     Debug.LogError(r.Error.Message);
                 }
                 else
                 {
                     StartCoroutine(StampSheet((sheet, result) =>
                     {
                         Debug.Log("Exchange success");
                         response(r.Result.Item);
                     }, r.Result.StampSheet));
                     //Debug.Log(r.Result.Item.Name); // string 交換レートの種類名
                     //Debug.Log(r.Result.Item.Metadata); // string 交換レートの種類のメタデータ
                     //Debug.Log(r.Result.Item.ConsumeActions); // list[ConsumeAction] 消費アクションリスト
                     //Debug.Log(r.Result.Item.AcquireActions); // list[AcquireAction] 入手アクションリスト
                     //Debug.Log(r.Result.StampSheet); // string 交換処理の実行に使用するスタンプシート
                 }
             },
             gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
             PublicPar.exchangeNS,   //  ネームスペース名
             rateName,   //  交換レートの種類名
             count,   //  交換するロット数
             config   //  設定値(オプション値)
         );
    }

    #endregion
    #region Showcase

    public void Buy(Action<EzSalesItem> response, string showcaseName, EzDisplayItem displayItem, List<Gs2.Unity.Gs2Showcase.Model.EzConfig> config = null)
    {
        StartCoroutine(BuyIE(response, showcaseName, displayItem, config));
    }
    private IEnumerator BuyIE(Action<EzSalesItem> response, string showcaseName, EzDisplayItem displayItem, List<Gs2.Unity.Gs2Showcase.Model.EzConfig> config = null)
    {
        yield return gs2.Showcase.Buy(
             r =>
             {
                 if (r.Error != null)
                 {
                     // エラーが発生した場合に到達
                     // r.Error は発生した例外オブジェクトが格納されている
                     Debug.LogErrorFormat("BuyIE fail!!!    showcaseName:{0}    displayItem:{1}", showcaseName, displayItem.SalesItem.Name);
                     Debug.LogError(r.Error.Message);
                 }
                 else
                 {
                     StartCoroutine(StampSheet((sheet, result) =>
                     {
                         Debug.Log("Buy success");
                         response(r.Result.Item);
                     }, r.Result.StampSheet));
                     //Debug.Log(r.Result.Item.Name); // string 商品名
                     //Debug.Log(r.Result.Item.Metadata); // string 商品のメタデータ
                     //Debug.Log(r.Result.Item.ConsumeActions); // list[ConsumeAction] 消費アクションリスト
                     //Debug.Log(r.Result.Item.AcquireActions); // list[AcquireAction] 入手アクションリスト
                     //Debug.Log(r.Result.StampSheet); // string 購入処理の実行に使用するスタンプシート
                 }
             },
             gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
             PublicPar.ShowcaseNS,   //  ネームスペース名
             showcaseName,   //  商品名
             displayItem.DisplayItemId,   //  陳列商品ID
             config   //  設定値(オプション値)
         );
    }

    public void GetShowcase(Action<EzShowcase> response, string showcaseName)
    {
        StartCoroutine(GetShowcaseIE(response, showcaseName));
    }
    private IEnumerator GetShowcaseIE(Action<EzShowcase> response, string showcaseName)
    {
        yield return gs2.Showcase.GetShowcase(
             r =>
             {
                 if (r.Error != null)
                 {
                     // エラーが発生した場合に到達
                     // r.Error は発生した例外オブジェクトが格納されている
                     Debug.LogError("GetShowcaseIE fail!!!");
                     Debug.LogError(r.Error.Message);
                 }
                 else
                 {
                     Debug.Log("GetShowcase success");
                     response(r.Result.Item);
                     //Debug.Log(r.Result.Item.Name); // string 商品名
                     //Debug.Log(r.Result.Item.Metadata); // string 商品のメタデータ
                     //Debug.Log(r.Result.Item.ConsumeActions); // list[ConsumeAction] 消費アクションリスト
                     //Debug.Log(r.Result.Item.AcquireActions); // list[AcquireAction] 入手アクションリスト
                     //Debug.Log(r.Result.StampSheet); // string 購入処理の実行に使用するスタンプシート
                 }
             },
             gameSession,    // GameSession ログイン状態を表すセッションオブジェクト
             PublicPar.ShowcaseNS,   //  ネームスペース名
             showcaseName   //  商品名
         );
    }

    #endregion




    #region StampSheet

    //UnityEvent<Gs2Exception>型を使用するための準備
    [System.Serializable]
    public class OnErrorCallback : UnityEvent<Gs2Exception> { }

    private IEnumerator StampSheet(UnityAction<EzStampSheet, Gs2.Unity.Gs2Distributor.Result.EzRunStampSheetResult> responce, string stampSheet)
    {
        var machine = new StampSheetStateMachine(stampSheet, gs2, PublicPar.DistributorNS, PublicPar.accountEncryptionKeyId);
        UnityEvent<Gs2Exception> m_events = new OnErrorCallback();
        m_events.AddListener(OnError);
        machine.OnCompleteStampSheet.AddListener(responce);
        yield return machine.Execute(m_events);
    }

    #endregion
}
