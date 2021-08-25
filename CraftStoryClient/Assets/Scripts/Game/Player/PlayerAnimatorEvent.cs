using System.Collections;
using UnityEngine;

/// <summary>
/// プレイヤーアニメのイベント
/// </summary>
public class PlayerAnimatorEvent : MonoBehaviour
{
    // アニメションから呼ぶメソッド
    public void Wait()
    {
        Logger.Log("PlayerAnimatorEvent");
        StartCoroutine(Over());
    }

    /// <summary>
    /// Wait　アニメション終了後のロジック
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
