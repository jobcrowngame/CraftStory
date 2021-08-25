using System.Collections;
using UnityEngine;

/// <summary>
/// �v���C���[�A�j���̃C�x���g
/// </summary>
public class PlayerAnimatorEvent : MonoBehaviour
{
    // �A�j���V��������Ăԃ��\�b�h
    public void Wait()
    {
        Logger.Log("PlayerAnimatorEvent");
        StartCoroutine(Over());
    }

    /// <summary>
    /// Wait�@�A�j���V�����I����̃��W�b�N
    /// </summary>
    /// <returns></returns>
    IEnumerator Over()
    {
        yield return new WaitForSeconds(0.05f);
        PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
        PlayerCtl.E.Lock = false;
        Logger.Log("Un Lock");
    }
}
