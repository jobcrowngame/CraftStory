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
    // GS2-Identifier �Ŕ��s�����N���C�A���gID
    public string clientId;

    // GS2-Identifier �Ŕ��s�����N���C�A���g�V�[�N���b�g
    public string clientSecret;

    // �A�J�E���g���쐬���� GS2-Account �̃l�[���X�y�[�X��
    public string accountNamespaceName;

    // �A�J�E���g�̔F�،��ʂɕt�^���鏐�����v�Z����̂Ɏg�p����Í���
    public string accountEncryptionKeyId;

    void Start()
    {
        StartCoroutine(CreateAndLoginAction());
    }

    public IEnumerator CreateAndLoginAction()
    {
        // GS2 SDK �̃N���C�A���g��������
        Debug.Log("GS2 SDK �̃N���C�A���g��������");

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

            // �R���[�`���̎��s���I���������_�ŁA�R�[���o�b�N�͕K���Ă΂�Ă��܂�

            // �N���C�A���g�̏������Ɏ��s�����ꍇ�͏I��
            if (asyncResult.Error != null)
            {
                OnError(asyncResult.Error);
                yield break;
            }
        }

        var gs2 = new Gs2.Unity.Client(profile);

        // �A�J�E���g��V�K�쐬
        Debug.Log("�A�J�E���g��V�K�쐬");

        EzAccount account = null;
        {
            AsyncResult<EzCreateResult> asyncResult = null;

            var current = gs2.Account.Create(
                r => { asyncResult = r; },
                accountNamespaceName
            );

            yield return current;

            // �R���[�`���̎��s���I���������_�ŁA�R�[���o�b�N�͕K���Ă΂�Ă��܂�

            // �A�J�E���g���쐬�ł��Ȃ������ꍇ�͏I��
            if (asyncResult.Error != null)
            {
                OnError(asyncResult.Error);
                yield break;
            }

            // �쐬�����A�J�E���g�����擾
            account = asyncResult.Result.Item;
        }

        // ���O�C��
        Debug.Log("���O�C��");

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

            // �R���[�`���̎��s���I���������_�ŁA�R�[���o�b�N�͕K���Ă΂�Ă��܂�

            // �Q�[���Z�b�V�����I�u�W�F�N�g���쐬�ł��Ȃ������ꍇ�͏I��
            if (asyncResult.Error != null)
            {
                OnError(asyncResult.Error);
                yield break;
            }

            // ���O�C����Ԃ�\���Q�[���Z�b�V�����I�u�W�F�N�g���擾
            session = asyncResult.Result;
        }

        // �����p�����̈ꗗ���擾

        Debug.Log("�����p�����̈ꗗ���擾");
        {
            AsyncResult<EzListTakeOverSettingsResult> asyncResult = null;

            var current = gs2.Account.ListTakeOverSettings(
                r => { asyncResult = r; },
                session,
                accountNamespaceName
            );

            yield return current;

            // �R���[�`���̎��s���I���������_�ŁA�R�[���o�b�N�͕K���Ă΂�Ă��܂�

            // API�̌Ăяo��������������ʒm�����R�[���o�b�N
            if (asyncResult.Error != null)
            {
                OnError(asyncResult.Error);
                yield break;
            }

            List<EzTakeOver> items = asyncResult.Result.Items;
            foreach (var item in items)
            {
                // �����p���Ɋւ����񂪎擾�����
            }
        }

        // GS2 SDK �̏I������

        Debug.Log("GS2 SDK �̏I������");
        {
            // �Q�[�����I������Ƃ��ȂǂɌĂяo���Ă��������B
            // �p�ɂɌĂяo�����Ƃ͑z�肵�Ă��܂���B
            var current = profile.Finalize();

            yield return current;
        }
    }

    private void OnError(Exception e)
    {
        Debug.Log(e.ToString());
    }
}