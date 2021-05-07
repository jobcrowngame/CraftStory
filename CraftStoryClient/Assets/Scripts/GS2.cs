using System;
using System.Collections;
using System.Collections.Generic;
using Gs2.Core;
using Gs2.Unity.Gs2Account.Model;
using Gs2.Unity.Gs2Account.Result;
using Gs2.Unity.Util;
using UnityEngine;

public class GS2 : MonoBehaviour
{
    private static GS2 entity;

    private Gs2.Unity.Client gs2;
    private Profile profile;

    public static GS2 E 
    { 
        get 
        {
            if (entity == null)
                entity = UICtl.CreateGlobalObject<GS2>();

            return entity;
        }
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

    private IEnumerator CreateAccountCoroutine(Action<string, string> response)
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

    private IEnumerator LoginCoroutine(Action<GameSession> response, string id, string pw)
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
        var session = asyncResult.Result;

        response(session);
    }

    private void OnError(Exception e)
    {
        Debug.Log(e.ToString());
    }

    public void CreateAccount(Action<string, string> response)
    {
        StartCoroutine(CreateAccountCoroutine(response));
    }

    public void Login(Action<GameSession> response, string userid, string pw)
    {
        StartCoroutine(LoginCoroutine(response, userid, pw));
    }
}
