using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    //Button CraftBtn;
    Button AdventureBtn { get => FindChiled<Button>("AdventureBtn"); }
    Button ShopBtn { get => FindChiled<Button>("ShopBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }

    MenuUIType menuType
    {
        set
        {
            switch (value)
            {
                case MenuUIType.Home:
                    AdventureBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    HomeBtn.gameObject.SetActive(false);
                    break;

                case MenuUIType.Brave:
                    AdventureBtn.gameObject.SetActive(false);
                    ShopBtn.gameObject.SetActive(false);
                    HomeBtn.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void Init(MenuUIType uiType)
    {
        base.Init();
        MenuLG.E.Init(this);

        menuType = uiType;

        CloseBtn.onClick.AddListener(() => { Close(); });

        //CraftBtn = FindChiled<Button>("CraftBtn");
        //CraftBtn.onClick.AddListener(() => { UICtl.E.OpenUI<CraftUI>(UIType.Craft); });

        DataMng.E.MapData.TransferGate = new EntityData(1000, ItemType.TransferGate);
        AdventureBtn.onClick.AddListener(CommonFunction.GoToNextScene);

        ShopBtn.onClick.AddListener(() => { UICtl.E.OpenUI<ShopUI>(UIType.Shop); });

        HomeBtn.onClick.AddListener(CommonFunction.GoToHome);
    }

    public enum MenuUIType
    {
        Home,
        Brave,
    }
}