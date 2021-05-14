using UnityEngine;
using UnityEngine.UI;

public class HomeUI : UIBase
{
    Button BagBtn;
    Button LotteryBtn;
    Button ShopBtn;

    Button[] cubeBtns;
    Transform cubeSelection;

    public override void Init(GameObject obj)
    {
        base.Init(obj);

        HomeLG.E.Init(this);

        InitUI();
    }

    void Update()
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

    private void InitUI()
    {
        BagBtn = FindChiled<Button>("BagBtn");
        BagBtn.onClick.AddListener(() => { UICtl.E.OpenUI<BagUI>(UIType.Bag); });

        LotteryBtn = FindChiled<Button>("LotteryBtn");
        LotteryBtn.onClick.AddListener(() => { UICtl.E.OpenUI<LotteryUI>(UIType.Lottery); });

        ShopBtn = FindChiled<Button>("ShopBtn");
        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });

        cubeSelection = FindChiled("CubeSelection");
        cubeBtns = new Button[4];
        cubeBtns[0] = FindChiled<Button>("BlackBtn");
        BagBtn.onClick.AddListener(OnClickBlackBtn);
        cubeBtns[1] = FindChiled<Button>("BlueBtn");
        BagBtn.onClick.AddListener(OnClickBlueBtn);
        cubeBtns[2] = FindChiled<Button>("RedBtn");
        BagBtn.onClick.AddListener(OnClickRedBtn);
        cubeBtns[3] = FindChiled<Button>("GreenBtn");
        BagBtn.onClick.AddListener(OnClickGreenBtn);
    }

    private void OnClickBlackBtn()
    {
        Debug.Log("Click button " + cubeBtns[0].name);
        cubeSelection.transform.position = cubeBtns[0].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Black);
    }
    private void OnClickBlueBtn()
    {
        Debug.Log("Click button " + cubeBtns[1].name);
        cubeSelection.transform.position = cubeBtns[1].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Blue);
    }
    private void OnClickRedBtn()
    {
        Debug.Log("Click button " + cubeBtns[2].name);
        cubeSelection.transform.position = cubeBtns[2].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Red);
    }
    private void OnClickGreenBtn()
    {
        Debug.Log("Click button " + cubeBtns[3].name);
        cubeSelection.transform.position = cubeBtns[3].transform.position;
        PlayerEntity.E.ChangeSelectMapCellType(MapCellType.Green);
    }
}
