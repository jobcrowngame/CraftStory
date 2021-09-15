using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタングループのサブ
/// </summary>
public class MyToggle : Toggle
{
    Image On { get => CommonFunction.FindChiledByName<Image>(transform, "Checkmark"); }
    Image Off { get => CommonFunction.FindChiledByName<Image>(transform, "Background"); }

    /// <summary>
    /// インデックス
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// ボタンサブ親
    /// </summary>
    public MyToggleGroupCtl toggleBtns { get; set; }

    protected override void Awake()
    {
        if (group != null)
        {
            toggleBtns = group.GetComponent<MyToggleGroupCtl>();

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

    /// <summary>
    /// ボタン画像設定
    /// </summary>
    /// <param name="on"></param>
    /// <param name="off"></param>
    public void SetToggleImage(string on, string off)
    {
        On.sprite = ResourcesMng.E.ReadResources<Sprite>(on);
        Off.sprite = ResourcesMng.E.ReadResources<Sprite>(off);
    }
}
