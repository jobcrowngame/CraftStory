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
    Button LockCursorBtn;

    Transform cubeSelection;
    Button[] cubBtns;

    private float fadeInTime = 0.05f;

    private void Start()
    {
        WorldMng.E.CreateGameObjects();

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

    void Update()
    {
        if (SettingMng.E.MouseCursorLocked)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                OnClickBlackBtn();
            if (Input.GetKeyDown(KeyCode.Alpha2))
                OnClickBlueBtn();
            if (Input.GetKeyDown(KeyCode.Alpha3))
                OnClickRedBtn();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                OnClickGreenBtn();
        }

        if (Input.GetKeyDown(KeyCode.F9))
            OnClickLockCursorBtn();
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

        LockCursorBtn = FindChiled<Button>("LockCursorBtn");
        FindChiled<Text>("Text", LockCursorBtn.gameObject).text = "F9 Key Lock Cursor";
        LockCursorBtn.onClick.AddListener(OnClickLockCursorBtn);

        cubeSelection = FindChiled("CubeSelection");

        cubBtns = new Button[4];
        cubBtns[0] = FindChiled<Button>("BlackBtn");
        cubBtns[0].onClick.AddListener(() => { OnClickBlackBtn(); });
        cubBtns[1] = FindChiled<Button>("BlueBtn");
        cubBtns[1].onClick.AddListener(() => { OnClickBlueBtn(); });
        cubBtns[2] = FindChiled<Button>("RedBtn");
        cubBtns[2].onClick.AddListener(() => { OnClickRedBtn(); });
        cubBtns[3] = FindChiled<Button>("GreenBtn");
        cubBtns[3].onClick.AddListener(() => { OnClickGreenBtn(); });

        PlayerEntity.E.joystick = FindChiled<SimpleInputNamespace.Joystick>("Joystick");
        PlayerEntity.E.screenDraggingCtl = FindChiled<ScreenDraggingCtl>("ScreenDraggingCtl");
    }

    #region ButtonClick

    private void OnClickBlackBtn()
    {
        Debug.Log("OnClickBlackBtn");
        cubeSelection.position = cubBtns[0].transform.position;

        PlayerEntity.E.ChangeSelectBlock(1001);
    }
    private void OnClickBlueBtn()
    {
        Debug.Log("OnClickBlueBtn");
        cubeSelection.position = cubBtns[1].transform.position;

        PlayerEntity.E.ChangeSelectBlock(1002);
    }
    private void OnClickRedBtn()
    {
        Debug.Log("OnClickRedBtn");
        cubeSelection.position = cubBtns[2].transform.position;

        PlayerEntity.E.ChangeSelectBlock(1003);
    }
    private void OnClickGreenBtn()
    {
        Debug.Log("OnClickGreenBtn");
        cubeSelection.position = cubBtns[3].transform.position;

        PlayerEntity.E.ChangeSelectBlock(1004);
    }
    private void OnClickLockCursorBtn()
    {
        Debug.Log("Lock MouseCursor");

        SettingMng.E.MouseCursorLocked = !SettingMng.E.MouseCursorLocked;
        FindChiled<Text>("Text", LockCursorBtn.gameObject).text = SettingMng.E.MouseCursorLocked ?
                "F9 Key UnLock Cursor" :
                "F9 Key Lock Cursor";
    }

    #endregion

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
