using UnityEngine.UI;

public class NowLoadingUI : UIBase
{
    Text text { get => FindChiled<Text>("Text"); }
    Slider slider { get => FindChiled<Slider>("Slider"); }

    float timer = 0;
    public float Percent { get => timer / 3; }

    private void Start()
    {
        base.Init();
        NowLoadingLG.E.Init(this);

        timer = 0;

        text.text = "Now Loading...";
        slider.value = 0;

        StartCoroutine(NowLoadingLG.E.LoadData());
    }

    private void FixedUpdate()
    {
        timer += 0.02f;
    }

    public void SetSlider(float v)
    {
        slider.value = v;
    }
}
