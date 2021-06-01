using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Text SceneName;
    Image FadeinImg;

    Button BagBtn;
    Button LotteryBtn;
    Button ShopBtn;

    Transform btnsParent;

    Transform BuilderPencil;
    Button BuilderBtn;
    Button CancelBtn;

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();
        UICtl.E.AddUI(this, UIType.Home);

        Init();
    }

    public override void Init()
    {
        base.Init();

        HomeLG.E.Init(this);

        InitUI();

        SceneName.text = ConfigMng.E.Map[DataMng.E.CurrentSceneID].Name;

        StartCoroutine("FadeIn");
    }

    private void InitUI()
    {
        SceneName = FindChiled<Text>("SceneName");

        FadeinImg = FindChiled<Image>("Fadein");

        BagBtn = FindChiled<Button>("BagBtn");
        BagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        LotteryBtn = FindChiled<Button>("LotteryBtn");
        LotteryBtn.onClick.AddListener(() => { UICtl.E.OpenUI<LotteryUI>(UIType.Lottery); });

        ShopBtn = FindChiled<Button>("ShopBtn");
        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });

        btnsParent = FindChiled("Grid");
        AddItemBtns();

        BuilderPencil = FindChiled("BuilderPencil");
        BuilderBtn = FindChiled<Button>("BuilderBtn", BuilderPencil.gameObject);
        BuilderBtn.onClick.AddListener(BuilderBlueprint);
        CancelBtn = FindChiled<Button>("CancelBtn", BuilderPencil.gameObject);
        CancelBtn.onClick.AddListener(CancelBuilderBlueprint);

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();
    }

    private void AddItemBtns()
    {
        AddItemBtn(101);
        AddItemBtn(102);
        AddItemBtn(103);
        AddItemBtn(104);
        AddItemBtn(3000);
    }

    private void AddItemBtn(int itemID)
    {
        var config = ConfigMng.E.Item[itemID];
        var cell = AddCell<ItemBtn>("Prefabs/UI/ItemBtn", btnsParent);
        if (cell == null)
            return;

        cell.Init(itemID);
    }

    private void BuilderBlueprint()
    {
        Debug.Log("BuilderBtn");

        ShowBuilderPencilBtn(false);
    }
    private void CancelBuilderBlueprint()
    {
        Debug.Log("CancelBtn");

        ShowBuilderPencilBtn(false);
    }
    public void ShowBuilderPencilBtn(bool b = true)
    {
        BuilderPencil.gameObject.SetActive(b);
        if (!b ) PlayerCtl.E.BuilderPencil.Cancel();
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
