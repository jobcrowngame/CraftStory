using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShopMarketUI : UIBase
{
    Title2UI Title { get => FindChiled<Title2UI>("Title2"); }
    Image FadeinImg { get => FindChiled<Image>("Fadein"); }

    /// <summary>
    /// マップボタン
    /// </summary>
    Button MapBtn { get => FindChiled<Button>("MapBtn"); }
    Button MenuBtn { get => FindChiled<Button>("MenuBtn"); }
    Button Jump { get => FindChiled<Button>("Jump"); }
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();
        WorldMng.E.GameTimeCtl.Active = false;

        UICtl.E.AddUI(this, UIType.Market);

        Init();
    }

    public override void Init()
    {
        base.Init();

        ShopMarketLG.E.Init(this);

        Title.Init();
        Title.ShowCoin(1);
        Title.ShowCoin(2);
        Title.ShowCoin(3);

        MapBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<MapUI>(UIType.Map);
        });
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

    public override void Open()
    {
        base.Open();

        Title.RefreshCoins();
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
