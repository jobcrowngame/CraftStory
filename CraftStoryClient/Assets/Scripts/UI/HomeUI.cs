using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Button BagBtn;
    Button LotteryBtn;
    Button ShopBtn;
    Button LockCursorBtn;

    Transform cubeSelection;
    Button[] cubBtns;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        HomeLG.E.Init(this);

        InitUI();
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
        {
            Debug.Log("Lock MouseCursor");
            SettingMng.E.MouseCursorLocked = !SettingMng.E.MouseCursorLocked;
            FindChiled<Text>("Text", LockCursorBtn.gameObject).text = "F10 Key UnLock Cursor";
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.Log("UnLock MouseCursor");
            SettingMng.E.MouseCursorLocked = !SettingMng.E.MouseCursorLocked;
            FindChiled<Text>("Text", LockCursorBtn.gameObject).text = "F9 Key Lock Cursor";
        }
    }

    private void InitUI()
    {
        BagBtn = FindChiled<Button>("BagBtn");
        BagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        LotteryBtn = FindChiled<Button>("LotteryBtn");
        LotteryBtn.onClick.AddListener(() => { UICtl.E.OpenUI<LotteryUI>(UIType.Lottery); });

        ShopBtn = FindChiled<Button>("ShopBtn");
        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });

        LockCursorBtn = FindChiled<Button>("LockCursorBtn");
        FindChiled<Text>("Text", LockCursorBtn.gameObject).text = "F9 Key Lock Cursor";
        LockCursorBtn.onClick.AddListener(() => 
        {
            Debug.Log("Lock MouseCursor");
            SettingMng.E.MouseCursorLocked = !SettingMng.E.MouseCursorLocked;
            FindChiled<Text>("Text", LockCursorBtn.gameObject).text = "F10 Key UnLock Cursor";
        });

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
    }

    private void OnClickBlackBtn()
    {
        Debug.Log("OnClickBlackBtn");
        cubeSelection.position = cubBtns[0].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Black);
    }
    private void OnClickBlueBtn()
    {
        Debug.Log("OnClickBlueBtn");
        cubeSelection.position = cubBtns[1].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Blue);
    }
    private void OnClickRedBtn()
    {
        Debug.Log("OnClickRedBtn");
        cubeSelection.position = cubBtns[2].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Red);
    }
    private void OnClickGreenBtn()
    {
        Debug.Log("OnClickGreenBtn");
        cubeSelection.position = cubBtns[3].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Green);
    }
}
