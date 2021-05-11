using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Util;
using Gs2.Unity.Gs2Inventory.Model;

using UnityEngine;

public class GS2 : MonoBehaviour
{
    private static GS2 entity;

    private Gs2.Unity.Client gs2;
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
        Debug.Log(e.ToString());
    }

    public void Init()
    {
        profile = new Gs2.Unity.Util.Profile(
           clientId: PublicPar.clientId,
           clientSecret: PublicPar.clientSecret,
           reopener: new Gs2BasicReopener()
       );

        StartCoroutine(InitCoroutine());
    }
    private IEnumerator InitCoroutine()
    {
        // GS2 SDK のクライアントを初期化
        Debug.Log("GS2 SDK のクライアントを初期化");

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
                MLog.Error(r.Error.Message);
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
                MLog.Error(r.Error.Message);
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
                    MLog.Error(r.Error.Message);
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

    public void Consume(ItemCell cell, string itemName, long consumeCount)
    {
        StartCoroutine(ConsumeIE(cell, itemName, consumeCount));
    }
    private IEnumerator ConsumeIE(ItemCell cell, string itemName, long consumeCount)
    {
        yield return gs2.Inventory.Consume(
            r => {
                if (r.Error != null)
                {
                    // エラーが発生した場合に到達
                    // r.Error は発生した例外オブジェクトが格納されている
                }
                else
                {
                    cell.DecreaseRespones(r.Result.Items);
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
            itemName,   //  アイテムマスターの名前
            consumeCount   //  消費する量
        );
    }
}
