using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BraveUI : UIBase
{
    Text SceneName;
    Image FadeinImg;

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();

        Init();
    }

    public override void Init()
    {
        base.Init();

        BraveLG.E.Init(this);

        InitUI();

        SceneName.text = ConfigMng.E.Map[DataMng.E.CurrentSceneID].Name;

        StartCoroutine("FadeIn");
    }

    private void InitUI()
    {
        SceneName = FindChiled<Text>("SceneName");

        FadeinImg = FindChiled<Image>("Fadein");

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();
    }

    IEnumerator FadeIn()
    {
        //　Colorのアルファを0.1ずつ下げていく
        for (var i = 1f; i > 0; i -= 0.1f)
        {
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //　指定秒数待つ
            yield return new WaitForSeconds(fadeInTime);
        }

        FadeinImg.gameObject.SetActive(false);
    }
}
