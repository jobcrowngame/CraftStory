using UnityEngine;
using UnityEngine.UI;

public partial class HomeUI
{
    Transform battle { get => FindChiled("Battle"); }
    Image hpBar { get => FindChiled<Image>("HpImg", battle); }
    Text hpText { get => FindChiled<Text>("HpText", battle); }

    public void OnHpChange(float percent)
    {
        hpBar.fillAmount = percent;
        hpText.text = (percent * 100).ToString("F1") + "%";
    }
}