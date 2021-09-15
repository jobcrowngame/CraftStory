using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタングループのサブ
/// </summary>
public class MyToggle : Toggle
{
    /// <summary>
    /// インデックス
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// ボタンサブ親
    /// </summary>
    public ToggleBtns toggleBtns { get; set; }

    protected override void Awake()
    {
        if (group != null)
        {
            toggleBtns = group.GetComponent<ToggleBtns>();

            onValueChanged.AddListener((b) => 
            {
                if (b)
                    toggleBtns.OnValueChange(Index);
            });
        }
    }

    /// <summary>
    /// インアクティブ
    /// </summary>
    public void Enable()
    {
        interactable = false;
        CommonFunction.FindChiledByName<Image>(transform, "Background").color = Color.grey;
    }
}
