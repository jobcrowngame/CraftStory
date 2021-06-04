using System.Collections;
using System.Collections.Generic;
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
    Button BuilderPencilCancelBtn;

    Transform Blueprint;
    Button SpinBtn;
    Button BlueprintCancelBtn;
    Button BuildBtn;

    List<ItemBtn> itemBtns;

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

        RefreshItemBtns();
        StartCoroutine("FadeIn");
    }

    private void InitUI()
    {
        SceneName = FindChiled<Text>("SceneName");

        FadeinImg = FindChiled<Image>("Fadein");
        FadeinImg.enabled = true;

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
        BuilderBtn.onClick.AddListener(CreateBlueprint);
        BuilderPencilCancelBtn = FindChiled<Button>("BuilderPencilCancelBtn", BuilderPencil.gameObject);
        BuilderPencilCancelBtn.onClick.AddListener(CancelBuilderPencilCancelBtn);

        Blueprint = FindChiled("Blueprint");
        SpinBtn = FindChiled<Button>("SpinBtn", Blueprint.gameObject);
        SpinBtn.onClick.AddListener(SpinBlueprint);
        BlueprintCancelBtn = FindChiled<Button>("BlueprintCancelBtn", Blueprint.gameObject);
        BlueprintCancelBtn.onClick.AddListener(CancelUserBlueprint);
        BuildBtn = FindChiled<Button>("BuildBtn", Blueprint.gameObject);
        BuildBtn.onClick.AddListener(BuildBlueprint);

        PlayerCtl.E.Joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerCtl.E.ScreenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
        PlayerCtl.E.CameraCtl = Camera.main.GetComponent<CameraCtl>();
    }

    private void AddItemBtns()
    {
        itemBtns = new List<ItemBtn>();

        for (int i = 0; i < 6; i++)
        {
            var cell = AddCell<ItemBtn>("Prefabs/UI/ItemBtn", btnsParent);
            if (cell == null)
                return;

            itemBtns.Add(cell);
        }
    }

    private void CreateBlueprint()
    {
        Debug.Log("BuilderBtn");

        PlayerCtl.E.BuilderPencil.CreateBlueprint();
    }
    private void CancelBuilderPencilCancelBtn()
    {
        Debug.Log("CancelBtn");

        PlayerCtl.E.BuilderPencil.CancelCreateBlueprint();
    }
    private void SpinBlueprint()
    {
        PlayerCtl.E.BuilderPencil.SpinBlueprint();
    }
    private void CancelUserBlueprint()
    {
        PlayerCtl.E.BuilderPencil.CancelUserBlueprint();
    }
    private void BuildBlueprint()
    {
        PlayerCtl.E.BuilderPencil.BuildBlueprint();
    }

    public void ShowBuilderPencilBtn(bool b = true)
    {
        if (BuilderPencil != null)
            BuilderPencil.gameObject.SetActive(b);
    }
    public void ShowBlueprintBtn(bool b = true)
    {
        if (Blueprint != null)
            Blueprint.gameObject.SetActive(b);
    }

    public void RefreshItemBtns()
    {
        for (int i = 0; i < itemBtns.Count; i++)
        {
            if (i > DataMng.E.Items.Count - 1)
            {
                itemBtns[i].Clear();
                continue;
            }

            itemBtns[i].Init(DataMng.E.Items[i]);
        }
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
