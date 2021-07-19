using System;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : Button
{
    public int Index { get; set; }
    public bool IsClicking { get => isClicking; }
    bool isClicking;
    Action<int> onClickEvent;
    Action onClickingEvent;

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

    public void AddClickListener(Action<int> ac)
    {
        onClickEvent = ac;
    }
    public void AddClickingListener(Action ac)
    {
        onClickingEvent = ac;
    }

    public void OnClicking()
    {
        isClicking = true;
    }
    public void EndClicking()
    {
        isClicking = false;
    }

    public void IsSelected(bool b)
    {
        GetComponent<Image>().color = b ? Color.white : Color.grey;
    }

}