using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// スクロール追加機能
/// </summary>
public class MyScrollRect : ScrollRect
{
    ScrollRect scrollRect { get => GetComponent<ScrollRect>(); }
    Transform Content { get => scrollRect.content; }

    /// <summary>
    /// スクロールしてるかのタグ
    /// </summary>
    bool isDraging = false;

    /// <summary>
    /// 目標Pos決めてるかのタグ
    /// </summary>
    bool targetPosGeted = false;

    /// <summary>
    /// 全部完了のタグ
    /// </summary>
    bool isEnd = true;

    /// <summary>
    /// 今のPos
    /// </summary>
    float curHorizontalNormalizedPosition;

    /// <summary>
    /// 目標Pos
    /// </summary>
    float targetHorizontalNormalizedPosition;

    /// <summary>
    /// 自動スクロール時間
    /// </summary>
    float ScrollTime = 0.5f;

    private void Update()
    {
        if (scrollRect != null && !isEnd)
        {
            if (!isDraging && Mathf.Abs(scrollRect.velocity.x) < 300 && !targetPosGeted)
            {
                CheckTargetPos();
            }

            if (targetPosGeted)
            {
                // 自動遷移スピードを計算
                float step = Mathf.Clamp(Time.deltaTime, 0, ScrollTime);

                // 自動遷移
                if(Mathf.Abs(targetHorizontalNormalizedPosition - curHorizontalNormalizedPosition) > step)
                {
                    if (curHorizontalNormalizedPosition > targetHorizontalNormalizedPosition)
                    {
                        curHorizontalNormalizedPosition -= step;
                    }
                    else
                    {
                        curHorizontalNormalizedPosition += step;
                    }

                    scrollRect.horizontalNormalizedPosition = curHorizontalNormalizedPosition;
                }
                else
                {
                    curHorizontalNormalizedPosition = targetHorizontalNormalizedPosition;
                    scrollRect.horizontalNormalizedPosition = targetHorizontalNormalizedPosition;
                    isEnd = true;
                }
            }
        }
    }

    /// <summary>
    /// スクロール開始
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isDraging = true;
        targetPosGeted = false;
        isEnd = false;
    }

    /// <summary>
    /// スクロール終了
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        isDraging = false;
    }

    /// <summary>
    /// ターゲットPosを決める
    /// </summary>
    private void CheckTargetPos()
    {
        // ターゲットインデックス
        int targetIndex = 0;

        // 毎インデックスのステップ
        float step = 1f / (Content.childCount - 1);

        // 現在のPos記録
        curHorizontalNormalizedPosition = scrollRect.horizontalNormalizedPosition;

        // 現在のIndexを計算
        int index = (int)(curHorizontalNormalizedPosition / step);

        // 現在のPos偏差を記録
        float nowOffset = curHorizontalNormalizedPosition % step;

        // 偏差がstep半分より小さい場合、前のページへ自動遷移
        if (nowOffset < step / 2)
        {
            targetIndex = index;
        }
        // 偏差がstep半分より大きい場合、後のページへ自動遷移
        else
        {
            targetIndex = index + 1;
        }

        targetHorizontalNormalizedPosition = targetIndex * step;
        targetPosGeted = true;
    }
}