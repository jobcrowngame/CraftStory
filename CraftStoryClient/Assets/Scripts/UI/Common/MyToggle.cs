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
}
