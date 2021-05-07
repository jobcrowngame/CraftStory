using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Util;
using UnityEngine;

public class Gs2AccountSample2 : MonoBehaviour
{
    // GS2-Identifier で発行したクライアントID
    public string clientId;

    // GS2-Identifier で発行したクライアントシークレット
    public string clientSecret;

    // アカウントを作成する GS2-Account のネームスペース名
    public string accountNamespaceName;

    // アカウントの認証結果に付与する署名を計算するのに使用する暗号鍵
    public string accountEncryptionKeyId;

    void Start()
    {
        StartCoroutine(CreateAndLoginAction());
    }

    public IEnumerator CreateAndLoginAction()
    {
        // GS2 SDK のクライアントを初期化
        Debug.Log("GS2 SDK のクライアントを初期化");

        var profile = new Gs2.Unity.Util.Profile(
            clientId: clientId,
            clientSecret: clientSecret,
            reopener: new Gs2BasicReopener()
        );

        {
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
        }

        var gs2 = new Gs2.Unity.Client(profile);

        // アカウントを新規作成
        Debug.Log("アカウントを新規作成");

        EzAccount account = null;
        {
            AsyncResult<EzCreateResult> asyncResult = null;

            var current = gs2.Account.Create(
                r => { asyncResult = r; },
                accountNamespaceName
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
            account = asyncResult.Result.Item;
        }

        // ログイン
        Debug.Log("ログイン");

        GameSession session = null;
        {
            AsyncResult<GameSession> asyncResult = null;
            var current = profile.Login(
               authenticator: new Gs2AccountAuthenticator(
                   session: profile.Gs2Session,
                   accountNamespaceName: accountNamespaceName,
                   keyId: accountEncryptionKeyId,
                   userId: account.UserId,
                   password: account.Password
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
            session = asyncResult.Result;
        }

        // 引き継ぎ情報の一覧を取得

        Debug.Log("引き継ぎ情報の一覧を取得");
        {
            AsyncResult<EzListTakeOverSettingsResult> asyncResult = null;

            var current = gs2.Account.ListTakeOverSettings(
                r => { asyncResult = r; },
                session,
                accountNamespaceName
            );

            yield return current;

            // コルーチンの実行が終了した時点で、コールバックは必ず呼ばれています

            // APIの呼び出しが完了したら通知されるコールバック
            if (asyncResult.Error != null)
            {
                OnError(asyncResult.Error);
                yield break;
            }

            List<EzTakeOver> items = asyncResult.Result.Items;
            foreach (var item in items)
            {
                // 引き継ぎに関する情報が取得される
            }
        }

        // GS2 SDK の終了処理

        Debug.Log("GS2 SDK の終了処理");
        {
            // ゲームを終了するときなどに呼び出してください。
            // 頻繁に呼び出すことは想定していません。
            var current = profile.Finalize();

            yield return current;
        }
    }

    private void OnError(Exception e)
    {
        Debug.Log(e.ToString());
    }
}