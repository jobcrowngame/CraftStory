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
        // �V�[���̓ǂݍ��݂�����
        async = SceneManager.LoadSceneAsync(DataMng.E.MapData.NextSceneName);

        //�@�ǂݍ��݂��I���܂Ői���󋵂��X���C�_�[�̒l�ɔ��f������
        while (!async.isDone)
        {
            var progressVal = Mathf.Clamp01(async.progress / 0.9f);
            slider.value = progressVal;
            yield return null;
        }
    }
}
