using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using UnityEngine.SceneManagement;

public class NowLoadingUI : UIBase
{
    Text text;
    Slider slider;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        base.Init();

        NowLoadingLG.E.Init(this);

        text = FindChiled<Text>("Text");
        text.text = "Now Loading...";

        slider = FindChiled<Slider>("Slider");
        slider.value = 0;

        StartCoroutine("LoadData");
    }

    private AsyncOperation async;

    IEnumerator LoadData()
    {
        // シーンの読み込みをする
        async = SceneManager.LoadSceneAsync(DataMng.E.MapData.NextSceneName);

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressVal;
            yield return null;
        }
    }
}
