using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MarketUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    Image FadeinImg { get => FindChiled<Image>("Fadein"); }
    Button MenuBtn { get => FindChiled<Button>("MenuBtn"); }
    Button Jump { get => FindChiled<Button>("Jump"); }
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();
        WorldMng.E.GameTimeCtl.Active = false;

        Init();
    }

    public override void Init()
    {
        base.Init();

        MarketLG.E.Init(this);

        Title.Init();
        Title.RefreshCoins();
        Title.ShowCoin(1);
        Title.ShowCoin(2);
        Title.ShowCoin(3);

        MenuBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<MenuUI>(UIType.Menu);
        });

        PlussBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(1); });
        MinusBtn.AddClickingListener(() => { PlayerCtl.E.CameraCtl.ChangeCameraPos(-1); });
        Jump.onClick.AddListener(PlayerCtl.E.Jump);

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        //�@Color�̃A���t�@��0.1�������Ă���
        for (var i = 1f; i > 0; i -= 0.1f)
        {
            FadeinImg.color = new Color(0f, 0f, 0f, i);
            //�@�w��b���҂�
            yield return new WaitForSeconds(fadeInTime);
        }

        FadeinImg.gameObject.SetActive(false);
    }
}