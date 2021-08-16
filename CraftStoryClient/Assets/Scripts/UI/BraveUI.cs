using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BraveUI : UIBase
{
    Text SceneName;
    Image FadeinImg;
    Button MenuBtn;
    Button Jump { get => FindChiled<Button>("Jump"); }
    MyButton PlussBtn { get => FindChiled<MyButton>("PlussBtn"); }
    MyButton MinusBtn { get => FindChiled<MyButton>("MinusBtn"); }

    Transform ItemMsgParent { get => FindChiled("Content"); }


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

        BraveLG.E.Init(this);

        SceneName = FindChiled<Text>("SceneName");

        FadeinImg = FindChiled<Image>("Fadein");

        MenuBtn = FindChiled<Button>("MenuBtn");
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

        SceneName.text = DataMng.E.MapData.Config.Name;

        StartCoroutine(FadeIn());
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

    public void AddItem(BraveLG.BraveCellItem item)
    {
        var cell = AddCell<BraveCell>("Prefabs/UI/BraveCell", ItemMsgParent);
        if (cell != null)
        {
            cell.Set(item);
        }
    }
}