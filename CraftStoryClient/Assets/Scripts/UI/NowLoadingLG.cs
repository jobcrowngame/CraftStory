using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using JsonConfigData;

public class NowLoadingLG : UILogicBase<NowLoadingLG, NowLoadingUI>
{
    private AsyncOperation async;

    public string NextSceneName { get; set; }
    public int NextMapID { get; set; }

    public int BeforTransferGateID { get; set; }

    public IEnumerator LoadData()
    {
        // シーンの読み込みをする
        async = SceneManager.LoadSceneAsync(NextSceneName);
        async.allowSceneActivation = false;

        float curPercent = 0;

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (!async.isDone)
        {
            curPercent = async.progress > UI.Percent ? UI.Percent : async.progress;

            //Debug.LogWarning(curPercent);

            var progressVal = Mathf.Clamp01(curPercent / 0.9f);
            UI.SetSlider(progressVal);

            if (curPercent >= 0.9f)
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}