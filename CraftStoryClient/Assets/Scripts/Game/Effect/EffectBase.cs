using System;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    private float deleteTime = 1f;
    private bool active = false;

    private void Update()
    {
        if (active)
        {
            deleteTime -= Time.deltaTime;

            if (deleteTime < 0)
            {
                active = false;
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void Init(float dTime = 1f)
    {
        deleteTime = dTime;
        active = true;
    }
}