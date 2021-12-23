using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 特別用ボタン
/// </summary>
public class MyButton : Button
{
    /// <summary>
    /// ボタンインデックス
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// 長い時間クリックかのチェック
    /// </summary>
    public bool IsClicking { get => isClicking; }
    bool isClicking;

    /// <summary>
    /// アイコン
    /// </summary>
    Image Icon { get => GetComponent<Image>(); }

    /// <summary>
    /// クリック場合のイベント
    /// </summary>
    Action<int> onClickEvent;

    /// <summary>
    /// 長い時間クリック場合のイベント
    /// </summary>
    Action onClickingEvent;

    Action onPointDown;

    protected override void Start()
    {
        onClick.AddListener(() =>
        {
            if (onClickEvent != null)
            {
                onClickEvent(Index);
            }
        });
    }

    void Update()
    {
        if (isClicking && onClickingEvent != null)
        {
            onClickingEvent();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (onPointDown != null)
            onPointDown();
    }

    /// <summary>
    /// クリックイベント追加
    /// </summary>
    /// <param name="ac"></param>
    public void AddClickListener(Action<int> ac)
    {
        onClickEvent = ac;
    }

    /// <summary>
    /// 長い時間クリックイベント追加
    /// </summary>
    /// <param name="ac"></param>
    public void AddClickingListener(Action ac)
    {
        onClickingEvent = ac;
    }
    public void OnPointerDown(Action ac)
    {
        onPointDown = ac;
    }

    public void OnClicking()
    {
        isClicking = true;
    }
    public void EndClicking()
    {
        isClicking = false;
    }

    /// <summary>
    /// ボタングループで使う場合、選択してないボタンはグレイにする
    /// </summary>
    /// <param name="b"></param>
    public void ColorChange(bool b)
    {
        GetComponent<Image>().color = b ? Color.white : Color.grey;
    }

    public void Enable(bool b)
    {
        ColorChange(b);
        this.enabled = b;
    }

    /// <summary>
    /// アイコン設定
    /// </summary>
    /// <param name="path">パス</param>
    public void SetIcon(string path)
    {
        Icon.sprite = UICtl.ReadResources<Sprite>(path);
    }
}