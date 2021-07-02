using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    }
}
