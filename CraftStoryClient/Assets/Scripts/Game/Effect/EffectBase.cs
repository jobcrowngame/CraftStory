using System;
using UnityEngine;

/// <summary>
/// エフェクトベース
/// </summary>
public class EffectBase : MonoBehaviour
{
    private float deleteTime = 1f; // 遅延削除時間（S）
    private bool active = false; // アクティブ化

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

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="dTime">遅延削除時間</param>
    public void Init(float dTime = 1f)
    {
        deleteTime = dTime;
        active = true;
    }
}