using UnityEngine.UI;

public class NowLoadingUI : UIBase
{
    Text text { get => FindChiled<Text>("Text"); }
    Slider slider { get => FindChiled<Slider>("Slider"); }

    private void Start()
    {
        base.Init();
        NowLoadingLG.E.Init(this);

        text.text = "Now Loading...";
        slider.value = 0;

        StartCoroutine(NowLoadingLG.E.LoadData());
    }

    public void SetSlider(float v)
    {
        slider.value = v;
    }
}
