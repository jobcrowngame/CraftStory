using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvent : MonoBehaviour
{
    public void Wait()
    {
        Logger.Log("PlayerAnimatorEvent");
        StartCoroutine(Over());
    }

    IEnumerator Over()
    {
        yield return new WaitForSeconds(0.05f);
        PlayerCtl.E.PlayerEntity.Behavior.Type = PlayerBehaviorType.Waiting;
        PlayerCtl.E.Lock = false;
        Logger.Log("Un Lock");
    }
}
