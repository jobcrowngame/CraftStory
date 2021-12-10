using UnityEngine;
using UnityEngine.UI;

public class NowLoadingUI : UIBase
{
    Text text { get => FindChiled<Text>("Text"); }
    Text Tips { get => FindChiled<Text>("Tips"); }
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

        SetTips();
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

    private void SetTips()
    {
        string tips = "";

        if (NowLoadingLG.E.FixtTips < 0)
        {
            var random = Random.Range(1, ConfigMng.E.LodingTips.Count);
            tips = ConfigMng.E.LodingTips[random].Text;
        }
        else
        {
            tips = ConfigMng.E.LodingTips[NowLoadingLG.E.FixtTips].Text;
        }

        Tips.text = tips;

        NowLoadingLG.E.FixtTips = -1;
    }
}
