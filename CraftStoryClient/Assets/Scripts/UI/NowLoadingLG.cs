using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NowLoadingLG : UILogicBase<NowLoadingLG, NowLoadingUI>
{
    private AsyncOperation async;

    public IEnumerator LoadData()
    {
        // シーンの読み込みをする
        async = SceneManager.LoadSceneAsync(DataMng.E.MapData.NextSceneName);

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            UI.SetSlider(progressVal);
            yield return null;
        }
    }
}