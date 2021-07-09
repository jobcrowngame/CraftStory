using UnityEngine.UI;

public class MenuUI : UIBase
{
    Button CloseBtn { get => FindChiled<Button>("CloseBtn"); }
    //Button CraftBtn;
    Button AdventureBtn { get => FindChiled<Button>("AdventureBtn"); }
    Button ShopBtn { get => FindChiled<Button>("ShopBtn"); }
    Button HomeBtn { get => FindChiled<Button>("HomeBtn"); }
    Button PlayDescriptionBtn { get => FindChiled<Button>("PlayDescriptionBtn"); }
    Button PersonalMessageBtn { get => FindChiled<Button>("PersonalMessageBtn"); }
    Button MyShopBtn { get => FindChiled<Button>("MyShopBtn"); }
    Button MessageBtn { get => FindChiled<Button>("MessageBtn"); }

    MenuUIType menuType
    {
        set
        {
            AdventureBtn.gameObject.SetActive(false);
            ShopBtn.gameObject.SetActive(false);
            HomeBtn.gameObject.SetActive(false);
            PlayDescriptionBtn.gameObject.SetActive(false);
            PersonalMessageBtn.gameObject.SetActive(false);
            MyShopBtn.gameObject.SetActive(false);

            switch (value)
            {
                case MenuUIType.Home:
                    AdventureBtn.gameObject.SetActive(true);
                    ShopBtn.gameObject.SetActive(true);
                    PlayDescriptionBtn.gameObject.SetActive(true);
                    PersonalMessageBtn.gameObject.SetActive(true);
                    MyShopBtn.gameObject.SetActive(true);
                    break;

                case MenuUIType.Brave:
                    HomeBtn.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public void Awake()
    {
        CloseBtn.onClick.AddListener(() => { Close(); });
        AdventureBtn.onClick.AddListener(() =>
        {
            DataMng.E.MapData.TransferGate = new EntityData(1000, ItemType.TransferGate);
            CommonFunction.GoToNextScene();
            Close();
        });
        ShopBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<ShopUI>(UIType.Shop);
            Close();
        });
        HomeBtn.onClick.AddListener(() =>
        {
            CommonFunction.GoToHome();
            Close();
        });
        PlayDescriptionBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<PlayDescriptionUI>(UIType.PlayDescription);
            Close();
        });
        PersonalMessageBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<PersonalMessageUI>(UIType.PersonalMessage);
            Close();
        });
        MyShopBtn.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(DataMng.E.UserData.NickName))
            {
                string msg = @"マイショップを利用するには、
ニックネームを登録する必要があります。";

                CommonFunction.ShowHintBox(msg, ()=> 
                {
                    UICtl.E.OpenUI<PersonalMessageUI>(UIType.PersonalMessage);
                    Close();
                });
                return;
            }

            UICtl.E.OpenUI<MyShopUI>(UIType.MyShop);
            Close();
        });
        MessageBtn.onClick.AddListener(() =>
        {
            UICtl.E.OpenUI<EmailUI>(UIType.Email);
            Close();
        });
    }

    public void Init(MenuUIType uiType)
    {
        base.Init();
        MenuLG.E.Init(this);

        menuType = uiType;
    }
    public override void Open()
    {
        base.Open();
        RefreshRedPoint();
    }
    public void RefreshRedPoint()
    {
        var RedPoint = FindChiled("RedPoint", MessageBtn.gameObject);
        if(RedPoint != null) RedPoint.gameObject.SetActive(CommonFunction.NewMessage());
    }

    public enum MenuUIType
    {
        Home,
        Brave,
    }
}