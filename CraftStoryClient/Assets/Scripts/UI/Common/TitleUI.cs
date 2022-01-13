using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 共通タイトル
/// </summary>
public class TitleUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    Text Title { get => FindChiled<Text>("TitleText"); }

    Action action;

    private void Awake()
    {
        CloseBtn.onClick.AddListener(()=> 
        {
            if (action != null)
                action();
        });
    }

    /// <summary>
    /// タイトル名をセット
    /// </summary>
    /// <param name="title">タイトル</param>
    public void SetTitle(string title)
    {
        Title.text = title;
    }

    /// <summary>
    /// Closeボタンをクリックする場合のイベント
    /// </summary>
    /// <param name="action">イベント</param>
    public void SetOnClose(Action action)
    {
        this.action = action;
    }
}
