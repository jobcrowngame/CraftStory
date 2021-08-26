using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// ボタングループ
/// </summary>
public class ToggleBtns : UIBase
{
    /// <summary>
    /// サブリスト
    /// </summary>
    MyToggle[] cells;

    /// <summary>
    /// クリックイベント
    /// </summary>
    UnityAction<int> callback;

    /// <summary>
    /// 初期化
    /// </summary>
    public override void Init()
    {
        base.Init();

        cells = new MyToggle[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            cells[i] = transform.GetChild(i).GetComponent<MyToggle>();
            cells[i].Index = i;
        }
        cells[0].isOn = true;
    }

    /// <summary>
    /// 選択が変更される場合のイベント
    /// </summary>
    /// <param name="index"></param>
    public void OnValueChange(int index)
    {
        if (callback != null)
        {
            callback(index);
        }
    }

    /// <summary>
    /// 選択が変更されるイベントの設定
    /// </summary>
    /// <param name="callback">イベント</param>
    public void OnValueChangeAddListener(UnityAction<int> callback)
    {
        this.callback = callback;
    }

    public void SetBtnText(int index, string msg)
    {
        if (cells != null && cells.Length > index)
        {
            CommonFunction.FindChiledByName<Text>(cells[index].transform, "Text").text = msg;
        }
    }
    public void SetValue(int index)
    {
        if (cells.Length > index)
        {
            cells[index].isOn = true;
        }
    }
}
